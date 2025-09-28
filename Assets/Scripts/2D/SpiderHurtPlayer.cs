using UnityEngine;
using System.Collections;

public class SpiderHurtPlayer : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    private Animator animator;
    private BoxCollider2D boxCollider;
    private Cockroach2DMove cockroach2DMove;
    private CameraLogic2D cameraLogic2D;
    private bool hasHurt = false;

    public float chaseSpeed = 2f;
    private float currentSpeed;
    public bool isChasing = false; // 直接公開給 SpiderEventTrigger 控制
    private Transform target;

    [Header("突襲設定")]
    public float burstSpeed = 15f;     // 突襲速度
    public float burstInterval = 1.5f; // 每隔多久突襲一次
    public float burstDuration = 3f;   // 突襲持續時間

    private void Awake()
    {
        target = GameObject.Find("2DCockroach").transform;

        // 矯正朝向：初始圖朝右，所以如果玩家在左邊就反轉
        if (target != null)
        {
            Vector3 scale = transform.localScale;
            if (target.position.x < transform.position.x)
            {
                scale.x = -Mathf.Abs(scale.x); // 朝左
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);  // 朝右
            }
            transform.localScale = scale;
        }
    }
    private void Start()
    {
        cockroach2DMove = GameObject.Find("2DCockroach").GetComponent<Cockroach2DMove>();
        cameraLogic2D = GameObject.Find("2DCamera").GetComponent<CameraLogic2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        currentSpeed = chaseSpeed;

        // 啟動突襲 Coroutine
        StartCoroutine(BurstSpeedRoutine());
    }

    void Update()
    {
        if (isChasing && target != null)
        {
            animator.SetBool("Moving", true);
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * currentSpeed * Time.deltaTime;

        }
    }

    private IEnumerator BurstSpeedRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(burstInterval);

            // 開始突襲
            currentSpeed = burstSpeed;
            yield return new WaitForSeconds(burstDuration);

            // 回到平時速度
            currentSpeed = chaseSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (boxCollider != null && !hasHurt)
        {
            if (other.CompareTag("Player"))
            {
                cockroach2DMove.beenEating();
                animator.SetTrigger("Eating");   // 播放咬人動畫
                hasHurt = true;
                target = null;
                cameraLogic2D.SpiderEating(this.gameObject);

                AllGameManager AGM = FindAnyObjectByType<AllGameManager>();
                AGM.InRoundKilledBySpider++;
            }
        }
    }
}
