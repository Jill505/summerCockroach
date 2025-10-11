using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpiderAI : MonoBehaviour
{
    [Header("References")]
    protected CockroachMove cockroachMove; // private �� protected
    public Transform spider;
    public BoxCollider moveRangeBox;
    public CapsuleCollider centerCapsule;
    public CapsuleCollider spiderDetection;
    public Transform startPos;
    public Animator animControl;
    protected Transform player;           // private �� protected
    protected List<Transform> chaseTargets = new List<Transform>(); // private �� protected

    [Header("Movement Settings")]
    protected float moveSpeed = 13f;          // private �� protected
    protected float chaseSpeed = 20f;       // private �� protected
    protected float turnSpeed = 180f;        // private �� protected
    protected float idleTime = 8f;
    protected float fadeDuration = 0.5f;

    protected bool canMove = false;          // private �� protected
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

    [Header("AI ���A����")]
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
            Debug.Log($"[RedSpiderAI] ��쪱�a����: {player.name}");
        }
        else
        {
            Debug.LogWarning("[RedSpiderAI] �������䤣�� Tag �� 'Player' ������I");
        }

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPCRoach");
        foreach (GameObject npc in npcs)
            chaseTargets.Add(npc.transform);

        Debug.Log("[RedSpiderAI] ��l�Ƨ����A�}�l Idle");
        StartCoroutine(IdleThenStartMoving());
    }

    private void Update()
    {
        if (!canSpiderMove)
        {
            return; // �������� AI ��s
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
            return; //  �Ȱ��Ҧ����z�P�����޿�
        }
        // �����w�����ؼ�
        CleanChaseTargets();

        if (!isChasing && !isReturning)
        {
            CheckCapsule();
            CheckBox();
        }

        DetectTarget();
    }

    #region --- ���a�����P�l���޿� ---
    private void DetectTarget()
    {
        Transform nearestTarget = null;
        float nearestDist = float.MaxValue;

        // ��X�̪�B�b�����d�򤺪��ؼ�
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
        Debug.Log($"[RedSpiderAI] ������ؼ� {target.name}�A�i�J�l���Ҧ�");
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
            Debug.Log("[RedSpiderAI] �l����H�����γQ�R���A����l���ê�^�_�I");
            StopChasingAndReturn();
            return;
        }
        if (currentChaseTarget == player && cockroachMove != null && cockroachMove.isInTheHole)
        {
            Debug.Log("[RedSpiderAI] ���a�i�J�}���A�j�ﰱ��l���ê�^�_�I");
            currentChaseTarget = null;
            StopChasingAndReturn();
            return;
        }

        // ���ୱ�V�ؼ�
        Vector3 dirToTarget = (currentChaseTarget.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToTarget.x, 0, -dirToTarget.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);

        // �e�i
        spider.position += -spider.forward * chaseSpeed * Time.deltaTime;
    }

    private void CleanChaseTargets()
    {
        for (int i = chaseTargets.Count - 1; i >= 0; i--)
        {
            if (chaseTargets[i] == null)
            {
                chaseTargets.RemoveAt(i);
                Debug.Log("[RedSpiderAI] �����w���`�ή������ؼ�");
            }
        }
    }

    protected virtual void StopChasingAndReturn()
    {
        Debug.Log("[RedSpiderAI] ���a���}���u�A��^�_�l�I");
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

    public void CheckBox()
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
        isTurning = true;
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
        isTurning = false;
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

    

    public void SetCanSpiderMove(bool value)
    {
        canSpiderMove = value;

        if (!canSpiderMove)
        {
            canMove = false;
            isChasing = false;
            isReturning = false;
            animControl.speed = 0f; // ����ʵe
            Debug.Log("[RedSpiderAI] �j��Ȱ����");
        }
        else
        {
            animControl.speed = 1f; // ��_�ʵe
            Debug.Log("[RedSpiderAI] �j���_���");
        }
    }

    public bool IsSpiderMovable()
    {
        return canSpiderMove;
    }
    #endregion

    #region --- Collider �P�_ ---
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
