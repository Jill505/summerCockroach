using UnityEngine;

public class CockroachClimbRay : MonoBehaviour
{
    [Header("Ref Component")]
    public CockroachClimb cockRoachClimb;

    [Header("Dec Bools")]
    public bool isTouchingGround = false;
    public bool rotateJudgeValve = false;
    public Quaternion groundRotation;

    [Header("idk lah don't care variable")]
    public float rotationThreshold = 1f; // 1度以內視為不動

    [Header("Cal target")]
    public Transform nowMyFootTransform;
    public Transform myFrontTransform; 
    public Transform myFrontTransformFar;
    public float footDisc = 5f; 
    public float myDirectDisc = 10f; 

    [Header("Optional")]
    public LayerMask climbableMask;
    public bool useLayerMask = false;
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
        Debug.DrawRay(transform.position, dirToFrontFar * myDirectDisc, Color.cyan);

        bool gotNearHit = false;
        bool gotFarHit = false;

        RaycastHit hitFoot, hitNear, hitFar;

        if (Cast(footRay, out hitFoot, footDisc))
        {
        }

        if (Cast(frontRay, out hitNear, myDirectDisc) && IsClimbable(hitNear.collider))
        {
            gotNearHit = true;
        }

        if (Cast(frontRayFar, out hitFar, myDirectDisc) && IsClimbable(hitFar.collider))
        {
            gotFarHit = true;
        }

        if (gotNearHit || gotFarHit)
        {
            Vector3 surfaceNormal;
            if (gotNearHit && gotFarHit)
                surfaceNormal = (hitNear.normal + hitFar.normal).normalized;
            else
                surfaceNormal = gotNearHit ? hitNear.normal : hitFar.normal;

            Vector3 up = surfaceNormal;

            Vector3 desiredForward = transform.forward;

            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(desiredForward, up).normalized;

            if (forwardOnSurface.sqrMagnitude < 1e-6f)
            {
                forwardOnSurface = Vector3.Cross(up, transform.right).normalized;
            }

            Quaternion desiredRotation = Quaternion.LookRotation(forwardOnSurface, up);

            // 計算新舊旋轉的角度差
            float angleDiff = Quaternion.Angle(Quaternion.Euler(cockRoachClimb.targetRotation), desiredRotation);

            // 只有當角度差超過閾值時才更新
            if (angleDiff > rotationThreshold)
            {
                groundRotation = desiredRotation;
                cockRoachClimb.targetRotation = groundRotation.eulerAngles;
            }
            // transform.rotation = Quaternion.Slerp(transform.rotation, groundRotation, rotateSpeed * Time.deltaTime);
        }
    }

    bool Cast(Ray ray, out RaycastHit hit, float dist)
    {
        if (useLayerMask)
            return Physics.Raycast(ray, out hit, dist, climbableMask, QueryTriggerInteraction.Ignore);

        return Physics.Raycast(ray, out hit, dist, ~0, QueryTriggerInteraction.Ignore);
    }

    bool IsClimbable(Collider col)
    {
        if (useLayerMask) return true;
        return col != null && col.CompareTag("ClimbableObject");
    }

}
