using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpiderAI : MonoBehaviour
{
    [Header("References")]
    protected CockroachMove cockroachMove; // private → protected
    public Transform spider;
    public BoxCollider moveRangeBox;
    public CapsuleCollider centerCapsule;
    public CapsuleCollider spiderDetection;
    public Transform startPos;
    public Animator animControl;
    protected Transform player;           // private → protected
    protected List<Transform> chaseTargets = new List<Transform>(); // private → protected

    [Header("Movement Settings")]
    protected float moveSpeed = 13f;          // private → protected
    protected float chaseSpeed = 20f;       // private → protected
    protected float turnSpeed = 180f;        // private → protected
    protected float idleTime = 8f;
    protected float fadeDuration = 0.5f;

    protected bool canMove = false;          // private → protected
    protected bool canCapsuleAction = true;
    protected bool canBoxAction = true;
    protected bool isChasing = false;
    protected bool isReturning = false;
    protected bool isTurning = false;

    protected List<Vector3> capsuleDirections = new List<Vector3>();
    protected Quaternion initialRotation;
    protected Vector3 currentDirection;
    protected bool wasInsideCapsule = true;
    protected bool wasInsideBox = true;
    public GameObject burstBlood;

    protected Coroutine fadeCoroutine;

    protected Transform currentChaseTarget;

    [Header("AI 狀態控制")]
    public bool canSpiderMove = true;

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
            cockroachMove = player.GetComponent<CockroachMove>();
            chaseTargets.Add(player.transform);
            Debug.Log($"[RedSpiderAI] 找到玩家物件: {player.name}");
        }
        else
        {
            Debug.LogWarning("[RedSpiderAI] 場景中找不到 Tag 為 'Player' 的物件！");
        }

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPCRoach");
        foreach (GameObject npc in npcs)
            chaseTargets.Add(npc.transform);

        Debug.Log("[RedSpiderAI] 初始化完成，開始 Idle");
        StartCoroutine(IdleThenStartMoving());
    }

    private void Update()
    {
        if (!canSpiderMove)
        {
            return; // 完全停止 AI 更新
        }
        if (isChasing)
        {
            Chase();
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
        if (!canSpiderMove)
        {
            return; //  暫停所有物理與偵測邏輯
        }
        // 移除已消失目標
        CleanChaseTargets();

        if (!isChasing && !isReturning)
        {
            CheckCapsule();
            CheckBox();
        }

        DetectTarget();
    }

    #region --- 玩家偵測與追擊邏輯 ---
    private void DetectTarget()
    {
        Transform nearestTarget = null;
        float nearestDist = float.MaxValue;

        // 找出最近且在偵測範圍內的目標
        foreach (Transform target in chaseTargets)
        {
            if (target == null) continue;

            if (IsInsideCollider(spiderDetection, target.position) )
            {
                float dist = Vector3.Distance(spider.position, target.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestTarget = target;
                }
            }
        }

        if (nearestTarget != null && !isChasing)
        {
            StartChasing(nearestTarget);
        }
        else if (nearestTarget == null && isChasing)
        {
            StopChasingAndReturn();
        }
    }


    protected virtual void StartChasing(Transform target)
    {
        currentChaseTarget = target;
        Debug.Log($"[RedSpiderAI] 偵測到目標 {target.name}，進入追擊模式");
        isChasing = true;
        isReturning = false;
        canMove = false;
        canBoxAction = false;
        canCapsuleAction = false;
        SpeedUpAnimator();
    }

    protected virtual void Chase()
    {
        if (currentChaseTarget == null)
        {
            Debug.Log("[RedSpiderAI] 追擊對象消失或被摧毀，停止追擊並返回起點");
            StopChasingAndReturn();
            return;
        }
        if (currentChaseTarget == player && cockroachMove != null && cockroachMove.isInTheHole)
        {
            Debug.Log("[RedSpiderAI] 玩家進入洞中，蜘蛛停止追擊並返回起點");
            currentChaseTarget = null;
            StopChasingAndReturn();
            return;
        }

        // 旋轉面向目標
        Vector3 dirToTarget = (currentChaseTarget.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToTarget.x, 0, -dirToTarget.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        // 前進
        spider.position += -spider.forward * chaseSpeed * Time.deltaTime;
    }

    private void CleanChaseTargets()
    {
        for (int i = chaseTargets.Count - 1; i >= 0; i--)
        {
            if (chaseTargets[i] == null)
            {
                chaseTargets.RemoveAt(i);
                Debug.Log("[RedSpiderAI] 移除已死亡或消失的目標");
            }
        }
    }

    protected virtual void StopChasingAndReturn()
    {
        Debug.Log("[RedSpiderAI] 玩家離開視線，返回起始點");
        isChasing = false;
        isReturning = true;
        SpeedUpAnimator();
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
    protected virtual void MoveForward()
    {
        SpeedUpAnimator();
        spider.position += currentDirection * moveSpeed * Time.deltaTime;
    }

    public void CheckCapsule()
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
        isTurning = true;
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
        isTurning = false;
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

    public void CheckBox()
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
        isTurning = true;
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
        isTurning = false;
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

    

    public void SetCanSpiderMove(bool value)
    {
        canSpiderMove = value;

        if (!canSpiderMove)
        {
            canMove = false;
            isChasing = false;
            isReturning = false;
            animControl.speed = 0f; // 停止動畫
            Debug.Log("[RedSpiderAI] 蜘蛛暫停行動");
        }
        else
        {
            animControl.speed = 1f; // 恢復動畫
            Debug.Log("[RedSpiderAI] 蜘蛛恢復行動");
        }
    }

    public bool IsSpiderMovable()
    {
        return canSpiderMove;
    }
    #endregion

    #region --- Collider 判斷 ---
    private bool IsInsideCollider(Collider col, Vector3 point)
    {
        Vector3 closest = col.ClosestPoint(point);
        return closest == point;
    }
    #endregion

    public void MakeDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        SoundManager.Play("SFX_Death_V1");
        Instantiate(burstBlood, transform.position, Quaternion.Euler(0, -90, 0));
    }
}
