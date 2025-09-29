using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public CockroachManager cockroachManager;
    public CameraLogic2D cameraLogic2D;
    public ParallaxBackground parallaxBackground;

    [Header("轉場設定")]
    public GameObject transitionQuad3D;
    public GameObject transitionQuad2D;
    public Material transitionMaterial;

    [Header("轉場動畫時間（秒）")]
    public float transitionDuration = 1f;
    public float scale = 30f;

    [Header("切換視角時蟑螂的位置設定")]
    //public Transform cockroach2DPos;
    //public Transform cockroach2DStartPoint;

    [Header("Shake Func")]
    public float CameraShakeVelocity = 0.5f;
    public float CameraShakeTime = 1f;


    private AllGameManager allGameManager;



    public bool is2D = false;

    private void Awake()
    {
        if (transitionQuad3D != null) transitionQuad3D.SetActive(false);
        if (transitionQuad2D != null) transitionQuad2D.SetActive(false);
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        cockroachManager = GameObject.Find("3DCockroach").GetComponent<CockroachManager>();
        WindGrass = GameObject.Find("Wind through Long Grass Sound Effect - Gentle Breeze");
    }

    private void Start()
    {
        StartCoroutine(Transition());
    }

    public void CallCameraShake3D()
    {

    }
    IEnumerator CameraShake()
    {
        float t = CameraShakeTime;
        
        while (t > 0)
        {

            t--;
            yield return null;
        }
        yield return null;

    }

    private IEnumerator Transition()
    {
        transitionQuad3D.SetActive(true);
        yield return StartCoroutine(AnimateShaderScale(45f, 0f, 0.6f));
        transitionQuad3D.SetActive(false);
    }

    public IEnumerator StartViewSwitch(bool is2D)
    {
        cockroachMove.myMoveMode = moveMode.ChangeSceneMoment;
        allGameManager.isTimerRunning = false;

        if (is2D == true)
        {
            transitionQuad2D.SetActive(true);
            cameraLogic2D.isFollowing = true;
            yield return StartCoroutine(AnimateShaderScale(0f, scale, transitionDuration));
            cameraLogic2D.isFollowing = false;
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
        SoundManager.StopWalkSound();
        BGMChangeTo2D(true);
        is2D = true;
        transitionQuad3D.SetActive(false);
        camera2D.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);
        parallaxBackground.ResetBackgrounds();

        camera2D.orthographic = true;
        //camera2D.orthographicSize = orthographicSize;

        //cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        //cockroach2DPos.transform.localScale = new Vector3(1,1,1);
        //camera2D.transform.position = cockroach2DStartPoint.position;

        StartCoroutine(End2DViewTransition());

        cockroachMove.myMoveMode = moveMode.twoDMove;


    }

    public void SetTo3DView()
    {
        BGMChangeTo2D(false);
        is2D = false;
        transitionQuad2D.SetActive(false);
        camera2D.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);
        parallaxBackground.ResetBackgrounds();

        camera3D.orthographic = false;
        camera3D.fieldOfView = fieldOfView;

        //cockroach2DPos.transform.position = cockroach2DStartPoint.position;
        //cockroach2DPos.transform.localScale = new Vector3(1, 1, 1);
        //camera2D.transform.position = cockroach2DStartPoint.position;

        StartCoroutine(End3DViewTransition());

        cockroachMove.myMoveMode = moveMode.AutoCameraMove;
    }

    public IEnumerator End2DViewTransition()
    {
        transitionQuad2D.SetActive(true);
        cameraLogic2D.StartSmoothZoom();
        yield return StartCoroutine(AnimateShaderScale(scale, 0f, transitionDuration));
        transitionQuad2D.SetActive(false);
        cockroachManager.CleanupAllSpiders();
        allGameManager.isTimerRunning = true;
    }

    public IEnumerator End3DViewTransition()
    {
        transitionQuad3D.SetActive(true);
        yield return StartCoroutine(AnimateShaderScale(scale, 0f, transitionDuration));
        transitionQuad3D.SetActive(false);
        cockroachManager.CleanupAllSpiders();
        allGameManager.isTimerRunning = true;
    }


    public bool Is2D()
    {
        return is2D;
    }

    public IEnumerator Camera3DShakeFunction()
    {
        yield return null;
    }

    private GameObject WindGrass;
    public void BGMChangeTo2D(bool To2D)
    {
        if(To2D == true)
        {
            SoundManager.StopWalkSound();
            BGMManager.Play("BGM_Cave Drainage");
            WindGrass.SetActive(false);
        }
        else
        {
            BGMManager.Play("BGM_Revival of Africa");
            WindGrass.SetActive(true);
        }
    }
    
}

