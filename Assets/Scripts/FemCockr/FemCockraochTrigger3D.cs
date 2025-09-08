using JetBrains.Annotations;
//using Unity.Android.Gradle;
using UnityEngine;

public class FemCockraochTrigger3D : MonoBehaviour
{
    private AllGameManager allGameManager;
    private FemaleCockroachInfo femaleCockroachInfo;

    [Header("3D")]
    public MeshRenderer subStatementShowcase;
    public Material getMat;

    [Header("Cockroach Breed Variable")]
    public float coolDownTime = 60f;
    public float coolDownCal;

    public bool getDNAAlready = false;
    public bool allowBreed;


    private void Start()
    {
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        femaleCockroachInfo = GetComponent<FemaleCockroachInfo>();
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !getDNAAlready && coolDownCal<0)
        {
            //Add DNA Number
            allGameManager.DNA++;
            getDNAAlready = true;

            //open dna pattern.
        }
        if (other.CompareTag("Player") && coolDownCal < 0)
        {
            //femaleCockroachInfo.finded = true;
            //allGameManager.femCockraochGet();
            //subStatementShowcase.material = getMat;

            //TODO: Cockroach Egg plate
            coolDownCal = coolDownTime;
        }
        if (other.CompareTag("NPCRoach"))
        {
            coolDownCal = coolDownTime;
        }

    }
}
