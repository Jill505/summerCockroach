using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
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
    public int Hp = 6;

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


    public void GameStart()
    {
        CockroachMoveable = true;
        Hp = 6;
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
        Debug.Log("L heal");
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
        for (int i = 0; i < cockroachModels.Length; i++)
        {
            cockroachModels[i].SetActive(false);
        }


        if (Hp >= 6)
        {
            cockroachModels[6].SetActive(true);
        }
        else if (Hp <= 0)
        {
            cockroachModels[0].SetActive(true);
        }
        else
        {
            cockroachModels[Hp].SetActive(true);
        }

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

        hungerDecayRate = maxHunger / hungerDuration;

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
}

