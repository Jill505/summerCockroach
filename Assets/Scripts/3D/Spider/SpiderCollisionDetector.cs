using UnityEngine;

public class SpiderCollisionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 檢查是否有 testSpieder 腳本
        TestSpieder spiederScript = other.GetComponent<TestSpieder>();
        if (spiederScript != null)
        {
            // 嘗試取得 RedSpiderAI 並呼叫 MakeDestroy
            Debug.Log("2");
            RedSpiderAI spider = GetComponent<RedSpiderAI>();
            if (spider != null)
            {
                Debug.Log("3");
                Debug.Log("[SpiderCollisionDetector] 碰到 testSpieder，執行 MakeDestroy()");
                spider.MakeDestroy();
            }
            else
            {
                Debug.LogWarning("[SpiderCollisionDetector] 找不到 RedSpiderAI 組件！");
            }
        }
    }

    // 若你想改用實體碰撞（非Trigger），改成這樣：
    private void OnCollisionEnter(Collision collision)
    {
        TestSpieder spiederScript = collision.gameObject.GetComponent<TestSpieder>();
        
        if (spiederScript != null)
        {
            RedSpiderAI spider = GetComponent<RedSpiderAI>();
            if (spider != null)
                spider.MakeDestroy();
        }
    }
}
