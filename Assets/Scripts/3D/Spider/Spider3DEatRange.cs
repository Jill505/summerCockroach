using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Spider3DEatRange : MonoBehaviour
{
    public Animator animControl;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[RedSpiderAI] 玩家進入捕食範圍");

            // 停止玩家移動（假設玩家腳本有 DelayedStop Coroutine）
            CockroachMove playerMovement = other.GetComponent<CockroachMove>();
            if (playerMovement != null)
            {
                playerMovement.StartCoroutine(playerMovement.DelayedStop(0f));
            }

            // 將玩家變透明
            Renderer[] renderers = other.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                Color c = r.material.color;
                c.a = 0.3f;  // 半透明，可自行調整
                r.material.color = c;
                r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                r.material.SetInt("_ZWrite", 0);
                r.material.DisableKeyword("_ALPHATEST_ON");
                r.material.EnableKeyword("_ALPHABLEND_ON");
                r.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                r.material.renderQueue = 3000;
            }

            // 播放蜘蛛咬玩家動畫
            animControl.SetBool("Eating", true);

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

}
