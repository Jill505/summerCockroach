using UnityEngine;

public class DoubleHoleSwitchManager : MonoBehaviour
{
    [System.Serializable]
    public class HoleData
    {
        public Collider holeTrigger3D;    // 3D 洞口的碰撞器
        public Collider2D holeTrigger2D;  // 2D 洞口的碰撞器
        public Transform exitPoint3D;     // 3D 出口位置
        public Transform exitPoint2D;     // 2D 出口位置
    }

    [Header("左右洞口")]
    public HoleData leftHole;
    public HoleData rightHole;

    [Header("蜘蛛的傷害腳本")]
    public SpiderHurtPlayer spiderHurtPlayer;

    [Header("蜘蛛相關設定")]
    public bool enableSpider = false;
    public GameObject spiderObject;

    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;

    [Header("蟑螂控制腳本")]
    public CockroachMove cockroachMove3D;
    public Cockroach2DMove cockroachMove2D;

    private bool isInTheTrigger = false;

    private void Start()
    {
        // 綁定事件
        if (leftHole.holeTrigger3D != null) leftHole.holeTrigger3D.gameObject.AddComponent<HoleTriggerBinder>().Setup(this, true, true);
        if (rightHole.holeTrigger3D != null) rightHole.holeTrigger3D.gameObject.AddComponent<HoleTriggerBinder>().Setup(this, false, true);
        if (leftHole.holeTrigger2D != null) leftHole.holeTrigger2D.gameObject.AddComponent<HoleTriggerBinder2D>().Setup(this, true, false);
        if (rightHole.holeTrigger2D != null) rightHole.holeTrigger2D.gameObject.AddComponent<HoleTriggerBinder2D>().Setup(this, false, false);
    }

    public void EnterHole(bool isLeft, bool from3D)
    {
        if (from3D && !viewToggle.Is2D())
        {
            // 3D → 2D
            Vector3 targetPos = isLeft ? leftHole.exitPoint2D.position : rightHole.exitPoint2D.position;
            StartCoroutine(viewToggle.StartViewSwitch(false)); // 切到 2D
            if (enableSpider)
            {
                spiderObject.SetActive(true);
                spiderHurtPlayer.ResetHurt();
            }
            // 傳送 2D 角色
            // twoDCockroach.transform.position = targetPos;
            cockroachMove2D.transform.position = targetPos;


            Debug.Log($"[傳送] 3D → 2D 從 {(isLeft ? "左" : "右")} 進，傳到 {targetPos}");
        }
        else if (!from3D && viewToggle.Is2D())
        {
            // 2D → 3D
            Vector3 targetPos = isLeft ? leftHole.exitPoint3D.position : rightHole.exitPoint3D.position;
            StartCoroutine(viewToggle.StartViewSwitch(true)); // 切到 3D
            spiderObject.SetActive(false);
            // 傳送 3D 角色
            // threeDCockroach.transform.position = targetPos;
            cockroachMove3D.transform.position = targetPos;
            Debug.Log($"[傳送] 2D → 3D 從 {(isLeft ? "左" : "右")} 出，傳到 {targetPos}");
        }
    }
}

// 3D 觸發器
public class HoleTriggerBinder : MonoBehaviour
{
    private DoubleHoleSwitchManager manager;
    private bool isLeft;
    private bool from3D;

    public void Setup(DoubleHoleSwitchManager m, bool left, bool from3DWorld)
    {
        manager = m;
        isLeft = left;
        from3D = from3DWorld;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.EnterHole(isLeft, from3D);
        }
    }
}

// 2D 觸發器
public class HoleTriggerBinder2D : MonoBehaviour
{
    private DoubleHoleSwitchManager manager;
    private bool isLeft;
    private bool from3D;

    public void Setup(DoubleHoleSwitchManager m, bool left, bool from3DWorld)
    {
        manager = m;
        isLeft = left;
        from3D = from3DWorld;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            manager.EnterHole(isLeft, from3D);
        }
    }
}
