using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FemCockraochTracker : MonoBehaviour
{
    public Text textShowcase;

    public testFemCockraoch[] sceneRoaches;
    public GameObject playerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SortRoachesByDistance();
        textShowcase.text = "";
        textShowcase.text += "���A�̪񪺥�������" + Vector3.Distance(playerPos.transform.position, sceneRoaches[0].gameObject.transform.position)+ "���ػ��I\n";
        textShowcase.text += "�����m�W�G" + sceneRoaches[0].cockroachName + "\n";
        textShowcase.text += "�����ԭz�G" + sceneRoaches[0].Disc;
    }


    void SortRoachesByDistance()
    {
        if (sceneRoaches == null || playerPos == null)
        {
            Debug.LogError("sceneRoaches or playerPos is not assigned.");
            return;
        }

        Vector3 playerPosition = playerPos.transform.position;

        // �L�o���w�Q��쪺����
        var filteredRoaches = sceneRoaches
            .Where(r => r != null && !r.finded)
            .OrderBy(r => (r.gameObject.transform.position - playerPosition).sqrMagnitude)
            .ToArray();

        // �N�Ƨǫ᪺���G��^��}�C�}�Y�A�O�d��}�C����
        for (int i = 0; i < filteredRoaches.Length; i++)
        {
            sceneRoaches[i] = filteredRoaches[i];
        }

        Debug.Log("Roaches sorted by distance to player (excluding those already found).");
    }
}
