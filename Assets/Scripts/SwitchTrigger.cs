using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.SwitchViewWithBlackout(false)); //切換到3d
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!viewToggle.Is2D())
        {
            StartCoroutine(viewToggle.SwitchViewWithBlackout(true));//切換到2d
        }
    }
}
