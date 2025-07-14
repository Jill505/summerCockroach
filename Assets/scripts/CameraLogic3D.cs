using System;
using UnityEngine;

public class CameraLogic3D : MonoBehaviour
{
    public CockroachManager myCManager;
    public float CameraDirect;

    public GameObject myCameraTrackerObject;
    public GameObject myCockroachMeshObject;
    public GameObject myCockroachManagerObject;

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
            transform.localPosition = PlayerCameraOffset;
        }
    }

    public void CockroachInitialize()
    {
        
    }
}
