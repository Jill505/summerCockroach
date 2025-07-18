using System;
using UnityEngine;

public class CameraLogic3D : MonoBehaviour
{
    public CockroachManager myCManager;
    public float CameraDirect;

    public GameObject myCameraTrackerObject;
    public GameObject myCockroachMeshObject;
    public GameObject myCockroachManagerObject;

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
        transform.localPosition = CameraOffset;
        nowCameraPosition = CameraOffset;
    }


    private void FixedUpdate()
    {
        if (myTrackMode == CameraTrackMode.autoCamera)
        {
            myCameraTrackerObject.transform.SetParent(myCockroachMeshObject.transform, false);
            //transform.localPosition = CameraOffset;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = AutoCameraOffset;
        }
        else if(myTrackMode == CameraTrackMode.playerCamera)
        {
            //CameraDirect = Mathf.LerpAngle(CameraDirect, myCManager.myCockroachMove.myDirect, myCManager.playerModeCameraFlu);
            myCameraTrackerObject.transform.SetParent(myCockroachManagerObject.transform, false);
            transform.localScale = new Vector3(1, 1, 1);

            transform.localPosition = PlayerCameraOffset;//TODO: 底下功能寫好之後把這行刪掉
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

            //TODO: 讓鏡頭繞著蟑螂的y軸轉向，但是要考慮父坐標系的旋轉軸
            //STEP1: 取得PlayerCameraOffset基於蟑螂Mesh物件座標的偏差位置
            //STEP2: 將其乘以z(myDirect cos)、x(myDirect sin)分量
            //STEP3: 順滑平移攝影機座標至計算好的新位置

            //transform.localPosition = PlayerCameraOffset;
        }
    }
    public void CockroachInitialize()
    {
        
    }
}
