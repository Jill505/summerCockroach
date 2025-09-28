using System.IO;
using Unity.Android.Gradle;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    static public SaveFile mySaveFile = new SaveFile();

    static public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "GameSaveFile");
        File.WriteAllText(path, JsonUtility.ToJson(mySaveFile));
    }

    static public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "GameSaveFile");
        if (!File.Exists(path))
        {
            File.WriteAllText(path,JsonUtility.ToJson(mySaveFile));
            mySaveFile = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path));
        }
        else
        {
            mySaveFile = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path));
        }
    }

    static public void CleanSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "GameSaveFile");
        mySaveFile = new SaveFile();
        File.WriteAllText(path, JsonUtility.ToJson(mySaveFile));
    }

    private void Start()
    {
        //CleanSaveFile();
        Load();
    }
}

[System.Serializable]
public class SaveFile
{
    [Header("Achievement process")]
    public bool[] AchievementUnlock = new bool[100];

    [Header("Stats")]
    public int FemRoachBreed = 0;
    public int RespawnCal = 0;
    public int EnterHoleCount = 0;
    public int FoodCollect = 0;
    public int NPCKillNum = 0;

    public int KillByThornTimes = 0;

    public int winCount = 0;
}