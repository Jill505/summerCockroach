using System.Collections;
using System.Collections.Generic;
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
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 moveVelocity;    // 給 SmoothDamp 使用
    private float zoomVelocity = 0f; // 視角縮放用

    private float currentVelocity = 0f;
    private bool isZooming = false;
    private float timer = 0f;

    private bool SpiderEvent = false;
    public bool spiderEating = false;
    private SpiderEventTrigger spiderTrigger;
    private AllGameManager allGameManager;



    void Awake()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        // 計算攝影機視野的一半高度與寬度
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        if (SpiderEvent) return;
        if(spiderEating) return;

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

    public void SpiderEating(GameObject targetObj)
    {
        // 計算目標位置
        spiderEating = true;    
        targetPos = targetObj.transform.position + offset;

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
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, startSize, ref zoomVelocity, smoothTime);
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

    public void MoveCameraToTarget(GameObject targetObj, float duration, float stayTime)
    {
        StartCoroutine(MoveCameraSmoothCoroutine(targetObj, duration, stayTime));
    }

    private IEnumerator MoveCameraSmoothCoroutine(GameObject targetObj, float duration, float stayTime)
    {
        if (targetObj == null) yield break;

        SpiderEvent = true;
        allGameManager.isTimerRunning = false;

        // 目標與初始設定
        Vector3 targetPos = new Vector3(targetObj.transform.position.x, targetObj.transform.position.y, transform.position.z);
        Vector3 velocity = Vector3.zero;
        float elapsed = 0f;

        // SmoothDamp 的 smoothTime 不等於 duration；用 duration 的比例來決定阻尼時間
        // Empirical: 用 duration * 0.3 ~ 0.4 可以讓 SmoothDamp 在大約 duration 時間內到達感覺合理
        float smoothTimeForDamp = Mathf.Max(0.01f, duration * 0.33f);

        // ---------- 移動到目標（以 elapsed 控制總時間） ----------
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 先計算下一個位置（但尚未套用）
            Vector3 next = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTimeForDamp);

            // 再對 next 做 clamp，避免超出邊界
            Vector3 clampedNext = ClampPosition(next);

            // 如果被 clamp（碰到邊界），將 velocity 同步或歸零，避免下一幀的 SmoothDamp 被錯誤 velocity 干擾
            if (clampedNext != next)
            {
                // Option A: 簡單做法 — 歸零 velocity（通常行為穩定）
                velocity = Vector3.zero;

                // Option B（可選）：重算 velocity 以接合 clampedNext（但可能需要微調）
                // velocity = (clampedNext - transform.position) / Time.deltaTime;
            }

            // 實際套用
            transform.position = clampedNext;

            yield return null;
        }

        

        // ---------- 停留（同樣持續 clamp，避免目標在邊界導致畫面瞬間跳開） ----------
        float stayElapsed = 0f;
        while (stayElapsed < stayTime)
        {
            stayElapsed += Time.deltaTime;
            transform.position = ClampPosition(transform.position);
            yield return null;
        }

        // ---------- 回到玩家位置（同樣方式） ----------
        Vector3 playerTarget = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        elapsed = 0f;
        velocity = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector3 next = Vector3.SmoothDamp(transform.position, playerTarget, ref velocity, smoothTimeForDamp);
            Vector3 clampedNext = ClampPosition(next);

            if (clampedNext != next)
            {
                velocity = Vector3.zero;
                // 或者使用 velocity = (clampedNext - transform.position) / Time.deltaTime;
            }

            transform.position = clampedNext;
            yield return null;
        }


        SpiderEvent = false;
        if (spiderTrigger != null)
            spiderTrigger.startChase = true;
            allGameManager.isTimerRunning = true;
    }

    /// <summary>
    /// 限制相機位置在 customBounds 內
    /// </summary>
    private Vector3 ClampPosition(Vector3 pos)
    {
        if (customBounds.size == Vector3.zero) return pos;

        float minX = customBounds.min.x + camHalfWidth;
        float maxX = customBounds.max.x - camHalfWidth;
        float minY = customBounds.min.y + camHalfHeight;
        float maxY = customBounds.max.y - camHalfHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }
    public void SetSpiderTrigger(SpiderEventTrigger trigger)
    {
        spiderTrigger = trigger;
    }
}


