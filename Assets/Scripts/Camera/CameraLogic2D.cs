using UnityEngine;

public class CameraLogic2D : MonoBehaviour
{
    [Header("攝影機位置")]
    public Vector3 offset = new Vector3(0, 2, -10);
    private Vector3 targetPos;
    private float velocityX = 0f;

    [Header("追蹤設定")]
    public float acceleration = 10f;  // 加速度
    public float maxSpeed = 20f;      // 最大追蹤速度

    [Header("攝影機限制範圍")]
    public BoxCollider2D cameraBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    public Camera cam;
    public GameObject player;
    public Rigidbody2D playerRB;

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // 計算攝影機視野的一半高度與寬度
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        UpdateCameraPosition();
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
}
