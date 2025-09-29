
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
//using Unity.Android.Gradle;

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

    public Button ReLocateButt;
    private GameObject Cockroach3D;

    public int InRoundKillNpc = 0;
    public int InRoundKilledBySpider = 0;


    void Start()
    {
        BGMManager.Play("BGM_Revival of Africa");
        Cockroach3D = GameObject.Find("3DCockroach");
        eraManager = GetComponent<EraManager>();
        nowLoadSceneSort = SceneManager.GetActiveScene().buildIndex;
        cManager = FindFirstObjectByType<CockroachManager>();

        timeRemaining = gameMinutes * 60f;

        InRoundKillNpc = 0;
        InRoundKilledBySpider = 0;
    }
    public void WHATEVERUPDATE()
    {
        FoodTrigger.eatDieCount += Time.deltaTime;

        bool _ACHI5UnlockKey = true;
        foreach (FemCockraochTrigger3D FCT3D in femCockroachTrackList)
        {
            if (!FCT3D.getDNAAlready)
            {
                _ACHI5UnlockKey = false;
                break;
            }
        }
        if (_ACHI5UnlockKey) GO_unlockAchievement(4);

        if (SaveSystem.mySaveFile.EnterHoleCount > 5)
        {
            GO_unlockAchievement(6);
        }

        if (SaveSystem.mySaveFile.RespawnCal > 10)
        {
            GO_unlockAchievement(7);
        }

        if (SaveSystem.mySaveFile.FoodCollect > 50)
        {
            GO_unlockAchievement(9);
        }

        if (SaveSystem.mySaveFile.FemRoachBreed > 11)
        {
            GO_unlockAchievement(10);
        }

        if (SaveSystem.mySaveFile.NPCKillNum > 10)
        {
            GO_unlockAchievement(11);
        }

        if (InRoundKilledBySpider > 3)
        {
            GO_unlockAchievement(13);
        }
        if (SaveSystem.mySaveFile.KillByThornTimes > 5)
        {
            GO_unlockAchievement(14);
        }
        if (SaveSystem.mySaveFile.EnterHoleCount > 50)
        {
            GO_unlockAchievement(15);
        }
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

        //DEBUG: 從未接觸過母蟑螂，無法觸發回到最近的母蟑螂位置
        int _IDontGiveAFuckAboutItsName = 0;
        foreach (FemCockraochTrigger3D FCT3D in femCockroachTrackList)
        {
            if (FCT3D.getDNAAlready)
            {
                _IDontGiveAFuckAboutItsName++;
            }
        }
        if (_IDontGiveAFuckAboutItsName > 0) ReLocateButt.interactable = true;
        else ReLocateButt.interactable = false;


        //I don't give a fuck about any of the coding rules
        if (gameProcessTime > 120)
        {
            GO_unlockAchievement(0);
        }
        if (gameProcessTime > 300)
        {
            GO_unlockAchievement(1);
        }

        WHATEVERUPDATE();
    }

    public void GO_unlockAchievement(int achiSOrt)
    {
        if (SaveSystem.mySaveFile.AchievementUnlock[achiSOrt] == false)
        {
            //UNLOCK
            SaveSystem.mySaveFile.AchievementUnlock[achiSOrt] = true;
            //CALL
            CallAchiComplete(achievementsSO[achiSOrt].myName, achievementsSO[achiSOrt].mySprite);
        }
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
        if (AnimationEventReceiver.prepared == false) return; // 暫停時不計時
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


        bool _ACHI9UnlockKey = true;
        foreach (FemCockraochTrigger3D FCT3D in femCockroachTrackList)
        {
            if (FCT3D.getDNAAlready)
            {
                _ACHI9UnlockKey = false;
                break;
            }
        }
        if (_ACHI9UnlockKey) GO_unlockAchievement(8);

        if (InRoundKillNpc == 0) GO_unlockAchievement(12);

        GO_unlockAchievement(16);
        SaveSystem.mySaveFile.winCount++;
        if (SaveSystem.mySaveFile.winCount > 2)
        {
            GO_unlockAchievement(17);
        }

        // 顯示結算畫面
        ShowGameResult();
        //DemoResult();
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
        SoundManager.StopAllSounds();
        SceneManager.LoadScene(nowLoadSceneSort);
    }

    public void BackToStartScreen()
    {
        SoundManager.StopAllSounds();
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

    [Header("成就系統")]
    public Text achiName;
    public Image achiImage;
    public Animator achiAnimator;
    [Header("Fuck Everything")]
    public Achievement[] achievementsSO;

    public void CallAchiComplete(string str, Sprite spr)
    {
        achiName.text = str;
        achiImage.sprite = spr;
        achiAnimator.SetTrigger("showAchi");
    }


    public void ReLocateAtFem()
    {
        Debug.Log("回到最近的已觸發母蟑螂");
        float d = Vector3.Distance(transform.position, femCockroachTrackList[0].gameObject.transform.position);
        int t = 0;
        for (int i = 1; i < femCockroachTrackList.Count; i++)
        {
            if (femCockroachTrackList[i].eggNumber > 0 && !femCockroachTrackList[i].getDNAAlready)
            {
                float nD = Vector3.Distance(transform.position, femCockroachTrackList[i].gameObject.transform.position);
                if (d > nD)
                {
                    d = nD;
                    t = i;
                }
            }
        }

        //重新定位自己到位置
        Vector3 debugUpper = new Vector3(0, 4, 2);
        if (femCockroachTrackList[t].coolDownCal < 15)
        {
            femCockroachTrackList[t].coolDownCal = 15f;
            //已防落地馬上有蛋
        }
        Cockroach3D.transform.position = femCockroachTrackList[t].myEggPos.position + debugUpper;
    }
    public void ButtonSound()
    {
        SoundManager.Play("SFX_Wooden Button Click");
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

