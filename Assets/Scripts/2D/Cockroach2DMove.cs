using UnityEngine;

public class Cockroach2DMove : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private CockroachMove mainMoveScript;

    [Header("元件")]
    public Rigidbody2D myRb;
    public BoxCollider2D myCol;
    public Transform mainObjectTransform; // 用來顯示蟑螂
    public Animator animator;

    [Header("移動設定")]
    public float moveSpeed = 4f;
    public float runSpeed = 2f;
    public float rayLength = 1.2f;
    public LayerMask groundLayer;

    private void Start()
    {
        mainMoveScript = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
    }

    [System.Obsolete]
    void Update()
    {

        if (mainMoveScript.myMoveMode == moveMode.twoDMove)
        {
            float moveX = 0f;
            if (Input.GetKey(KeyCode.A)) moveX = -1f;
            else if (Input.GetKey(KeyCode.D)) moveX = 1f;

            // ======= 左鍵衝刺功能沿用 =======
            float currentSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) && mainMoveScript.runAbleTimeCal > 0)
            {
                currentSpeed = moveSpeed * runSpeed;
                mainMoveScript.runNotCDCal = mainMoveScript.runNotCD;
                mainMoveScript.runAbleTimeCal -= Time.deltaTime;
            }
            else
            {
                mainMoveScript.runNotCDCal -= Time.deltaTime;
                if (mainMoveScript.runNotCDCal < 0)
                {
                    mainMoveScript.runAbleTimeCal += mainMoveScript.runRecoverPerSec * Time.deltaTime;
                    if (mainMoveScript.runAbleTimeCal > mainMoveScript.runAbleTime)
                    {
                        mainMoveScript.runAbleTimeCal = mainMoveScript.runAbleTime;
                    }
                }
            }


            // Raycast 偵測地面
            Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * (myCol.size.y * 0.5f - 0.05f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);

            // 左右翻轉
            Vector3 scale = mainObjectTransform.localScale;
            if (moveX > 0)
            {
                scale.x = 1;
                // 碰撞箱 offset 恢復正值
                Vector2 offset = myCol.offset;
                offset.x = Mathf.Abs(offset.x);
                myCol.offset = offset;
            }
            else if (moveX < 0)
            {
                scale.x = -1;
                // 碰撞箱 offset 取反，鏡像翻轉碰撞箱
                Vector2 offset = myCol.offset;
                offset.x = -Mathf.Abs(offset.x);
                myCol.offset = offset;
            }
            mainObjectTransform.localScale = scale;

            if (hit.collider != null)
            {
                Vector2 normal = hit.normal;
                Vector2 tangent = new Vector2(normal.y, -normal.x);

                // 沿切線方向移動
                Vector2 targetVelocity = tangent.normalized * moveX * currentSpeed;
                myRb.velocity = new Vector2(targetVelocity.x, myRb.velocity.y);

                // 計算角度，讓蟑螂跟地形傾斜對齊
                float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
                mainObjectTransform.rotation = Quaternion.Euler(0, 0, angle);                
            }
            else
            {
                // 無地面時直接水平移動，角度回正
                myRb.velocity = new Vector2(moveX * currentSpeed, myRb.velocity.y);
                mainObjectTransform.rotation = Quaternion.identity;
            }

            // 動畫控制
            animator.SetBool("isMoving", Mathf.Abs(myRb.velocity.x) > 0.01f);
        }
        else
        {
            // 非移動模式停下
            myRb.velocity = new Vector2(0f, myRb.velocity.y);
            animator.SetBool("isMoving", false);
        }
    }
}
