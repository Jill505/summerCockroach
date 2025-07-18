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

        // 播放黑幕遮蓋動畫
        BlackScene.SetBool("LoadOut",true);
        yield return new WaitForSeconds(1.2f); // 根據動畫時間調整

        // 切換視角
        if (to2D)
            SwitchTo2D();
        else
            SwitchTo3D();

        // 黑幕淡出動畫（可加 LoadIn 觸發）
        BlackScene.SetBool("LoadOut",false);

        yield return new WaitForSeconds(1.2f); // 保險的延遲
        isSwitching = false;
    }

    public void SwitchTo2D()
    {
        is2D = true;
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);

        camera2D.orthographic = true;
        camera2D.orthographicSize = orthographicSize;

        cockroachMove.myMoveMode = moveMode.twoDMove;
    }

    public void SwitchTo3D()
    {
        is2D = false;
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        cockroachMove.myMoveMode = moveMode.AutoCameraMove;
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
