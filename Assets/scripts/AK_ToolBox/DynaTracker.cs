using UnityEngine;
using System.Collections.Generic;
public class DynaTracker : MonoBehaviour
{
    [Header("Allow Tracking")]
    public bool allowTracking;
    public List<GameObject> trackTargetRoach;
    public Rigidbody myRb;
    private CockroachMove mainMoveScript;

    public float mySpeed = 6f;

    private void Start()
    {
        mainMoveScript = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
    }

    void Update()
    {
        //Cal all distance
        if (trackTargetRoach.Count > 0 && allowTracking)
        {
            //do sort
            GameObject closestObj = trackTargetRoach[0];
            float dist = Vector3.Distance(transform.position, closestObj.transform.position);
            for (int i = 1; i < trackTargetRoach.Count; i++)
            {
                if (Vector3.Distance(transform.position, trackTargetRoach[i].transform.position) < dist)
                {
                    //swap
                    closestObj = trackTargetRoach[i];
                }
            }
            //do track
            //自己朝最近的物體移動
            Vector3 dir = (closestObj.transform.position - transform.position);
            dir = dir.normalized;
            dir = new Vector3(dir.x,0,dir.z);
            myRb.linearVelocity = dir * mySpeed;
        }
        else
        {
            myRb.linearVelocity = Vector3.zero;
        }
        if (mainMoveScript.isInTheHole == true)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                trackTargetRoach.Remove(playerObj);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPCRoach") )
        {
            if (!trackTargetRoach.Contains(other.gameObject))
            {
                //register it into the obj zone
                trackTargetRoach.Add(other.gameObject);
            }
        }

        if (other.CompareTag("Player") && mainMoveScript.isInTheHole == false)
        {
            if (!trackTargetRoach.Contains(other.gameObject))
            {
                //register it into the obj zone
                trackTargetRoach.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        trackTargetRoach.Remove(other.gameObject);
    }
}
