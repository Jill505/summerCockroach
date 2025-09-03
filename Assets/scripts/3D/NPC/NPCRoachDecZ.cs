using Unity.VisualScripting;
using UnityEngine;

public class NPCRoachDecZ : MonoBehaviour
{
    [Header("Ref Com")]
    public NPCRoach myNPC;

    bool roachValve;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FemRoach"))
        {
            //Set it as target;
            myNPC.hasFemInZone = true;
            myNPC.targetFemPos = other.transform.position;
        }

        if (other.CompareTag("Food"))
        {
            //Set it as target
            myNPC.hasFoodInZone = true;
            myNPC.targetFoodPos = other.transform.position;
        }
    }
}
