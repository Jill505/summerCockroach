using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CockroachMove : MonoBehaviour
{
    [Header("參考組件")]
    public CockroachManager myCManager;
    public Rigidbody myRb;

    public Transform mainObjectTransform;
    public Transform subObjectTransform;
    public Transform lookingReferencePoint;

    public Image myRunAmount;

    [Header("調整係數")]
    public float runSpeed = 1.4f;
    public float runAbleTime = 4f;
    public float runAbleTimeCal = 4f;
    public float runRecoverPerSec = 2f;
    public float runNotCD = 1f;
    public float runNotCDCal = 1f;

    [Header("功能變數")]
    public float myDirect;

    public float Hp6maxVelocity = 14f;
    public float Hp5maxVelocity = 14f;
    public float Hp4maxVelocity = 14f;
    public float Hp3maxVelocity = 14f;
    public float Hp2maxVelocity = 14f;
    public float Hp1maxVelocity = 14f;
    public float Hp0maxVelocity = 0f;

    public float myVelocity = 7f;
    public float myMaxVelocity = 14f;
    public float myRealVelocity = 0f;
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

    public void UISync()
    {
        myRunAmount.fillAmount = runAbleTimeCal / runAbleTime;
    }

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

            if (HorVelocity <= myRealVelocity)
            {
                //HorVelocity += myVelocity * Time.deltaTime;
                HorVelocity = Mathf.MoveTowards(HorVelocity, myRealVelocity, testVelocityXZValue);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dampClogZ = true;

            if (HorVelocity >= -1 * myRealVelocity)
            {
                //HorVelocity -= myVelocity * Time.deltaTime;
                HorVelocity = Mathf.MoveTowards(HorVelocity, -1 * myRealVelocity, testVelocityXZValue);
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
            anglesY -= AutoAngleSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anglesY += AutoAngleSpeed * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.LeftShift) && runAbleTimeCal > 0)
        {
            myRealVelocity = myMaxVelocity * runSpeed;
            runNotCDCal = runNotCD;
            runAbleTimeCal -= Time.deltaTime;
        }
        else
        {
            myRealVelocity = myMaxVelocity;

            if (HorVelocity > myMaxVelocity)
            {
                HorVelocity -= myMaxVelocity;
            }
            runNotCDCal -= Time.deltaTime;
            if (runNotCDCal < 0)
            {
                runAbleTimeCal += runRecoverPerSec * Time.deltaTime;
                if (runAbleTimeCal > runAbleTime)
                {
                    runAbleTimeCal = runAbleTime;
                }
            }
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
        UISync();
    }
    
    private IEnumerator DelayedStop(float delayStopTime)
    {
        yield return new WaitForSeconds(delayStopTime);

        HorVelocity = 0;
        myRb.linearVelocity = Vector3.zero;
    }   

}

