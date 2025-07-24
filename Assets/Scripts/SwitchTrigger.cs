using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;

    [Header("蜘蛛的傷害腳本")]
    public SpiderHurtPlayer spiderHurtPlayer;

    [Header("蜘蛛相關設定")]
    public bool enableSpider = false;                // 是否顯示蜘蛛
    public GameObject spiderObject;                  // 被隱藏的蜘蛛物件



    private void OnTriggerEnter2D(Collider2D other)
    {
        SwitchTo3DAndHideSpider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.SwitchViewWithBlackout(true)); //切換到2D

            // 顯示蜘蛛（如果有勾選）
            if (enableSpider)
            {
                spiderObject.SetActive(true);
                spiderHurtPlayer.ResetHurt();
            }
        }
    }
    
    public void SwitchTo3DAndHideSpider()
    {
        if (viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.SwitchViewWithBlackout(false)); // 切換到3D
            spiderObject.SetActive(false);
        }
    }
}






