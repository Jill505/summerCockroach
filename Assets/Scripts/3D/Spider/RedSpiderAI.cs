using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpiderAI : MonoBehaviour
{
    [Header("References")]
    public Transform spider;              // 蜘蛛本體
    public BoxCollider moveRangeBox;      // 定點移動範圍
    public CapsuleCollider centerCapsule; // 中心判定
    public CapsuleCollider spiderDetection;
    public Transform startPos;            // 起始位置
    public Animator animControl;
    private Transform player;              // 玩家目標

    [Header("Movement Settings")]
    public float moveSpeed = 2f;          // 前進速度
    public float chaseSpeed = 10f;       // 追擊速度
    public float turnSpeed = 180f;        // 旋轉速度
    public float idleTime = 8f;           // 停留時間
    public float fadeDuration = 1.5f;     // 動畫速度變化時間

    private bool canMove = false;
    private bool canCapsuleAction = true;
    private bool canBoxAction = true;
    private bool isChasing = false;
    private bool isReturning = false;

    private List<Vector3> capsuleDirections = new List<Vector3>();
    private Quaternion initialRotation;
    private Vector3 currentDirection;
    private bool wasInsideCapsule = true;
    private bool wasInsideBox = true;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        spider.position = startPos.position;
        currentDirection = -spider.forward;
        initialRotation = spider.rotation;
        ResetCapsuleDirections();

        GameObject foundPlayer = GameObject.Find("3DCockroach");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            Debug.Log($"[RedSpiderAI] 找到玩家物件: {player.name}");
        }
        else
        {
            Debug.LogWarning("[RedSpiderAI] 場景中找不到 Tag 為 'Player' 的物件！");
        }

        Debug.Log("[RedSpiderAI] 初始化完成，開始 Idle");
        StartCoroutine(IdleThenStartMoving());
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else if (canMove)
        {
            MoveForward();
        }

        Debug.DrawRay(spider.position, currentDirection, Color.green);
    }

    private void FixedUpdate()
    {
        if (!isChasing && !isReturning)
        {
            CheckCapsule();
            CheckBox();
        }

        DetectPlayer();
    }

    #region --- 玩家偵測與追擊邏輯 ---
    private void DetectPlayer()
    {
        if (player == null) return;

        bool playerInSight = IsInsideCollider(spiderDetection, player.position);

        if (playerInSight && !isChasing)
        {
            StartChasing();
        }
        else if (!playerInSight && isChasing)
        {
            StopChasingAndReturn();
        }
    }

    private void StartChasing()
    {
        Debug.Log("[RedSpiderAI] 偵測到玩家，進入追擊模式");
        isChasing = true;
        isReturning = false;
        canMove = false;
        canBoxAction = false;
        canCapsuleAction = false;
        SpeedUpAnimator();
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // 旋轉面向玩家
        Vector3 dirToPlayer = (player.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToPlayer.x, 0, -dirToPlayer.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        // 前進
        spider.position += -spider.forward * chaseSpeed * Time.deltaTime;
    }

    private void StopChasingAndReturn()
    {
        Debug.Log("[RedSpiderAI] 玩家離開視線，返回起始點");
        isChasing = false;
        isReturning = true;
        SlowDownAnimator();
    }

    private void ReturnToStart()
    {
        Vector3 dirToStart = (startPos.position - spider.position);
        float distance = dirToStart.magnitude;

        if (distance < 0.2f)
        {
            Debug.Log("[RedSpiderAI] 已回到起點，恢復巡邏");
            StartCoroutine(SmoothRotateToInitial());
            isReturning = false;
            StartCoroutine(IdleThenStartMoving());
            canCapsuleAction = true;
            canBoxAction = true;            
            return;
        }

        // 旋轉回起點方向
        Vector3 moveDir = dirToStart.normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-moveDir.x, 0, -moveDir.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        spider.position += -spider.forward * moveSpeed * Time.deltaTime;
    }
    #endregion

    #region --- 原本的巡邏邏輯 ---
    private void MoveForward()
    {
        SpeedUpAnimator();
        spider.position += currentDirection * moveSpeed * Time.deltaTime;
    }

    private void CheckCapsule()
    {
        bool inside = IsInsideCollider(centerCapsule, spider.position);

        if (wasInsideCapsule && !inside && canCapsuleAction)
        {
            canCapsuleAction = false;
            Debug.Log("[RedSpiderAI] 離開 CapsuleCollider，執行 CapsuleAction");
            StartCoroutine(CapsuleAction());
        }

        wasInsideCapsule = inside;
    }

    private IEnumerator CapsuleAction()
    {
        int index = Random.Range(0, capsuleDirections.Count);
        Vector3 targetDir = capsuleDirections[index];
        capsuleDirections.RemoveAt(index);

        if (capsuleDirections.Count == 0)
            ResetCapsuleDirections();

        if (targetDir != -spider.forward)
            yield return StartCoroutine(TurnToDirection(targetDir));
        else
            currentDirection = targetDir;

        yield return null;
        canCapsuleAction = true;
    }

    private void ResetCapsuleDirections()
    {
        capsuleDirections.Clear();
        capsuleDirections.Add(-spider.forward);
        capsuleDirections.Add(spider.right);
        capsuleDirections.Add(-spider.right);
    }

    private IEnumerator TurnToDirection(Vector3 targetDir)
    {
        canMove = false;
        SlowDownAnimator();
        yield return new WaitForSeconds(idleTime);
        SpeedUpAnimator();

        float angle = Vector3.SignedAngle(spider.forward, targetDir, Vector3.up);
        float rotated = 0f;
        float currentTurnSpeed = 0f;
        float maxTurnSpeed = turnSpeed;
        float acceleration = turnSpeed;

        while (Mathf.Abs(rotated) < Mathf.Abs(angle))
        {
            currentTurnSpeed += acceleration * Time.deltaTime;
            if (currentTurnSpeed > maxTurnSpeed)
                currentTurnSpeed = maxTurnSpeed;

            float step = Mathf.Sign(angle) * currentTurnSpeed * Time.deltaTime;
            if (Mathf.Abs(rotated + step) > Mathf.Abs(angle))
                step = angle - rotated;

            spider.Rotate(0, step, 0);
            rotated += step;
            yield return null;
        }

        currentDirection = -spider.forward;
        canMove = true;
        Debug.Log("[RedSpiderAI] 旋轉完成，開始移動");
    }

    private IEnumerator SmoothRotateToInitial()
    {
        float rotated = 0f;
        Quaternion startRot = spider.rotation;
        float totalAngle = Quaternion.Angle(startRot, initialRotation);
        float currentTurnSpeed = 0f;
        float acceleration = turnSpeed; // 每秒加速度
        float maxTurnSpeed = turnSpeed;

        while (rotated < totalAngle)
        {
            // 每幀增加旋轉速度
            currentTurnSpeed += acceleration * Time.deltaTime;
            if (currentTurnSpeed > maxTurnSpeed)
                currentTurnSpeed = maxTurnSpeed;

            // 計算這幀旋轉角度
            float step = currentTurnSpeed * Time.deltaTime;

            // 防止超過總角度
            if (rotated + step > totalAngle)
                step = totalAngle - rotated;

            // 平滑旋轉
            spider.rotation = Quaternion.RotateTowards(spider.rotation, initialRotation, step);
            rotated += step;

            yield return null;
        }

        spider.rotation = initialRotation; // 保證最終對齊
        Debug.Log("[RedSpiderAI] 平滑回正完成");
    }

    private void CheckBox()
    {
        bool inside = IsInsideCollider(moveRangeBox, spider.position);

        if (wasInsideBox && !inside && canBoxAction)
        {
            canBoxAction = false;
            Debug.Log("[RedSpiderAI] 離開 BoxCollider，執行 BoxAction");
            StartCoroutine(BoxAction());
        }

        wasInsideBox = inside;
    }

    private IEnumerator BoxAction()
    {
        canMove = false;
        SlowDownAnimator();
        yield return new WaitForSeconds(idleTime);
        SpeedUpAnimator();

        Debug.Log("[RedSpiderAI] BoxAction 開始旋轉 180 度");
        float rotated = 0f;
        float currentTurnSpeed = 0f;
        float maxTurnSpeed = turnSpeed;
        float acceleration = turnSpeed;

        while (rotated < 180f)
        {
            currentTurnSpeed += acceleration * Time.deltaTime;
            if (currentTurnSpeed > maxTurnSpeed)
                currentTurnSpeed = maxTurnSpeed;

            float step = currentTurnSpeed * Time.deltaTime;
            if (rotated + step > 180f)
                step = 180f - rotated;

            spider.Rotate(0, step, 0);
            rotated += step;
            yield return null;
        }

        SlowDownAnimator();
        yield return new WaitForSeconds(idleTime);
        SpeedUpAnimator();

        currentDirection = -spider.forward;
        canMove = true;
        canBoxAction = true;
    }
    #endregion

    #region --- Idle & Animator 控制 ---
    private IEnumerator IdleThenStartMoving()
    {
        canMove = false;
        SlowDownAnimator();
        yield return new WaitForSeconds(idleTime);
        canMove = true;
        Debug.Log("[RedSpiderAI] Idle 結束，開始移動");
    }

    public void SlowDownAnimator()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeAnimatorSpeed(0f));
    }

    public void SpeedUpAnimator()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeAnimatorSpeed(1f));
    }

    private IEnumerator FadeAnimatorSpeed(float targetSpeed)
    {
        float startSpeed = animControl.speed;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            animControl.speed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }

        animControl.speed = targetSpeed;
    }
    #endregion

    #region --- Collider 判斷 ---
    private bool IsInsideCollider(Collider col, Vector3 point)
    {
        Vector3 closest = col.ClosestPoint(point);
        return closest == point;
    }
    #endregion
}
