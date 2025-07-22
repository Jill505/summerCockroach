using System;
using UnityEngine;

public class CameraLogic3D : MonoBehaviour
{
    public CockroachManager myCManager;
    public float CameraDirect;

    public GameObject myCameraTrackerObject;
    public GameObject myCockroachMeshObject;
    public GameObject myCockroachManagerObject;
    public GameObject my3DCameraReferencePoint;
    public GameObject CameraObject;

    [Header("Auto Camera �Ѽ�")]
    public float autoAngleSpeed = 0.2f;
    public Vector3 AutoCameraLookOffset = Vector3.zero;
    
    [Header("Player mouse setting")]
    public float mouseInputLeast = 0.05f;
    public float myDirect = 0f;
    public float cameraSensitivity = 1f;
    public enum CameraTrackMode
    {
        autoCamera,
        playerCamera
    }

    public CameraTrackMode myTrackMode = CameraTrackMode.autoCamera;

    public Vector3 CameraOffset = new Vector3(0, 1.67f, -4.63f);
    public Vector3 AutoCameraOffset = new Vector3(0, 4.44f, -1.75f);
    public Vector3 PlayerCameraOffset = new Vector3(0, 1.67f, -4.63f);
    public Vector3 nowCameraPosition = Vector3.zero;
    //public float cameraOffsetX = 0;
    //public float cameraOffsetY = 1.67f;
    //public float cameraOffsetZ = -4.63f;

    private void Awake()
    {
        CockroachInitialize();
        CameraObject.transform.localPosition = CameraOffset;
        nowCameraPosition = CameraOffset;
    }


    private void FixedUpdate()
    {
        if (myTrackMode == CameraTrackMode.autoCamera)
        {
            //myCameraTrackerObject.transform.SetParent(myCockroachMeshObject.transform, false);
            myCameraTrackerObject.transform.SetParent(myCockroachManagerObject.transform, false);
            Debug.Log(myCockroachMeshObject.transform.rotation.eulerAngles);
            my3DCameraReferencePoint.transform.position = myCockroachMeshObject.transform.position + RotationMatrixCal(AutoCameraOffset, myCockroachMeshObject.transform.rotation.eulerAngles);

            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, my3DCameraReferencePoint.transform.position, autoAngleSpeed * Time.fixedDeltaTime);

            //transform.localPosition = CameraOffset;
            //CameraObject.transform.localScale = new Vector3(1, 1, 1);
            //transform.localPosition = AutoCameraOffset;

            CameraObject.transform.LookAt(myCockroachMeshObject.transform.position + AutoCameraLookOffset);
        }
        else if(myTrackMode == CameraTrackMode.playerCamera)
        {
            //CameraDirect = Mathf.LerpAngle(CameraDirect, myCManager.myCockroachMove.myDirect, myCManager.playerModeCameraFlu);
            myCameraTrackerObject.transform.SetParent(myCockroachManagerObject.transform, false);
            CameraObject.transform.localScale = new Vector3(1, 1, 1);

            CameraObject.transform.localPosition = PlayerCameraOffset;//TODO: ���U�\��g�n�����o��R��
        }
    }
    private void Update()
    {
        if (myTrackMode == CameraTrackMode.playerCamera)
        {
            float mouseXInput = Input.GetAxis("Mouse X");
            if (mouseXInput > mouseInputLeast)
            {
                myDirect += mouseXInput * cameraSensitivity * Time.deltaTime;
            }

            //TODO: �����Y¶��������y�b��V�A���O�n�Ҽ{�����Шt������b
            //STEP1: ���oPlayerCameraOffset�������Mesh����y�Ъ����t��m
            //STEP2: �N�䭼�Hz(myDirect cos)�Bx(myDirect sin)���q
            //STEP3: ���ƥ�����v���y�Цܭp��n���s��m

            //transform.localPosition = PlayerCameraOffset;
        }
    }
    public void CockroachInitialize()
    {
        
    }

    public static Vector3 RotationMatrixCal(Vector3 inputVector, Vector3 inputRotation)
    {
        // ��کԨ� (degree) �ഫ�� Quaternion
        Quaternion rotation = Quaternion.Euler(inputRotation);

        // �α���|���ƭ��W�V�q�A�۷�����α���x�}
        Vector3 rotatedVector = rotation * inputVector;
        
        return rotatedVector;
    }
}
