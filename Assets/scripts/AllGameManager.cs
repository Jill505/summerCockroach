using UnityEngine;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cockroachCollectProcessShowcase.text = "母蟑螂收集進度：" + cockroachCollectNum + "/" + cockroachCollectTarget;

        

        if (GameFinished && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);//重啟場景
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
}


public enum moveMode
{
    AutoCameraMove,
    PlayerCameraMove,

    twoDMove,
    ChangeSceneMoment
}