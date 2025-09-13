using UnityEngine;

public class SpiderEventTrigger : MonoBehaviour
{

    private CockroachMove cockroachMove;
    private CameraLogic2D cameraLogic2D;
    private DoubleHoleSystem doubleHoleSystem;

    private GameObject spiderPrefab;
    private GameObject spiderInstance; // �O�s�ͦ����j��ޥ�

    private bool eventStarted = false; //  �ƥ�O�_�w�}�l
    [HideInInspector] public bool startChase = false;





    private Scene2DManager.Scene2D sceneData;
    void Start()
    {
        // ���o3D���������ʲե�
        cockroachMove = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        doubleHoleSystem = GameObject.Find("DoubleHoleManager").GetComponent<DoubleHoleSystem>();
        // ���o�������
        sceneData = Scene2DManager.Instance.GetScene(Scene2DDoubleHole.Cave);
        spiderPrefab = Scene2DManager.Instance.Spider2D.gameObject;
    }

    void Update()
    {
        if (startChase)
        {
            cockroachMove.myMoveMode = moveMode.twoDMove; // �}�񪱮a����

            if (spiderInstance != null)
            {
                var spiderHurt = spiderInstance.GetComponent<SpiderHurtPlayer>();
                if (spiderHurt != null)
                    spiderHurt.isChasing = true; // �l�v�}�l
            }
            startChase = false; // �קK����
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���]2D�������@��Tag�s "Player"
        if (other.CompareTag("Player") && !eventStarted) 
        {
            // 2D�����i�JĲ�o�I�A�T���
            eventStarted = true;

            cockroachMove.myMoveMode = moveMode.SpiderEvent;

            Transform spiderSpawnPoint = (doubleHoleSystem.lastEnterSide == HoleSide.Left)
                                         ? sceneData.insPos1
                                         : sceneData.insPos2;

             spiderInstance = Instantiate(spiderPrefab, spiderSpawnPoint.position, Quaternion.identity);


            GetComponent<Collider2D>().enabled = false;
            SpiderEvent(spiderInstance);
        }
    }

    void SpiderEvent(GameObject spider)
    {
        if (spider != null)
        {
            // �۾��ƦV�j��A1��ưʡA���d2��
            cameraLogic2D = GameObject.Find("2DCamera").GetComponent<CameraLogic2D>();
            cameraLogic2D.SetSpiderTrigger(this);
            cameraLogic2D.MoveCameraToTarget(spider, 3f, 2f);
        }

    }
}
