using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Spider3DEatRange : MonoBehaviour
{
    public Animator animControl;
    public GameObject spiderObject;
    public GameObject beEatnPlayer;
    private GameObject player;
    private CockroachManager cockroachManager;

    private void Start()
    {
        player = GameObject.Find("3DCockroach");
        cockroachManager = player.GetComponent<CockroachManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
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
        if (other.CompareTag("Player"))
        {
            Debug.Log("[RedSpiderAI] 玩家進入捕食範圍");

            // 停止玩家移動
            CockroachMove playerMovement = other.GetComponent<CockroachMove>();
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
            }

            // 停止蜘蛛移動
            RedSpiderAI spiderAI = spiderObject.GetComponent<RedSpiderAI>();
            spiderAI.SetCanSpiderMove(false);

            // 將玩家變透明
            SkinnedMeshRenderer[] skinnedRenderers = other.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in skinnedRenderers)
            {
                smr.enabled = false;
            }

            beEatnPlayer.SetActive(true);

            // 播放蜘蛛咬玩家動畫(結尾播放音效，恢復兩者移動，扣血)
            animControl.speed = 1f;
            animControl.SetBool("Eating", true);
            StartCoroutine(eatUp());

            // 成就紀錄
            AllGameManager AGM = FindAnyObjectByType<AllGameManager>();
            if (AGM != null)
            {
                AGM.InRoundKilledBySpider++;
            }
        }
        else if (other.CompareTag("NPCRoach"))
        {
            Debug.Log("[RedSpiderAI] NPC蟑螂進入捕食範圍，立即死亡");
            NPCRoach npc = other.GetComponent<NPCRoach>();
            if (npc != null)
            {
                npc.DynDestroy();
            }
        }
    }
    private IEnumerator eatUp()
    {
        yield return new WaitForSeconds(4f);
        SpiderEat();
    }
    // 播放蜘蛛咬玩家動畫(結尾播放音效，恢復兩者移動，扣血)
    public void SpiderEat()
    {
        SoundManager.Play("SFX_SpiderCrunchy-bite");
        cockroachManager.CockroachInjury(2, "這一世，我被蜘蛛殺死了");

        CockroachMove playerMovement = player.GetComponent<CockroachMove>();
        playerMovement.SetCanMove(true);

        SkinnedMeshRenderer[] skinnedRenderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            smr.enabled = true;
        }

        RedSpiderAI spiderAI = spiderObject.GetComponent<RedSpiderAI>();
        spiderAI.SetCanSpiderMove(true);
        beEatnPlayer.SetActive(false);
    }

}
