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
    public bool playerbeEaten = false;

    private void Start()
    {
        player = GameObject.Find("3DCockroach");
        cockroachManager = player.GetComponent<CockroachManager>();
    }
    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player") && cockroachManager.EatingBySpider == false)
        {
            Debug.Log("[RedSpiderAI] ���a�i�J�����d��");
            cockroachManager.EatingBySpider = true;
            playerbeEaten = true;

            // ����a����
            CockroachMove playerMovement = other.GetComponent<CockroachMove>();
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
            }

            // ����j�ﲾ��
            RedSpiderAI spiderAI = spiderObject.GetComponent<RedSpiderAI>();
            spiderAI.SetCanSpiderMove(false);

            // �N���a�ܳz��
            SkinnedMeshRenderer[] skinnedRenderers = other.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in skinnedRenderers)
            {
                smr.enabled = false;
            }

            beEatnPlayer.SetActive(true);

            // ����j��r���a�ʵe(�������񭵮ġA��_��̲��ʡA����)
            animControl.speed = 1f;
            animControl.SetBool("Eating", true);
            StartCoroutine(eatUp());

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
    private IEnumerator eatUp()
    {
        yield return new WaitForSeconds(3f);
        SpiderEat();
    }
    // ����j��r���a�ʵe(�������񭵮ġA��_��̲��ʡA����)
    public void SpiderEat()
    {
        SoundManager.Play("SFX_SpiderCrunchy-bite");
        cockroachManager.CockroachInjury(2, "�o�@�@�A�ڳQ�j������F");

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
