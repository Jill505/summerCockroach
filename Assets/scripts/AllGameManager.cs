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
    
    public GameObject gameEndCanvas;
    public GameObject gameFailCanvas;

    public float pressTime = 2f;
    float pressTimeCal = 0f;
    public int nowLoadSceneSort = 0;

    void Start()
    {
        nowLoadSceneSort = SceneManager.GetActiveScene().buildIndex;
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
}


public enum moveMode
{
    AutoCameraMove,
    PlayerCameraMove,

    twoDMove,
    ChangeSceneMoment
}