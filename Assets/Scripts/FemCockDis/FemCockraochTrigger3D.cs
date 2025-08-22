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

    private void Start()
    {
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        femaleCockroachInfo = GetComponent<FemaleCockroachInfo>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !femaleCockroachInfo.finded)
        {
            femaleCockroachInfo.finded = true;
            allGameManager.femCockraochGet();
            subStatementShowcase.material = getMat;
        }
    }
}
