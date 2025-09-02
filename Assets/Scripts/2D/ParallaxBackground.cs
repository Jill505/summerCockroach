using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class DoubleHoleBGGroup
    {
        public Transform parent;
        [HideInInspector] public Transform CaveStonecolumn;
        [HideInInspector] public Transform StoneCrevice2;
        [HideInInspector] public Transform CaveStonecolumn2;
        [HideInInspector] public Transform StoneCrevice1;
        [HideInInspector] public Transform StoneCrevice4;

        public void Initialize()
        {
            if (parent == null) return;

            CaveStonecolumn = parent.Find("CaveStonecolumn");
            StoneCrevice2 = parent.Find("StoneCrevice2");
            CaveStonecolumn2 = parent.Find("CaveStonecolumn2");
            StoneCrevice1 = parent.Find("StoneCrevice1");
            StoneCrevice4 = parent.Find("StoneCrevice4");
        }
    }

    [System.Serializable]
    public class OneHoleBGGroup
    {
        public Transform parent;
        [HideInInspector] public Transform trunk1;
        [HideInInspector] public Transform trunk2;
        [HideInInspector] public Transform trunk3;
        [HideInInspector] public Transform treeHole1;
        [HideInInspector] public Transform treeHole3;

        public void Initialize()
        {
            if (parent == null) return;

            trunk1 = parent.Find("trunk1");
            trunk2 = parent.Find("trunk2");
            trunk3 = parent.Find("trunk3");
            treeHole1 = parent.Find("treeHole1");
            treeHole3 = parent.Find("treeHole3");
        }
    }



    [Header("玩家 Transform")]
    public Transform player;

    [Header("背景組設定")]
    public DoubleHoleBGGroup[] doubleHoleBGGroups;
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

        foreach (var group in doubleHoleBGGroups) group.Initialize();
        foreach (var group in oneHoleBGGroup) group.Initialize();

        initialPositions = new Vector3[doubleHoleBGGroups.Length, 5];
        for (int i = 0; i < doubleHoleBGGroups.Length; i++)
        {
            initialPositions[i, 1] = GetPosition(doubleHoleBGGroups[i].StoneCrevice2);
            initialPositions[i, 2] = doubleHoleBGGroups[i].CaveStonecolumn2 != null ? GetPosition(doubleHoleBGGroups[i].CaveStonecolumn2) : Vector3.zero; // 可為零
            initialPositions[i, 3] = GetPosition(doubleHoleBGGroups[i].StoneCrevice1);
            initialPositions[i, 4] = GetPosition(doubleHoleBGGroups[i].StoneCrevice4);
        }
        initialTreeHolePositions = new Vector3[oneHoleBGGroup.Length, 5];
        for (int i = 0; i < oneHoleBGGroup.Length; i++)
        {
            initialTreeHolePositions[i, 0] = GetPosition(oneHoleBGGroup[i].trunk1);
            initialTreeHolePositions[i, 1] = GetPosition(oneHoleBGGroup[i].trunk2);
            initialTreeHolePositions[i, 2] = GetPosition(oneHoleBGGroup[i].trunk3);
            initialTreeHolePositions[i, 3] = GetPosition(oneHoleBGGroup[i].treeHole1);
            initialTreeHolePositions[i, 4] = GetPosition(oneHoleBGGroup[i].treeHole3);
        }
    }

    void Update()
    {
        Vector3 playerDelta = player.position - previousPlayerPosition;

        foreach (var group in doubleHoleBGGroups)
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
            MoveBackground(group.treeHole1, playerDelta, TreeHole1Scale);
            MoveBackground(group.treeHole3, playerDelta, TreeHole3Scale);
        }

        previousPlayerPosition = player.position;
    }

    public void ResetBackgrounds()
    {
        for (int i = 0; i < doubleHoleBGGroups.Length; i++)
        {
            SetPosition(doubleHoleBGGroups[i].CaveStonecolumn, initialPositions[i, 0]);
            SetPosition(doubleHoleBGGroups[i].StoneCrevice2, initialPositions[i, 1]);
            if (doubleHoleBGGroups[i].CaveStonecolumn2 != null)
                SetPosition(doubleHoleBGGroups[i].CaveStonecolumn2, initialPositions[i, 2]);
            SetPosition(doubleHoleBGGroups[i].StoneCrevice1, initialPositions[i, 3]);
            SetPosition(doubleHoleBGGroups[i].StoneCrevice4, initialPositions[i, 4]);
        }
        for (int i = 0; i < oneHoleBGGroup.Length; i++)
        {
            SetPosition(oneHoleBGGroup[i].trunk1, initialTreeHolePositions[i, 0]);
            SetPosition(oneHoleBGGroup[i].trunk2, initialTreeHolePositions[i, 1]);
            SetPosition(oneHoleBGGroup[i].trunk3, initialTreeHolePositions[i, 2]);
            SetPosition(oneHoleBGGroup[i].treeHole1, initialTreeHolePositions[i, 3]);
            SetPosition(oneHoleBGGroup[i].treeHole3, initialTreeHolePositions[i, 4]);
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
