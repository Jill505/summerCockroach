using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public int gameCount;

    public GameObject AchievementCanvas;


    public LevelDesc nowLoadingLevel;
    public LevelDesc[] levelInfo;


    [Header("Achievements")]
    public GameObject Context;
    public Achievement[] achievements;
    public GameObject Prefab_achievements;

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

    public void OpenAchieve()
    {
        AchievementCanvas.SetActive(true);
        CleanContext();
        LoadAchievement();
    }
    public void CloseAchieve()
    {
        AchievementCanvas.SetActive(false);
    }

    public void CleanContext()
    {
        
        for (int i = Context.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(Context.transform.GetChild(i).gameObject);
        }
    }
    public void LoadAchievement()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            GameObject obj = Instantiate(Prefab_achievements);
            obj.transform.parent = Context.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);

            //Bg
            //obj.transform.GetChild(0);

            //Sprite
            obj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = achievements[i].mySprite;

            //Name
            obj.transform.GetChild(2).gameObject.GetComponent<Text>().text = achievements[i].name;

            //Desc
            obj.transform.GetChild(3).gameObject.GetComponent<Text>().text = achievements[i].myDescription ;

            //Unlock
            obj.transform.GetChild(4).gameObject.SetActive(!SaveSystem.mySaveFile.AchievementUnlock[i]);
        }
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