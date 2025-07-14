using UnityEngine;

public class CockroachGravityMaker : MonoBehaviour
{
    public CockroachManager cManager;
    public CockroachMove cMover;

    public float cockroachGravityScale = 9.81f;
    public float debugGravity = -0.1f;
    public float fallMaxSpeed = 25f;
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
        Debug.Log("touch name: " + other.gameObject.name    );

        if (other.gameObject.tag == "ClimbableObject")
        {
            //no need to use gravity
            cMover.GravityVector = Vector3.zero + new Vector3(0, debugGravity, 0) * Time.deltaTime;
            Debug.Log("Cockroach is on the ground");
        }
        else
        {
            if (cockroachGravityScale < fallMaxSpeed)
            {
                cMover.GravityVector += new Vector3(0, -1 * cockroachGravityScale, 0) * Time.deltaTime;
                Debug.Log("Cockroach is taking gravity");
            }
        }
    }
    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ClimbableObject")
        {
            //no need to use gravity
            cMover.GravityVector = Vector3.zero + new Vector3(0, debugGravity, 0);
            Debug.Log("Cockroach is on the ground");
        }
        else
        {
            cMover.GravityVector += new Vector3(0, cockroachGravityScale, 0) * Time.deltaTime;
            Debug.Log("Cockroach is taking grvity");
        }
    }*/
}
