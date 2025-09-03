using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using TMPro;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasFoodSpawn = new bool[FoodPos.Count];
    }

    // Update is called once per frame
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
            int ranIndex= Random.Range(0, readySpawnPosSort.Count);
            //Debug.Log(FoodPos[readySpawnPosSort[ranIndex]]);
            //Debug.Log(FoodPos[readySpawnPosSort[ranIndex]].transform.position);
            GameObject obj =Instantiate(FoodPrefab, FoodPos[readySpawnPosSort[ranIndex]].transform.position, Quaternion.identity);
            hasFoodSpawn[ranIndex] = true;
            obj.transform.GetChild(0).gameObject.GetComponent<FoodTrigger>().mySort = readySpawnPosSort[ranIndex];

        }
        else
        {
            Debug.LogWarning("ak error: 未成功生成，檢查容器大小");
        }
    }
}
