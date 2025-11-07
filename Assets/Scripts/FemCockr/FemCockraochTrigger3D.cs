using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class FemCockraochTrigger3D : MonoBehaviour
{
    private AllGameManager allGameManager;
    private FemaleCockroachInfo femaleCockroachInfo;
    private CameraLogic3D cameraLogic;

    [Header("Cockroach Breed Variable")]
    public float coolDownTime = 60f;
    public float coolDownCal;

    public bool getDNAAlready = false;
    public bool allowBreed;

    [Header("Cockroach egg")]
    public int eggNumber;
    public Transform myEggPos;
    public GameObject myEgg;

    [Header("Kiss prefab")]
    public GameObject kissPrefab;

    [Header("Breed visuals / control")]
    public float breedDuration = 3f; // 交配期間長度（秒）。到時會自動還原顯示與移動
    private GameObject player;
    private CockroachMove cockroachMove;
    private CockroachManager cockroachManager;

    private Coroutine breedCoroutine;

    private void Start()
    {
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        loveAttention = GameObject.Find("loveAttention").GetComponent<Image>();
        femaleCockroachInfo = GetComponent<FemaleCockroachInfo>();
        allGameManager.femCockroachTrackList.Add(this);
        myEgg = transform.GetChild(0).gameObject;

        player = GameObject.Find("3DCockroach");
        cockroachMove = player.GetComponent<CockroachMove>();
        cockroachManager = player.GetComponent<CockroachManager>();
        cameraLogic = GameObject.Find("3DCamera").GetComponent<CameraLogic3D>();
    }
    private void Update()
    {
        coolDownCal -= Time.deltaTime;

        if (coolDownCal > 0)
        {
            allowBreed = false;
        }
        else
        {
            allowBreed = true;
        }

        if (eggNumber > 0)
        {
            myEgg.SetActive(true);
        }
        else
        {
            myEgg.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !getDNAAlready && coolDownCal<0)
        {
            //Add DNA Number           
            SoundManager.Play("SFX_ReproductiveBehavior_V1");           
            
            allGameManager.femCockraochGet();
            //subStatementShowcase.material = getMat;

            //TODO: Cockroach Egg plate
           
            coolDownCal = coolDownTime;
            allGameManager.AddScore(allGameManager.findFem);
            if (breedCoroutine != null)
            {
                StopCoroutine(breedCoroutine);
                // 嘗試還原
                RestorePlayerRenderersAndMovement();
                RestoreFemaleRenderers();
                breedCoroutine = null;
            }
            breedCoroutine = StartCoroutine(BreedSequenceCoroutine());
        }
        if (other.CompareTag("NPCRoach"))
        {
            coolDownCal = coolDownTime;
            SoundManager.Play("SFX_ReproductiveBehavior_V1");
        }

    }
    private IEnumerator BreedSequenceCoroutine()
    {
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        // 1. 將玩家禁止移動並隱藏模型
        if (cockroachMove != null)
        {
            cockroachMove.SetCanMove(false);
        }
        HidePlayerRenderers();

        // 2. 將母蟑螂隱藏 (用跟玩家同樣方式)
        HideFemaleRenderers();

        // 3. 生成 Kiss Prefab（放在玩家與母蟑螂中間）並設定位置/朝向
        StartLoveAttentionBlink();
        GameObject kissObj = null;
        if (kissPrefab != null && player != null)
        {
            kissObj = Instantiate(kissPrefab, transform.position, transform.rotation);

            // 如果 prefab 沒有 parent，可以 parent 在場景某物件下（或不需要）
            // kissObj.transform.SetParent(null);

            // 4. 把攝影機對準到親吻物上（使用 CameraLogic3D 的臨時焦點）
            if (cameraLogic != null)
            {
                cameraLogic.StartFocusOn(kissObj.transform);
            }

            // 5. 播放 Kiss 動畫（SetBool）
            Animator kissAnimator = kissObj.GetComponent<Animator>();
            if (kissAnimator != null)
            {
                
                kissAnimator.SetBool("StartKiss", true);

                // 等待動畫進入指定 state
                bool entered = false;
                float waitTimeout = 0.5f;
                float waitClock = 0f;
                while (entered == false && waitClock < waitTimeout)
                {
                    waitClock += Time.deltaTime;
                    if (kissAnimator.GetCurrentAnimatorStateInfo(0).IsName("RIG-骨架|RIG-骨架Action 0") ||
                        kissAnimator.GetNextAnimatorStateInfo(0).IsName("RIG-骨架|RIG-骨架Action 0"))
                    {
                        entered = true;
                    }
                    yield return null;
                }

                // 如果已經進入該 state，等待跑完一次（normalizedTime >= 1）
                if (entered)
                {
                    // 等到該狀態跑完一次
                    while (kissAnimator.GetCurrentAnimatorStateInfo(0).IsName("RIG-骨架|RIG-骨架Action 0") == true &&
                           kissAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                    {
                        yield return null;
                    }
                    // 如果有 transition 到 next state，也要等 next state 的 normalizedTime >= 1（若需）
                }
            }
        }
        float timer = 0f;
        while (timer < breedDuration)
        {
            timer += Time.unscaledDeltaTime;  // 不受 Time.timeScale 影響
            yield return null;
        }

        // 還原遊戲
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (cameraLogic != null)
            cameraLogic.StopFocus();


        RestorePlayerRenderersAndMovement();
        RestoreFemaleRenderers();
        allGameManager.DNA++;
        getDNAAlready = true;
        eggNumber++;
        femaleCockroachInfo.finded = true;
        //open dna pattern.

        SaveSystem.mySaveFile.FemRoachBreed++;
        allGameManager.OpenDNASelect();
        StopLoveAttentionBlink();
        // 7. 銷毀親吻物
        if (kissObj != null)
        {
            Destroy(kissObj);
        }

        
        breedCoroutine = null;
    }

    #region Helpers: hide / restore renderers & movement

    private void HidePlayerRenderers()
    {
        SkinnedMeshRenderer[] skinnedRenderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            smr.enabled = false;
        }
    }

    private void RestorePlayerRenderersAndMovement()
    {
        SkinnedMeshRenderer[] skinnedRenderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            smr.enabled = true;
        }
        cockroachMove.SetCanMove(true);
    }


    private void HideFemaleRenderers()
    {
        MeshRenderer femaleRenderers = GetComponent<MeshRenderer>();
        femaleRenderers.enabled = false;
    }

    private void RestoreFemaleRenderers()
    {
        MeshRenderer femaleRenderers = GetComponent<MeshRenderer>();
        femaleRenderers.enabled = true;
    }
    #endregion

    [Header("Love attention UI")]
    private Image loveAttention;
    private Coroutine loveCoroutine;
    private bool isLoving;
    public void StartLoveAttentionBlink()
    {
        if (loveCoroutine == null && loveAttention != null)
        {
            isLoving = true;
            loveCoroutine = StartCoroutine(LoveAttentionBlink());
            Debug.Log("1");
        }
    }

    public void StopLoveAttentionBlink()
    {
        isLoving = false;
        if (loveCoroutine != null)
        {
            StopCoroutine(loveCoroutine);
            loveCoroutine = null;
        }
        if (loveAttention != null)
        {
            Color c = loveAttention.color;
            c.a = 0f;
            loveAttention.color = c;
        }
    }

    private IEnumerator LoveAttentionBlink()
    {
        while (isLoving)
        {
            float t = 0f;
            // 淡入
            while (t < 1f && isLoving)
            {
                t += Time.unscaledDeltaTime;
                if (loveAttention != null)
                {
                    Color c = loveAttention.color;
                    c.a = Mathf.Lerp(0f, 1f, t);
                    loveAttention.color = c;
                }
                yield return null;
            }

            t = 0f;
            // 淡出
            while (t < 1f && isLoving)
            {
                t += Time.unscaledDeltaTime;
                if (loveAttention != null)
                {
                    Color c = loveAttention.color;
                    c.a = Mathf.Lerp(1f, 0f, t);
                    loveAttention.color = c;
                }
                yield return null;
            }
        }

        if (loveAttention != null)
        {
            Color c = loveAttention.color;
            c.a = 0f;
            loveAttention.color = c;
        }
        loveCoroutine = null;
    }
}
