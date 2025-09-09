using JetBrains.Annotations;
using UnityEngine;
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
    public int hungerLevelMax =3;
    public int shield = 0;


    public void GameStart()
    {
        CockroachMoveable = true;
        Hp = 1;
    }
    public void JudgeCockroachDie()
    {
        if (Hp <= 0)
        {
            CockroachDie();
        }
    }
    public void CockroachHealing(int healNum)
    {
        Debug.Log(" heal");
        Hp += healNum;
        if (Hp > 6) Hp = 6;
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
        //開始死亡計算
        if (allGameManger == null)
        {
            allGameManger = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();

            //Debug.LogError("Ak Error: the all game manager is empty & null");
        }
        else
        {
            allGameManger.GameFail();
        }
    }

    public void CockroachInjury(int injNum)
    {
        Debug.Log("K inj");
        Hp -= injNum;
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

    }

    void Update()
    {
       if (Input.GetKeyDown(KeyCode.K))
       {
          CockroachInjury(1);
       }
       if (Input.GetKeyDown(KeyCode.L))
       {
          CockroachHealing(1);
       }

       // 每秒衰減飢餓值
       currentHunger -= hungerDecayRate * Time.deltaTime;

        UISync();
        // 避免飢餓值小於0
        if (currentHunger < 0f)
       {
          CockroachDie();
       }
    }

    public void SetHungerDuration(float newDuration)
    {
        if (newDuration > 0)
        {
            hungerDuration = newDuration;
            hungerDecayRate = maxHunger / hungerDuration;
            Debug.Log($"飢餓耗盡時間已設定為: {hungerDuration} 秒");
        }
        else
        {
            Debug.LogWarning("SetHungerDuration: 輸入值必須大於 0");
        }
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
        resetAllTheImage(dashRecoverLevelImage);
        resetAllTheImage(basicSpeedLevelImage);
        resetAllTheImage(hungerLevelImage);
        resetAllTheImage(shieldImage);
    }
    void resetAllTheImage(Image[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = lightOff;
        }
    }

    public void ApplyBuff(int sort)
    {

    }
}

