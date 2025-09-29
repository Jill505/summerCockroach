using UnityEngine;

public class SpiderEatUp : MonoBehaviour
{
    private CockroachManager cockroachManager;
    private AllGameManager allGameManager;
    private CameraLogic2D cameraLogic2D;
    private void Start()
    {
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        cameraLogic2D = GameObject.Find("2DCamera").GetComponent<CameraLogic2D>();
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
    }

    public void EatUp()
    {
        //cockroachManager.CockroachDie();
        SoundManager.Play("SFX_SpiderCrunchy-bite");
        cockroachManager.CockroachInjury(2, "�o�@�@�A�ڳQ�j������F");
        cameraLogic2D.spiderEating = false;
        DestroySelfAndParent();
    }

    public void DestroySelfAndParent()
    {
        // �����o������
        GameObject parentObj = transform.parent != null ? transform.parent.gameObject : null;
        allGameManager.AddScore(allGameManager.OutTheSpiderHole);

        // �R���ۤv
        Destroy(gameObject);

        // �R��������]�p�G�s�b�^
        if (parentObj != null)
        {
            Destroy(parentObj);
        }
    }
    public void SpiderMovingSound()
    {
        SoundManager.Play("SFX_SpiderMoving");
    }
}
