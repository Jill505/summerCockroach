using UnityEngine;

public class Scene2DManager : MonoBehaviour
{
    [System.Serializable]
    public class Scene2D
    {
        public string sceneName;          // 2D 場景名稱
        public BoxCollider2D cameraBounds;       // 攝影機範圍
        public EdgeCollider2D spawnBounds;        // 物件生成範圍
        public Transform randomMotherCockroachRange1; // 隨機生成母蟑螂位置範圍
        public Transform randomMotherCockroachRange2; // 隨機生成母蟑螂位置範圍
        public Transform randomMotherCockroachRange3; // 隨機生成母蟑螂位置範圍
        public Transform motherCockroachPoints;  // 固定生成位置
        public Transform insPos1;
        public Transform insPos2;
    }
    public static Scene2DManager Instance;

    public Scene2D[] scenes; // 所有場景資料

    private void Awake()
    {
        Instance = this;
    }

    // 取得場景資料
    public Scene2D GetSceneByName(string name)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneName == name)
                return scene;
        }
        Debug.LogWarning("找不到場景名稱：" + name);
        return null;
    }
}
