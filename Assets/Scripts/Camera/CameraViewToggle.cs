using UnityEngine;

public class CameraViewToggle : MonoBehaviour
{
    private bool is2D = false;

    [Header("攝影機")]
    public Camera camera3D;   // 指定 Main Camera（3D 用）
    public Camera camera2D;   // 指定 2D Camera（2D 用）

    [Header("2D 視角設定")]
    public float orthographicSize = 5f;

    [Header("3D 視角設定")]
    public float fieldOfView = 60f;

    [Header("蟑螂控制腳本")]
    public CockroachMove cockroachMove;

    private void Start()
    {
        SwitchTo3D(); // 預設為 3D 模式
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (is2D)
            {
                SwitchTo3D();
            }
            else
            {
                SwitchTo2D();
            }
        }
    }

    public void SwitchTo2D()
    {
        is2D = true;
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);

        camera2D.orthographic = true;
        camera2D.orthographicSize = orthographicSize;

        cockroachMove.myMoveMode = CockroachMove.moveMode.twoDMove;
    }

    public void SwitchTo3D()
    {
        is2D = false;
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        cockroachMove.myMoveMode = CockroachMove.moveMode.AutoCameraMove;
    }

    public bool Is2D()
    {
        return is2D;
    }
}
