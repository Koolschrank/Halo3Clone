using UnityEngine;

public class EnemyPatrolTargetPositions : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;

    // singelton instance
    public static EnemyPatrolTargetPositions Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public Transform GetRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned.");
            return null;
        }
        int randomIndex = Random.Range(0, patrolPoints.Length);
        return patrolPoints[randomIndex];
    }

    public Transform GetFartestPatrolPoint(Vector3 startPosition)
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned.");
            return null;
        }
        Transform farthestPoint = patrolPoints[0];
        float maxDistance = Vector3.Distance(startPosition, farthestPoint.position);
        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(startPosition, patrolPoints[i].position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPoint = patrolPoints[i];
            }
        }
        return farthestPoint;
    }

    public Transform GetClosesPatrolPoint(Vector3 startPosition)
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned.");
            return null;
        }
        Transform closestPoint = patrolPoints[0];
        float minDistance = Vector3.Distance(startPosition, closestPoint.position);
        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(startPosition, patrolPoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = patrolPoints[i];
            }
        }
        return closestPoint;
    }

}
