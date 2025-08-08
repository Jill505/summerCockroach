using UnityEngine;

public class testSpieder : MonoBehaviour
{
    public int damage = 1;

    public float damageCoolDown = 2.0f;
    public float countDown = 0f;
    public CockroachManager cManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cManager = FindFirstObjectByType<CockroachManager>();
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
            if (countDown <= 0)
            {
                cManager.CockroachInjury(damage);
                countDown = damageCoolDown;
            }
            else
            {
            }
        }
    }
}
