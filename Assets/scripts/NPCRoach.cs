using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class NPCRoach : MonoBehaviour
{

    [Header("Ref Component")]
    public NewAllGameManager newAllGameManager;
    public AllGameManager allGameManager;
    public FoodGenManger foodGenManager;
    public CockroachManager cockroachManager;
    public NPCRoachManager nPCRoachManager;

    public Rigidbody myRb;

    [Header("Ref Objects")]
    public GameObject burstBlood;

    [Header("Roach Brain Logic")]
    public Coroutine brainLogic;

    public float moveSpeed = 7f;
    public Vector3 targetPos;

    public Vector3 nextPos;

    public bool finJ = false;

    public bool clogFin;

    public bool hasFemInZone;
    public bool hasFoodInZone;

    public Vector3 targetFemPos;
    public Vector3 targetFoodPos;

    public GameObject myMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newAllGameManager = FindFirstObjectByType<NewAllGameManager>();
        allGameManager = FindFirstObjectByType<AllGameManager>();
        foodGenManager = FindFirstObjectByType<FoodGenManger>();
        cockroachManager = FindFirstObjectByType<CockroachManager>();
        nPCRoachManager = FindFirstObjectByType<NPCRoachManager>();

        nPCRoachManager.nPCRoaches.Add(this);

        rollTarget();

        //StartCoroutine(myCoroutineUpdate());
    }
    bool fixedUpdateAllowCheckClog = false;

    private void FixedUpdate()
    {
        hasFemInZone = false;
        hasFoodInZone = false;
        fixedUpdateAllowCheckClog = false;
    }
    private void Update()
    {
        if (!fixedUpdateAllowCheckClog)
        {
            if (hasFemInZone)
            {
                //Do Track Fem
                goFem();
            }
            else
            {
                if (hasFoodInZone)
                {
                    //Do Track Food
                    goFood();
                }
                else
                {
                    //Do Normal
                    goTarget();
                }
            }
            fixedUpdateAllowCheckClog = true;
        }
    }

    public IEnumerator myCoroutineUpdate()
    {
        yield return null;
    }

    public void goTarget()
    {
        transform.LookAt(nextPos);
        myRb.linearVelocity = transform.forward * moveSpeed;
    }
    public void goFem()
    {
        transform.LookAt(targetFemPos);
        myRb.linearVelocity = transform.forward * moveSpeed * 4;
    }
    public void goFood()
    {
        transform.LookAt(targetFoodPos);
        myRb.linearVelocity = transform.forward * moveSpeed * 2;
    }

    public void rollTarget()
    {
        int randomPlace = Random.Range(0, foodGenManager.FoodPos.Count);
        nextPos = foodGenManager.FoodPos[randomPlace].transform.position;
        Debug.Log("My Next dest is" + foodGenManager.FoodPos[randomPlace].transform.position);

        Invoke("rollTarget", Random.Range(3, 5));
    }

    public void areaContainFem()
    {

    }

    public void areaContainFood()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cockroachManager.dashing == true)
        {
            //KYS
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        Instantiate(burstBlood, transform.position, Quaternion.Euler(0,-90,0));
        nPCRoachManager.nPCRoaches.Remove(this);
    }
}
