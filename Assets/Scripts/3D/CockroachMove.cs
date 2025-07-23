using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class CockroachMove : MonoBehaviour
{
    [Header("參考組件")]
    public CockroachManager myCManager;
    public Rigidbody myRb;

    public Transform mainObjectTransform;
    public Transform subObjectTransform;
    public Transform lookingReferencePoint;

    [Header("功能變數")]
    public float myDirect;

    public float myVelocity0;
    public float myVelocity1;
    public float myVelocity2;
    public float myVelocity3;



    public float myVelocity = 7f;
    public float myMaxVelocity = 14f;
    float velocityX;
    float velocityY;
    //float velocityZ;
    public float HorVelocity;

    float anglesY;
    public float AutoAngleSpeed = 50f;
    public float stopVelocity = 10f;

    public float GravityForce = 9.81f;
    public Vector3 GravityVector = new Vector3();

    [Tooltip("滑鼠靈敏度")]
    public float cameraSensitivity = 1f;
    public float mouseInputLeast = 0.05f;

    [Header("測試變數")]
    public float testVelocityDampValue = 0.2f;
    public float testVelocityXZValue = 0.5f;
    float savedDampValueZ;
    bool dampClogZ;

    [Header("延遲停止設定")]
    public float delayStopTime = 0.5f;


    public moveMode myMoveMode = moveMode.AutoCameraMove;

    public void MakeGravity()
    {

    }

    public void AutoPlayerMove()
    {
        float swapAngle = anglesY * Mathf.Deg2Rad;

        float verAngleX = Mathf.Sin(swapAngle);
        float verAngleZ = Mathf.Cos(swapAngle);


        if (Input.GetKey(KeyCode.W))
        {
            dampClogZ = true;

            if (HorVelocity <= myMaxVelocity)
            {
                //HorVelocity += myVelocity * Time.deltaTime;
                HorVelocity = Mathf.MoveTowards(HorVelocity, myMaxVelocity, testVelocityXZValue);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dampClogZ = true;

            if (HorVelocity >= -1 * myMaxVelocity)
            {
                //HorVelocity -= myVelocity * Time.deltaTime;
                HorVelocity = Mathf.MoveTowards(HorVelocity, -1 * myMaxVelocity, testVelocityXZValue);
            }
        }
        else
        {
            if (dampClogZ == true)
            {
                dampClogZ = false;

                savedDampValueZ = HorVelocity;
            }
            HorVelocity = Mathf.MoveTowards(HorVelocity, 0, stopVelocity * Time.deltaTime);
            //velocityZ = Mathf.SmoothDamp(savedDampValueZ, 0, ref velocityZ, testVelocityDampValue);
        }

        if (Input.GetKey(KeyCode.A))
        {
            anglesY -= AutoAngleSpeed * Time.timeScale;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anglesY += AutoAngleSpeed * Time.timeScale;
        }

        subObjectTransform.transform.localEulerAngles = new Vector3(0, anglesY, 0);

        //Vector3 lookingDirection = Quaternion.LookRotation(subObjectTransform.position, lookingReferencePoint.position).eulerAngles;
        Vector3 lookingDirection = subObjectTransform.forward;
        //lookingDirection.Normalize();
        //Debug.Log("Player facing: " + lookingDirection);

        myRb.linearVelocity = (lookingDirection * HorVelocity) + GravityVector;
        //myRb.linearVelocity = subObjectTransform.eulerAngles;
    }


    void Start()
    {

    }

    void Update()
    {
        MakeGravity();

        if (myMoveMode == moveMode.AutoCameraMove)
        {
            AutoPlayerMove();
            if (myDirect != myCManager.myCameraLogic.CameraDirect)
            {
                //TODO: make the camera move smoother. 
                //myDirect = Mathf.LerpAngle(myDirect,myCManager.myCameraLogic.CameraDirect, myCManager.autoModeCameraFlu);
            }
            //Face cockroach direction.

        }
        else if (myMoveMode == moveMode.PlayerCameraMove)
        {
            //TODO: Let Player control the camera freely.
            //TODO: there's a bug need to be fix.
            float mouseXInput = Input.GetAxis("Mouse X");
            if (mouseXInput > mouseInputLeast)
            {
                myDirect += mouseXInput * cameraSensitivity;
            }
            //Free move camera, mouse x move = camera direct move.
            //Sync myDirect to camera dir, multiply the 
        }


        else if (myMoveMode == moveMode.ChangeSceneMoment)
        {
            StartCoroutine(DelayedStop(delayStopTime));
        }
    }
    
    private IEnumerator DelayedStop(float delayStopTime)
    {
        yield return new WaitForSeconds(delayStopTime);

        HorVelocity = 0;
        myRb.linearVelocity = Vector3.zero;
    }   

}

