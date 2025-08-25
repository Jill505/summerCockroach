using System.Collections.Generic;
using UnityEngine;
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

    [Header("大便生成設定")]
    public bool enableSpawn = false;                 // 是否啟用生成
    public GameObject shit;

    public int minCount = 1;
    public int maxCount = 3;
    [HideInInspector]public float spawnOffsetY = 0.5f;

    [Header("蜘蛛生成設定")]
    public bool enableSpider = false;                // 是否顯示蜘蛛
    public GameObject spiderObject;                  // 被隱藏的蜘蛛物件    
    public int SpiderMinCount = 1;
    public int SpiderMaxCount = 3;

    [Header("母蟑螂生成設定")]
    public bool enableFemaleCockroach = false;
    public GameObject femaleCockroach;

    public SpawnMode spawnMode;
    public enum SpawnMode
    {
        Random,
        Select
    }    

    public enum SelectedScene
    {
        石洞,
        新石洞
    }
    [Header("Scene")]
    public SelectedScene selectedScene; // 在 Inspector 用下拉選
    [HideInInspector]public string selectedSceneName;    // 顯示/保存字串名稱

    // 初始化方法：把 enum 轉成字串
    public void InitSelectedScene()
    {
        selectedSceneName = selectedScene.ToString();
    }

    [Header("Information")]
    public string cockroachName;
    public string Disc;
    [HideInInspector]public bool finded;
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


    [Header("RandomPos")]
    private Transform position1;
    private Transform position2;
    private Transform position3;

    [Header("SelectPos")]
    private Transform selectedPosition;

    [Header("攝影機限制範圍")]
    private BoxCollider2D cameraBounds;
    private EdgeCollider2D spawnArea;                 // 生成範圍

    private bool isInTheTrigger = false;
    private List<GameObject> spawnedShit = new List<GameObject>();
    private List<GameObject> spawnedSpider = new List<GameObject>();
    private List<GameObject> spawnedFemCockroach = new List<GameObject>();




    private void Awake()
    {
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

    void Start()
    {
        foreach (var pair in pairs)
        {
            if (pair.enableFemaleCockroach)
            {
                EnableFemaleForPair(pair);
            }
        }
    }

    void EnableFemaleForPair(DoubleHolePair pair)
    {
        if (pair.leftHoleTrigger3D != null)
        {
            var femaleInfo = pair.leftHoleTrigger3D.GetComponent<FemaleCockroachInfo>();
            if (femaleInfo != null)
            {
                femaleInfo.enabled = true; // 啟用腳本
                femaleInfo.cockroachName = pair.cockroachName;
                femaleInfo.Disc = pair.Disc;
            }
        }

        if (pair.rightHoleTrigger3D != null)
        {
            var femaleInfo = pair.rightHoleTrigger3D.GetComponent<FemaleCockroachInfo>();
            if (femaleInfo != null)
            {
                femaleInfo.enabled = true; // 啟用腳本
                femaleInfo.cockroachName = pair.cockroachName;
                femaleInfo.Disc = pair.Disc;
            }
        }
    }



    // —— 3D → 2D：外部洞口觸發時呼叫 ——
    public void EnterFrom3D(int pairIndex, HoleSide side)
    {
        if (!IsValidPair(pairIndex)) return;
        if (cockroachMove2D == null) return;
        if (viewToggle.Is2D()) return;

        // 取得對應 Pair 的場景名稱
        var pair = pairs[pairIndex];
        if (pair == null) return;

        if (Scene2DManager.Instance != null)
        {
            var sceneData = Scene2DManager.Instance.GetSceneByName(pair.selectedSceneName);
            if (sceneData != null)
            {
                Debug.Log("套用場景：" + sceneData.sceneName);
                cameraBounds = sceneData.cameraBounds;
                spawnArea = sceneData.spawnBounds;
                position1 = sceneData.randomMotherCockroachRange1;
                position2 = sceneData.randomMotherCockroachRange2;
                position3 = sceneData.randomMotherCockroachRange3;
                selectedPosition = sceneData.motherCockroachPoints;
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

        if (pair.enableSpawn)
            SpawnRandomShitOnPath(pair); // 傳入該 Pair 的資料

        if (pair.enableSpider)
            SpawnRandomSpiderOnPath(pair);

        if (pair.enableFemaleCockroach &&  pair.finded == false)
            SpawnFemaleCockroach(pair);
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
    }

    private bool IsValidPair(int index)
    {
        return pairs != null && index >= 0 && index < pairs.Length;
    }

    void DesObj()
    {
        foreach (GameObject obj in spawnedShit)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedShit.Clear();

        foreach (GameObject obj in spawnedSpider)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedSpider.Clear();

        foreach (GameObject obj in spawnedFemCockroach)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedFemCockroach.Clear();
    }

    void SpawnRandomShitOnPath(DoubleHolePair pair)
    {
        Vector2[] points = spawnArea.points;
        int spawnCount = Random.Range(pair.minCount, pair.maxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int segmentIndex = Random.Range(0, points.Length - 1);
            Vector2 worldStart = spawnArea.transform.TransformPoint(points[segmentIndex]);
            Vector2 worldEnd = spawnArea.transform.TransformPoint(points[segmentIndex + 1]);
            float t = Random.Range(0f, 1f);
            Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);
            spawnPos2D.y += pair.spawnOffsetY;

            Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);
            GameObject newObj = Instantiate(pair.shit, spawnPos3D, Quaternion.identity);
            spawnedShit.Add(newObj);
        }
    }

    void SpawnRandomSpiderOnPath(DoubleHolePair pair)
    {
        Vector2[] points = spawnArea.points;
        int spawnCount = Random.Range(pair.SpiderMinCount, pair.SpiderMaxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int segmentIndex = Random.Range(0, points.Length - 1);
            Vector2 worldStart = spawnArea.transform.TransformPoint(points[segmentIndex]);
            Vector2 worldEnd = spawnArea.transform.TransformPoint(points[segmentIndex + 1]);
            float t = Random.Range(0f, 1f);
            Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);
            spawnPos2D.y += pair.spawnOffsetY;

            Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);
            GameObject newObj = Instantiate(pair.spiderObject, spawnPos3D, Quaternion.identity);
            spawnedSpider.Add(newObj);

            var spiderHurt = newObj.GetComponent<SpiderHurtPlayer>();
            if (spiderHurt != null) spiderHurt.ResetHurt();
        }
    }

    void SpawnFemaleCockroach(DoubleHolePair pair)
    {
        Transform spawnPoint = null;

        if (pair.spawnMode == DoubleHolePair.SpawnMode.Random)
        {
            Transform[] points = { position1, position2, position3 };
            int index = Random.Range(0, points.Length);
            spawnPoint = points[index];
        }
        else if (pair.spawnMode == DoubleHolePair.SpawnMode.Select)
        {
            spawnPoint = selectedPosition;
        }

        if (spawnPoint != null && pair.femaleCockroach != null)
        {
            GameObject FemaleCockroach2D = Instantiate(pair.femaleCockroach, spawnPoint.position, spawnPoint.rotation);
            spawnedFemCockroach.Add(FemaleCockroach2D);
            FemaleCockroachInfo2D CockroachFemaleInfo2D = FemaleCockroach2D.GetComponent<FemaleCockroachInfo2D>();
            CockroachFemaleInfo2D.cockroachName = pair.cockroachName;
            CockroachFemaleInfo2D.Disc = pair.Disc;
            CockroachFemaleInfo2D.generatorScript2 = pair;
        }
        else
        {
            Debug.LogWarning("Spawn 失敗：Prefab 或生成位置未設置");
        }
            
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

