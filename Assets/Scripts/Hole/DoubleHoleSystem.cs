using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum HoleSide
{
    Left,
    Right
}

[System.Serializable]
public class DoubleHolePair
{
    [Header("3D 外部洞口 Trigger（左右）")]
    public Collider leftHoleTrigger3D;
    public Collider rightHoleTrigger3D;

    [Header("3D 出口傳送點（左右）")]
    [HideInInspector] public Transform leftHoleExit3D;
    [HideInInspector] public Transform rightHoleExit3D;

    [Header("食物生成設定")]
    public bool enableFood = true;                 // 是否啟用生成
    public enum FoodAmount
    {
        少量, // 3 個
        大量  // 5 個
    }
    [Tooltip("選擇食物生成數量")]
    public FoodAmount selectedFoodAmount = FoodAmount.少量;
    [HideInInspector]public float spawnOffsetY = 0.5f;

    [Header("蜘蛛生成設定")]
    [HideInInspector] public bool enableSpider = false;                // 是否顯示蜘蛛
    

    [HideInInspector] public Scene2DDoubleHole selectedScene;

    // 初始化方法：把 enum 轉成字串
    public void InitSelectedScene()
    {
        enableSpider = Random.Range(0, 2) == 0;
        if (enableSpider)
        {
            selectedScene = Scene2DDoubleHole.Cave;
        }
        else
        {
            // 隨機選 HalfCave01 或 HalfCave02
            int rand = Random.Range(0, 2); // 0 或 1
            if (rand == 0)
                selectedScene = Scene2DDoubleHole.HalfCave01;
            else
                selectedScene = Scene2DDoubleHole.HalfCave02;
        }
    }
}


public class DoubleHoleSystem : MonoBehaviour
{
    [Header("多組 3D 洞口")]
    public DoubleHolePair[] pairs;

    [Header("玩家物件")]
    private CockroachMove cockroachMove3D;
    private Cockroach2DMove cockroachMove2D;
    private CameraViewToggle viewToggle;
    public CameraLogic2D cameraLogic2D;

    // 記錄目前是從哪一組洞進入洞穴（-1 表示未在洞穴流程中）
    private int currentPairIndex = -1;

    [Header("2D 洞穴內生成點（從3D進入時的出現位置）")]
    private Transform leftInsideSpawn2D;
    private Transform rightInsideSpawn2D;


    [Header("攝影機限制範圍")]
    private BoxCollider2D cameraBounds;
    private EdgeCollider2D spawnArea;                 // 生成範圍

    private List<GameObject> spawnedFood = new List<GameObject>();
    private List<GameObject> spawnedSpider = new List<GameObject>();

    private GameObject food;
    private GameObject spider;                  // 被隱藏的蜘蛛物件
                                                // 
    public HoleSide lastEnterSide { get; private set; } // 紀錄玩家是從哪邊進來的


    private void Awake()
    {
        food = GameObject.Find("AllGameManager").GetComponent<Scene2DManager>().Food2D;
        spider = GameObject.Find("AllGameManager").GetComponent<Scene2DManager>().Spider2DTrigger;
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        cockroachMove3D = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        cockroachMove2D = GameObject.Find("2DCockroach").GetComponent<Cockroach2DMove>();

        
        // 自動取得出口 & 綁定 pairIndex、side 到 Trigger
        if (pairs != null)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                var pair = pairs[i];

                // 初始化場景字串
                pair.InitSelectedScene();

                // 自動抓出口 Transform
                if (pair.leftHoleTrigger3D != null)
                {
                    pair.leftHoleExit3D = pair.leftHoleTrigger3D.transform.Find("3D StartPos L");

                    // 綁定 Trigger3D 上的 Hole3DTrigger 腳本
                    var triggerComp = pair.leftHoleTrigger3D.gameObject.GetComponent<Hole3DTrigger>();
                    if (triggerComp == null) triggerComp = pair.leftHoleTrigger3D.gameObject.AddComponent<Hole3DTrigger>();
                    triggerComp.Init(this, i, HoleSide.Left);
                }

                if (pair.rightHoleTrigger3D != null)
                {
                    pair.rightHoleExit3D = pair.rightHoleTrigger3D.transform.Find("3D StartPos R");

                    var triggerComp = pair.rightHoleTrigger3D.gameObject.GetComponent<Hole3DTrigger>();
                    if (triggerComp == null) triggerComp = pair.rightHoleTrigger3D.gameObject.AddComponent<Hole3DTrigger>();
                    triggerComp.Init(this, i, HoleSide.Right);
                }
            }
        }

    }




    // —— 3D → 2D：外部洞口觸發時呼叫 ——
    public void EnterFrom3D(int pairIndex, HoleSide side)
    {
        if (!IsValidPair(pairIndex)) return;
        if (cockroachMove2D == null) return;
        if (viewToggle.Is2D()) return;

        lastEnterSide = side; // ✅ 紀錄進來的方向

        // 取得對應 Pair 的場景名稱
        var pair = pairs[pairIndex];
        if (pair == null) return;

        pair.InitSelectedScene();

        if (Scene2DManager.Instance != null)
        {
            var sceneData = Scene2DManager.Instance.GetScene(pair.selectedScene);
            if (sceneData != null)
            {
                cameraBounds = sceneData.cameraBounds;
                spawnArea = sceneData.spawnBounds;
                leftInsideSpawn2D = sceneData.insPos1;
                rightInsideSpawn2D = sceneData.insPos2;
            }
        }

        Transform spawn = null;
        if (side == HoleSide.Left)
            spawn = leftInsideSpawn2D;
        else
            spawn = rightInsideSpawn2D;

        if (spawn == null) return;
        DesObj();

        cameraLogic2D.SetCustomBounds(cameraBounds.bounds);
        StartCoroutine(viewToggle.StartViewSwitch(false));

        cockroachMove2D.transform.position = spawn.position;
        currentPairIndex = pairIndex;

        if (pair.enableFood)
            SpawnRandomFoodOnPath(pair); // 傳入該 Pair 的資料

        if (pair.enableSpider)
            SpawnRandomSpiderOnPath(pair);

    }

    // —— 2D → 3D —— 
    public void ExitTo3D(HoleSide side)
    {
        if (!IsValidPair(currentPairIndex)) return;
        if (!viewToggle.Is2D()) return;
        if (cockroachMove3D == null) return;

        Transform exitPoint = null;
        if (side == HoleSide.Left)
            exitPoint = pairs[currentPairIndex].leftHoleExit3D;
        else
            exitPoint = pairs[currentPairIndex].rightHoleExit3D;

        if (exitPoint == null) return;

        StartCoroutine(viewToggle.StartViewSwitch(true));
        cockroachMove3D.transform.position = exitPoint.position;
        currentPairIndex = -1;
        DesObj();
    }

    private bool IsValidPair(int index)
    {
        return pairs != null && index >= 0 && index < pairs.Length;
    }

    public void DesObj()
    {
        foreach (GameObject obj in spawnedFood)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedFood.Clear();

        foreach (GameObject obj in spawnedSpider)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedSpider.Clear();
    }

    void SpawnRandomFoodOnPath(DoubleHolePair pair)
    {
        Vector2[] points = spawnArea.points;
        int spawnCount = 0;

        if (pair.selectedFoodAmount == DoubleHolePair.FoodAmount.少量)
        {
            spawnCount = 3;
        }
        else if (pair.selectedFoodAmount == DoubleHolePair.FoodAmount.大量)
        {
            spawnCount = 5;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            int segmentIndex = Random.Range(0, points.Length - 1);
            Vector2 worldStart = spawnArea.transform.TransformPoint(points[segmentIndex]);
            Vector2 worldEnd = spawnArea.transform.TransformPoint(points[segmentIndex + 1]);
            float t = Random.Range(0f, 1f);
            Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);
            spawnPos2D.y += pair.spawnOffsetY;

            Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);
            GameObject newObj = Instantiate(food, spawnPos3D, Quaternion.identity);
            spawnedFood.Add(newObj);
        }
    }

    void SpawnRandomSpiderOnPath(DoubleHolePair pair)
    {
        if (spider == null) return;

        EdgeCollider2D spiderArea = null;

        // 依照玩家進洞方向決定生成區域
        if (Scene2DManager.Instance != null)
        {
            var sceneData = Scene2DManager.Instance.GetScene(pair.selectedScene);
            if (sceneData != null)
            {
                if (lastEnterSide == HoleSide.Left)
                    spiderArea = sceneData.spawnSpiderAreaL;
                else
                    spiderArea = sceneData.spawnSpiderAreaR;
            }
        }

        if (spiderArea == null) return;

        Vector2[] points = spiderArea.points;

        // 固定生成 1 隻
        int segmentIndex = Random.Range(0, points.Length - 1);
        Vector2 worldStart = spiderArea.transform.TransformPoint(points[segmentIndex]);
        Vector2 worldEnd = spiderArea.transform.TransformPoint(points[segmentIndex + 1]);
        float t = Random.Range(0f, 1f);
        Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);
        spawnPos2D.y += pair.spawnOffsetY;

        Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);
        GameObject newObj = Instantiate(spider, spawnPos3D, Quaternion.identity);
        spawnedSpider.Add(newObj);

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (pairs == null) return;

        for (int i = 0; i < pairs.Length; i++)
        {
            var p = pairs[i];
            if (p == null) continue;

            if (p.leftHoleTrigger3D != null && p.leftHoleExit3D != null)
            {
                Gizmos.DrawLine(p.leftHoleTrigger3D.bounds.center, p.leftHoleExit3D.position);
            }
            if (p.rightHoleTrigger3D != null && p.rightHoleExit3D != null)
            {
                Gizmos.DrawLine(p.rightHoleTrigger3D.bounds.center, p.rightHoleExit3D.position);
            }
           
        }
    }
#endif
}

