using Unity.VisualScripting;
using UnityEngine;

public class AlwaysFaceToCameraObject : MonoBehaviour
{
    private Transform cam;


    void LateUpdate()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
