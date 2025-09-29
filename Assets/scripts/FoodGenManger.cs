using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using TMPro;
using System;

public class FoodGenManger : MonoBehaviour
{
    [Header("Food Tracker")]
    public List<GameObject> FoodPos;
    public bool[] hasFoodSpawn;
    public List<GameObject> Foods;

    [Header("Ref Objects")]
    public GameObject FoodPrefab;


    [Header("Cal Variable")]
    public int GenFoodCount;

    // 獨立隨機器
    private System.Random rng;
    void Start()
    {
        hasFoodSpawn = new bool[FoodPos.Count];
        int seed = Guid.NewGuid().GetHashCode(); // 保證唯一
        rng = new System.Random(seed);
    }

    void Update()
    {
        while (GenFoodCount > 0)
        {
            RandomSpawnFood();
            GenFoodCount--;
        }
    }


    public void RandomSpawnFood()
    {
        //List all pos allow spawn
        //Debug.Log(hasFoodSpawn.Length);
        List<int> readySpawnPosSort = new List<int>();
        for (int i = 0; i < FoodPos.Count; i++)
        {
            if (hasFoodSpawn[i] == false)
            {
                //add into pull list
                readySpawnPosSort.Add(i);
            }
        }

        if (readySpawnPosSort.Count > 0)
        {
            //Do ran spawn
            int ranIndex = rng.Next(0, readySpawnPosSort.Count);
            GameObject obj = Instantiate(FoodPrefab, FoodPos[readySpawnPosSort[ranIndex]].transform.position, Quaternion.identity);
            hasFoodSpawn[readySpawnPosSort[ranIndex]] = true;
            //Debug.Log(FoodPos[readySpawnPosSort[ranIndex]]);
            //Debug.Log(FoodPos[readySpawnPosSort[ranIndex]].transform.position);
            obj.transform.GetChild(0).gameObject.GetComponent<FoodTrigger>().mySort = readySpawnPosSort[ranIndex];

        }
        else
        {
            Debug.LogWarning("ak error: 未成功生成，檢查容器大小");
        }
    }

    public void SetGenFoodCount(int count)
    {
        if (count > 0)
        {
            GenFoodCount += count;
        }
        else
        {
            Debug.LogWarning("SetGenFoodCount: 輸入值必須大於 0");
        }
    }

    public void ClearAllFoods()
    {
        // 刪除場景中所有食物物件
        foreach (GameObject food in Foods)
        {
            if (food != null)
            {
                Destroy(food);
            }
        }

        // 清空清單
        Foods.Clear();

        // 重置所有生成點狀態
        for (int i = 0; i < hasFoodSpawn.Length; i++)
        {
            hasFoodSpawn[i] = false;
        }

        Debug.Log("所有食物已清除");
    }
}
