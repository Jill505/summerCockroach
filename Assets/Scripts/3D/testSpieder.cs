using UnityEngine;

public class TestSpieder : MonoBehaviour
{
    public int damage = 1;

    public float damageCoolDown = 2.0f;
    public float countDown = 0f;
    private CockroachManager cManager;
    private CameraViewToggle viewToggle;

    public DamageType myDamageType;

    public string deadReason = "這一世，我被飢餓殺死";

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
        if (collision.gameObject.CompareTag("Player")) // 先判斷是不是玩家
        {
            if (countDown <= 0 && !viewToggle.Is2D())
            {
                // 玩家受傷
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
        // 嘗試先找 RedSpiderAI
        RedSpiderAI spider = obj.GetComponent<RedSpiderAI>();

        // 如果自己沒有，再去父物件找
        if (spider == null)
            spider = obj.GetComponentInParent<RedSpiderAI>();

        // 如果父物件沒有，再去子物件找
        if (spider == null)
            spider = obj.GetComponentInChildren<RedSpiderAI>();

        // 如果找到就呼叫 MakeDestroy
        if (spider != null)
        {
            spider.MakeDestroy();
            return;
        }

        // 如果沒有找到 RedSpiderAI，再找名為 "3DObj_Spider" 的子物件
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