using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public enum Era
{
    PrehistoricEra,  //史前時代
    DinosaurEra,    //恐龍時代
    MassExtinctionEra //大滅絕時代
}

public enum EraMode
{
    隨機輪替,
    依序輪替
}

[System.Serializable]
public class EraValue
{
    [Header("隨機輪替間隔設定")]
    public float eraInterval = 20f; // 玩家可設定的時間 a 秒

    [Header("依序輪替間隔設定")]
    public float intervalPEToDE = 20f; // 史前 -> 恐龍
    public float intervalDEToME = 30f; // 恐龍 -> 大滅絕
    public float intervalMEToPE = 25f; // 大滅絕 -> 史前（可選）

    public EraMode mode = EraMode.依序輪替;

    [Header("PE")]
    public int PEFood = 10;
    public float hungerDuration = 12f;

    [Header("DE")]
    public int DEFood = 5;
    public GameObject[] dynaSpawnPt;

    [Header("ME")]
    public int hitPlayerChanceVar = 4;      //砸向玩家的機會
    public float spawnMeteoriteDur = 6f;    //每隔幾秒生成隕石
    public float hotMaxTime = 5f;
}

public class EraManager : MonoBehaviour
{
    public EraValue eraValue;

    [Header("時代")]
    public static Era currentEra;
    private int currentIndex = 0; // 依序輪替用
    [HideInInspector] public Era[] eras;
    [HideInInspector] public Coroutine eraCoroutine;
    private Coroutine meColorCoroutine;
    private Coroutine meMonitorCoroutine;

    [Header("恐龍時代變數")]
   
    public GameObject dynaPrefab;
    private List<GameObject> spawnedDynas = new List<GameObject>();

    [Header("大滅絕時代變數")]
    MeteoriteManager meteoriteManager;
    public GameObject hotSprite;          // 指向UI物件
    private Image hotImage;               // hotSprite 的 Image
    private float StayInTime = 0f;     // 計時用



    [Header("引用腳本")]
    private FoodGenManger foodGenManger;
    private CockroachManager cockroachManager;
    private CameraViewToggle viewToggle;
    private AllGameManager allGameManager;

    [Header("食物生成間隔")]
    public int FoodGenNum = 7;
    public int FoodGenDur = 3;

    private void Start()
    {
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        foodGenManger = GameObject.Find("FoodGenManager").GetComponent<FoodGenManger>();
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        meteoriteManager = FindFirstObjectByType<MeteoriteManager>();
        eras = (Era[])System.Enum.GetValues(typeof(Era));
        currentEra = eras[0];

        // 開始定時切換
        eraCoroutine = StartCoroutine(EraRoutine());
        StartCoroutine(cycleCall());

        if (hotSprite != null)
        {
            hotImage = hotSprite.GetComponent<Image>();
            hotSprite.SetActive(false);
        }
    }

    private IEnumerator EraRoutine()
    {
        while (true)
        {
            while (!allGameManager.isTimerRunning)
                yield return null;
            float waitTime = eraValue.eraInterval; // 預設用隨機輪替的時間

            if (eraValue.mode == EraMode.依序輪替)
            {
                if (currentEra == Era.PrehistoricEra)
                {
                    waitTime = eraValue.intervalPEToDE;
                }
                else if (currentEra == Era.DinosaurEra)
                {
                    waitTime = eraValue.intervalDEToME;
                }
                else if (currentEra == Era.MassExtinctionEra)
                {
                    waitTime = eraValue.intervalMEToPE;
                }
            }

            float timer = 0f;
            while (timer < waitTime)
            {
                while (!allGameManager.isTimerRunning)
                    yield return null;

                timer += Time.deltaTime;
                yield return null;
            }

            if (eraValue.mode == EraMode.隨機輪替)
            {
                ChangeEraInternal(eras[Random.Range(0, eras.Length)]);
            }
            else if (eraValue.mode == EraMode.依序輪替)
            {
                currentIndex++;
                if (currentIndex >= eras.Length)
                {
                    currentIndex = 0;
                }
                ChangeEraInternal(eras[currentIndex]);
            }
        }
    }

    // 內部切換方法
    private void ChangeEraInternal(Era newEra)
    {
        ClearEraObjects();
        currentEra = newEra;
        Debug.Log("切換到：" + currentEra);

        EraLogic();
    }

    void EraLogic()
    {
        switch (currentEra)
        {
            case Era.PrehistoricEra:
                PEEvent();
                break;
            case Era.DinosaurEra:
                DEEvent();
                break;
            case Era.MassExtinctionEra:
                MEEvent();
                break;
        }
    }

    public void CycleSpawnFood()
    {
        foodGenManger.SetGenFoodCount(FoodGenNum);
    }
    IEnumerator cycleCall()
    {
        CycleSpawnFood();
        yield return new WaitForSeconds(FoodGenDur);
        StartCoroutine (cycleCall());
    }

    void PEEvent()
    {
        foodGenManger.SetGenFoodCount(eraValue.PEFood);
        cockroachManager.SetHungerDuration(eraValue.hungerDuration);
    }

    void DEEvent()
    {
        foodGenManger.SetGenFoodCount(eraValue.DEFood);
        spawnDyna();
    }

    void MEEvent()
    {
        cycleCallMeteorite();
        spawnDyna();

        // 開始持續監控視角
        if (meMonitorCoroutine != null) StopCoroutine(meMonitorCoroutine);
        meMonitorCoroutine = StartCoroutine(MEMonitorRoutine());
    }

    

    public void spawnDyna()
    {
        for (int i = 0; i < eraValue.dynaSpawnPt.Length; i++)
        {
            GameObject newDyna = Instantiate(dynaPrefab, eraValue.dynaSpawnPt[i].transform.position, Quaternion.identity);
            spawnedDynas.Add(newDyna); 
        }
    }

    public void ClearAllDyna()
    {
        if (spawnedDynas.Count > 0)
        {
            foreach (GameObject dyna in spawnedDynas)
            {
                if (dyna != null)
                {
                    Destroy(dyna);
                }
            }
            spawnedDynas.Clear(); // 清空追蹤列表
            Debug.Log("已清除所有恐龍腳");
        }
        else
        {
            Debug.Log("目前沒有恐龍腳可清除");
        }
    }

    public void ClearEraObjects()
    {
        // 清除恐龍腳
        ClearAllDyna();

        // 清除食物
        foodGenManger.ClearAllFoods();

        // 重設飢餓值
        cockroachManager.SetHungerDuration(cockroachManager.hungerDuration);

        ResetME();
    }


    IEnumerator MEMonitorRoutine()
    {
        while (currentEra == Era.MassExtinctionEra)
        {
            while (!allGameManager.isTimerRunning)
            yield return null;
            if (viewToggle.Is2D())
            {
                hotSprite.SetActive(true);

                // 開啟顏色/透明度 Coroutine（如果尚未開啟）
                if (meColorCoroutine == null)
                    meColorCoroutine = StartCoroutine(MEColorRoutine());

                if (StayInTime >= eraValue.hotMaxTime)
                {
                    cockroachManager.CockroachDie();
                    StayInTime = 0f; // 可選：死亡後重置時間
                }
            }
            else
            {
                ResetME();
            }


            yield return null; // 每幀偵測
        }

        // 離開大滅絕時代後，確保重置
        ResetME();
    }

    IEnumerator MEColorRoutine()
    {
        float duration = eraValue.hotMaxTime;
        StayInTime = 0f;

        while (StayInTime < duration)
        {
            StayInTime += Time.deltaTime;
            float t = StayInTime / duration;

            if (hotImage != null)
            {
                Color c = hotImage.color;
                // 顏色從白到紅
                c.g = Mathf.Lerp(1f, 0f, t);
                c.b = Mathf.Lerp(1f, 0f, t);
                // 透明度從 0 到 30/255
                c.a = Mathf.Lerp(0f, 30f / 255f, t);

                hotImage.color = c;
            }
            yield return null;
        }

        meColorCoroutine = null; // Coroutine 結束後清空追蹤
    }

    public void ResetME()
    {
        StayInTime = 0f;

        if (hotImage != null)
        {
            Color c = hotImage.color;
            c.g = 1f;
            c.b = 1f;
            c.a = 0f;
            hotImage.color = c;
        }

        if (hotSprite != null)
            hotSprite.SetActive(false);

        if (meColorCoroutine != null)
        {
            StopCoroutine(meColorCoroutine);
            meColorCoroutine = null;
        }
    }



    // 公開給外部手動呼叫
    public void ChangeEra(Era newEra)
    {
        ChangeEraInternal(newEra);

        // 如果依序輪替，要更新 currentIndex
        if (eraValue.mode == EraMode.依序輪替)
        {
            for (int i = 0; i < eras.Length; i++)
            {
                if (eras[i] == newEra)
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        // 重置定時器
        if (eraCoroutine != null)
        {
            StopCoroutine(eraCoroutine);
        }
        eraCoroutine = StartCoroutine(EraRoutine());
    }

   public void cycleCallMeteorite()
    {
        if (currentEra != Era.MassExtinctionEra) return;
        int hitPlayerChance = Random.Range(0, eraValue.hitPlayerChanceVar);
        bool isAim = false;
        if (hitPlayerChance == 0) isAim= true;
        meteoriteManager.SpawnMeteorite(isAim);

        Invoke("cycleCallMeteorite", eraValue.spawnMeteoriteDur);
    }

}
