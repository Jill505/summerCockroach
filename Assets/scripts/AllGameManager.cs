
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AllGameManager : MonoBehaviour
{
    [Header("Ref component")]
    public CockroachManager cManager;

    static public float GravityVariable = 9.81f;

    public Text cockroachCollectProcessShowcase;

    [Header("統計")]
    public int cockroachCollectTarget = 3;
    public int cockroachCollectNum = 0;

    public bool GameFinished = false;

    public int foodCollect = 0;
    public int fuckNPCCollect = 0;


    public float pressTime = 2f;
    float pressTimeCal = 0f;
    public int nowLoadSceneSort = 0;

    [Header("UI 設定")]
    public Text timerText; // 顯示時間的 UI Text
    public Text demoResultShowcase;// 顯示demo結束的時間的Text

    [Header("計時設定")]
    public float gameMinutes = 3f; // 可以在 Inspector 設定幾分鐘
    private float timeRemaining;
    public bool isTimerRunning = true;
    public float gameProcessTime = 0;
    private float scoreTimer = 0f;

    [Header("Result")]
    public GameObject gameEndCanvas;
    public GameObject gameFailCanvas;
    public GameObject DemoResultCanvas;
    public GameObject showGameResultCanvas;


    [Header("Score")]
    public float fuckNPCScore = 150f;
    public float eatFood = 100f;
    public float survive30Seconds = 200f;
    public float findFem = 500f;
    public float FTheWeb = 50f;
    public float OutTheSpiderHole = 450f;
    private float score = 0f;

    [Header("加分特效")]
    public Canvas worldCanvas3D;
    public GameObject scoreTextPrefab; // 你的 Text Prefab
    public BoxCollider2D scoreSpawnArea;



    [Header("Trackers")]
    public Text scoreShowcase;
    public Text surTimeShowcase;
    public Text femCockroachCollectShowcase;
    public Text foodCollectShowcase;
    public Text fuckNPCShowcase;

    private EraManager eraManager;

    [Header("Evolution")]
    public int DNA = 0;
    public GameObject CockroachEvolutionCanvas;

    [Header("Fem Cockroach")]
    public int allLifeCount = 0;
    public List<FemCockraochTrigger3D> femCockroachTrackList;


    void Start()
    {
        eraManager = GetComponent<EraManager>();
        nowLoadSceneSort = SceneManager.GetActiveScene().buildIndex;
        cManager = FindFirstObjectByType<CockroachManager>();

        timeRemaining = gameMinutes * 60f;
    }

    void Update()
    {
        cockroachCollectProcessShowcase.text = "母蟑螂收集進度：" + cockroachCollectNum + "/" + cockroachCollectTarget;

        scoreTimer += Time.deltaTime;

        if (scoreTimer >= 30f)
        {
            AddScore(survive30Seconds); // 加分數
            scoreTimer = 0f;            // 重置計時器
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pressTimeCal = 0f;
        }

        if (GameFinished && Input.GetKeyDown(KeyCode.R))
        {
            eraManager.ClearEraObjects();
            SceneManager.LoadScene(nowLoadSceneSort);//重啟場景
        }

        if (Input.GetKey(KeyCode.R))
        {
            pressTimeCal += Time.deltaTime;
            if (pressTimeCal >= pressTime)
            {
                eraManager.ClearEraObjects();
                SceneManager.LoadScene(nowLoadSceneSort);//重啟場景
            }
        }
        GameTimer();

        allLifeCount = 0;
        for (int i = 0; i < femCockroachTrackList.Count; i++)
        {
            allLifeCount += femCockroachTrackList[i].eggNumber;
        }
        //Sync all life complete
    }

    public void femCockraochGet()
    {
        cockroachCollectNum++;
        //if (cockroachCollectNum >= cockroachCollectTarget)
        //{
        //    //過關
        //    GameFinished = true;
        //    gameEndCanvas.SetActive(true);
        //}
    }

    public void GameFail()
    {
        GameFinished = true;
        ShowGameResult();
        //gameFailCanvas.SetActive(true);  
    }


    void GameTimer()
    {
        if (!isTimerRunning) return; // 暫停時不計時

        if (timeRemaining > 0)
        {
            // 遞減時間
            timeRemaining -= Time.deltaTime;
            gameProcessTime += Time.deltaTime;

            // 更新 UI
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            // 時間到
            timeRemaining = 0;
            isTimerRunning = false;
            TimeUp();
        }

    }
    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timeToDisplay < 0)
            timeToDisplay = 0;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // 顯示成 mm:ss 格式
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimeUp()
    {
        Debug.Log("時間到！");

        // 顯示結算畫面
        ShowGameResult();
        DemoResult();


    }

    void DemoResult()
    {
        DemoResultCanvas.SetActive(true);
        demoResultShowcase.text = gameMinutes + "分鐘測試Demo結束!!!";
        demoResultShowcase.text += "感謝試玩!!!";
    }
    public void ShowGameResult()
    {
        if (showGameResultCanvas != null)
        {
            SyncInformationResultCanvas();
            showGameResultCanvas.SetActive(true);
        }
    }
    public void SyncInformationResultCanvas()
    {
        scoreShowcase.text = score.ToString();
        surTimeShowcase.text = "存活時間" + "                   " + string.Format("{0:00}:{1:00}", gameProcessTime / 60, gameProcessTime % 60);
        femCockroachCollectShowcase.text = "母蟑螂收集數" + "            " + cockroachCollectNum;
        foodCollectShowcase.text = "食物收集數" + "               " + foodCollect;
        fuckNPCShowcase.text = "撞飛同類次數" + "           " + fuckNPCCollect;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(nowLoadSceneSort);
    }

    public void BackToStartScreen()
    {
        SceneManager.LoadScene(0);
    }


    public void OpenDNASelect()
    {
        CockroachEvolutionCanvas.SetActive(true);
        cManager.RenderPlayerBuffs();
        Time.timeScale = 0.0000001f;
    }

    public void CloseDNASelect()
    {
        CockroachEvolutionCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void AddScore(float add)
    {
        score += add;

        Bounds bounds = scoreSpawnArea.bounds;

        // 在 Collider 範圍內隨機生成位置
        float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0f); // 文字 Z 軸可視 Canvas 設定調整

        // 生成 Prefab，掛在 3D Canvas 下
        GameObject go = Instantiate(scoreTextPrefab, worldCanvas3D.transform);

        // 設定位置
        go.transform.position = spawnPos;

        // 設定文字內容
        UnityEngine.UI.Text t = go.GetComponent<UnityEngine.UI.Text>();
        if (t != null)
        {
            t.text = "+" + add.ToString();

            // 如果加分大於 400，放大文字
            if (add > 400f)
            {
                go.GetComponent<RectTransform>().localScale = Vector3.one * 3f; // 放大 2 倍
            }
            else
            {
                go.GetComponent<RectTransform>().localScale = Vector3.one *2f; // 原本大小
            }

            // 啟動淡出 Coroutine
            StartCoroutine(RainbowFadeText(t, 2f));
        }

    }

    private IEnumerator FadeOutText(UnityEngine.UI.Text text, float duration)
    {
        float elapsed = 0f;
        Color originalColor = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        Destroy(text.gameObject); // 完成後刪除文字
    }
    private IEnumerator RainbowFadeText(UnityEngine.UI.Text text, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 透明度淡出
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            // 彩虹顏色
            float h = Mathf.Repeat(Time.time * 0.1f, 1f); // H 從 0~1，0.?f 控制變化速度
            Color color = Color.HSVToRGB(h, 1f, 1f);
            color.a = alpha;
            text.color = color;

            yield return null;
        }

        Destroy(text.gameObject);
    }

}

    public enum moveMode
    {
        AutoCameraMove,
        PlayerCameraMove,
        SpiderEvent,
        twoDMove,
        ChangeSceneMoment
    }

