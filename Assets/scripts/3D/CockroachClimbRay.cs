using UnityEngine;

public class CockroachClimbRay : MonoBehaviour
{
    [Header("Ref Component")]
    public CockroachClimb cockRoachClimb;

    [Header("Dec Bools")]
    public bool isTouchingGround = false;
    public bool rotateJudgeValve = false;
    public Quaternion groundRotation;

    [Header("Cal target")]
    public Transform nowMyFootTransform;   // �}�U�ؼ�
    public Transform myFrontTransform;      // �e��ؼ�
    public Transform myFrontTransformFar;      // �e��2�ؼ�
    public float footDisc = 5f;            // �}�U�g�u�Z��
    public float myDirectDisc = 10f;       // �e��g�u�Z��

    void Update()
    {
        Vector3 dirToFoot = (nowMyFootTransform.position - transform.position).normalized;
        Vector3 dirToFront = (myFrontTransform.position - transform.position).normalized;
        Vector3 dirToFrontFar = (myFrontTransformFar.position - transform.position).normalized;

        Ray footRay = new Ray(transform.position, dirToFoot);
        Ray frontRay = new Ray(transform.position, dirToFront);
        Ray frontRayFar = new Ray(transform.position, dirToFrontFar);

        Debug.DrawRay(transform.position, dirToFoot * footDisc, Color.green);
        Debug.DrawRay(transform.position, dirToFront * myDirectDisc, Color.red);
        Debug.DrawRay(transform.position, dirToFrontFar * myDirectDisc, Color.cyan); ;

        bool _touching = false;
        
        // �i��G���z�˴�
        if (Physics.Raycast(footRay, out RaycastHit hit1, footDisc))
        {
            //Debug.Log("Hit Foot Target: " + hit1.collider.name);
            //Do Gravity;
        }
        if (Physics.Raycast(frontRay, out RaycastHit hit2, myDirectDisc))
        {
            //Debug.Log("Hit Front Target: " + hit2.collider.name);
            if (hit2.collider.CompareTag("ClimbableObject"))
            {
                _touching = true;
            }

        }
        if (Physics.Raycast(frontRayFar, out RaycastHit hit3, myDirectDisc))
        {
            if (hit3.collider.CompareTag("ClimbableObject"))
            {
                if (_touching)
                {
                    //TriggerCal
                    groundRotation = GetRotationBetweenPoints(hit2.point, hit3.point);
                    cockRoachClimb.targetRotation = groundRotation.eulerAngles;
                    cockRoachClimb.targetRotation = new Vector3(cockRoachClimb.targetRotation.x, 0, cockRoachClimb.targetRotation.z);
                    Debug.Log("Ground Rotation is:" + cockRoachClimb.targetRotation);
                }
            }
        }


    }

    public Quaternion GetRotationBetweenPoints(Vector3 a, Vector3 b)
    {
        if ((a.y) > (b.y))
        {
            Vector3 tmp = a;
            a = b;
            b = tmp;
        }


        Vector3 direction = b - a;
        if (direction.sqrMagnitude < 1e-8f) return Quaternion.identity; // �קK�s�V�q

        return Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

}
