using System.Collections;
using UnityEngine;

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

public class EraManager : MonoBehaviour
{
    [Header("時代切換設定")]
    public float eraInterval = 5f; // 玩家可設定的時間 a 秒
    public EraMode mode = EraMode.依序輪替;

    public static Era currentEra;

    private int currentIndex = 0; // 依序輪替用
    private Era[] eras;
    private Coroutine eraCoroutine;

    private void Start()
    {
        eras = (Era[])System.Enum.GetValues(typeof(Era));
        currentEra = eras[0];

        // 開始定時切換
        eraCoroutine = StartCoroutine(EraRoutine());
    }

    private void Update()
    {
        if (currentEra == Era.PrehistoricEra)
        {
            Debug.Log("現在是史前時代");
        }
        else if (currentEra == Era.DinosaurEra)
        {
            Debug.Log("現在是恐龍時代");
        }
        else if (currentEra == Era.MassExtinctionEra)
        {
            Debug.Log("現在是大滅絕時代");
        }
    }

    private IEnumerator EraRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(eraInterval);

            if (mode == EraMode.隨機輪替)
            {
                ChangeEraInternal(eras[Random.Range(0, eras.Length)]);
            }
            else if (mode == EraMode.依序輪替)
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

    // 公開給外部手動呼叫
    public void ChangeEra(Era newEra)
    {
        ChangeEraInternal(newEra);

        // 如果依序輪替，要更新 currentIndex
        if (mode == EraMode.依序輪替)
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

    // 內部切換方法
    private void ChangeEraInternal(Era newEra)
    {
        currentEra = newEra;
        Debug.Log("切換到：" + currentEra);
    }
}
