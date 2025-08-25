using UnityEngine;

public class Scene2DManager : MonoBehaviour
{
    [System.Serializable]
    public class Scene2D
    {
        public string sceneName;          // 2D �����W��
        public BoxCollider2D cameraBounds;       // ��v���d��
        public EdgeCollider2D spawnBounds;        // ����ͦ��d��
        public Transform randomMotherCockroachRange1; // �H���ͦ���������m�d��
        public Transform randomMotherCockroachRange2; // �H���ͦ���������m�d��
        public Transform randomMotherCockroachRange3; // �H���ͦ���������m�d��
        public Transform motherCockroachPoints;  // �T�w�ͦ���m
        public Transform insPos1;
        public Transform insPos2;
    }
    public static Scene2DManager Instance;

    public Scene2D[] scenes; // �Ҧ��������

    private void Awake()
    {
        Instance = this;
    }

    // ���o�������
    public Scene2D GetSceneByName(string name)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneName == name)
                return scene;
        }
        Debug.LogWarning("�䤣������W�١G" + name);
        return null;
    }
}
