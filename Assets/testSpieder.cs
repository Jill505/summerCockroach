using UnityEngine;

public class testSpieder : MonoBehaviour
{
    public CockroachManager cManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cManager.CockroachInjury(1);
        }
    }
}
