using UnityEngine;

public class OneHoleSwitchTrigger : MonoBehaviour
{
    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;
    public CameraLogic2D cameraLogic2D;

    [Header("蜘蛛的傷害腳本")]
    public SpiderHurtPlayer spiderHurtPlayer;

    [Header("蜘蛛相關設定")]
    public bool enableSpider = false;                // 是否顯示蜘蛛
    public GameObject spiderObject;                  // 被隱藏的蜘蛛物件

    [Header("蟑螂控制腳本")]
    public CockroachMove cockroachMove3D;
    public Cockroach2DMove cockroachMove2D;

    [Header("傳送位置")]
    public Transform StartPos2D;
    public Transform StartPos3D;

    [Header("攝影機限制範圍")]
    public BoxCollider2D cameraBounds;



    private bool isInTheTrigger = false;



    private void OnTriggerEnter2D(Collider2D other)
    {
        SwitchTo3DAndHideSpider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !viewToggle.Is2D())
        {
            isInTheTrigger = true;
            cameraLogic2D.SetCustomBounds(cameraBounds.bounds);
            StartCoroutine(viewToggle.StartViewSwitch(false)); //切換到2D

            // 顯示蜘蛛（如果有勾選）
            if (enableSpider)
            {
                spiderObject.SetActive(true);
                spiderHurtPlayer.ResetHurt();
            }
            cockroachMove2D.transform.position = StartPos2D.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInTheTrigger == true)
        {
            isInTheTrigger = false;
        }
    }

    public void SwitchTo3DAndHideSpider()
    {
        if (viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.StartViewSwitch(true)); // 切換到3D
            spiderObject.SetActive(false);
            cockroachMove3D.transform.position = StartPos3D.position;
        }
    }
}






