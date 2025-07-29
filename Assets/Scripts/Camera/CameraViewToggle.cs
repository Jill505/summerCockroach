using System.Collections;
using UnityEngine;

public class CameraViewToggle : MonoBehaviour
{
    private bool is2D = false;

    [Header("攝影機")]
    public Camera camera3D;
    public Camera camera2D;

    [Header("2D 視角設定")]
    public float orthographicSize = 5f;

    [Header("3D 視角設定")]
    public float fieldOfView = 60f;

    [Header("蟑螂控制腳本")]
    public CockroachMove cockroachMove;

    [Header("黑幕動畫")]
    public Animator BlackScene;

    [Header("切換視角時蟑螂的位置設定")]
    public Transform cockroach2DPos;
    public Transform cockroach2DStartPoint;

    private bool isSwitching = false; // 避免切換過程中重複觸發

    private void Start()
    {
        SwitchTo3D_Immediate(); // 預設為 3D，不用動畫
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isSwitching)
        {
            if (is2D)
                StartCoroutine(SwitchViewWithBlackout(false));
            else
                StartCoroutine(SwitchViewWithBlackout(true));
        }
    }

    public IEnumerator SwitchViewWithBlackout(bool to2D)
    {
        isSwitching = true;
        cockroachMove.myMoveMode = moveMode.ChangeSceneMoment;

        // 播放黑幕遮蓋動畫
        BlackScene.SetBool("LoadOut", true);
        yield return new WaitForSeconds(1.2f); // 根據動畫時間調整

        // 切攝影機
        if (to2D)
            SetTo2DView_OnlyCamera();
        else
            SetTo3DView_OnlyCamera();

        // 黑幕淡出動畫
        BlackScene.SetBool("LoadOut", false);
        yield return new WaitForSeconds(0.8f); // 等淡出完成再切換控制模式

        if (to2D)
            cockroachMove.myMoveMode = moveMode.twoDMove;
        else
            cockroachMove.myMoveMode = moveMode.AutoCameraMove;

        isSwitching = false;
    }

    private void SetTo2DView_OnlyCamera()
    {
        is2D = true;
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);

        camera2D.orthographic = true;
        camera2D.orthographicSize = orthographicSize;

        cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        camera2D.transform.position = cockroach2DStartPoint.position;
    }

    private void SetTo3DView_OnlyCamera()
    {
        is2D = false;
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        camera2D.transform.position = cockroach2DStartPoint.position;
    }

    public bool Is2D()
    {
        return is2D;
    }

    // 不播動畫的立即切換（初始用）
    public void SwitchTo3D_Immediate()
    {
        is2D = false;
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        cockroachMove.myMoveMode = moveMode.AutoCameraMove;
    }
}
