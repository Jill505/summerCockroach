using UnityEngine;
using System.Collections.Generic;
public class DynaTracker : MonoBehaviour
{
    [Header("Allow Tracking")]
    public bool allowTracking;
    public List<GameObject> trackTargetRoach;
    public Rigidbody myRb;

    public float mySpeed = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPCRoach") || other.CompareTag("Player"))
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
