using System;
using UnityEngine;

public class AI_LookForPlayer : MonoBehaviour
{
    public Action<Transform> OnTargetDetected;
    public Action<Vector3> OnTargetLost;

    Transform target;

    [SerializeField] AI_Target targetScript;
    [SerializeField] float range = 10f;
    [SerializeField] float fieldOfViewAngle = 110f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] int frameInterval = 5;

    public void Update()
    {
        if (Time.frameCount % frameInterval == 0)
        {
            LookForPlayer();
        }
    }

    private void LookForPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, range, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            if (target.gameObject.tag == "AIEnemy") continue;


            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    if (this.target != target)
                    {
                        this.target = target;
                        OnTargetDetected?.Invoke(target);
                        targetScript.AssignTarget(target);
                        return;
                    }
                }
            }
        }

        if (this.target != null)
        {
            OnTargetLost?.Invoke(target.position);
            this.target = null;
        }
    }


}
