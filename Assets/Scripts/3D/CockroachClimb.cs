using UnityEngine;

public class CockroachClimb : MonoBehaviour
{
    public Transform MainObjectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ClimbableObject")
        {
            MainObjectTransform.rotation = Quaternion.Euler(other.gameObject.transform.rotation.eulerAngles);
        }
    }
    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ClimbableObject")
        {
            MainObjectTransform.rotation= Quaternion.Euler(collision.gameObject.transform.rotation.eulerAngles);
        }
    }*/
}
