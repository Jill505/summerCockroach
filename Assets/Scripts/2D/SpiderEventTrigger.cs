using UnityEngine;

public class SpiderEventTrigger : MonoBehaviour
{

    private CockroachMove cockroachMove;
    private CameraLogic2D cameraLogic2D;
    private DoubleHoleSystem doubleHoleSystem;

    private GameObject spiderPrefab;
    private GameObject spiderInstance; // 保存生成的蜘蛛引用

    private bool eventStarted = false; //  事件是否已開始
    [HideInInspector] public bool startChase = false;





    private Scene2DManager.Scene2D sceneData;
    void Start()
    {
        // 取得3D蟑螂的移動組件
        cockroachMove = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
        doubleHoleSystem = GameObject.Find("DoubleHoleManager").GetComponent<DoubleHoleSystem>();
        // 取得場景資料
        sceneData = Scene2DManager.Instance.GetScene(Scene2DDoubleHole.Cave);
        spiderPrefab = Scene2DManager.Instance.Spider2D.gameObject;
    }

    void Update()
    {
        if (startChase)
        {
            cockroachMove.myMoveMode = moveMode.twoDMove; // 開放玩家移動

            if (spiderInstance != null)
            {
                var spiderHurt = spiderInstance.GetComponent<SpiderHurtPlayer>();
                if (spiderHurt != null)
                    spiderHurt.isChasing = true; // 追逐開始
            }
            startChase = false; // 避免重複
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 假設2D蟑螂有一個Tag叫 "Player"
        if (other.CompareTag("Player") && !eventStarted) 
        {
            // 2D蟑螂進入觸發點，禁止移動
            eventStarted = true;

            cockroachMove.myMoveMode = moveMode.SpiderEvent;

            Transform spiderSpawnPoint = (doubleHoleSystem.lastEnterSide == HoleSide.Left)
                                         ? sceneData.insPos1
                                         : sceneData.insPos2;
            

             spiderInstance = Instantiate(spiderPrefab, spiderSpawnPoint.position, Quaternion.identity);

            if (spiderSpawnPoint == sceneData.insPos1)
            {
                SetSpiderWebActive(Scene2DManager.Instance.RSpiderweb.gameObject, true);
            }
            else if (spiderSpawnPoint == sceneData.insPos2)
            {
                SetSpiderWebActive(Scene2DManager.Instance.LSpiderweb.gameObject, true);
            }

            GetComponent<Collider2D>().enabled = false;
            SpiderEvent(spiderInstance);
        }
    }

    private void SetSpiderWebActive(GameObject spiderWebRoot, bool active)
    {
        if (spiderWebRoot == null) return;

        // 啟用本體
        spiderWebRoot.SetActive(active);

        // 啟用子物件
        for (int i = 0; i < spiderWebRoot.transform.childCount; i++)
        {
            spiderWebRoot.transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    void SpiderEvent(GameObject spider)
    {
        if (spider != null)
        {
            // 相機滑向蜘蛛，1秒滑動，停留2秒
            cameraLogic2D = GameObject.Find("2DCamera").GetComponent<CameraLogic2D>();
            cameraLogic2D.SetSpiderTrigger(this);
            cameraLogic2D.MoveCameraToTarget(spider, 3f, 0.7f);
        }

    }

    public void EndSpiderEvent()
    {
        // 刪除蜘蛛
        if (spiderInstance != null)
        {
            Destroy(spiderInstance);
            spiderInstance = null;
        }

        // 隱藏右邊蜘蛛網
        if (Scene2DManager.Instance.RSpiderweb != null)
            SetSpiderWebActive(Scene2DManager.Instance.RSpiderweb.gameObject, false);

        // 隱藏左邊蜘蛛網
        if (Scene2DManager.Instance.LSpiderweb != null)
            SetSpiderWebActive(Scene2DManager.Instance.LSpiderweb.gameObject, false);

        // 開放玩家移動
        if (cockroachMove != null)
            cockroachMove.myMoveMode = moveMode.twoDMove;

        // 如果需要，重置事件狀態
        eventStarted = false;
        GetComponent<Collider2D>().enabled = true;
    }
}
