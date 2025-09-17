using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CockroachManager : MonoBehaviour
{
    [Header("My Component References")]
    public AllGameManager allGameManger;
    public CockroachMove myCockroachMove;
    public CameraLogic3D myCameraLogic;

    public MeshFilter myMeshFilter;
    public MeshRenderer myRenderer;

    [Header("Cockroach Values")]
    public int Hp = 1;

    public float autoModeCameraFlu = 0.1f;
    public float playerModeCameraFlu = 0.1f;

    public GameObject[] cockroachModels = new GameObject[7];

    public bool dashing = false;
    

    [Header("蟑螂操作變數")]
    public bool CockroachMoveable = false;
    public bool onDieImm = false;

    [Header("Hungry Value")]
    private float maxHunger = 100f;     // 飢餓最大值
    private float currentHunger = 100f;
    public float hungerDuration = 15f; // 從滿值到0所需時間（秒）

    public UnityEngine.UI.Image myHungryAmount;
    private float hungerDecayRate;

    [Header("Buffs")]
    public int dashLevel = 0;
    public int dashLevelMax = 3;
    public int dashRecoverLevel = 0;
    public int dashRecoverLevelMax = 3;
    public int basicSpeedLevel = 0;
    public int basicSpeedLevelMax = 3;
    public int hungerLevel = 0;
    public int hungerLevelMax = 3;
    public int shield = 0;

    [Header("Dead Variable")]
    public GameObject deadBody;
    public Animator deadCanvasAnimator;
    public Text leftHealthText;
    public GameObject[] deadCanvasFadeHideGroupe;
    public GameObject deadCanvas;
    public Coroutine cDCoroutine;

    public string lastDeadVale;

    public void GameStart()
    {
        CockroachMoveable = true;
        Hp = 1;
    }
    public void CockroachHealing(int healNum)
    {
        Debug.Log(" heal");
        Hp += healNum;

        if (shield >= 1)
        {
            if (Hp > 2) Hp = 2;
        }
        else { 
            if(Hp>1) Hp = 1;
        }
        CockroachBodyPartSwitch();

        //Collect Food Stats
        allGameManger.foodCollect++;

        currentHunger += healNum;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

    }


    public void CockroachDie()
    {
        if (cDCoroutine == null)
        {
            CleanupAllSpiders();
            cDCoroutine = StartCoroutine(CockroachDieCoroutine());
        }
        else
        {
            Debug.Log("dead coroutine on clog");
        }
        /*
        if (allGameManger.allLifeCount > 0)
        {
            if (cDCoroutine == null)
            {
                cDCoroutine = StartCoroutine(CockroachDieCoroutine());
            }
        else
        {
            Debug.Log("dead coroutine on clog");
        }
        }
        else
        {
            allGameManger.GameFail();
        }*/
    }
    public IEnumerator CockroachDieCoroutine()
    {
        onDieImm = true;
        allGameManger.isTimerRunning = true;
        deadCanvas.SetActive(true);
        for (int i = 0; i < deadCanvasFadeHideGroupe.Length; i++)
        {
            deadCanvasFadeHideGroupe[i].SetActive(true);
        }
        //play sound effect

        yield return new WaitForSeconds(0);


        //wait animation play ready
        deadCanvasAnimator.SetTrigger("nextAct");

        leftHealthText.text = "剩餘子代  x" + (allGameManger.allLifeCount);
        Debug.Log(allGameManger.allLifeCount + "bef");
        yield return new WaitForSeconds(0.85f);
        //Play life -- animation
        allGameManger.allLifeCount--;
        leftHealthText.text = "剩餘子代  x" + allGameManger.allLifeCount;

        bool _shouldDie = false;
        if (allGameManger.allLifeCount < 0)
        { 
            leftHealthText.color = Color.red;
            _shouldDie = true;
        }
            Debug.Log(allGameManger.allLifeCount + "aft");

        yield return new WaitForSeconds(1);

        if (_shouldDie)
        {

            allGameManger.GameFail();

            StopCoroutine(cDCoroutine);
        }
        else
        {
            //glow
            deadCanvasAnimator.SetTrigger("nextAct");
            CameraViewToggle cameraViewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>(); ;
            cameraViewToggle.SetTo3DView();
        }
        yield return new WaitForSeconds(1);

        bool _revB = false;

        //執行復活
        if (allGameManger.allLifeCount < 0)
        {
        }
        else
        {
            //rev
            //track all the fem roach and return the closest one
            float d = Vector3.Distance(transform.position, allGameManger.femCockroachTrackList[0].gameObject.transform.position);
            int t = 0;
            for (int i = 1; i < allGameManger.femCockroachTrackList.Count; i++)
            {
                if (allGameManger.femCockroachTrackList[i].eggNumber > 0)
                {
                    float nD = Vector3.Distance(transform.position, allGameManger.femCockroachTrackList[i].gameObject.transform.position);
                    if (d > nD)
                    {
                        d = nD;
                        t = i;
                    }
                }
            }
            //rev at the current pos.
            Vector3 debugUpper = new Vector3(0, 4, 2);
            if (allGameManger.femCockroachTrackList[t].coolDownCal < 15)
            {
                allGameManger.femCockroachTrackList[t].coolDownCal = 15f;
                //已防落地馬上有蛋
            }
            transform.position = allGameManger.femCockroachTrackList[t].myEggPos.position + debugUpper;
            allGameManger.femCockroachTrackList[t].eggNumber -= 1;


            deadCanvasAnimator.SetTrigger("nextAct");
            FillHunger();
            //fade
            for (int i = 0; i < deadCanvasFadeHideGroupe.Length; i++)
            {
                deadCanvasFadeHideGroupe[i].SetActive(false);
            }
            yield return new WaitForSeconds(1f);
            _revB = true;
        }

        if (_revB)
        {
            Debug.Log("Rev B");
            deadCanvas.SetActive(false);
        }
        onDieImm = false;
        allGameManger.isTimerRunning = false;

        deadCanvas.SetActive(false);

        cDCoroutine = null;
        StopAllCoroutines();
        yield return null;
    }

    public void insDeadBody()
    {

    }

    public void CockroachInjury(int injNum, string deadReason)
    {
        Hp -= injNum;
        lastDeadVale = deadReason;
        CockroachBodyPartSwitch();
    }
    public void CockroachBodyPartSwitch()
    {
        myCockroachMove.HorVelocity = 0;
        //for (int i = 0; i < cockroachModels.Length; i++)
        //{
        //    cockroachModels[i].SetActive(false);
        //}


        //if (Hp >= 6)
        //{
        //    cockroachModels[6].SetActive(true);
        //}
        //else if (Hp <= 0)
        //{
        //    cockroachModels[0].SetActive(true);
        //}
        //else
        //{
        //    cockroachModels[Hp].SetActive(true);
        //}

        switch (Hp)
        {

            case int n when Hp >= 6:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp6maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 5:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp5maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 4:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp4maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 3:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp3maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 2:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp2maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 1:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp1maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            default:
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp0maxVelocity;
                myCockroachMove.HorVelocity = 0;
                CockroachDie();
                break;
        }
    }

    public void UISync()
    {
        if (myHungryAmount != null) myHungryAmount.fillAmount = currentHunger / maxHunger;
        else Debug.LogError("NO FILL AMOUNT SHOW UI PICTURE");
    }
    void Start()
    {
        if (allGameManger == null)
        {
            allGameManger = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        }
        GameStart();//AutoGameStart

        SetHungerDuration(hungerDuration);


        _shield_Obj = GameObject.Find("shield");
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K))
        {
            CockroachInjury(1, "測試中的自殺");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CockroachHealing(1);
        }*/

        // 每秒衰減飢餓值
        currentHunger -= hungerDecayRate * Time.deltaTime;

        UISync();
        // 避免飢餓值小於0
        if (currentHunger < 0f)
        {
            //TODO: update system
            //CockroachDie();
            CockroachInjury(1, "這一世，我被飢餓殺死");
        }

        if (shield > 0)
        {
            _shield_Obj.SetActive(true);
        }
        else
        {
            _shield_Obj.SetActive(false);
        }
    }
    GameObject _shield_Obj;

    private float baseHungerDuration;   // 基礎飢餓時間（不含buff）
    private float finalHungerDuration;
    public void SetHungerDuration(float newDuration)
    {
        if (newDuration > 0)
        {
            baseHungerDuration = newDuration; // 只存基礎值
            ApplyHungerBuff();                // 重新計算buff後的值
        }
        else
        {
            Debug.LogWarning("SetHungerDuration: 輸入值必須大於 0");
        }
    }

    // 套用buff計算最終耗損時間
    private void ApplyHungerBuff()
    {
        float multiplier = 1f + (0.2f * hungerLevel);

        finalHungerDuration = baseHungerDuration * multiplier;

        hungerDecayRate = maxHunger / finalHungerDuration;

        Debug.Log($"飢餓耗盡時間: 基礎{baseHungerDuration} 秒 → 最終 {finalHungerDuration} 秒 (等級 {hungerLevel})");
    }

    public void FillHunger()
    {
        currentHunger = maxHunger;  // 直接填滿
        UISync();                   // 更新 UI 顯示
        Debug.Log("飢餓值已經回滿！");
    }
    [Header("UI系統")]
    public Button dashLevelButt;
    public Button dashRecoverLevelButt;
    public Button basicSpeedLevelButt;
    public Button hungerLevelButt;
    public Button shieldButt;

    public Image[] dashLevelImage = new Image[3];
    public Image[] dashRecoverLevelImage = new Image[3];
    public Image[] basicSpeedLevelImage = new Image[3];
    public Image[] hungerLevelImage = new Image[3];
    public Image[] shieldImage = new Image[1];

    public Sprite lightOff;
    public Sprite lightOn;
    public void RenderPlayerBuffs()
    {
        //reset all the Image to gray;
        resetAllTheImage(dashLevelImage);
        setState(dashLevelImage, dashLevel, dashLevelButt);
        resetAllTheImage(dashRecoverLevelImage);
        setState(dashRecoverLevelImage, dashRecoverLevel, dashRecoverLevelButt);
        resetAllTheImage(basicSpeedLevelImage);
        setState(basicSpeedLevelImage, basicSpeedLevel, basicSpeedLevelButt);
        resetAllTheImage(hungerLevelImage);
        setState(hungerLevelImage, hungerLevel, hungerLevelButt);
        resetAllTheImage(shieldImage);
        setState(shieldImage, shield, shieldButt);
    }
    void resetAllTheImage(Image[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = lightOff;
        }
    }
    void setState(Image[] images, int currentValue, Button tarButton)
    {
        for (int i = 0; i < images.Length && i < currentValue; i++)
        {
            images[i].sprite = lightOn;
        }
        if ((currentValue < images.Length))
        {
            tarButton.interactable = true;
        }
        else
        {
            tarButton.interactable = false;
        }
    }

    public void AddBuff(int sort)
    {
        switch (sort)
        {

            //dash level
            case 0:
                dashLevel++;
                break;

            //dash recover level
            case 1:
                dashRecoverLevel++;
                break;

            //basic Speed Level
            case 2:
                basicSpeedLevel++;
                break;

            //hunger level
            case 3:
                hungerLevel++;
                ApplyHungerBuff();
                break;

            //shield
            case 4:
                shield++;
                break;
        }
        allGameManger.CloseDNASelect();
    }

    public void CleanupAllSpiders()
    {
        if (Scene2DManager.Instance == null) return;

        if (Scene2DManager.Instance.LSpiderweb != null)
            Scene2DManager.Instance.LSpiderweb.gameObject.SetActive(false);

        if (Scene2DManager.Instance.RSpiderweb != null)
            Scene2DManager.Instance.RSpiderweb.gameObject.SetActive(false);

        SpiderEatUp[] spiderObjects = GameObject.FindObjectsOfType<SpiderEatUp>();
        foreach (var spider in spiderObjects)
        {
            spider.DestroySelfAndParent();
        }
    }
}

