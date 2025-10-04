using UnityEngine;

public class FoodTrigger : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private CockroachManager cManager;
    [HideInInspector] public FoodGenManger foodGenManager;

    [Header("Com Ref")]
    public GameObject myFather;

    [Header("Track variable")]
    public int mySort;

    [Header("回血量")]
    public int healAmount = 55;

    [Header("成就4特別變數")]
    public static float eatDieCount;
    private void Start()
    {
        cManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        foodGenManager = Object.FindFirstObjectByType<FoodGenManger>();

        myFather = transform.parent != null ? transform.parent.gameObject : null;
        foodGenManager.Foods.Add(myFather);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            heal();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            heal();
        }
        if (other.CompareTag("NPCRoach"))
        {
            Destroy(gameObject);
        }
    }
    public void heal()
    {
        cManager.CockroachHealing(healAmount);

        Debug.Log("回血了，目前血量: " + cManager.Hp);

        AllGameManager AGM = FindAnyObjectByType<AllGameManager>();
        AGM.GO_unlockAchievement(2);

        eatDieCount = 1;
        SaveSystem.mySaveFile.FoodCollect++;

        OnDestroy();
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        foodGenManager.Foods.Remove(myFather);
        foodGenManager.hasFoodSpawn[mySort] = false;
    }

}
