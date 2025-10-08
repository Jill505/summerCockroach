using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FemCockraochTracker : MonoBehaviour
{
    public Text textShowcase;
    private CameraViewToggle viewToggle;

    private FemaleCockroachInfo[] sceneRoaches3D;
    private GameObject playerPos3D;

    [System.Obsolete]
    void Awake()
    {
        viewToggle = GameObject.Find("CameraManager").GetComponent<CameraViewToggle>();
        playerPos3D = GameObject.Find("3DCockroach");
    }
    private void Start()
    {
        FindSceneRoaches(true);
    }
    void Update()
    {
        SortRoaches3DByDistance();
        if (!viewToggle.Is2D())
        {
            TextShowcase3D();
        }
        else
        {
            FindSceneRoaches(false);
            TextShowcase2D();
        }
    }

    void  FindSceneRoaches(bool find3D)
    {
        if(find3D)
        {
            sceneRoaches3D = FindObjectsOfType<FemaleCockroachInfo>();
            if (sceneRoaches3D.Length == 0)
            {
                Debug.LogWarning("場景中沒有找到任何 FemaleCockroachInfo！");
            }
        }
    }
    void TextShowcase3D()
    {
        var aliveRoaches = sceneRoaches3D.Where(r => r != null && !r.finded).ToArray();
        if (aliveRoaches.Length > 0)
        {
            textShowcase.text = "";
            float distance = Vector3.Distance(playerPos3D.transform.position, sceneRoaches3D[0].gameObject.transform.position);

            // 四捨五入
            int roundedDistance = Mathf.RoundToInt(distance);

            // 直接取整數（無條件捨去）
            int floorDistance = (int)distance;

            // 顯示
            textShowcase.text += "離你最近的母蟑螂有 " + roundedDistance + " 公尺遠！\n";
            textShowcase.text += "蟑螂姓名：" + sceneRoaches3D[0].cockroachName + "\n";
            textShowcase.text += "蟑螂敘述：" + sceneRoaches3D[0].Disc;
        }
        else
        {
            textShowcase.text = "沒有母蟑螂囉!";
        }
    }
    void TextShowcase2D()
    {
        textShowcase.text = "隕石來臨時，不能待太久，會被燒死的。";
    }



    void SortRoaches3DByDistance()
    {
        if (sceneRoaches3D == null || playerPos3D == null)
        {
            Debug.LogError("sceneRoaches or playerPos is not assigned.");
            return;
        }

        Vector3 playerPosition = playerPos3D.transform.position;

        // 過濾掉已被找到的蟑螂
        var filteredRoaches = sceneRoaches3D
            .Where(r => r != null && !r.finded && r.enabled)
            .OrderBy(r => (r.gameObject.transform.position - playerPosition).sqrMagnitude)
            .ToArray();

        // 將排序後的結果放回原陣列開頭，保留原陣列長度
        for (int i = 0; i < filteredRoaches.Length; i++)
        {
            sceneRoaches3D[i] = filteredRoaches[i];
        }

        //Debug.Log("Roaches sorted by distance to player (excluding those already found).");
    }
}
