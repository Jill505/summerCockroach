
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class AllGameManager : MonoBehaviour
{
    static public float GravityVariable = 9.81f;

    public Text cockroachCollectProcessShowcase;

    [Header("�έp")]
    public int cockroachCollectTarget = 3;
    public int cockroachCollectNum = 0;

    public bool GameFinished = false;

    public int foodCollect = 0 ;
    

    public float pressTime = 2f;
    float pressTimeCal = 0f;
    public int nowLoadSceneSort = 0;

    [Header("UI �]�w")]
    public Text timerText; // ��ܮɶ��� UI Text

    [Header("�p�ɳ]�w")]
    public float gameMinutes = 3f; // �i�H�b Inspector �]�w�X����
    private float timeRemaining;   
    private bool isTimerRunning = true;
    public float gameProcessTime = 0;

    [Header("����e��")]
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
        cockroachCollectProcessShowcase.text = "�����������i�סG" + cockroachCollectNum + "/" + cockroachCollectTarget;

        if (Input.GetKeyDown(KeyCode.R))
        {
            pressTimeCal = 0f;
        }

        if (GameFinished && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(nowLoadSceneSort);//���ҳ���
        }

        if (Input.GetKey(KeyCode.R))
        {
            pressTimeCal += Time.deltaTime;
            if (pressTimeCal >= pressTime)
            {
                SceneManager.LoadScene(nowLoadSceneSort);//���ҳ���
            }
        }

        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                // ����ɶ�
                timeRemaining -= Time.deltaTime;
                gameProcessTime += Time.deltaTime;

                // ��s UI
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                // �ɶ���
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
            //�L��
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

        // ��ܦ� mm:ss �榡
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimeUp()
    {
        Debug.Log("�ɶ���I");

        // ��ܵ���e��
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
        surTimeShowcase.text = "�s���ɶ�\n" + string.Format("{0:00}:{1:00}", gameProcessTime /60, gameProcessTime%60);
        femCockroachCollectShowcase.text += "������������\n" + cockroachCollectNum;
        foodCollectShowcase.text += "����������\n" + foodCollect;
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