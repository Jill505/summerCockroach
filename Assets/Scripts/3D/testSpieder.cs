using UnityEngine;

public class TestSpieder : MonoBehaviour
{
    public int damage = 1;

    public float damageCoolDown = 2.0f;
    public float countDown = 0f;
    private CockroachManager cManager;
    private CameraViewToggle viewToggle;

    public DamageType myDamageType;

    public string deadReason = "�o�@�@�A�ڳQ���j����";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cManager = FindFirstObjectByType<CockroachManager>();
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= 1 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // ���P�_�O���O���a
        {
            if (countDown <= 0 && !viewToggle.Is2D())
            {
                // ���a����
                cManager.CockroachInjury(damage, deadReason);
                cManager.shield = 0;
                countDown = damageCoolDown;

                switch (myDamageType)
                {
                    case DamageType.thorn:
                        SaveSystem.mySaveFile.KillByThornTimes++;
                        break;
                }
            }
        }
        if (collision.gameObject.CompareTag("NPCRoach"))
        {
            NPCRoach npc = collision.gameObject.GetComponent<NPCRoach>();
            if (npc != null)
                npc.DynDestroy();
        }

        if (collision.gameObject.CompareTag("Spider"))
            HandleSpiderHit(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
            HandleSpiderHit(other.gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (countDown <= 0 && !viewToggle.Is2D())
            {
                //cManager.CockroachDie();
                cManager.CockroachInjury(damage, deadReason);
                cManager.shield = 0;
                countDown = damageCoolDown;

                switch (myDamageType)
                {
                    case DamageType.thorn:
                        SaveSystem.mySaveFile.KillByThornTimes++;
                        break;
                }
            }
            else
            {
            }
        }

        if (other.CompareTag("NPCRoach"))
        {
            NPCRoach npc = other.GetComponent<NPCRoach>();
            if (npc != null)
                npc.DynDestroy();
        }
        if (other.gameObject.CompareTag("Spider"))
            HandleSpiderHit(other.gameObject);
    }
    private void HandleSpiderHit(GameObject obj)
    {
        // ���ե��� RedSpiderAI
        RedSpiderAI spider = obj.GetComponent<RedSpiderAI>();

        // �p�G�ۤv�S���A�A�h�������
        if (spider == null)
            spider = obj.GetComponentInParent<RedSpiderAI>();

        // �p�G������S���A�A�h�l�����
        if (spider == null)
            spider = obj.GetComponentInChildren<RedSpiderAI>();

        // �p�G���N�I�s MakeDestroy
        if (spider != null)
        {
            spider.MakeDestroy();
            return;
        }

        // �p�G�S����� RedSpiderAI�A�A��W�� "3DObj_Spider" ���l����
        Transform spiderObj = obj.transform.Find("3DObj_Spider");
        if (spiderObj != null)
        {
            RedSpiderAI spiderInChild = spiderObj.GetComponent<RedSpiderAI>();
            if (spiderInChild != null)
            {
                spiderInChild.MakeDestroy();
            }
        }
    }

}

public enum DamageType
{
    thorn,
    dinosaur,
    spider
}