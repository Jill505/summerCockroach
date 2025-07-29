using JetBrains.Annotations;
//using Unity.Android.Gradle;
using UnityEngine;

public class testFemCockraoch : MonoBehaviour
{
    public AllGameManager allGameManager;
    public MeshRenderer subStatementShowcase;
    public Material getMat;

    public string cockroachName = "阿花";
    public string Disc = "是個三八，住在水湖邊上，綁著辮子的可愛小蟑螂";

    public bool finded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
}
