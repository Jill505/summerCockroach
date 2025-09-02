using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AllGameManager : MonoBehaviour
{
    static public float GravityVariable = 9.81f;

    public Text cockroachCollectProcessShowcase;

    public int cockroachCollectTarget = 3;
    public int cockroachCollectNum = 0;

    public bool GameFinished = false;
    
    

    public float pressTime = 2f;
    float pressTimeCal = 0f;
    public int nowLoadSceneSort = 0;

    [Header("UI 設定")]
    public Text timerText; // 顯示時間的 UI Text

    [Header("計時設定")]
    public float gameMinutes = 3f; // 可以在 Inspector 設定幾分鐘
    private float timeRemaining;   
    private bool isTimerRunning = true;

    [Header("結算畫面")]
    public GameObject gameEndCanvas;
    public GameObject gameFailCanvas;
    public GameObject DemoResultCanvas;

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
        gameFailCanvas.SetActive(true);  
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
        if (DemoResultCanvas != null)
        {
            DemoResultCanvas.SetActive(true);
        }
    }
}


public enum moveMode
{
    AutoCameraMove,
    PlayerCameraMove,

    twoDMove,
    ChangeSceneMoment
}