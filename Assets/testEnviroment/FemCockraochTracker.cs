using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FemCockraochTracker : MonoBehaviour
{
    public Text textShowcase;

    public testFemCockraoch[] sceneRoaches;
    public GameObject playerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SortRoachesByDistance();
        textShowcase.text = "";
        textShowcase.text += "離你最近的母蟑螂有" + Vector3.Distance(playerPos.transform.position, sceneRoaches[0].gameObject.transform.position)+ "公尺遠！\n";
        textShowcase.text += "蟑螂姓名：" + sceneRoaches[0].cockroachName + "\n";
        textShowcase.text += "蟑螂敘述：" + sceneRoaches[0].Disc;
    }


    void SortRoachesByDistance()
    {
        if (sceneRoaches == null || playerPos == null)
        {
            Debug.LogError("sceneRoaches or playerPos is not assigned.");
            return;
        }

        Vector3 playerPosition = playerPos.transform.position;

        // 過濾掉已被找到的蟑螂
        var filteredRoaches = sceneRoaches
            .Where(r => r != null && !r.finded)
            .OrderBy(r => (r.gameObject.transform.position - playerPosition).sqrMagnitude)
            .ToArray();

        // 將排序後的結果放回原陣列開頭，保留原陣列長度
        for (int i = 0; i < filteredRoaches.Length; i++)
        {
            sceneRoaches[i] = filteredRoaches[i];
        }

        Debug.Log("Roaches sorted by distance to player (excluding those already found).");
    }
}
