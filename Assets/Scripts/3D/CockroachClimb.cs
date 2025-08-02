using UnityEngine;

public class CockroachClimb : MonoBehaviour
{
    public Transform MainObjectTransform;

    public float RotationSpeed = 0.5f;
    public Vector3 targetRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MainObjectTransform.rotation = Quaternion.Lerp(MainObjectTransform.rotation, Quaternion.Euler(targetRotation), RotationSpeed*Time.deltaTime);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ClimbableObject")
        {
            targetRotation = other.gameObject.transform.rotation.eulerAngles;
            //MainObjectTransform.rotation = Quaternion.Euler(other.gameObject.transform.rotation.eulerAngles);
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
