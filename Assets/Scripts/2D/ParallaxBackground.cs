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
    public class LSpiderWeb
    {
        public Transform parent;
        [HideInInspector] public Transform Lspiderweb_1;
        [HideInInspector] public Transform Lspiderweb_2;
        [HideInInspector] public Transform Lspiderweb_3;

        public void Initialize()
        {
            if (parent == null) return;
            Lspiderweb_1 = parent.Find("Lspiderweb_1");
            Lspiderweb_2 = parent.Find("Lspiderweb_2");
            Lspiderweb_3 = parent.Find("Lspiderweb_3");
        }
    }

    [System.Serializable]
    public class RSpiderWeb
    {
        public Transform parent;
        [HideInInspector] public Transform Rspiderweb_1;
        [HideInInspector] public Transform Rspiderweb_2;
        [HideInInspector] public Transform Rspiderweb_3;

        public void Initialize()
        {
            if (parent == null) return;
            Rspiderweb_1 = parent.Find("Rspiderweb_1");
            Rspiderweb_2 = parent.Find("Rspiderweb_2");
            Rspiderweb_3 = parent.Find("Rspiderweb_3");
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
            trunk1 = parent?.Find("trunk1");
            trunk2 = parent?.Find("trunk2");
            trunk3 = parent?.Find("trunk3");
            treeHole1 = parent?.Find("treeHole1");
            treeHole3 = parent?.Find("treeHole3");
        }
    }

    [Header("玩家 Transform")]
    public Transform player;

    [Header("背景組設定")]
    public DoubleHoleBGGroup[] doubleHoleBGGroups;
    public OneHoleBGGroup[] oneHoleBGGroup;
    public LSpiderWeb[] lSpiderWebs;
    public RSpiderWeb[] rSpiderWebs;

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
    public float spiderWebScale = 0.1f;

    private Vector3 previousPlayerPosition;

    private Vector3[,] initialPositions;
    private Vector3[,] initialTreeHolePositions;
    private Vector3[,] initialLSpiderWebPositions;
    private Vector3[,] initialRSpiderWebPositions;

    void Start()
    {
        previousPlayerPosition = player.position;

        foreach (var group in doubleHoleBGGroups) group.Initialize();
        foreach (var group in oneHoleBGGroup) group.Initialize();
        foreach (var web in lSpiderWebs) web.Initialize();
        foreach (var web in rSpiderWebs) web.Initialize();

        // DoubleHoleBGGroup 初始化位置
        initialPositions = new Vector3[doubleHoleBGGroups.Length, 5];
        for (int i = 0; i < doubleHoleBGGroups.Length; i++)
        {
            initialPositions[i, 0] = GetPosition(doubleHoleBGGroups[i].CaveStonecolumn);
            initialPositions[i, 1] = GetPosition(doubleHoleBGGroups[i].StoneCrevice2);
            initialPositions[i, 2] = doubleHoleBGGroups[i].CaveStonecolumn2 != null ? GetPosition(doubleHoleBGGroups[i].CaveStonecolumn2) : Vector3.zero;
            initialPositions[i, 3] = GetPosition(doubleHoleBGGroups[i].StoneCrevice1);
            initialPositions[i, 4] = GetPosition(doubleHoleBGGroups[i].StoneCrevice4);
        }

        // OneHoleBGGroup 初始化位置
        initialTreeHolePositions = new Vector3[oneHoleBGGroup.Length, 5];
        for (int i = 0; i < oneHoleBGGroup.Length; i++)
        {
            initialTreeHolePositions[i, 0] = GetPosition(oneHoleBGGroup[i].trunk1);
            initialTreeHolePositions[i, 1] = GetPosition(oneHoleBGGroup[i].trunk2);
            initialTreeHolePositions[i, 2] = GetPosition(oneHoleBGGroup[i].trunk3);
            initialTreeHolePositions[i, 3] = GetPosition(oneHoleBGGroup[i].treeHole1);
            initialTreeHolePositions[i, 4] = GetPosition(oneHoleBGGroup[i].treeHole3);
        }

        // LSpiderWeb 初始化位置
        initialLSpiderWebPositions = new Vector3[lSpiderWebs.Length, 3];
        for (int i = 0; i < lSpiderWebs.Length; i++)
        {
            initialLSpiderWebPositions[i, 0] = GetPosition(lSpiderWebs[i].Lspiderweb_1);
            initialLSpiderWebPositions[i, 1] = GetPosition(lSpiderWebs[i].Lspiderweb_2);
            initialLSpiderWebPositions[i, 2] = GetPosition(lSpiderWebs[i].Lspiderweb_3);
        }

        // RSpiderWeb 初始化位置
        initialRSpiderWebPositions = new Vector3[rSpiderWebs.Length, 3];
        for (int i = 0; i < rSpiderWebs.Length; i++)
        {
            initialRSpiderWebPositions[i, 0] = GetPosition(rSpiderWebs[i].Rspiderweb_1);
            initialRSpiderWebPositions[i, 1] = GetPosition(rSpiderWebs[i].Rspiderweb_2);
            initialRSpiderWebPositions[i, 2] = GetPosition(rSpiderWebs[i].Rspiderweb_3);
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

        foreach (var web in lSpiderWebs)
        {
            MoveBackground(web.Lspiderweb_1, playerDelta, spiderWebScale);
            MoveBackground(web.Lspiderweb_2, playerDelta, spiderWebScale);
            MoveBackground(web.Lspiderweb_3, playerDelta, spiderWebScale);
        }

        foreach (var web in rSpiderWebs)
        {
            MoveBackground(web.Rspiderweb_1, playerDelta, spiderWebScale);
            MoveBackground(web.Rspiderweb_2, playerDelta, spiderWebScale);
            MoveBackground(web.Rspiderweb_3, playerDelta, spiderWebScale);
        }

        previousPlayerPosition = player.position;
    }

    public void ResetBackgrounds()
    {
        for (int i = 0; i < doubleHoleBGGroups.Length; i++)
        {
            SetPosition(doubleHoleBGGroups[i].CaveStonecolumn, initialPositions[i, 0]);
            SetPosition(doubleHoleBGGroups[i].StoneCrevice2, initialPositions[i, 1]);
            if (doubleHoleBGGroups[i].CaveStonecolumn2 != null) SetPosition(doubleHoleBGGroups[i].CaveStonecolumn2, initialPositions[i, 2]);
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

        for (int i = 0; i < lSpiderWebs.Length; i++)
        {
            SetPosition(lSpiderWebs[i].Lspiderweb_1, initialLSpiderWebPositions[i, 0]);
            SetPosition(lSpiderWebs[i].Lspiderweb_2, initialLSpiderWebPositions[i, 1]);
            SetPosition(lSpiderWebs[i].Lspiderweb_3, initialLSpiderWebPositions[i, 2]);
        }

        for (int i = 0; i < rSpiderWebs.Length; i++)
        {
            SetPosition(rSpiderWebs[i].Rspiderweb_1, initialRSpiderWebPositions[i, 0]);
            SetPosition(rSpiderWebs[i].Rspiderweb_2, initialRSpiderWebPositions[i, 1]);
            SetPosition(rSpiderWebs[i].Rspiderweb_3, initialRSpiderWebPositions[i, 2]);
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
        return t != null ? t.position : Vector3.zero;
    }

    private void SetPosition(Transform t, Vector3 pos)
    {
        if (t == null) return;
        t.position = pos;
    }
}
