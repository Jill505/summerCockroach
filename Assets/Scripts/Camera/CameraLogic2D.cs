using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraLogic2D : MonoBehaviour
{
    [Header("Base")]
    public GameObject Camera;
    public Camera cam;
    public GameObject player;
    public Rigidbody2D playerRB;

    [Header("Camera")]
    public Vector3 offset = new Vector3(0, 2, -10);
    public float moveSpeed = 5f;
    public float SetOrthographicSize = 5f;
    public float acceleration = 10f;      // X軸加速度
    public float maxSpeed = 20f;          // 最大速度
    public float smoothTime = 0.3f;       // 平滑時間（與切換模式共用）

    public float targetSize = 5f;     // 目標 orthographicSize
    public float startSize = 2f;

    [Header("Collider")]
    private Bounds customBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    [Header("View")]
    public bool isFollowing = false;
    public Transform cockroach2DPos;
    public CameraViewToggle viewToggle;


    //  5. 內部計算用變數
    private Vector3 targetPos;
    private float velocityX = 0f;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 moveVelocity;    // 給 SmoothDamp 使用
    private float zoomVelocity = 0f; // 視角縮放用

    private float currentVelocity = 0f;
    private bool isZooming = false;
    private float timer = 0f;



    void Awake()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // 計算攝影機視野的一半高度與寬度
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        if (isZooming)
        {
            // 平滑調整 orthographicSize
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref currentVelocity, smoothTime);

            // **重新計算半高半寬（因為 zoom 中會變）**
            camHalfHeight = cam.orthographicSize;
            camHalfWidth = cam.aspect * camHalfHeight;

            // 限制範圍（使用 customBounds）
            if (customBounds.size != Vector3.zero)
            {
                float minX = customBounds.min.x + camHalfWidth;
                float maxX = customBounds.max.x - camHalfWidth;
                float minY = customBounds.min.y + camHalfHeight;
                float maxY = customBounds.max.y - camHalfHeight;

                Vector3 clampedPos = transform.position;
                clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
                clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
                transform.position = clampedPos;
            }

            // 更新經過時間
            timer += Time.deltaTime;

            // 當 smoothTime 過了，就停止 zoom
            if (timer >= smoothTime)
            {
                isZooming = false;
            }
        }
        else
        {
            if (!isFollowing)
            {
                UpdateCameraPosition();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
    }


    

    void UpdateCameraPosition()
    {
        // 計算目標位置
        targetPos = player.transform.position + offset;

        // 使用 SmoothDamp 平滑跟隨
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref smoothVelocity, smoothTime);

        // 限制範圍
        Bounds bounds = customBounds;

        float minX = bounds.min.x + camHalfWidth;
        float maxX = bounds.max.x - camHalfWidth;
        float minY = bounds.min.y + camHalfHeight;
        float maxY = bounds.max.y - camHalfHeight;

        smoothPos.x = Mathf.Clamp(smoothPos.x, minX, maxX);
        smoothPos.y = Mathf.Clamp(smoothPos.y, minY, maxY);

        transform.position = smoothPos;
    }

    private void MoveTowardsTarget()
    {
        // 設定目標位置（保持Z軸不變）
        Vector3 targetPosition = new Vector3(cockroach2DPos.position.x, cockroach2DPos.position.y, offset.z);

        // 使用 SmoothDamp 平滑移動攝影機
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, smoothTime);

        // 使用 SmoothDamp 平滑調整攝影機的 Orthographic Size（視野縮放）
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, SetOrthographicSize, ref zoomVelocity, smoothTime);
    }

    public void StartSmoothZoom()
    {
        transform.position = new Vector3(cockroach2DPos.position.x, cockroach2DPos.position.y, transform.position.z);

        // 初始視野大小
        cam.orthographicSize = startSize;

        // 啟動 zoom 過程
        isZooming = true;
        timer = 0f;
        currentVelocity = 0f;
    }

    public void SetCustomBounds(Bounds bounds)
    {
        customBounds = bounds;
    }
}
