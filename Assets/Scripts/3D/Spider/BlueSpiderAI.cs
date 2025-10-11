using System.Collections;
using UnityEngine;

public class BlueSpiderAI : RedSpiderAI
{
    [Header("Blue Spider Settings")]
    public float jumpForce = 5f;           // ���D����
    public float patrolJumpInterval = 2f;  // ���޸��D���j
    public float chaseJumpInterval = 0.5f; // �l�����D���j���
    private bool isJumping = false;

    public float patrolMoveSpeed = 13f;    // ���޲��ʳt��
    public float chaseMoveSpeed = 20f;     // �l�����ʳt�ק��

    protected override void MoveForward()
    {
        if (!isJumping&& !isTurning)
        {
            StartCoroutine(JumpForward(false)); // ���޼Ҧ�
        }
    }

    protected override void Chase()
    {
        if (!isJumping && currentChaseTarget != null)
        {
            StartCoroutine(JumpForward(true)); // �l���Ҧ�
        }

        if (currentChaseTarget == null)
        {
            StopChasingAndReturn();           
            return;
        }

        if (currentChaseTarget == player && cockroachMove != null && cockroachMove.isInTheHole)
        {
            Debug.Log("[RedSpiderAI] ���a�i�J�}���A�j�ﰱ��l���ê�^�_�I");
            SoundManager.StopSpiderChaseSound();
            hasPlayedChaseSound = false;
            currentChaseTarget = null;
            StopChasingAndReturn();
            return;
        }
        if (!IsInsideCollider(spiderDetection, player.position) && currentChaseTarget == player)
        {
            Debug.Log("[RedSpiderAI] ���a�]���F�A�j�ﰱ��l���ê�^�_�I");
            currentChaseTarget = null;
            StopChasingAndReturn();
            SoundManager.StopSpiderChaseSound();
            hasPlayedChaseSound = false;
            return;
        }

        // ���ୱ�V�ؼ�
        Vector3 dirToTarget = (currentChaseTarget.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToTarget.x, 0, -dirToTarget.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    protected override void StopChasingAndReturn()
    {
        Debug.Log("[BlueSpiderAI] ���a���}���u�A��^�_�l�I");
        isChasing = false;
        isReturning = true;
        SpeedUpAnimator();
    }

    private IEnumerator JumpForward(bool isChase)
    {
        isJumping = true;
        SpeedUpAnimator();

        float jumpInterval = isChase ? chaseJumpInterval : patrolJumpInterval;
        float moveSpeed = isChase ? chaseMoveSpeed : patrolMoveSpeed;

        Vector3 jumpDir;
        if (isChase && currentChaseTarget != null)
        {
            // �C�����D�e�۰ʭץ���V���V���a/�ؼ�
            jumpDir = (currentChaseTarget.position - spider.position).normalized;
        }
        else
        {
            jumpDir = currentDirection.normalized;
        }

        float jumpDuration = 0.5f; // ���D�ɶ��A�i�վ�
        float elapsed = 0f;
        Vector3 startPos = spider.position;
        Vector3 endPos = spider.position + jumpDir * moveSpeed * jumpInterval;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            // ���u���� y = 4h * t * (1 - t)
            float height = 4 * jumpForce * t * (1 - t);
            spider.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, height, 0);

            if (isChase && currentChaseTarget != null)
            {
                Vector3 dirToTarget = (currentChaseTarget.position - spider.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToTarget.x, 0, -dirToTarget.z));
                spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            yield return null;
        }

        spider.position = endPos;

        SlowDownAnimator();

        yield return new WaitForSeconds(jumpInterval);

        
        isJumping = false;
    }
}
