using UnityEngine;

public class AI_Aim : MonoBehaviour
{
    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] PlayerAim aim;
    [SerializeField] GameObject head;
    [SerializeField] AI_Target target;

    [SerializeField] float aimSpeedMultiplier = 5f;

    Vector3 targetPosition;

    private void Update()
    {
        Vector2 aimInput = Vector2.zero;
        
        targetPosition = target.GetTargetPosition();
        // head forward direction
        Vector3 headForward = head.transform.forward;
        float angleX = Vector3.SignedAngle(headForward, targetPosition - head.transform.position, head.transform.up);

        angleX = Mathf.Clamp(angleX, -1f, 1f); 
        // aim input
        aimInput.x = angleX * aimSpeedMultiplier;

        // head right direction
        Vector3 headRight = head.transform.right;
        float angleY = Vector3.SignedAngle(headRight, targetPosition - head.transform.position, head.transform.forward);
        angleY = Mathf.Clamp(angleY, -1f, 1f);
        // aim input
        aimInput.y = angleY * aimSpeedMultiplier;





        aim.UpdateAimInput(aimInput);
    }

    private void OnDisable()
    {
        aim.UpdateAimInput(Vector2.zero);
    }
}
