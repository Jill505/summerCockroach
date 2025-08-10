using UnityEngine;

public class SpiderHurtPlayer : MonoBehaviour
{
    [Header("設定蟑螂管理腳本")]
    public CockroachManager cockroachManager;
    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;

    [Header("視角切換與蜘蛛控制腳本")]
    public OneHoleSwitchTrigger switchTrigger;


    private bool hasHurt = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHurt) return;

        if (other.CompareTag("Player"))
        {
            cockroachManager.CockroachInjury(1);
            Debug.Log("扣血了，目前血量: " + cockroachManager.Hp);
            hasHurt = true;
            switchTrigger.SwitchTo3DAndHideSpider();
        }
    }

    public void ResetHurt()
    {
        hasHurt = false;
    }
}
