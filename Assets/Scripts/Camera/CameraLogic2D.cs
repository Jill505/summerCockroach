using UnityEditor.Rendering;
using UnityEngine;

public class CameraLogic2D : MonoBehaviour
{
    [Header("Base")]
    public Camera cam;
    public GameObject player;
    public Rigidbody2D playerRB;

    [Header("Camera")]
    public Vector3 offset = new Vector3(0, 2, -10);
    public float moveSpeed = 5f;
    public float SetOrthographicSize = 5f;
    public float acceleration = 10f;      // X�b�[�t��
    public float maxSpeed = 20f;          // �̤j�t��
    public float smoothTime = 0.3f;       // ���Ʈɶ��]�P�����Ҧ��@�Ρ^

    [Header("collider")]
    public BoxCollider2D cameraBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    [Header("view")]
    public bool isFollowing = false;
    public Transform cockroach2DPos;
    public CameraViewToggle viewToggle;

    //  5. �����p����ܼ�
    private Vector3 targetPos;
    private float velocityX = 0f;
    private Vector3 moveVelocity;    // �� SmoothDamp �ϥ�
    private float zoomVelocity = 0f; // �����Y���



    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // �p����v���������@�b���׻P�e��
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        if (isFollowing == false)
        {
            UpdateCameraPosition();
        }
        else
        {
            MoveTowardsTarget();
        }
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

    private void MoveTowardsTarget()
    {
        // �]�w�ؼЦ�m�]�O��Z�b���ܡ^
        Vector3 targetPosition = new Vector3(cockroach2DPos.position.x, cockroach2DPos.position.y, offset.z);

        // �ϥ� SmoothDamp ���Ʋ�����v��
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, smoothTime);

        // �ϥ� SmoothDamp ���ƽվ���v���� Orthographic Size�]�����Y��^
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, SetOrthographicSize, ref zoomVelocity, smoothTime);
    }
}
