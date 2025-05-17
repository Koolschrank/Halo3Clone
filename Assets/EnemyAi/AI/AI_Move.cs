using UnityEngine;
using UnityEngine.AI;

public class AI_Move : MonoBehaviour
{


    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] AI_Target target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerAim playerAim;


    Vector3 targetPosition;
    Vector3 targetOffset = Vector3.zero;

    [SerializeField] int framesToUpdateNavAgent = 100;
    [SerializeField] int framesToUpdateNavAgentIfClose = 10;
    [SerializeField] float distanceToUpdateNavAgent = 5f;



    private void Awake()
    {
        targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;
    }


    public void Start()
    {
        float offsetDistance = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        Vector2 offsetDirection = Random.insideUnitCircle.normalized  * Random.Range(0,offsetDistance);
        targetOffset = new Vector3(offsetDirection.x, 0, offsetDirection.y);

    }

    private void Update()
    {
        targetPosition = target.GetTargetPosition();
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        targetPosition += targetOffset;
        if (playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange )
        {
            playerMovement.UpdateMoveInput(Vector2.zero);
            return;
        }

        if (distanceToTarget < distanceToUpdateNavAgent)
        {
            if (Time.frameCount % framesToUpdateNavAgentIfClose == 0)
            {
                agent.transform.localPosition = Vector3.zero;
                agent.SetDestination(targetPosition);
                
            }
        }
        else
        {
            if (Time.frameCount % framesToUpdateNavAgent == 0)
            {
                agent.transform.localPosition = Vector3.zero;
                agent.SetDestination(targetPosition);
                
            }
        }


        Vector3 direction = agent.desiredVelocity.normalized;

        var speed = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.moveSpeedWithGun;
        playerMovement.UpdateMoveInput(new Vector2(direction.x, direction.z) * speed);
    }

    private void OnTargetFound(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void OnTargetLost(Vector3 lastPosition)
    {
        Vector3 directionToTarget = lastPosition - transform.position;
        targetPosition = lastPosition + directionToTarget.normalized;
    }

    private void OnDisable()
    {
        playerMovement.UpdateMoveInput(Vector2.zero);
    }

}
