using JetBrains.Annotations;
//using Unity.Android.Gradle;
using UnityEngine;

public class testFemCockraoch : MonoBehaviour
{
    public AllGameManager allGameManager;

    [Header("3D")]
    public MeshRenderer subStatementShowcase;
    public Material getMat;


    [Header("Information")]
    public string cockroachName = "阿花";
    public string Disc = "是個三八，住在水湖邊上，綁著辮子的可愛小蟑螂";

    public bool finded = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !finded)
        {
            finded = true;
            allGameManager.femCockraochGet();
            subStatementShowcase.material = getMat;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !finded)
        {
            finded = true;
            allGameManager.femCockraochGet();
            Debug.Log("2d母蟑螂get");
        }
    }
}
