using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Header("時代切換設定")]
    public float eraInterval = 20f; // 玩家可設定的時間 a 秒
    public EraMode mode = EraMode.依序輪替;

    [Header("史前時代")]
    public int PEFood = 10;
    public float hungerDuration = 12f;

    [Header("恐龍時代")]
    public int DEFood = 5;

    //[Header("大滅絕時代")]

}

public class EraManager : MonoBehaviour
{
    public EraValue eraValue;

    [Header("時代")]
    public static Era currentEra;
    private int currentIndex = 0; // 依序輪替用
    [HideInInspector] public Era[] eras;
    [HideInInspector] public Coroutine eraCoroutine;

    [Header("恐龍時代變數")]
    public GameObject[] dynaSpawnPt;
    public GameObject dynaPrefab;

    private List<GameObject> spawnedDynas = new List<GameObject>();


    [Header("引用腳本")]
    private FoodGenManger foodGenManger;
    private CockroachManager cockroachManager;

    private void Start()
    {
        foodGenManger = GameObject.Find("FoodGenManager").GetComponent<FoodGenManger>();
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        eras = (Era[])System.Enum.GetValues(typeof(Era));
        currentEra = eras[0];

        // 開始定時切換
        eraCoroutine = StartCoroutine(EraRoutine());
    }

    private IEnumerator EraRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(eraValue.eraInterval);

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
        spawnDyna();
    }

    public void spawnDyna()
    {
        for (int i = 0; i < dynaSpawnPt.Length; i++)
        {
            GameObject newDyna = Instantiate(dynaPrefab, dynaSpawnPt[i].transform.position, Quaternion.identity);
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

   

}
