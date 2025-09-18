using System.Collections.Generic;
using Unity.VisualScripting;
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
    private static Transform StartPos3D;

    [Header("食物生成設定")]
    public bool enableFood = false;                 // 是否啟用生成
    private GameObject food;                     
    
    public int minCount = 1;
    public int maxCount = 3;
    private float spawnOffsetY = 0.5f;
    
    [Header("攝影機限制範圍")]
    private BoxCollider2D cameraBounds;
    private EdgeCollider2D spawnArea;                 // 生成範圍

    private bool isInTheTrigger = false;
    private List<GameObject> spawnedFood = new List<GameObject>(); 


    [Header("Scene")]
    private Scene2DOneHole selectedScene; // 在 Inspector 用下拉選


    private void Start()
    {
        food = GameObject.Find("AllGameManager").GetComponent<Scene2DManager>().Food2D;
        selectedScene = Scene2DOneHole.TreeHole;
        //int rand = Random.Range(0, 2); // 0 或 1
        //if (rand == 0)
        //    selectedScene = Scene2DOneHole.HalfTreeHole01;
        //else if(rand == 1)
        //    selectedScene = Scene2DOneHole.HalfTreeHole02;
        //else
        //    selectedScene = Scene2DOneHole.TreeHole;

        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        cockroachMove3D = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        cockroachMove2D = GameObject.Find("2DCockroach").GetComponent<Cockroach2DMove>();

        if (Scene2DManager.Instance != null)
        {
            var sceneData = Scene2DManager.Instance.GetScene(selectedScene);
            if (sceneData != null)
            {
                cameraBounds = sceneData.cameraBounds;
                spawnArea = sceneData.spawnBounds;
                StartPos2D = sceneData.insPos1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SoundManager.Play("Transition - Sound Effects");
        SwitchTo3D();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !viewToggle.Is2D())
        {
            SoundManager.Play("Transition - Sound Effects");
            DesObj();
            isInTheTrigger = true;
            cameraLogic2D.SetCustomBounds(cameraBounds.bounds);
            StartPos3D = transform.GetChild(0);
            StartCoroutine(viewToggle.StartViewSwitch(false)); //切換到2D

            if (enableFood)
            {
                SpawnRandomFoodOnPath();
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

    public void SwitchTo3D()
    {
        if (viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.StartViewSwitch(true)); // 切換到3D
            cockroachMove3D.transform.position = StartPos3D.position;
        }
    }

    void DesObj()
    {
        foreach (GameObject obj in spawnedFood)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedFood.Clear();
    }

    void SpawnRandomFoodOnPath()
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

            GameObject newObj = Instantiate(food, spawnPos3D, Quaternion.identity);
            spawnedFood.Add(newObj);
        }
    }
}






