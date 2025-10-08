using System.Collections;
using UnityEngine;

public class BlueSpiderAI : RedSpiderAI
{
    [Header("Blue Spider Settings")]
    public float jumpForce = 5f;           // 跳躍高度
    public float patrolJumpInterval = 2f;  // 巡邏跳躍間隔
    public float chaseJumpInterval = 0.5f; // 追擊跳躍間隔更快
    private bool isJumping = false;

    public float patrolMoveSpeed = 13f;    // 巡邏移動速度
    public float chaseMoveSpeed = 20f;     // 追擊移動速度更快

    protected override void MoveForward()
    {
        if (!isJumping&& !isTurning)
        {
            StartCoroutine(JumpForward(false)); // 巡邏模式
        }
    }

    protected override void Chase()
    {
        if (!isJumping && currentChaseTarget != null)
        {
            StartCoroutine(JumpForward(true)); // 追擊模式
        }

        if (currentChaseTarget == null)
        {
            StopChasingAndReturn();
            return;
        }

        // 旋轉面向目標
        Vector3 dirToTarget = (currentChaseTarget.position - spider.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(-dirToTarget.x, 0, -dirToTarget.z));
        spider.rotation = Quaternion.RotateTowards(spider.rotation, targetRot, turnSpeed * Time.deltaTime);
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
            // 每次跳躍前自動修正方向指向玩家/目標
            jumpDir = (currentChaseTarget.position - spider.position).normalized;
        }
        else
        {
            jumpDir = currentDirection.normalized;
        }

        float jumpDuration = 0.5f; // 跳躍時間，可調整
        float elapsed = 0f;
        Vector3 startPos = spider.position;
        Vector3 endPos = spider.position + jumpDir * moveSpeed * jumpInterval;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            // 弧線公式 y = 4h * t * (1 - t)
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

        CheckCapsule();
        CheckBox();
        SlowDownAnimator();

        yield return new WaitForSeconds(jumpInterval);
        isJumping = false;
    }
}
