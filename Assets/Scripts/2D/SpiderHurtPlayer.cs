using UnityEngine;

public class SpiderHurtPlayer : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private CockroachManager cockroachManager;
    private bool hasHurt = false;

    private void Start()
    {
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHurt) return;

        if (other.CompareTag("Player"))
        {
            cockroachManager.CockroachInjury(1);
            Debug.Log("扣血了，目前血量: " + cockroachManager.Hp);
            hasHurt = true;
            Destroy(gameObject);
        }
    }

    public void ResetHurt()
    {
        hasHurt = false;
    }
}
