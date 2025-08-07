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

    public Mesh Hp6Mesh;
    public Material[] Hp6Material;
    public Vector3 debugScaleVectorHp6;

    public Mesh Hp5Mesh;
    public Material[] Hp5Material;
    public Vector3 debugScaleVectorHp5;

    public Mesh Hp4Mesh;
    public Material[] Hp4Material;
    public Vector3 debugScaleVectorHp4;

    public Mesh Hp3Mesh;
    public Material[] Hp3Material;
    public Vector3 debugScaleVectorHp3;

    public Mesh Hp2Mesh;
    public Material[] Hp2Material;
    public Vector3 debugScaleVectorHp2;

    public Mesh Hp1Mesh;
    public Material[] Hp1Material;
    public Vector3 debugScaleVectorHp1;

    public Mesh Hp0Mesh;
    public Material[] Hp0Material;
    public Vector3 debugScaleVectorHp0;

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
        switch (Hp)
        {

            case 6:
                myMeshFilter.mesh = Hp6Mesh;
                myRenderer.materials = Hp6Material;
                myCockroachMove.myMaxVelocity = myCockroachMove.Hp6maxVelocity;
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
        }
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
