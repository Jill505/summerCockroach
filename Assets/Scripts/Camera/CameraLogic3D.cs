using System;
using System.Collections;
//using Unity.Android.Gradle;
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


    public bool overriddenByTimeline = false;


    [Header("Auto Camera 參數")]
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

    [Header("shake time")]
    public float shakeTime = 0.8f;
    public float shakeVelocity = 0.4f;
    public void CameraShake(float Time, float Velocity)
    {
        StartCoroutine(shake(Time, Velocity));
    }
    IEnumerator shake(float time, float velocity)
    {
        float t = time;
        while (t > 0)
        {
            Vector3 ranSV = new Vector3(UnityEngine.Random.Range((-1* velocity),velocity), UnityEngine.Random.Range((-1 * velocity), velocity), UnityEngine.Random.Range((-1 * velocity), velocity));
            CameraObject.transform.localPosition += ranSV;
            t-= Time.deltaTime; 
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (overriddenByTimeline) return;

        if (myTrackMode == CameraTrackMode.autoCamera)
        {
            //myCameraTrackerObject.transform.SetParent(myCockroachMeshObject.transform, false);
            myCameraTrackerObject.transform.SetParent(myCockroachManagerObject.transform, false);
            //Debug.Log(myCockroachMeshObject.transform.rotation.eulerAngles);
            my3DCameraReferencePoint.transform.position = myCockroachMeshObject.transform.position + RotationMatrixCal(AutoCameraOffset, myCockroachMeshObject.transform.rotation.eulerAngles);

            /*
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, my3DCameraReferencePoint.transform.position, autoAngleSpeed * Time.fixedDeltaTime);

            //transform.localPosition = CameraOffset;
            //CameraObject.transform.localScale = new Vector3(1, 1, 1);
            //transform.localPosition = AutoCameraOffset;

            CameraObject.transform.LookAt(myCockroachMeshObject.transform.position + AutoCameraLookOffset);*/
        }
        else if(myTrackMode == CameraTrackMode.playerCamera)
        {
            //CameraDirect = Mathf.LerpAngle(CameraDirect, myCManager.myCockroachMove.myDirect, myCManager.playerModeCameraFlu);
            myCameraTrackerObject.transform.SetParent(myCockroachManagerObject.transform, false);
            CameraObject.transform.localScale = new Vector3(1, 1, 1);

            CameraObject.transform.localPosition = PlayerCameraOffset;//TODO: 底下功能寫好之後把這行刪掉
        }
    }

    private void LateUpdate()
    {
        if (overriddenByTimeline) return;
        if (isFocusing && focusTarget != null)
        {
            // 將參考點設在 focusTarget 上，並給一個小偏移，讓攝影機可以看得更好
            Vector3 targetOffset = new Vector3(4f, 5f, -6f);
            
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, focusTarget.position + targetOffset, autoAngleSpeed * Time.deltaTime);
            CameraObject.transform.LookAt(focusTarget.position);
        }
        else
        {
            // 原本的行為（你的既有 LateUpdate 內容）
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, my3DCameraReferencePoint.transform.position, autoAngleSpeed * Time.deltaTime);
            CameraObject.transform.LookAt(myCockroachMeshObject.transform.position + AutoCameraLookOffset);
        }
    }
    private void Update()
    {
        if (overriddenByTimeline) return;
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

        if (Input.GetKeyDown(KeyCode.F8))
        {
            CameraShake(shakeTime, shakeVelocity);
        }
    }
    public void CockroachInitialize()
    {
        
    }

    public static Vector3 RotationMatrixCal(Vector3 inputVector, Vector3 inputRotation)
    {
        // 把歐拉角 (degree) 轉換為 Quaternion
        Quaternion rotation = Quaternion.Euler(inputRotation);

        // 用旋轉四元數乘上向量，相當於應用旋轉矩陣
        Vector3 rotatedVector = rotation * inputVector;
        
        return rotatedVector;
    }


    public Transform focusTarget = null; // 臨時焦點
    private bool isFocusing = false;
    private Vector3 savedAutoCameraOffset;
    private Vector3 savedPlayerCameraOffset;
    private CameraTrackMode savedTrackMode;


    // 這個方法由外部呼叫，開始以 target 作為臨時焦點
    public void StartFocusOn(Transform target)
    {
        if (target == null) return;
        focusTarget = target;
        isFocusing = true;

        // 儲存原本狀態以便還原
        savedTrackMode = myTrackMode;
        savedAutoCameraOffset = AutoCameraOffset;
        savedPlayerCameraOffset = PlayerCameraOffset;

        // 切入 playerCamera 模式或 autoCamera 都可以，這邊直接切成 autoCamera 讓 camera 以目標位置為參考
        myTrackMode = CameraTrackMode.autoCamera;
    }

    // 停止臨時焦點並還原
    public void StopFocus()
    {
        focusTarget = null;
        isFocusing = false;

        // 還原原本的跟隨模式
        myTrackMode = savedTrackMode;
        // 如果需要還原 offset 則可還原（目前我也保留）
        AutoCameraOffset = savedAutoCameraOffset;
        PlayerCameraOffset = savedPlayerCameraOffset;
    }


}
