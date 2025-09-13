using UnityEngine;

public class SpiderEatUp : MonoBehaviour
{
    private CockroachManager cockroachManager;
    private CameraLogic2D cameraLogic2D;
    private void Start()
    {
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        cameraLogic2D = GameObject.Find("2DCamera").GetComponent<CameraLogic2D>();
    }

    public void EatUp()
    {
        cockroachManager.CockroachDie();
        cameraLogic2D.spiderEating = false;
        DestroySelfAndParent();
    }

    public void DestroySelfAndParent()
    {
        // 先取得母物件
        GameObject parentObj = transform.parent != null ? transform.parent.gameObject : null;

        // 刪除自己
        Destroy(gameObject);

        // 刪除母物件（如果存在）
        if (parentObj != null)
        {
            Destroy(parentObj);
        }
    }
}
