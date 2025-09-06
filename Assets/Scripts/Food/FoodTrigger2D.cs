using UnityEngine;

public class FoodTrigger2D : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private CockroachManager cManager;

    [Header("回血量")]
    public int healAmount = 55;
    private void Start()
    {
        cManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            heal();
        }

     }

    public void heal()
    {
        cManager.CockroachHealing(healAmount);
        Debug.Log("回血了，目前血量: " + cManager.Hp);
        Destroy(gameObject);
    }
}
