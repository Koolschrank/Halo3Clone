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

    [SerializeField] float straveDistance = 2f;
    [SerializeField] float straveTime = 0.5f;
    [SerializeField] Vector3 straveOffset = Vector3.zero;

    [SerializeField] float followObjectiveChance = 0.5f;

    [SerializeField] float crouchRecoveryTime = 1f; // time to recover from crouch to stand up
    float crouchRecoveryTimer = 0f;

    bool followObjective = false;


    float straveTimer = 0f;





    private void Awake()
    {
        targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;
    }


    public void Start()
    {
        if (GameModeSelector.gameModeManager is KingOfTheHillManager )
        {
            // check probability to follow objective
            if (Random.Range(0f, 1f) < followObjectiveChance)
            {
                followObjective = true;
                
            }
            
        }

        float offsetDistance = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        Vector2 offsetDirection = Random.insideUnitCircle.normalized  * Random.Range(0,offsetDistance);
        targetOffset = new Vector3(offsetDirection.x, 0, offsetDirection.y);

    }

    private void Update()
    {
        targetPosition = target.GetTargetPosition();
        if (followObjective)
        {
            targetPosition = ObjectiveIndicator.instance.GetObjective(0).Position;
        }


        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);


        if (playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.crouchDistance)
        {
            playerMovement.TryCrouch();
            crouchRecoveryTimer = crouchRecoveryTime;
        }
        else
        {
            crouchRecoveryTimer -= Time.deltaTime;
            if (crouchRecoveryTimer <= 0f)
            {
                playerMovement.TryStandUp();
            }
        }


        targetPosition += targetOffset;
        if ((!followObjective ||distanceToTarget < 3f ) && playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange )
        {
            playerMovement.UpdateMoveInput(Vector2.zero);
            //if (straveTimer <=0)
            //{
            //    straveTimer = 0;
            //    bool straveToLeft = Random.Range(0, 2) == 0;
            //    Vector3 straveDirection = straveToLeft ? -transform.right : transform.right;

            //    Vector3 straveTarget = straveDirection * straveDistance;

            //    straveTarget.y = transform.position.y;

            //    straveOffset = straveTarget;



            //}


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
