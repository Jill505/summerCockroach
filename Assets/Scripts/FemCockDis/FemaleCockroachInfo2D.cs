using UnityEngine;

public class FemaleCockroachInfo2D : MonoBehaviour
{
    [Header("Information")]
    public string cockroachName;
    public string Disc;
    public bool finded = false;
    [HideInInspector]public OneHoleSwitchTrigger generatorScript;
    [HideInInspector] public DoubleHolePair generatorScript2;

    private void Update()
    {
        FemCockroach2DFinded();
    }
    public void FemCockroach2DFinded()
    {
        if(finded == true && generatorScript != null)
        {
            generatorScript.FemCockroach2DFindOut();
        }
        if (finded == true && generatorScript2 != null)
        {
            generatorScript2.finded = true;
            if (generatorScript2.leftHoleTrigger3D != null)
            {
                FemaleCockroachInfo leftInfo = generatorScript2.leftHoleTrigger3D.GetComponent<FemaleCockroachInfo>();
                if (leftInfo != null)
                {
                    leftInfo.finded = true;
                }
            }

            // 更新 rightHoleTrigger3D 上的 FemaleCockroachInfo
            if (generatorScript2.rightHoleTrigger3D != null)
            {
                FemaleCockroachInfo rightInfo = generatorScript2.rightHoleTrigger3D.GetComponent<FemaleCockroachInfo>();
                if (rightInfo != null)
                {
                    rightInfo.finded = true;
                }
            }
        }
    }
}
