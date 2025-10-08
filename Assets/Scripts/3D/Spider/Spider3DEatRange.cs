using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Spider3DEatRange : MonoBehaviour
{
    public Animator animControl;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[RedSpiderAI] ���a�i�J�����d��");

            // ����a���ʡ]���]���a�}���� DelayedStop Coroutine�^
            CockroachMove playerMovement = other.GetComponent<CockroachMove>();
            if (playerMovement != null)
            {
                playerMovement.StartCoroutine(playerMovement.DelayedStop(0f));
            }

            // �N���a�ܳz��
            Renderer[] renderers = other.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                Color c = r.material.color;
                c.a = 0.3f;  // �b�z���A�i�ۦ�վ�
                r.material.color = c;
                r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                r.material.SetInt("_ZWrite", 0);
                r.material.DisableKeyword("_ALPHATEST_ON");
                r.material.EnableKeyword("_ALPHABLEND_ON");
                r.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                r.material.renderQueue = 3000;
            }

            // ����j��r���a�ʵe
            animControl.SetBool("Eating", true);

            // ���N����
            AllGameManager AGM = FindAnyObjectByType<AllGameManager>();
            if (AGM != null)
            {
                AGM.InRoundKilledBySpider++;
            }
        }
        else if (other.CompareTag("NPCRoach"))
        {
            Debug.Log("[RedSpiderAI] NPC�����i�J�����d��A�ߧY���`");
            NPCRoach npc = other.GetComponent<NPCRoach>();
            if (npc != null)
            {
                npc.DynDestroy();
            }
        }
    }

}
