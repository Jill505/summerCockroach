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

    [Header("Cockroach egg")]
    public int eggNumber;
    public Transform myEggPos;
    public GameObject myEgg;


    private void Start()
    {
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        femaleCockroachInfo = GetComponent<FemaleCockroachInfo>();
        allGameManager.femCockroachTrackList.Add(this);
        myEgg = transform.GetChild(0).gameObject;
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
            allGameManager.DNA++;
            getDNAAlready = true;

            //open dna pattern.
            allGameManager.OpenDNASelect();

            SaveSystem.mySaveFile.FemRoachBreed++;
        }
        if (other.CompareTag("Player") && coolDownCal < 0)
        {
            //femaleCockroachInfo.finded = true;
            //allGameManager.femCockraochGet();
            //subStatementShowcase.material = getMat;

            //TODO: Cockroach Egg plate
            eggNumber++;
            coolDownCal = coolDownTime;
            allGameManager.AddScore(allGameManager.findFem);
        }
        if (other.CompareTag("NPCRoach"))
        {
            coolDownCal = coolDownTime;
            SoundManager.Play("SFX_ReproductiveBehavior_V1");
        }

    }
}
