using System.Collections;
using UnityEngine;

public class CameraViewToggle : MonoBehaviour
{
    [Header("攝影機")]
    public Camera camera3D;
    public Camera camera2D;

    [Header("2D 視角設定")]
    public float orthographicSize = 5f;

    [Header("3D 視角設定")]
    public float fieldOfView = 60f;

    [Header("蟑螂控制腳本")]
    public CockroachMove cockroachMove;

    [Header("轉場設定")]
    public GameObject transitionQuad3D;
    public GameObject transitionQuad2D;
    public Material transitionMaterial;

    [Header("轉場動畫時間（秒）")]
    public float transitionDuration = 1f;
    public float scale = 30f;

    [Header("切換視角時蟑螂的位置設定")]
    public Transform cockroach2DPos;
    public Transform cockroach2DStartPoint;

    private bool is2D = false;
    private bool isSwitching = false;

    private void Start()
    {
        if (transitionQuad3D != null) transitionQuad3D.SetActive(false);
        if (transitionQuad2D != null) transitionQuad2D.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isSwitching) //forTest
        {
            if (is2D == true)
                StartCoroutine(StartViewSwitch(true));
            else
                StartCoroutine(StartViewSwitch(false));
        }
    }

    public IEnumerator StartViewSwitch(bool is2D)
    {
        isSwitching = true;
        cockroachMove.myMoveMode = moveMode.ChangeSceneMoment;

        if (is2D == true)
        {
            transitionQuad2D.SetActive(true);
            yield return StartCoroutine(AnimateShaderScale(0f, scale, transitionDuration));
            SetTo3DView();
        }
            
        else if (is2D == false)
        {
            transitionQuad3D.SetActive(true);
            yield return StartCoroutine(AnimateShaderScale(0f, scale, transitionDuration));
            SetTo2DView();
        }
    }

    private IEnumerator AnimateShaderScale(float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            float value = Mathf.Lerp(start, end, t);
            transitionMaterial.SetFloat("_Scale", value);
            time += Time.deltaTime;
            yield return null;
        }

        // 最後確保設到目標值
        transitionMaterial.SetFloat("_Scale", end);
    }

    private void SetTo2DView()  
    {
        is2D = true;
        transitionQuad3D.SetActive(false);
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);

        camera2D.orthographic = true;
        camera2D.orthographicSize = orthographicSize;

        cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        camera2D.transform.position = cockroach2DStartPoint.position;

        StartCoroutine(End2DViewTransition());


    }

    private void SetTo3DView()
    {
        is2D = false;
        transitionQuad2D.SetActive(false);
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        camera2D.transform.position = cockroach2DStartPoint.position;

        StartCoroutine(End3DViewTransition());
    }

    public IEnumerator End2DViewTransition()
    {
        transitionQuad2D.SetActive(true);
        yield return StartCoroutine(AnimateShaderScale(scale, 0f, transitionDuration));
        transitionQuad2D.SetActive(false);

        cockroachMove.myMoveMode = moveMode.twoDMove;
        isSwitching = false;
    }

    public IEnumerator End3DViewTransition()
    {
        transitionQuad3D.SetActive(true);
        yield return StartCoroutine(AnimateShaderScale(scale, 0f, transitionDuration));
        transitionQuad3D.SetActive(false);

        cockroachMove.myMoveMode = moveMode.AutoCameraMove;
        isSwitching = false;
    }


    public bool Is2D()
    {
        return is2D;
    }
}

