using System.Collections.Generic;
using UnityEngine;

public class DoubleHoleSwitchManager : MonoBehaviour
{
    [System.Serializable]
    public class HoleData
    {
        public Collider holeTrigger3D;    // 3D 洞口的碰撞器
        public Collider2D holeTrigger2D;  // 2D 洞口的碰撞器
        public Transform exitPoint3D;     // 3D 出口位置
        public Transform exitPoint2D;     // 2D 出口位置
    }

    [Header("左右洞口")]
    public HoleData leftHole;
    public HoleData rightHole;


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

    [Header("控制相機與角色的腳本")]
    private CameraViewToggle viewToggle;
    public CameraLogic2D cameraLogic2D;

    [Header("蟑螂控制腳本")]
    private CockroachMove cockroachMove3D;
    private Cockroach2DMove cockroachMove2D;
    private Transform Cockroach2DSprite;

    [Header("限制範圍")]
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
        Cockroach2DSprite = GameObject.Find("Cockroach2DSprite").GetComponent<Transform>();
        // 綁定事件
        if (leftHole.holeTrigger3D != null) leftHole.holeTrigger3D.gameObject.AddComponent<HoleTriggerBinder>().Setup(this, true, true);
        if (rightHole.holeTrigger3D != null) rightHole.holeTrigger3D.gameObject.AddComponent<HoleTriggerBinder>().Setup(this, false, true);
        if (leftHole.holeTrigger2D != null) leftHole.holeTrigger2D.gameObject.AddComponent<HoleTriggerBinder2D>().Setup(this, true, false);
        if (rightHole.holeTrigger2D != null) rightHole.holeTrigger2D.gameObject.AddComponent<HoleTriggerBinder2D>().Setup(this, false, false);
    }

    public void EnterHole(bool isLeft, bool from3D)
    {
        cameraLogic2D.SetCustomBounds(cameraBounds.bounds);
        if (from3D && !viewToggle.Is2D())
        {
            // 3D → 2D
            Vector3 targetPos;
            Vector3 scale2D = Cockroach2DSprite.transform.localScale;
            if (isLeft)
            {
                targetPos = leftHole.exitPoint2D.position;
                if (scale2D.x < 0)
                {
                    scale2D.x = -scale2D.x;
                }
            }
            else
            {
                targetPos = rightHole.exitPoint2D.position;
                if (scale2D.x >0)
                {
                    scale2D.x = -scale2D.x;
                }
            }
            StartCoroutine(viewToggle.StartViewSwitch(false)); // 切到 2D
            if (enableSpider)
            {
                SpawnRandomSpiderOnPath();
            }
            if (enableSpawn)
            {
                SpawnRandomShitOnPath();
            }
            // 傳送 2D 角色
            cockroachMove2D.transform.position = targetPos;
            Cockroach2DSprite.transform.localScale = scale2D;



            Debug.Log($"[傳送] 3D → 2D 從 {(isLeft ? "左" : "右")} 進，傳到 {targetPos}");
        }
        else if (!from3D && viewToggle.Is2D())
        {
            // 2D → 3D
            Vector3 targetPos;
            if (isLeft)
            {
                targetPos = leftHole.exitPoint3D.position;
            }
            else
            {
                targetPos = rightHole.exitPoint3D.position;
            }
            StartCoroutine(viewToggle.StartViewSwitch(true)); // 切到 3D
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
            // 傳送 3D 角色
            cockroachMove3D.transform.position = targetPos;
            Debug.Log($"[傳送] 2D → 3D 從 {(isLeft ? "左" : "右")} 出，傳到 {targetPos}");
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

    // 3D 觸發器
    public class HoleTriggerBinder : MonoBehaviour
    {
        private DoubleHoleSwitchManager manager;
        private bool isLeft;
        private bool from3D;

        public void Setup(DoubleHoleSwitchManager m, bool left, bool from3DWorld)
        {
            manager = m;
            isLeft = left;
            from3D = from3DWorld;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                manager.EnterHole(isLeft, from3D);
            }
        }
    }

    // 2D 觸發器
    public class HoleTriggerBinder2D : MonoBehaviour
    {
        private DoubleHoleSwitchManager manager;
        private bool isLeft;
        private bool from3D;

        public void Setup(DoubleHoleSwitchManager m, bool left, bool from3DWorld)
        {
            manager = m;
            isLeft = left;
            from3D = from3DWorld;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                manager.EnterHole(isLeft, from3D);
            }
        }
    }
}
