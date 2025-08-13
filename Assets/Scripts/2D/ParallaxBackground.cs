using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundGroup
    {
        [Header("Background")]
        public Transform CaveStonecolumn;
        public Transform StoneCrevice2;
        public Transform CaveStonecolumn2;
        public Transform StoneCrevice3;
        public Transform StoneCrevice4;
    }

    [Header("玩家 Transform")]
    public Transform player;

    [Header("背景組設定")]
    public BackgroundGroup[] backgroundGroups;

    [Header("(0 = speed0, 1 = playerSpeed)")]
    public float caveStonecolumnScale = 0.2f;
    public float stoneCrevice2Scale = 0.4f;
    public float caveStonecolumn2Scale = 0.25f;
    public float stoneCrevice3Scale = 0.3f;
    public float stoneCrevice4Scale = 0.5f;

    private Vector3 previousPlayerPosition;

    // 紀錄每個背景的初始位置
    private Vector3[,] initialPositions;

    void Start()
    {
        previousPlayerPosition = player.position;

        initialPositions = new Vector3[backgroundGroups.Length, 5];
        for (int i = 0; i < backgroundGroups.Length; i++)
        {
            initialPositions[i, 0] = GetPosition(backgroundGroups[i].CaveStonecolumn);
            initialPositions[i, 1] = GetPosition(backgroundGroups[i].StoneCrevice2);
            initialPositions[i, 2] = GetPosition(backgroundGroups[i].CaveStonecolumn2);
            initialPositions[i, 3] = GetPosition(backgroundGroups[i].StoneCrevice3);
            initialPositions[i, 4] = GetPosition(backgroundGroups[i].StoneCrevice4);
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
            MoveBackground(group.StoneCrevice3, playerDelta, stoneCrevice3Scale);
            MoveBackground(group.StoneCrevice4, playerDelta, stoneCrevice4Scale);
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
            SetPosition(backgroundGroups[i].StoneCrevice3, initialPositions[i, 3]);
            SetPosition(backgroundGroups[i].StoneCrevice4, initialPositions[i, 4]);
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
