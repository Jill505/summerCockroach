using UnityEngine;

public class KYSTool : MonoBehaviour
{

    [Header("KYS Setting")]
    public float kysSec = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,kysSec);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
