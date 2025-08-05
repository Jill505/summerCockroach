using System.Collections;
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

    public float targetSize = 5f;     // �ؼ� orthographicSize
    public float startSize = 2f;

    [Header("Collider")]
    public BoxCollider2D cameraBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    [Header("View")]
    public bool isFollowing = false;
    public Transform cockroach2DPos;
    public CameraViewToggle viewToggle;


    //  5. �����p����ܼ�
    private Vector3 targetPos;
    private float velocityX = 0f;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 moveVelocity;    // �� SmoothDamp �ϥ�
    private float zoomVelocity = 0f; // �����Y���

    private float currentVelocity = 0f;
    private bool isZooming = false;
    private float timer = 0f;



    void Awake()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        // �p����v���������@�b���׻P�e��
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        if (isZooming)
        {
            // ���ƽվ� orthographicSize
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref currentVelocity, smoothTime);

            // ��s�g�L�ɶ�
            timer += Time.deltaTime;

            // �� smoothTime �L�F�A�N���� zoom
            if (timer >= smoothTime)
            {
                cam.orthographicSize = targetSize;
                isZooming = false;
            }
        }
        else
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
    }


    

    void UpdateCameraPosition()
    {
        // �p��ؼЦ�m
        targetPos = player.transform.position + offset;

        // �ϥ� SmoothDamp ���Ƹ��H
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref smoothVelocity, smoothTime);

        // ����d��
        Bounds bounds = cameraBounds.bounds;

        float minX = bounds.min.x + camHalfWidth;
        float maxX = bounds.max.x - camHalfWidth;
        float minY = bounds.min.y + camHalfHeight;
        float maxY = bounds.max.y - camHalfHeight;

        smoothPos.x = Mathf.Clamp(smoothPos.x, minX, maxX);
        smoothPos.y = Mathf.Clamp(smoothPos.y, minY, maxY);

        transform.position = smoothPos;
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

    public void StartSmoothZoom()
    {
        transform.position = new Vector3(cockroach2DPos.position.x, cockroach2DPos.position.y, transform.position.z);

        // ��l�����j�p
        cam.orthographicSize = startSize;

        // �Ұ� zoom �L�{
        isZooming = true;
        timer = 0f;
        currentVelocity = 0f;
    }
}
