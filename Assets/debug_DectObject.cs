using UnityEngine;

public class debug_DectObject : MonoBehaviour
{
    public GameObject Father;
    public Vector3 Offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        transform.position = Father.transform.position - Offset;   
    }
}
