using UnityEngine;

public class AI_DangerAvoidance : MonoBehaviour
{
    [SerializeField] float dangerAvoidanceRadius = 5f;
    [SerializeField] LayerMask dangerLayerMask;
    [SerializeField] int framePause = 5; // Number of frames to wait before checking again
    [SerializeField] AI_Move move;


    private void Update()
    {
        // circlecast
        Collider[] dangers = Physics.OverlapSphere(transform.position, dangerAvoidanceRadius, dangerLayerMask);

        if (dangers.Length > 0)
        {
            move.RollAwayFromDanger(dangers[0].transform.position);
        }

    }




}
