using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    [Header("控制相機與角色的腳本")]
    public CameraViewToggle viewToggle;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (viewToggle.Is2D())
        {
            Debug.Log("2D碰撞，切換為3D");
            viewToggle.SwitchTo3D();
        }
        else
        {
            Debug.Log("2D碰撞，切換為2D（沒變）");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (viewToggle.Is2D())
        {
            Debug.Log("3D碰撞，切換為3D（沒變）");
        }
        else
        {
            Debug.Log("3D碰撞，切換為2D");
            viewToggle.SwitchTo2D();
        }
    }
}
