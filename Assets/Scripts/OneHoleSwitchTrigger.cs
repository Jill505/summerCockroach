using System.Collections.Generic;
using UnityEngine;

public class OneHoleSwitchTrigger : MonoBehaviour
{
    [Header("控制腳本")]
    private CameraViewToggle viewToggle;
    public CameraLogic2D cameraLogic2D;
    private CockroachMove cockroachMove3D;
    private Cockroach2DMove cockroachMove2D;

    [Header("大便生成設定")]
    public bool enableSpawn = false;                 // 是否啟用生成
    public GameObject shit;                     
    
    public int minCount = 1;
    public int maxCount = 3;
    private float spawnOffsetY = 0.5f;
    
    [Header("蜘蛛生成設定")]
    public bool enableSpider = false;                // 是否顯示蜘蛛
    public GameObject spiderObject;                  // 被隱藏的蜘蛛物件    
    public int SpiderMinCount = 1;
    public int SpiderMaxCount = 3;

    [Header("傳送位置")]
    public Transform StartPos2D;
    public Transform StartPos3D;

    [Header("攝影機限制範圍")]
    public BoxCollider2D cameraBounds;
    public EdgeCollider2D spawnArea;                 // 生成範圍

    private bool isInTheTrigger = false;
    private List<GameObject> spawnedShit = new List<GameObject>(); 
    private List<GameObject> spawnedSpider = new List<GameObject>(); 

    private void Start()
    {
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        cockroachMove3D = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        cockroachMove2D = GameObject.Find("2DCockroach").GetComponent<Cockroach2DMove>();
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
            if (enableSpawn)
            {
                SpawnRandomShitOnPath();
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
}






