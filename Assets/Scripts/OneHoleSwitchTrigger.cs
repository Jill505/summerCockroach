using System.Collections.Generic;
using UnityEngine;

public class OneHoleSwitchTrigger : MonoBehaviour
{
    [Header("控制腳本")]
    private CameraViewToggle viewToggle;
    public CameraLogic2D cameraLogic2D;
    private CockroachMove cockroachMove3D;
    private Cockroach2DMove cockroachMove2D;

    [Header("傳送位置")]
    private Transform StartPos2D;
    public Transform StartPos3D;

    [Header("大便生成設定")]
    public bool enableShit = false;                 // 是否啟用生成
    public GameObject shit;                     
    
    public int minCount = 1;
    public int maxCount = 3;
    private float spawnOffsetY = 0.5f;
    
    [Header("蜘蛛生成設定")]
    public bool enableSpider = false;                // 是否顯示蜘蛛
    public GameObject spiderObject;                  // 被隱藏的蜘蛛物件    
    public int SpiderMinCount = 1;
    public int SpiderMaxCount = 3;

    

    [Header("攝影機限制範圍")]
    private BoxCollider2D cameraBounds;
    private EdgeCollider2D spawnArea;                 // 生成範圍

    private bool isInTheTrigger = false;
    private List<GameObject> spawnedShit = new List<GameObject>(); 
    private List<GameObject> spawnedSpider = new List<GameObject>();

    [Header("母蟑螂生成設定")]
    public bool enableFemaleCockroach = false;

    [Header("RandomPos")]
    private Transform position1;
    private Transform position2;
    private Transform position3;

    public enum SpawnMode
    {
        Random,
        Select
    }

    [Header("SpawnMode")]
    public SpawnMode spawnMode;

    [Header("SelectPos")]
    private Transform selectedPosition;

    [Header("Prefab")]
    public GameObject prefab;

    public enum SelectedScene
    {
        //樹洞,
        石洞,
        舊場景
    }
    [Header("Scene")]
    public SelectedScene selectedScene; // 在 Inspector 用下拉選
    private string selectedSceneName;    // 顯示/保存字串名稱

    private void Start()
    {
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        cockroachMove3D = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        cockroachMove2D = GameObject.Find("2DCockroach").GetComponent<Cockroach2DMove>();
        StartPos2D = GameObject.Find("StartPos2D").transform;

        selectedSceneName = selectedScene.ToString();

        if (Scene2DManager.Instance != null)
        {
            var sceneData = Scene2DManager.Instance.GetSceneByName(selectedSceneName);
            if (sceneData != null)
            {
                Debug.Log("套用場景：" + sceneData.sceneName);
                cameraBounds = sceneData.cameraBounds;
                spawnArea = sceneData.spawnBounds;
                position1 = sceneData.randomMotherCockroachRange1;
                position2 = sceneData.randomMotherCockroachRange2;
                position3 = sceneData.randomMotherCockroachRange3;
                selectedPosition = sceneData.motherCockroachPoints;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SwitchTo3DAndDesObj();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !viewToggle.Is2D())
        {
            isInTheTrigger = true;
            cameraLogic2D.SetCustomBounds(cameraBounds.bounds);
            StartCoroutine(viewToggle.StartViewSwitch(false)); //切換到2D

            // 顯示蜘蛛（如果有勾選）
            if (enableSpider)
            {
                SpawnRandomSpiderOnPath();
            }
            if (enableShit)
            {
                SpawnRandomShitOnPath();
            }
            if (enableFemaleCockroach)
            {
                SpawnFemaleCockroach();
            }
            cockroachMove2D.transform.position = StartPos2D.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInTheTrigger == true)
        {
            isInTheTrigger = false;
        }
    }

    public void SwitchTo3DAndDesObj()
    {
        if (viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.StartViewSwitch(true)); // 切換到3D
            cockroachMove3D.transform.position = StartPos3D.position;

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
        }
    }

    void SpawnRandomShitOnPath()
    {
        Vector2[] points = spawnArea.points;

        int spawnCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            // 1. 隨機選擇一段線
            int segmentIndex = Random.Range(0, points.Length - 1);

            // 2. 該段線的起點與終點（轉世界座標）
            Vector2 worldStart = spawnArea.transform.TransformPoint(points[segmentIndex]);
            Vector2 worldEnd = spawnArea.transform.TransformPoint(points[segmentIndex + 1]);

            // 3. 在這段線上隨機取一個位置
            float t = Random.Range(0f, 1f);
            Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);

            spawnPos2D.y += spawnOffsetY;

            // 5. 組合成 3D 座標（固定 Z 軸）
            Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);

            GameObject newObj = Instantiate(shit, spawnPos3D, Quaternion.identity);
            spawnedShit.Add(newObj);
        }
    }

    void SpawnRandomSpiderOnPath()
    {
        Vector2[] points = spawnArea.points;

        int spawnCount = Random.Range(SpiderMinCount, SpiderMaxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            // 1. 隨機選擇一段線
            int segmentIndex = Random.Range(0, points.Length - 1);

            // 2. 該段線的起點與終點（轉世界座標）
            Vector2 worldStart = spawnArea.transform.TransformPoint(points[segmentIndex]);
            Vector2 worldEnd = spawnArea.transform.TransformPoint(points[segmentIndex + 1]);

            // 3. 在這段線上隨機取一個位置
            float t = Random.Range(0f, 1f);
            Vector2 spawnPos2D = Vector2.Lerp(worldStart, worldEnd, t);

            spawnPos2D.y += spawnOffsetY;

            // 5. 組合成 3D 座標（固定 Z 軸）
            Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 303.8198f);

            GameObject newObj = Instantiate(spiderObject, spawnPos3D, Quaternion.identity);
            spawnedSpider.Add(newObj);
            SpiderHurtPlayer spiderHurt = newObj.GetComponent<SpiderHurtPlayer>();
            if (spiderHurt != null)
            {
                spiderHurt.ResetHurt();
            }
        }
    }
    public void SpawnFemaleCockroach()
    {
        Transform spawnPoint = null;

        if (spawnMode == SpawnMode.Random)
        {
            Transform[] points = { position1, position2, position3 };
            int index = Random.Range(0, points.Length);
            spawnPoint = points[index];
        }
        else if (spawnMode == SpawnMode.Select)
        {
            spawnPoint = selectedPosition;
        }

        if (spawnPoint != null && prefab != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Spawn 失敗：Prefab 或生成位置未設置");
        }
    }
}






