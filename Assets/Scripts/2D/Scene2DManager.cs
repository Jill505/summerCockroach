using UnityEngine;


public enum Scene2DOneHole
{
    TreeHole,  //樹洞
}
public enum Scene2DDoubleHole
{
    Cave,    //大縫
    HalfCave01, //砍半縫01
    HalfCave02  //砍半縫02
}
public class Scene2DManager : MonoBehaviour
{
    [System.Serializable]
    public class Scene2D
    {
        public Transform parent;

        [HideInInspector] public BoxCollider2D cameraBounds; // 攝影機範圍
        [HideInInspector] public EdgeCollider2D spawnBounds; // 物件生成範圍
        [HideInInspector] public Transform insPos1;
        [HideInInspector] public Transform insPos2;
        [HideInInspector] public EdgeCollider2D spawnSpiderAreaL;
        [HideInInspector] public EdgeCollider2D spawnSpiderAreaR;
        public void Initialize()
        {
            if (parent == null)
            {
                Debug.LogWarning("Scene2D 的 parent 沒有指定！");
                return;
            }
            cameraBounds = parent.Find("cameraBounds")?.GetComponent<BoxCollider2D>();
            spawnBounds = parent.Find("spawnBounds")?.GetComponent<EdgeCollider2D>();
            insPos1 = parent.Find("insPos1");
            insPos2 = parent.Find("insPos2");
            spawnSpiderAreaL = parent.Find("spawnSpiderAreaL")?.GetComponent<EdgeCollider2D>();
            spawnSpiderAreaR = parent.Find("spawnSpiderAreaR")?.GetComponent<EdgeCollider2D>();



            if (cameraBounds == null) Debug.LogWarning(parent.name + " 缺少 cameraBounds");
            if (spawnBounds == null) Debug.LogWarning(parent.name + " 缺少 spawnBounds");
            if (insPos1 == null) Debug.LogWarning(parent.name + " 缺少 insPos1");
        }
    }
    public static Scene2DManager Instance;

    public Scene2D TreeHole;
    public Scene2D Cave;
    public Scene2D HalfCave01;
    public Scene2D HalfCave02;

    [Header("2DScenePrefab")]
    public GameObject Food2D;
    public GameObject Spider2D;
    public GameObject Spider2DTrigger;

    private void Awake()
    {
        Instance = this;

        TreeHole?.Initialize();
        Cave?.Initialize();
        HalfCave01?.Initialize();
        HalfCave02?.Initialize();
    }

    // 取得場景資料
    public Scene2D GetScene(Scene2DOneHole scene)
    {
        switch (scene)
        {
            case Scene2DOneHole.TreeHole:
                return TreeHole;
        }
        Debug.LogWarning("找不到單洞場景：" + scene);
        return null;
    }

    public Scene2D GetScene(Scene2DDoubleHole scene)
    {
        switch (scene)
        {
            case Scene2DDoubleHole.Cave:
                return Cave;
            case Scene2DDoubleHole.HalfCave01:
                return HalfCave01;
            case Scene2DDoubleHole.HalfCave02:
                return HalfCave02;
        }
        Debug.LogWarning("找不到雙洞場景：" + scene);
        return null;
    }
}


