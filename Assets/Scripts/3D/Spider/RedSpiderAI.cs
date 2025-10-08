using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpiderAI : MonoBehaviour
{
    [Header("References")]
    public Transform spider;              // �j�糧��
    public BoxCollider moveRangeBox;      // �w�I���ʽd��
    public CapsuleCollider centerCapsule; // ���ߧP�w
    public CapsuleCollider spiderDetection;
    public Transform startPos;            // �_�l��m
    public Animator animControl;
    private Transform player;              // ���a�ؼ�

    [Header("Movement Settings")]
    public float moveSpeed = 2f;          // �e�i�t��
    public float chaseSpeed = 10f;       // �l���t��
    public float turnSpeed = 180f;        // ����t��
    public float idleTime = 8f;           // ���d�ɶ�
    public float fadeDuration = 1.5f;     // �ʵe�t���ܤƮɶ�

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
            Debug.Log($"[RedSpiderAI] ��쪱�a����: {player.name}");
        }
        else
        {
            Debug.LogWarning("[RedSpiderAI] �������䤣�� Tag �� 'Player' ������I");
        }

        Debug.Log("[RedSpiderAI] ��l�Ƨ����A�}�l Idle");
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

    #region --- ���a�����P�l���޿� ---
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
        Debug.Log("[RedSpiderAI] �����쪱�a�A�i�J�l���Ҧ�");
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

        // ���ୱ�V���a
        Vector3 dirToPlayer = (player.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToPlayer.x, 0, -dirToPlayer.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        // �e�i
        spider.position += -spider.forward * chaseSpeed * Time.deltaTime;
    }

    private void StopChasingAndReturn()
    {
        Debug.Log("[RedSpiderAI] ���a���}���u�A��^�_�l�I");
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
            Debug.Log("[RedSpiderAI] �w�^��_�I�A��_����");
            StartCoroutine(SmoothRotateToInitial());
            isReturning = false;
            StartCoroutine(IdleThenStartMoving());
            canCapsuleAction = true;
            canBoxAction = true;            
            return;
        }

        // ����^�_�I��V
        Vector3 moveDir = dirToStart.normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-moveDir.x, 0, -moveDir.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        spider.position += -spider.forward * moveSpeed * Time.deltaTime;
    }
    #endregion

    #region --- �쥻�������޿� ---
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
            Debug.Log("[RedSpiderAI] ���} CapsuleCollider�A���� CapsuleAction");
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
        Debug.Log("[RedSpiderAI] ���৹���A�}�l����");
    }

    private IEnumerator SmoothRotateToInitial()
    {
        float rotated = 0f;
        Quaternion startRot = spider.rotation;
        float totalAngle = Quaternion.Angle(startRot, initialRotation);
        float currentTurnSpeed = 0f;
        float acceleration = turnSpeed; // �C��[�t��
        float maxTurnSpeed = turnSpeed;

        while (rotated < totalAngle)
        {
            // �C�V�W�[����t��
            currentTurnSpeed += acceleration * Time.deltaTime;
            if (currentTurnSpeed > maxTurnSpeed)
                currentTurnSpeed = maxTurnSpeed;

            // �p��o�V���ਤ��
            float step = currentTurnSpeed * Time.deltaTime;

            // ����W�L�`����
            if (rotated + step > totalAngle)
                step = totalAngle - rotated;

            // ���Ʊ���
            spider.rotation = Quaternion.RotateTowards(spider.rotation, initialRotation, step);
            rotated += step;

            yield return null;
        }

        spider.rotation = initialRotation; // �O�ҳ̲׹��
        Debug.Log("[RedSpiderAI] ���Ʀ^������");
    }

    private void CheckBox()
    {
        bool inside = IsInsideCollider(moveRangeBox, spider.position);

        if (wasInsideBox && !inside && canBoxAction)
        {
            canBoxAction = false;
            Debug.Log("[RedSpiderAI] ���} BoxCollider�A���� BoxAction");
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

        Debug.Log("[RedSpiderAI] BoxAction �}�l���� 180 ��");
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

    #region --- Idle & Animator ���� ---
    private IEnumerator IdleThenStartMoving()
    {
        canMove = false;
        SlowDownAnimator();
        yield return new WaitForSeconds(idleTime);
        canMove = true;
        Debug.Log("[RedSpiderAI] Idle �����A�}�l����");
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

    #region --- Collider �P�_ ---
    private bool IsInsideCollider(Collider col, Vector3 point)
    {
        Vector3 closest = col.ClosestPoint(point);
        return closest == point;
    }
    #endregion
}
