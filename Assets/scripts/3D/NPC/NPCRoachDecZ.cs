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
            if (other.gameObject.GetComponent<FemCockraochTrigger3D>().allowBreed)
            {
                //Set it as target;
                myNPC.hasFemInZone = true;
                myNPC.targetFemPos = other.transform.position;
            }
        }

        if (other.CompareTag("Food"))
        {
            //Set it as target
            myNPC.hasFoodInZone = true;
            myNPC.targetFoodPos = other.transform.position;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("FemRoach"))
        {
            if (collision.gameObject.GetComponent<FemCockraochTrigger3D>().allowBreed)
            {
                //Set it as target;
                myNPC.hasFemInZone = true;
                myNPC.targetFemPos = collision.transform.position;
            }
        }

        if (collision.gameObject.CompareTag("Food"))
        {
            //Set it as target
            myNPC.hasFoodInZone = true;
            myNPC.targetFoodPos = collision.gameObject.transform.position;
        }
    }
}
