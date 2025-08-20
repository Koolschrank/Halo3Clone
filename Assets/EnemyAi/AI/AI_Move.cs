using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class AI_Move : MonoBehaviour
{


    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] AI_Target target;
    [SerializeField] TargetHitCollector targetHitCollector;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] GameObject playerBody;
    [SerializeField] CharacterHealth playerHealth;
    [SerializeField] AI_Stun aiStun;
    [SerializeField] AI_Shoot aiShoot;
    [SerializeField] PlayerTeam aiTeam;

    [SerializeField] float gravityMultiplyerMaxForAiToStartJumping = 0.45f; // max gravity multiplyer to start jumping
    [SerializeField] float jumpCooldown = 7f;


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
    bool goCloseToTarget = false; // flag to check if AI should go close to target
    float jumpCooldownTimer = 0f; // timer to track jump cooldown

	bool IsInTBagStance = false;
    [SerializeField] float tBagStanceTime = 10f; // time to stay in T-Bag stance
    [SerializeField] float tBagSpeed = 0.8f; // speed to move towards T-Bag target
    [SerializeField] float tBagDistance = 0.4f; // distance to T-Bag target before starting to T-Bag
    float tBagStanceTimer = 0f; // timer to track T-Bag stance time
    GameObject tbagTarget;
    float tBagTimer = 0f; // timer to track T-Bag stance time

    float timeToIgnorePathInvalid = 1.5f; // time to ignore path invalid status
	float pathInvalidTime = 0f;
    bool inJumpState = false;


	public void Jump(Vector3 goalPosition)
	{
		//var direction = goalPosition - transform.position;
  //      direction.y = 0; // keep the jump horizontal
  //      var localDirection = playerBody.transform.InverseTransformDirection(direction);

		//// X = right/left, Z = forward/back in local space
		//Vector2 input = new Vector2(localDirection.x, localDirection.z);

		//// If target is extremely close, treat as no directional input
		//if (input.sqrMagnitude < 0.000001f)
		//{
		//	input = Vector2.zero;
		//}
		//else if (input.sqrMagnitude > 1f)
		//{
		//	// Prevent diagonal boost; normalize only when >1
		//	input.Normalize();
		//}
        var direction = goalPosition - transform.position;
        direction.y = 0; // keep the jump horizontal
        
        Vector2 input = new Vector2(direction.x, direction.z);

		playerMovement.UpdateMoveInput(input.normalized);

		inJumpState = true;
		jumpCooldownTimer = jumpCooldown; // reset cooldown
		playerMovement.TryJump();
	}

	public bool CanJump()
    {
        if (inJumpState) return false; // already in jump state
        if (jumpCooldownTimer > 0) return false; // jump cooldown is active
        if (playerMovement.gravityMultiplier <= gravityMultiplyerMaxForAiToStartJumping)
        {
            return true; // gravity is too low to jump
		}
        return false;
	}

	public void RollAwayFromDanger(Vector3 dangerPosition)
    {
        

        if (aiShoot.hasShild) return;
        if (rollCooldownTimer > 0) return;

        if (CanJump())
        {
            Jump(targetPosition);
		}

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

        targetHitCollector.OnTbagStanceTriggered += EnterTBagStance;

		agent.updatePosition = false;
		agent.updateRotation = false;
	}

    public void EnterTBagStance(GameObject tBagTarget)
    {
        if (IsInTBagStance) return;
        IsInTBagStance = true;
        this.tbagTarget = tBagTarget;
        tBagStanceTimer = tBagStanceTime;
        playerMovement.UpdateMoveInput(Vector2.zero);
        playerMovement.TryStandUp();
    }

    public void DamageTaken(DamagePackage damage)
    {
        if (rollCooldownTimer > -rollCooldownTime) return;


        var currentWeapon = playerArms.RightArm.GetWeaponInHand();
        if (currentWeapon == null) return;

        var distanceToDamage = Vector3.Distance(transform.position, damage.origin);
        if (!aiShoot.hasShild&&distanceToDamage > currentWeapon.Data.GunAiBehaviour.minDistanceToDogeWhenTakingDamage)
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
        if (GameModeSelector.gameModeManager is KingOfTheHillManager || GameModeSelector.gameModeManager is CrownManager)
        {
            // check probability to follow objective
            if (aiTeam.TeamIndex == 0 || UnityEngine.Random.Range(0f, 1f) < followObjectiveChance)
            {
                followObjective = true;
                if (GameModeSelector.gameModeManager is CrownManager)
                {
                    goCloseToTarget = true;
                    targetOffset = Vector3.zero; // no offset for CrownManager
                }
            }

           

        }

    }

    bool targetCanReach = true;

    private void Update()
    {


		agent.nextPosition = transform.position;

		rollCooldownTimer -= Time.deltaTime;
		jumpCooldownTimer -= Time.deltaTime;
		if (agent.isOnOffMeshLink && jumpCooldownTimer <= 0)
		{
			// Find the current link end in the path
			Vector3 linkEnd = agent.currentOffMeshLinkData.endPos;
            //Debug.Log("OffMeshLink detected, jumping to: " + linkEnd);
			/*
			var corners = agent.path.corners;
			// Find the index in the path closest to that end
			int closestIndex = 0;
			float closestDist = float.MaxValue;
			for (int i = 0; i < corners.Length; i++)
			{
				float dist = Vector3.SqrMagnitude(corners[i] - linkEnd);
				if (dist < closestDist)
				{
					closestDist = dist;
					closestIndex = i;
				}
			}

			// Look ahead by N waypoints, clamp to array end
			int targetIndex = Mathf.Min(closestIndex + 3, corners.Length - 1);*/
			//linkEnd.y = transform.position.y; // keep the y position of the player


			Jump(linkEnd);



			return;
		}
        

       





        if (inJumpState)
        {
            if (playerMovement.CanJump())
            {
				inJumpState = false; // reset jump state if player can jump
				agent.SetDestination(targetPosition);
			}
                
			return;
        }

		if (IsInTBagStance)
		{
			if (Time.frameCount % framesToUpdateNavAgentIfClose == 0)
			{

				agent.SetDestination(tbagTarget.transform.position);

			}
			Vector3 tBagdirection = agent.desiredVelocity.normalized;
			float distanceToTBagTarget = Vector3.Distance(transform.position, tbagTarget.transform.position);

			if (distanceToTBagTarget > tBagDistance)
			{
				playerMovement.UpdateMoveInput(new Vector2(tBagdirection.x, tBagdirection.z) * 1);

				tBagTimer -= Time.deltaTime / 3;

			}
			else
			{
				tBagStanceTimer -= Time.deltaTime;
				playerMovement.UpdateMoveInput(Vector2.zero);

				tBagTimer -= Time.deltaTime;
				if (tBagTimer <= 0f)
				{
					tBagTimer = tBagSpeed;
					playerMovement.ToggleCrouch();
				}



			}
			if (tBagStanceTimer <= 0f)
			{
				IsInTBagStance = false;
				playerMovement.TryStandUp();
				playerMovement.UpdateMoveInput(Vector2.zero);
				tbagTarget = null;
			}


			return;
		}



		targetPosition = target.GetTargetPosition();



        var followObjectiveThisFrame = false;

        if (!followObjective || playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange < 1)
        {

			if (agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                pathInvalidTime += Time.deltaTime;
                if (pathInvalidTime > timeToIgnorePathInvalid)
                {
                    followObjectiveThisFrame = true;
					targetCanReach = false;


				}
            }
			else if ( !targetCanReach && Time.frameCount % framesToUpdateNavAgent == 0)
			{
				NavMeshPath path = new NavMeshPath();
				bool hasPath = NavMesh.CalculatePath(agent.transform.position, targetPosition, NavMesh.AllAreas, path);

				if (hasPath && path.status == NavMeshPathStatus.PathComplete)
				{
                    Debug.Log("target positble");

					followObjectiveThisFrame = false;
					pathInvalidTime = 0f;
                    targetCanReach = true;
				}
			}
            followObjectiveThisFrame = !targetCanReach;
		}
        else
        {
            followObjectiveThisFrame = true;
		}


		if (followObjectiveThisFrame)
        {
			var gamemode = GameModeSelector.gameModeManager as KingOfTheHillManager;
			targetPosition = gamemode.currentHill.transform.position;

		}

        Vector3 offsetPosition = targetPosition + targetOffset;
		

		float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        

        float distanceToOffsetPosition = Vector3.Distance(transform.position, offsetPosition);
        if (playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.crouchDistance)
        {
            if (distanceToTarget >5 && CanJump())
            {
                Jump(targetPosition);
            }

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

        float targetValue = 3f;
        float targetValue2 = 1f;
        
        if (goCloseToTarget)
        {
            targetValue = 0.75f;
            targetValue2 = 0.75f;


        }

        bool closeToObjective = false;
        if (followObjectiveThisFrame)
        {
            //if (distanceToTarget < targetValue + 0.5f)
            var gamemode = GameModeSelector.gameModeManager as KingOfTheHillManager;
			if (distanceToTarget < gamemode.currentHill.Radius)
            {
                closeToObjective = true;
            }
        }
        
        if (playerArms.RightArm.InGranadeThrow)
        {
			playerMovement.UpdateMoveInput(Vector2.zero);
            return;
		}


            var idealRange = playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange;
        if ( closeToObjective ||
            (((!followObjectiveThisFrame && playerAim.OnTarget && distanceToTarget < playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.IdealRange) 
            || (followObjectiveThisFrame &&  (  distanceToTarget < targetValue || distanceToOffsetPosition < targetValue2))) ) )
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
        if (Vector3.Distance(transform.position, targetPosition) < 1.75f)
        {
			playerMovement.UpdateMoveInput(new Vector2(0,0));
            return;
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
