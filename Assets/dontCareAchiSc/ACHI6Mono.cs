using UnityEngine;

public class ACHI6Mono : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AllGameManager AGM = FindAnyObjectByType<AllGameManager>();
            AGM.GO_unlockAchievement(5);
        }
    }
}
