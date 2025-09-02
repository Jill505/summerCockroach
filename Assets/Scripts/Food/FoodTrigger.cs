using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class FoodTrigger : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private CockroachManager cManager;

    [Header("回血量")]
    public int healAmount = 1;
    private void Start()
    {
        cManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cManager.CockroachHealing(healAmount);
            Debug.Log("回血了，目前血量: " + cManager.Hp);
            Destroy(gameObject); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cManager.CockroachHealing(healAmount);
            Debug.Log("回血了，目前血量: " + cManager.Hp);
            Destroy(gameObject);
        }
    }
}
