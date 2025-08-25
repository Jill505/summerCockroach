using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class DoubleHoleBGGroup
    {
        [Header("DoubleHoleBG")]
        public Transform CaveStonecolumn;
        public Transform StoneCrevice2;
        public Transform CaveStonecolumn2;
        public Transform StoneCrevice1;
        public Transform StoneCrevice4;
    }

    [System.Serializable]
    public class OneHoleBGGroup
    {
        [Header("OneHoleBGGroup")]
        public Transform trunk1;
        public Transform trunk2;
        public Transform trunk3;
        public Transform TreeHole1;
        public Transform TreeHole3;
    }



    [Header("玩家 Transform")]
    public Transform player;

    [Header("背景組設定")]
    public DoubleHoleBGGroup[] backgroundGroups;
    public OneHoleBGGroup[] oneHoleBGGroup;

    [Header("(0 = speed0, 1 = playerSpeed)")]
    public float caveStonecolumnScale = 0.5f;
    public float stoneCrevice2Scale = 0.05f;
    public float caveStonecolumn2Scale = 0.05f;
    public float stoneCrevice3Scale = 0.1f;
    public float stoneCrevice4Scale = 0.08f;

    public float trunk1Scale = 0.2f;
    public float trunk2Scale = 0.2f;
    public float trunk3Scale = 0.2f;
    public float TreeHole1Scale = 0.05f;
    public float TreeHole3Scale = 0.08f;



    private Vector3 previousPlayerPosition;

    // 紀錄每個背景的初始位置
    private Vector3[,] initialPositions;
    private Vector3[,] initialTreeHolePositions;

    void Start()
    {
        previousPlayerPosition = player.position;

        initialPositions = new Vector3[backgroundGroups.Length, 5];
        for (int i = 0; i < backgroundGroups.Length; i++)
        {
            initialPositions[i, 0] = GetPosition(backgroundGroups[i].CaveStonecolumn);
            initialPositions[i, 1] = GetPosition(backgroundGroups[i].StoneCrevice2);
            initialPositions[i, 2] = GetPosition(backgroundGroups[i].CaveStonecolumn2);
            initialPositions[i, 3] = GetPosition(backgroundGroups[i].StoneCrevice1);
            initialPositions[i, 4] = GetPosition(backgroundGroups[i].StoneCrevice4);
        }
        initialTreeHolePositions = new Vector3[oneHoleBGGroup.Length, 5];
        for (int i = 0; i < oneHoleBGGroup.Length; i++)
        {
            initialTreeHolePositions[i, 0] = GetPosition(oneHoleBGGroup[i].trunk1);
            initialTreeHolePositions[i, 1] = GetPosition(oneHoleBGGroup[i].trunk2);
            initialTreeHolePositions[i, 2] = GetPosition(oneHoleBGGroup[i].trunk3);
            initialTreeHolePositions[i, 3] = GetPosition(oneHoleBGGroup[i].TreeHole1);
            initialTreeHolePositions[i, 4] = GetPosition(oneHoleBGGroup[i].TreeHole3);
        }
    }

    void Update()
    {
        Vector3 playerDelta = player.position - previousPlayerPosition;

        foreach (var group in backgroundGroups)
        {
            MoveBackground(group.CaveStonecolumn, playerDelta, caveStonecolumnScale);
            MoveBackground(group.StoneCrevice2, playerDelta, stoneCrevice2Scale);
            MoveBackground(group.CaveStonecolumn2, playerDelta, caveStonecolumn2Scale);
            MoveBackground(group.StoneCrevice1, playerDelta, stoneCrevice3Scale);
            MoveBackground(group.StoneCrevice4, playerDelta, stoneCrevice4Scale);
        }
        foreach (var group in oneHoleBGGroup)
        {
            MoveBackground(group.trunk1, playerDelta, trunk1Scale);
            MoveBackground(group.trunk2, playerDelta, trunk2Scale);
            MoveBackground(group.trunk3, playerDelta, trunk3Scale);
            MoveBackground(group.TreeHole1, playerDelta, TreeHole1Scale);
            MoveBackground(group.TreeHole3, playerDelta, TreeHole3Scale);
        }

        previousPlayerPosition = player.position;
    }

    public void ResetBackgrounds()
    {
        for (int i = 0; i < backgroundGroups.Length; i++)
        {
            SetPosition(backgroundGroups[i].CaveStonecolumn, initialPositions[i, 0]);
            SetPosition(backgroundGroups[i].StoneCrevice2, initialPositions[i, 1]);
            SetPosition(backgroundGroups[i].CaveStonecolumn2, initialPositions[i, 2]);
            SetPosition(backgroundGroups[i].StoneCrevice1, initialPositions[i, 3]);
            SetPosition(backgroundGroups[i].StoneCrevice4, initialPositions[i, 4]);
        }
        for (int i = 0; i < oneHoleBGGroup.Length; i++)
        {
            SetPosition(oneHoleBGGroup[i].trunk1, initialTreeHolePositions[i, 0]);
            SetPosition(oneHoleBGGroup[i].trunk2, initialTreeHolePositions[i, 1]);
            SetPosition(oneHoleBGGroup[i].trunk3, initialTreeHolePositions[i, 2]);
            SetPosition(oneHoleBGGroup[i].TreeHole1, initialTreeHolePositions[i, 3]);
            SetPosition(oneHoleBGGroup[i].TreeHole3, initialTreeHolePositions[i, 4]);
        }
    }

    private void MoveBackground(Transform background, Vector3 delta, float scale)
    {
        if (background == null) return;
        Vector3 newPosition = background.position + delta * scale;
        background.position = new Vector3(newPosition.x, background.position.y, background.position.z);
    }

    private Vector3 GetPosition(Transform t)
    {
        return t.position;
    }

    private void SetPosition(Transform t, Vector3 pos)
    {
        t.position = pos;
    }
}
