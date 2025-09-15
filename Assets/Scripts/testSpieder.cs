using UnityEngine;

public class testSpieder : MonoBehaviour
{
    public int damage = 1;

    public float damageCoolDown = 2.0f;
    public float countDown = 0f;
    public CockroachManager cManager;
    private CameraViewToggle viewToggle;
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
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (countDown <= 0 && !viewToggle.Is2D())
            {
                //cManager.CockroachDie();
                cManager.CockroachInjury(1);
                cManager.shield = 0;
                countDown = damageCoolDown;
            }
            else
            {
            }
        }

       if (other.CompareTag("NPCRoach"))
        {

        }
    }
}
