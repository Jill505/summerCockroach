using UnityEngine;

// 掛在「洞穴內 2D 洞口 Trigger」物件上（該物件要有 Collider2D 且 isTrigger=true）
public class Hole2DTrigger : MonoBehaviour
{
    public DoubleHoleSystem system;
    public HoleSide side;

    private void Start()
    {
        system = GameObject.Find("DoubleHoleManager").GetComponent<DoubleHoleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Cockroach2DMove>() != null)
        {
            system.ExitTo3D(side);
        }
    }
}
