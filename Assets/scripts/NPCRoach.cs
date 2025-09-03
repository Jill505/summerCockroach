using UnityEngine;

public class NPCRoach : MonoBehaviour
{

    [Header("Ref Component")]
    public NewAllGameManager newAllGameManager;
    public AllGameManager allGameManager;
    public FoodGenManger foodGenManager;
    public CockroachManager cockroachManager;
    public NPCRoachManager nPCRoachManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newAllGameManager = FindFirstObjectByType<NewAllGameManager>();
        allGameManager = FindFirstObjectByType<AllGameManager>();
        foodGenManager = FindFirstObjectByType<FoodGenManger>();
        cockroachManager = FindFirstObjectByType<CockroachManager>();
        nPCRoachManager = FindFirstObjectByType<NPCRoachManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
