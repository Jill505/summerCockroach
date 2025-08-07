using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

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
    
    [Header("蟑螂操作變數")]
    public bool CockroachMoveable = false;

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

    }
    public void CockroachDie()
    {
        //開始死亡計算
        if (allGameManger == null)
        {
            Debug.LogError("Ak Error: the all game manager is empty & null");
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

        /*
        switch (Hp)
        {

            case int n when Hp >= 6:

                myCockroachMove.HorVelocity = 0;
                break;

            case 5:
                myMeshFilter.mesh = Hp5Mesh;
                myRenderer.materials = Hp5Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp5maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 4:
                myMeshFilter.mesh = Hp4Mesh;
                myRenderer.materials = Hp4Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp4maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 3:
                myMeshFilter.mesh = Hp3Mesh;
                myRenderer.materials = Hp3Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp3maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 2:
                myMeshFilter.mesh = Hp2Mesh;
                myRenderer.materials = Hp2Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp2maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            case 1:
                myMeshFilter.mesh = Hp1Mesh;
                myRenderer.materials = Hp1Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp1maxVelocity;
                myCockroachMove.HorVelocity = 0;
                break;

            default:
                myMeshFilter.mesh = Hp0Mesh;
                myRenderer.materials = Hp0Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp0maxVelocity;
                myCockroachMove.HorVelocity = 0;
                CockroachDie();
                break;
        }*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (allGameManger == null)
        {
            allGameManger = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        }
        GameStart();//AutoGameStart

    }

    // Update is called once per frame
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
    }
}
