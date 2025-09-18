using UnityEngine;

// 掛在「外部 3D 洞口 Trigger」物件上（該物件要有 Collider 且 isTrigger=true）
public class Hole3DTrigger : MonoBehaviour
{
    private DoubleHoleSystem system;
    private CameraViewToggle viewToggle;
    private int pairIndex;
    private HoleSide side;
    private bool isInTheTrigger = false;

    private void Start()
    {
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        system = GameObject.Find("DoubleHoleManager").GetComponent<DoubleHoleSystem>();
    }

    // 初始化方法，由 DoubleHoleSystem 呼叫
    public void Init(DoubleHoleSystem system, int pairIndex, HoleSide side)
    {
        this.system = system;
        this.pairIndex = pairIndex;
        this.side = side;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !viewToggle.Is2D() && isInTheTrigger == false)
        {
            SoundManager.Play("Transition - Sound Effects");
            isInTheTrigger = true;
            system.EnterFrom3D(pairIndex, side);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInTheTrigger == true)
        {
            isInTheTrigger = false;
        }
    }
}
