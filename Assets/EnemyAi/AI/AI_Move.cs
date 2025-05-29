using System.Collections;
using Unity.Mathematics;
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
    [SerializeField] GameObject playerBody;
    [SerializeField] CharacterHealth playerHealth;
    [SerializeField] AI_Stun aiStun;


    Vector3 targetPosition;
    Vector3 targetOffset = Vector3.zero;

    [SerializeField] int framesToUpdateNavAgent = 100;
    [SerializeField] int framesToUpdateNavAgentIfClose = 10;
    [SerializeField] float distanceToUpdateNavAgent = 5f;



    [SerializeField] float followObjectiveChance = 0.5f;

    [SerializeField] float crouchRecoveryTime = 1f; // time to recover from crouch to stand up
    [SerializeField] float rollCooldownTime = 1f; // time to recover from roll to stand up
    [SerializeField] float maxStraveOffsetDistance = 5f; // max distance from target to offset the position
    [SerializeField] LayerMask wallLayerMask; // layer mask to check for walls when rolling away from danger
    [SerializeField] float wallCheckDistance = 2f; // distance to strave to the left or right

    float rollCooldownTimer = 0f; // timer to track roll cooldown
    float crouchRecoveryTimer = 0f;

    bool followObjective = false;




    public void RollAwayFromDanger(Vector3 dangerPosition)
    {
        if (rollCooldownTimer > 0) return;
        bool isWallToTheLeft = IsWallToTheLeft();
        bool isWallToTheRight = IsWallToTheRight();
        if (isWallToTheLeft && !isWallToTheRight)
        {
            RollToTheRightSide();
            return;
        }
        else if (isWallToTheRight && !isWallToTheLeft)
        {
            RollToTheLeftSide();
            return;
        }



        var dangerDirection = (transform.position - dangerPosition).normalized;

        if (Vector3.Dot(dangerDirection, transform.right) > 0.5f)
        {
            RollToTheRightSide();
            
        }
        else if (Vector3.Dot(dangerDirection, -transform.right) > 0.5f)
        {
            RollToTheLeftSide();
        }
        else
        {
            // If the danger is not clearly to the left or right, roll in a random direction
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                RollToTheRightSide();
            }
            else
            {
                RollToTheLeftSide();
                
            }
        }
    }

    public bool IsWallToTheLeft()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = -transform.right; // left direction
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, wallCheckDistance, wallLayerMask))
        {
            return true; // Wall detected to the left
        }
        return false; // No wall detected to the left
    }

    public bool IsWallToTheRight()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.right; // right direction
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, wallCheckDistance, wallLayerMask))
        {
            return true; // Wall detected to the right
        }
        return false; // No wall detected to the right
    }


    public void ForceRotationToLeftSide()
    {
        playerBody.transform.Rotate(0, -90, 0);
    }

    public void ForceRotationToRightSide()
    {
        playerBody.transform.Rotate(0, 90, 0);
    }


    public void RollToTheLeftSide()
    {
        ForceRotationToLeftSide();
        playerMovement.UpdateMoveInput(Vector2.up);
        playerMovement.TryRoll();
        rollCooldownTimer = rollCooldownTime; // reset roll cooldown timer
    }

    public void RollToTheRightSide()
    {
        ForceRotationToRightSide();
        playerMovement.UpdateMoveInput(Vector2.up);
        playerMovement.TryRoll();
        rollCooldownTimer = rollCooldownTime;
    }



    private void Awake()
    {
        targetPosition = EnemyPatrolTargetPositions.Instance.GetRandomPatrolPoint().position;


        playerHealth.OnDamageTaken += DamageTaken;


        var randomInSperee = UnityEngine.Random.insideUnitSphere;
        randomInSperee.y = 0; // keep it on the ground
        targetOffset = randomInSperee.normalized * maxStraveOffsetDistance;
    }

    public void DamageTaken(DamagePackage damage)
    {
        if (rollCooldownTimer > -rollCooldownTime) return;


        var currentWeapon = playerArms.RightArm.GetWeaponInHand();
        if (currentWeapon == null) return;

        var distanceToDamage = Vector3.Distance(transform.position, damage.origin);
        if (distanceToDamage > currentWeapon.Data.GunAiBehaviour.minDistanceToDogeWhenTakingDamage)
        {
            StartCoroutine(RollWothDelay(damage.origin));
        }
    }

    IEnumerator RollWothDelay(Vector3 damage)
    {
        yield return new WaitForSeconds(0.2f);

        if (!aiStun.IsStunned())
        {
            RollAwayFromDanger(damage);
        }
            
    }



    public void Start()
    {
        if (GameModeSelector.gameModeManager is KingOfTheHillManager )
        {
            // check probability to follow objective
            if (UnityEngine.Random.Range(0f, 1f) < followObjectiveChance)
            {
                followObjective = true;
                
            }
            
        }

    }

    private void Update()
    {
        rollCooldownTimer -= Time.deltaTime;
        


        targetPosition = target.GetTargetPosition();
        if (followObjective)
        {
            targetPosition = ObjectiveIndicator.instance.GetObjective(0).Position;
        }

        Vector3 offsetPosition = targetPosition + targetOffset;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float distanceToOffsetPosition = Vector3.Distance(transform.position, offsetPosition);
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


        var idealRange = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        if (
            ((!followObjective && playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange) 
            || (followObjective &&  (distanceToTarget < 3f || distanceToOffsetPosition < 1f))) )
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


        if (playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange < targetOffset.magnitude)
        {
            targetOffset = targetOffset.normalized * playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        }

        // do a raycast to check if there is a wall between the target and target+offset
        
        RaycastHit hit;
        if (!Physics.Raycast(targetPosition, targetOffset.normalized, out hit, targetOffset.magnitude, wallLayerMask))
        {
            targetPosition += targetOffset;
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
