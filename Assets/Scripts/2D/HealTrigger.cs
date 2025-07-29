using UnityEngine;

public class HealTrigger : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    public CockroachManager cockroachManager;

    [Header("回血量")]
    public int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cockroachManager.CockroachHealing(healAmount);

            Debug.Log("回血了，目前血量: " + cockroachManager.Hp);

            Destroy(gameObject); // 吃掉就消失
        }
    }
}
