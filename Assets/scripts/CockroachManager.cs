using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class CockroachManager : MonoBehaviour
{
    [Header("My Component References")]
    public CockroachMove myCockroachMove;
    public CameraLogic3D myCameraLogic;

    public MeshFilter myMeshFilter;
    public MeshRenderer myRenderer;

    [Header("Cockroach Values")]
    public int Hp = 3;

    public float autoModeCameraFlu = 0.1f;
    public float playerModeCameraFlu = 0.1f;

    public Mesh fullHealthMesh;
    public Material fullHealthMaterial;

    public Mesh injF0Mesh;
    public Material injF0Material;

    public Mesh injF1Mesh;
    public Material injF1Material;


    [Header("¡≠Ω∏æﬁß@≈‹º∆")]
    public bool CockroachMoveable = false;

    public void GameStart()
    {
        CockroachMoveable = true;
        Hp = 3;
    }
    public void JudgeCockroachDie()
    {
        if (Hp <= 0)
        {
            CockroachDie();
        }
    }
    public void CockroachDie()
    {

    }

    public void CockroachInjury(int injNum)
    {
        Debug.Log("K inj");
        Hp -= injNum;
        
        if (Hp == 3)
        {
            myMeshFilter.mesh = fullHealthMesh;
            myRenderer.material = fullHealthMaterial;
            myCockroachMove.myMaxVelocity = myCockroachMove.myVelocity0;
            myCockroachMove.HorVelocity = 0;
        }
        else if (Hp == 2)
        {
            myMeshFilter.mesh = injF0Mesh;
            myRenderer.material = injF0Material;
            myCockroachMove.myMaxVelocity = myCockroachMove.myVelocity1;
            myCockroachMove.HorVelocity = 0;
        }
        else if (Hp == 1)
        {
            myMeshFilter.mesh = injF1Mesh;
            myRenderer.material = injF1Material;
            myCockroachMove.myMaxVelocity = myCockroachMove.myVelocity2;
            myCockroachMove.HorVelocity = 0;
        }
        else if (Hp <= 0)
        {
            myCockroachMove.HorVelocity = 0;
            CockroachDie();
        }
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameStart();//AutoGameStart

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CockroachInjury(1);
        }
    }
}
