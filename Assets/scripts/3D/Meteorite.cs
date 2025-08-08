using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public Rigidbody myRb;

    public bool isLanded = false;
    public float MeteoriteSpeed = 10f;
    float metCalSpeed = 0f;

    public float clampDelta = 1f;

    public UnityEngine.Vector3 from;
    public UnityEngine.Vector3 to;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (from == null || to == null)
        {
            Debug.LogError("Ak Error: Meteorite translate error");
            return;
        }
        if (!isLanded)
        {
            UnityEngine.Vector3 dir = to - transform.position;
            myRb.linearVelocity = dir * MeteoriteSpeed; 
            //myRb.linearVelocity = MeteoriteSpeed *  UnityEngine.Vector3.MoveTowards(from, to, 1f) * Time.deltaTime;
            if (UnityEngine.Vector3.Distance(transform.position, to) < clampDelta)
            {
                isLanded = true;
                myRb.linearVelocity = UnityEngine.Vector3.zero;
            }
        } 
    }
}
