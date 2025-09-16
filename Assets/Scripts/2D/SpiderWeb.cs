using UnityEngine;
using UnityEngine.UI;

public class SpiderWeb : MonoBehaviour
{
    private CockroachMove cockroachMove;

    [Header("UI 設定")]
    private GameObject spiderWebUI; // 提示用的 Image (UI)

    [Header("掙脫設定")]
    public int requiredPressCount = 10; // 需要按 F 的次數
    private int currentPressCount = 0;

    private bool isTrapped = false;

    void Start()
    {
        spiderWebUI = Scene2DManager.Instance.Ftip.gameObject;
        cockroachMove = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        if (spiderWebUI != null)
            spiderWebUI.SetActive(false); // 開始時隱藏提示
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTrapped)
        {
            // 玩家被網住
            isTrapped = true;
            cockroachMove.myMoveMode = moveMode.SpiderEvent; // 停止移動
            currentPressCount = 0;

            if (spiderWebUI != null)
                spiderWebUI.SetActive(true); // 顯示掙脫提示
        }
    }

    void Update()
    {
        if (isTrapped)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentPressCount++;

                if (currentPressCount >= requiredPressCount)
                {
                    ReleasePlayer();
                }
            }
        }
    }

    void ReleasePlayer()
    {
        isTrapped = false;
        cockroachMove.myMoveMode = moveMode.twoDMove; // 恢復移動

        if (spiderWebUI != null)
            spiderWebUI.SetActive(false); // 掙脫後隱藏提示

        // 隱藏蜘蛛網物件 (可改成 Destroy(gameObject) 直接刪除)
        gameObject.SetActive(false);
    }
}
