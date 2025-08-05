using UnityEditor.Rendering;
using UnityEngine;

public class CameraLogic2D : MonoBehaviour
{
    [Header("Base")]
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

    [Header("collider")]
    public BoxCollider2D cameraBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    [Header("view")]
    public bool isFollowing = false;
    public Transform cockroach2DPos;
    public CameraViewToggle viewToggle;

    //  5. 內部計算用變數
    private Vector3 targetPos;
    private float velocityX = 0f;
    private Vector3 moveVelocity;    // 給 SmoothDamp 使用
    private float zoomVelocity = 0f; // 視角縮放用



    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // 計算攝影機視野的一半高度與寬度
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        if (isFollowing == false)
        {
            UpdateCameraPosition();
        }
        else
        {
            MoveTowardsTarget();
        }
    }


    void UpdateCameraPosition()
    {
        targetPos = player.transform.position + offset;

        // 橫向距離
        float distanceX = targetPos.x - transform.position.x;

        // 根據距離推進速度（加速）
        float accelerationForce = acceleration * Time.deltaTime;

        if (Mathf.Abs(distanceX) > 0.01f)
        {
            velocityX += Mathf.Sign(distanceX) * accelerationForce;
        }
        else
        {
            velocityX = 0f; // 幾乎貼齊就停止
        }

        // 限制最大速度
        velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);

        // 移動攝影機（只動X軸）
        Vector3 newCamPos = new Vector3(transform.position.x + velocityX * Time.deltaTime, targetPos.y, targetPos.z);

        // 限制範圍：從 BoxCollider2D 得到範圍
        Bounds bounds = cameraBounds.bounds;

        float minX = bounds.min.x + camHalfWidth;
        float maxX = bounds.max.x - camHalfWidth;
        float minY = bounds.min.y + camHalfHeight;
        float maxY = bounds.max.y - camHalfHeight;

        newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);


        transform.position = newCamPos;
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
}
