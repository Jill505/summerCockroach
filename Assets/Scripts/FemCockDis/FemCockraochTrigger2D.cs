using UnityEngine;

public class FemCockraochTrigger2D : MonoBehaviour
{
    private AllGameManager allGameManager;
    private FemaleCockroachInfo2D femaleCockroachInfo2D;

    private void Start()
    {
        allGameManager = GameObject.Find("AllGameManager").GetComponent<AllGameManager>();
        femaleCockroachInfo2D = GetComponent<FemaleCockroachInfo2D>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !femaleCockroachInfo2D.finded)
        {
            femaleCockroachInfo2D.finded = true;
            allGameManager.femCockraochGet();
        }
    }
}
