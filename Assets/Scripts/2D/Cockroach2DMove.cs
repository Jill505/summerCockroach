using UnityEngine;

public class Cockroach2DMove : MonoBehaviour
{
    public CockroachMove mainMoveScript;
    public Rigidbody2D myRb;
    public float moveSpeed = 7f;
    public Transform mainObjectTransform;

    [System.Obsolete]
    void Update()
    {
        // 只有在 moveMode 是 twoDMove 才允許動作
        if (mainMoveScript.myMoveMode == moveMode.twoDMove)
        {
            float moveX = 0f;

            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveX = 1f;
            }

            Vector3 moveDir = new Vector3(moveX, 0, 0);
            myRb.velocity = new Vector3(moveDir.x * moveSpeed, myRb.velocity.y, 0f);

            // 角色左右翻轉（可選）
            if (moveX != 0)
            {
                Vector3 scale = mainObjectTransform.localScale;

                if (moveX > 0)
                {
                    scale.x = 1;
                }
                else
                {
                    scale.x = -1;
                }
                mainObjectTransform.localScale = scale;
            }

        }
        else
        {
            myRb.velocity = new Vector2(0f, myRb.velocity.y);
        }


    }
}