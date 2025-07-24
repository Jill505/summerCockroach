using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class testFood : MonoBehaviour
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
            cManager.CockroachHealing(1);
            Destroy(gameObject);
        }
    }
}
