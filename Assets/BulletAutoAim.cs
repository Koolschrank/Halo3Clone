using UnityEngine;

public class BulletAutoAim : MonoBehaviour
{
    public float detectionRadius = 10f;
    public LayerMask detectionLayer;

	public LayerMask groundLayer;
	public float detectionAngle = 45f;


    public float rotationSpeed = 5f;

    GameObject targetEnemy = null;
    public float followDuration = 3f;
    float followTimer = -10f;


	private void Update()
	{
		if (targetEnemy == null)
        {
            DetectEnemy();
        }
        else
        {
            AimAtTarget();
		}
	}

    void DetectEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        float closestAngle = Mathf.Infinity;
        GameObject closestEnemy = null;
        foreach (var hit in hits)
        {
            Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized;
            float distranceToHit = Vector3.Distance(transform.position, hit.transform.position);
			float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            if (angleToEnemy < detectionAngle / 2)
            {
				// make raycast to check if there is a wall in the way
                if (Physics.Raycast(transform.position, directionToEnemy, out RaycastHit wallHit, distranceToHit, groundLayer))
                {
					continue;
				}

				// angle to enemy is within detection angle
                if (angleToEnemy < closestAngle)
                {
                    closestAngle = angleToEnemy;
                    closestEnemy = hit.gameObject;
				}

			}
		}
        if (closestEnemy != null )
        {
            targetEnemy = closestEnemy;
            if ( followTimer < -5f)
				followTimer = followDuration;

		}
    }
    void AimAtTarget()
    {
		if (followTimer <= 0f)
		{
			return;
		}
		followTimer -= Time.deltaTime;
        
		if (targetEnemy == null)
            return;


        Vector3 directionToTarget = (targetEnemy.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * (followTimer/followDuration));

        // Optional: If you want to stop aiming when very close to the target
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > detectionAngle) // Threshold angle to consider "aimed"
        {
            targetEnemy = null;

		}
    }
    



}
