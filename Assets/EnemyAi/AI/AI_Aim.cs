using UnityEngine;

public class AI_Aim : MonoBehaviour
{
    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] PlayerAim aim;
    [SerializeField] GameObject head;
    [SerializeField] AI_Target target;

    [SerializeField] float aimSpeedMultiplier_X = 5f;
    [SerializeField] float aimSpeedMultiplier_Y = 5f;

    Vector3 targetPosition;

    private void Update()
    {
        Vector2 aimInput = Vector2.zero;

        targetPosition = target.GetTargetPosition();

        var angles = GetYawPitchToTarget(transform,head.transform, targetPosition);



        aimInput.x = angles.x * aimSpeedMultiplier_X;

        // vertical angle (pitch)
       
        
        aimInput.y = angles.y * aimSpeedMultiplier_Y;
        Debug.Log(angles);


        aim.UpdateAimInput(aimInput);
    }

    Vector2 GetYawPitchToTarget(Transform body, Transform head, Vector3 target)
    {
        // Direction to target in world space
        Vector3 toTargetWorld = (target - head.position).normalized;

        // Convert toTarget into the local space of the body (for yaw)
        Vector3 toTargetLocalToBody = body.InverseTransformDirection(toTargetWorld);
        float yaw = Mathf.Atan2(toTargetLocalToBody.x, toTargetLocalToBody.z) * Mathf.Rad2Deg;

        // Convert toTarget into the local space of the head (for pitch)
        Vector3 toTargetLocalToHead = head.InverseTransformDirection(toTargetWorld);
        float pitch = -Mathf.Asin(toTargetLocalToHead.y) * Mathf.Rad2Deg;

        return new Vector2(yaw, -pitch); // x = yaw for body, y = pitch for head
    }


    private void OnDisable()
    {
        aim.UpdateAimInput(Vector2.zero);
    }
}
