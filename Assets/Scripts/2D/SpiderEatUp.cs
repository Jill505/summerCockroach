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
        // �����o������
        GameObject parentObj = transform.parent != null ? transform.parent.gameObject : null;

        // �R���ۤv
        Destroy(gameObject);

        // �R��������]�p�G�s�b�^
        if (parentObj != null)
        {
            Destroy(parentObj);
        }
    }
}
