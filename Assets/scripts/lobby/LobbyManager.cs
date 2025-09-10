using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    [Header("轉場設定")]
    public GameObject transitionUI;
    public Material transitionMaterial;

    [Header("動畫")]
    public Animator animator;



    private void Start()
    {
        if (transitionUI != null) transitionUI.SetActive(false);
    }

    public void LoadLevelInfo(int levelSort)
    {
        StartCoroutine(TransitionAndLoad(levelSort));
    }

    private IEnumerator TransitionAndLoad(int levelSort)
    {
        if (transitionUI != null) transitionUI.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSort);
        asyncLoad.allowSceneActivation = false;

        float value = 0f;

        float duration = 2.5f;
        float targetInitialScale = 800f; // 最初放大的目標值
        float speed = 320f;

        animator.SetBool("Transtion", true);

        while (value < targetInitialScale)
        {
            value += speed * Time.deltaTime;
            if (value > targetInitialScale) value = targetInitialScale;

            transitionMaterial.SetFloat("_Scale", value);
            yield return null;
        }

        // 切換到新場景
        asyncLoad.allowSceneActivation = true;
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
            obj.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = achievements[i].mySprite;

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