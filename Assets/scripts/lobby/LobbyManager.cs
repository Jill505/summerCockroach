using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public int gameCount;

    public LevelDesc nowLoadingLevel;
    public LevelDesc[] levelInfo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevelInfo(int levelSort)
    {
        SceneManager.LoadScene(levelSort);
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

[System.Serializable]
public class LevelDesc
{
    public int sceneSort;
    public string levelName;
    public string levelDesc;

    public Sprite sceneImage;
}