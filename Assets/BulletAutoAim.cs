using System.IO.IsolatedStorage;
using UnityEngine;

public class BulletAutoAim : MonoBehaviour
{
    public float detectionRadius = 10f;
    public LayerMask detectionLayer;

	public LayerMask groundLayer;
	public float detectionAngle = 45f;
    float originalDetectionAngle;
	public float angleIncreaseSpeed = 30f;

	public float detectionAngleMax = 45f;


	public float rotationSpeed = 5f;

    public Bullet bullet;

    GameObject targetEnemy = null;
    public float followDuration = 3f;
    float followTimer = -10f;
    Vector3 originPosition;
    Vector3 originRotation;

	private void Start()
	{
		originalDetectionAngle =detectionAngle;
        originPosition = transform.position;
        originRotation = bullet.Owner.GetComponent<BodyMindConnection>().BulletSpawner.transform.forward;

	}
	private void Update()
	{
        if (bullet.inStick) return;

		detectionAngle = Mathf.Min(detectionAngle + (angleIncreaseSpeed * Time.deltaTime), detectionAngleMax);
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


		Collider[] hits = Physics.OverlapSphere(originPosition, detectionRadius, detectionLayer);
        float closestAngle = Mathf.Infinity;
        GameObject closestEnemy = null;
        foreach (var hit in hits)
        {
           
			Vector3 directionToEnemy = (hit.transform.position - originPosition).normalized;
            float distranceToHit = Vector3.Distance(originPosition, hit.transform.position);
			float angleToEnemy = Vector3.Angle(originRotation, directionToEnemy);
            if (angleToEnemy < detectionAngle / 2)
            {
				


				// make raycast to check if there is a wall in the way
				if (Physics.Raycast(transform.position, directionToEnemy, out RaycastHit wallHit, distranceToHit, groundLayer) || hit.gameObject == bullet.Owner)
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
