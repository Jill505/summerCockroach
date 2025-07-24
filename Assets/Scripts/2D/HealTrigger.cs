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
            cockroachManager.Hp += healAmount;

            // 防止超過最大血量（假設最大血是3）
            if (cockroachManager.Hp > 3)
                cockroachManager.Hp = 3;

            Debug.Log("回血了，目前血量: " + cockroachManager.Hp);

            Destroy(gameObject); // 吃掉就消失
        }
    }
}
