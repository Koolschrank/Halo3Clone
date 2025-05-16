using UnityEngine;
using UnityEngine.AI;

public class AI_Move : MonoBehaviour
{

    [SerializeField] AI_StateMachine stateMachine;
    [SerializeField] AI_LookForPlayer lookForPlayer;

    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] AI_Target target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerAim playerAim;


    Vector3 targetPosition;
    Vector3 targetOffset = Vector3.zero;


    private void Awake()
    {
        stateMachine.OnStateChange += OnStateChange;
        stateMachine.OnTargetFound += OnTargetFound;
        lookForPlayer.OnTargetLost += OnTargetLost;
        targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;
    }

    private void OnStateChange(AIState state)
    {
        if (state == AIState.Patrol)
        {
            targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;
        }
    }

    public void Start()
    {
        float offsetDistance = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        Vector2 offsetDirection = Random.insideUnitCircle.normalized  * Random.Range(0,offsetDistance);
        targetOffset = new Vector3(offsetDirection.x, 0, offsetDirection.y);

    }

    private void Update()
    {
        

        if (alwaysKnowsWherePlayerIs || stateMachine.CurrentState == AIState.Attack)
            targetPosition = target.GetTargetPosition() ;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (alwaysKnowsWherePlayerIs ||stateMachine.CurrentState == AIState.Attack)
            targetPosition += targetOffset;
        if (playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange )
        {
            playerMovement.UpdateMoveInput(Vector2.zero);
            return;
        }
        agent.SetDestination(targetPosition);
        agent.transform.localPosition = Vector3.zero;



        Vector3 direction = agent.desiredVelocity.normalized;

        var speed = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.moveSpeedWithGun;
        playerMovement.UpdateMoveInput(new Vector2(direction.x, direction.z) * speed);


        if (stateMachine.CurrentState == AIState.Patrol || stateMachine.CurrentState == AIState.Chase)
        {
            if (distanceToTarget < 5f)
            {
                if (stateMachine.CurrentState == AIState.Chase)
                {
                    targetPosition = EnemyPatrolTargetPositions.Instance.GetClosesPatrolPoint(targetPosition).position;
                }
                else
                {
                    targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;
                }


                    stateMachine.CurrentState = AIState.Patrol;
            }
        }
        

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

}
