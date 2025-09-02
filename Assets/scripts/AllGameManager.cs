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

    [Header("UI �]�w")]
    public Text timerText; // ��ܮɶ��� UI Text

    [Header("�p�ɳ]�w")]
    public float gameMinutes = 3f; // �i�H�b Inspector �]�w�X����
    private float timeRemaining;   
    private bool isTimerRunning = true;

    [Header("����e��")]
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
        gameFailCanvas.SetActive(true);  
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