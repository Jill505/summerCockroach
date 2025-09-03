
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class AllGameManager : MonoBehaviour
{
    static public float GravityVariable = 9.81f;

    public Text cockroachCollectProcessShowcase;

    [Header("統計")]
    public int cockroachCollectTarget = 3;
    public int cockroachCollectNum = 0;

    public bool GameFinished = false;

    public int foodCollect = 0 ;
    

    public float pressTime = 2f;
    float pressTimeCal = 0f;
    public int nowLoadSceneSort = 0;

    [Header("UI 設定")]
    public Text timerText; // 顯示時間的 UI Text

    [Header("計時設定")]
    public float gameMinutes = 3f; // 可以在 Inspector 設定幾分鐘
    private float timeRemaining;   
    private bool isTimerRunning = true;
    public float gameProcessTime = 0;

    [Header("結算畫面")]
    public GameObject gameEndCanvas;
    public GameObject gameFailCanvas;
    public GameObject DemoResultCanvas;
    public GameObject showGameResultCanvas;

    [Header("Trackers")]
    public Text surTimeShowcase;
    public Text femCockroachCollectShowcase;
    public Text foodCollectShowcase;
    

    void Start()
    {
        nowLoadSceneSort = SceneManager.GetActiveScene().buildIndex;

        timeRemaining = gameMinutes * 60f;
    }
    
    void Update()
    {
        cockroachCollectProcessShowcase.text = "母蟑螂收集進度：" + cockroachCollectNum + "/" + cockroachCollectTarget;

        if (Input.GetKeyDown(KeyCode.R))
        {
            pressTimeCal = 0f;
        }

        if (GameFinished && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(nowLoadSceneSort);//重啟場景
        }

        if (Input.GetKey(KeyCode.R))
        {
            pressTimeCal += Time.deltaTime;
            if (pressTimeCal >= pressTime)
            {
                SceneManager.LoadScene(nowLoadSceneSort);//重啟場景
            }
        }

        if (isTimerRunning)
        {
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
    }

    public void femCockraochGet()
    {
        cockroachCollectNum++;

        if (cockroachCollectNum >= cockroachCollectTarget)
        {
            //過關
            GameFinished = true;
            gameEndCanvas.SetActive(true);
        }
    }

    public void GameFail()
    {
        GameFinished = true;
        ShowGameResult();
        //gameFailCanvas.SetActive(true);  
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

        if (DemoResultCanvas != null)
        {
            DemoResultCanvas.SetActive(true);
        }
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
        surTimeShowcase.text = "存活時間\n" + string.Format("{0:00}:{1:00}", gameProcessTime /60, gameProcessTime%60);
        femCockroachCollectShowcase.text += "母蟑螂收集數\n" + cockroachCollectNum;
        foodCollectShowcase.text += "食物收集數\n" + foodCollect;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(nowLoadSceneSort);
    }

    public void BackToStartScreen()
    {
        SceneManager.LoadScene(0);
    }
}


public enum moveMode
{
    AutoCameraMove,
    PlayerCameraMove,

    twoDMove,
    ChangeSceneMoment
}