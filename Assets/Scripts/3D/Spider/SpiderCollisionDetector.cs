using UnityEngine;

public class SpiderCollisionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // �ˬd�O�_�� testSpieder �}��
        TestSpieder spiederScript = other.GetComponent<TestSpieder>();
        if (spiederScript != null)
        {
            // ���ը��o RedSpiderAI �éI�s MakeDestroy
            Debug.Log("2");
            RedSpiderAI spider = GetComponent<RedSpiderAI>();
            if (spider != null)
            {
                Debug.Log("3");
                Debug.Log("[SpiderCollisionDetector] �I�� testSpieder�A���� MakeDestroy()");
                spider.MakeDestroy();
            }
            else
            {
                Debug.LogWarning("[SpiderCollisionDetector] �䤣�� RedSpiderAI �ե�I");
            }
        }
    }

    // �Y�A�Q��ι���I���]�DTrigger�^�A�令�o�ˡG
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
