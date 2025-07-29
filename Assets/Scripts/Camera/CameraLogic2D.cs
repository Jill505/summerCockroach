using UnityEngine;

public class CameraLogic2D : MonoBehaviour
{
    [Header("��v����m")]
    public Vector3 offset = new Vector3(0, 2, -10);
    private Vector3 targetPos;
    private float velocityX = 0f;

    [Header("�l�ܳ]�w")]
    public float acceleration = 10f;  // �[�t��
    public float maxSpeed = 20f;      // �̤j�l�ܳt��

    [Header("��v������d��")]
    public BoxCollider2D cameraBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    public Camera cam;
    public GameObject player;
    public Rigidbody2D playerRB;

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // �p����v���������@�b���׻P�e��
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        targetPos = player.transform.position + offset;

        // ��V�Z��
        float distanceX = targetPos.x - transform.position.x;

        // �ھڶZ�����i�t�ס]�[�t�^
        float accelerationForce = acceleration * Time.deltaTime;

        if (Mathf.Abs(distanceX) > 0.01f)
        {
            velocityX += Mathf.Sign(distanceX) * accelerationForce;
        }
        else
        {
            velocityX = 0f; // �X�G�K���N����
        }

        // ����̤j�t��
        velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);

        // ������v���]�u��X�b�^
        Vector3 newCamPos = new Vector3(transform.position.x + velocityX * Time.deltaTime, targetPos.y, targetPos.z);

        // ����d��G�q BoxCollider2D �o��d��
        Bounds bounds = cameraBounds.bounds;

        float minX = bounds.min.x + camHalfWidth;
        float maxX = bounds.max.x - camHalfWidth;
        float minY = bounds.min.y + camHalfHeight;
        float maxY = bounds.max.y - camHalfHeight;

        newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);
        

        transform.position = newCamPos;
    }
}
