using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class PlayerMovement : MonoBehaviour
{

    public Action OnJump;
    public Action OnCrouch;
    public Action OnStandUp;
    public Action<Vector3> OnMoveUpdated;
    public Action<Vector2> OnAimUpdated;

    public Action<Vector3, float> OnRollStarted;
    public Action OnRollEnded;

    // character controller 
    [Header("References")]
    [SerializeField] CharacterController cc;
    [SerializeField] Transform head;
    [SerializeField] Transform head_normalPosition;
    [SerializeField] Transform head_crouchPosition;
    [SerializeField] PlayerArms arms;
    [SerializeField] PlayerBodyStatSheet statSheet;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] PlayerSlide3D playerSlide3D;
	[Header("Settings")]


    // movement speed
    [SerializeField] float maxMoveSpeed = 12f;
    [SerializeField] float moveSpeedRoolMultiplier = 1.6f; // multiplier for the move speed, used by modifiers and other things
    [SerializeField] float moveSpeedCrouchMultiplier = 0.4f;
    [SerializeField] float acceleration_ground = 10f;
    [SerializeField] float acceleration_air = 5f;
    [SerializeField] float deceleration_ground = 10f;
    [SerializeField] float deceleration_air = 5f;
    [SerializeField] float acceleration_roll = 5f;
	[SerializeField] float acceleration_pushed = 5f;
	[SerializeField] float acceleration_slideJump = 0f;

	[SerializeField] float minPushTime = 0.1f;

	[SerializeField] float jumpPower = 9.8f;
    [SerializeField] float jumpCooldown = 0.5f;
    float jumpCooldownTimer = 0;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float cyoteTime = 0.2f;
    bool isGrounded => cc.isGrounded || Time.time - lastGroundTouch < cyoteTime;
    float lastGroundTouch;
    [SerializeField] float crouchSpeed = 0.5f;
    [SerializeField] bool ignoreAimDirection = false;

    [SerializeField] PlayerHitBoxSize playerStandingHitbox;
    [SerializeField] PlayerHitBoxSize playerCrouchingHitbox;
    [SerializeField] float rollTime = 1; // time it takes to roll
    [SerializeField] AnimationCurve rollCurve; // curve for the roll animation, used to determine the speed of the roll
    public float physicsImpactIgnoreGravityResetTime = 0.8f;

    float ignoreGravityResetTimer = 0f; // timer to reset the ignore gravity state
	float maxMoveSpeedMultiplier = 1f;
    public float weaponMoveSpeedMultiplier = 1f;

    public float slideJumpPlayerInputStrength = 1.5f;
    public float slideJumpPlayerInputStrenghtScaledByMoveSpeed = 0.2f;


	[Header("Sound")]
    [SerializeField] EventReference walkSound;
    [SerializeField] float distanceForWalkSound = 1f;
    float distanceToWalkSoundLeft = 0;
    [SerializeField] EventReference jumpSound;



    [NonSerialized]
    public Vector3 moveVelocity = Vector3.zero;
    [NonSerialized]
    public float gravityVelocity = 0;
    Vector2 moveInput = Vector2.zero;

    public float MaxMoveSpeed => maxMoveSpeed * maxMoveSpeedMultiplier;
    [NonSerialized]
    public bool inCrouch = false;

	[NonSerialized]
	public float gravityMultiplier = 1f;

    bool inRoll = false;
    Vector3 rollDirection = Vector3.zero;
    float rollTimer = 0;


    float moveSpeedStatSheetMultiplier = 1f; // multiplier for the move speed, used by modifiers and other things
    [NonSerialized]
    public float aura_moveSpeedReduction = 0f;

    Vector3 distanceWentThisFrame = Vector3.zero;

    [NonSerialized]
    public bool inPushedState = false; // if the player is in a pushed state, used to determine if the player can move or not
    float pushedTimer = 0f; // timer for the pushed state, used to determine if the player can move or not

    public LayerMask GroundLayer;

    public bool canSlide;
    public float slideCancelMultiplier = 1.3f;
    [NonSerialized]
	public bool inSlide = false;
    bool inSlideJump = false;

    public void MultiplyJumpForce(float multiplier)
        {
        jumpPower *= multiplier;
	}
	public void ApplyImpact(PlayerImpactStruct impact)
    {
        if (meleeAttacker.hasPowerArmor)
        {
            impact.impactForce *= meleeAttacker.powerArmorDamageMultiplier;
		}

		if ( impact.resetGravity)
		{
			gravityVelocity = 0;
		}
		moveVelocity += new Vector3(impact.impactForce.x, 0, impact.impactForce.z);
		gravityVelocity += impact.impactForce.y; // apply vertical impact to gravity
        ignoreGravityResetTimer = physicsImpactIgnoreGravityResetTime; // reset the ignore gravity timer

        inPushedState = true; // set the player in a pushed state
        pushedTimer = minPushTime; // set the pushed timer to the minimum push time
        if (impact.impactForce.magnitude > maxMoveSpeed)
            OnJump?.Invoke();

        meleeAttacker.CancelLaunch(); // cancel the melee attack launch if the player is in a pushed state
	}

    public void MultiplyMaxMoveSpeed(float multiplier)
    {
        maxMoveSpeed *= multiplier;
    }


    public void MultiplySpeed(float multiplier)
    {
        maxMoveSpeedMultiplier *= multiplier;
    }

    private void Awake()
    {
        if (statSheet != null)
        {
            statSheet.OnStatSheetUpdated += UpdateStatSheet;
        }


		if ( GravityOverrider.Instance != null)
		{
            gravityMultiplier = GravityOverrider.Instance.playerGravityMultiplier;
		}

        if (playerSlide3D != null)
			playerSlide3D.OnStopSlide += CancelSlide;
	}

    public void UpdateStatSheet()
    {
        if (!statSheet.useStatSheet) return;

        moveSpeedStatSheetMultiplier = statSheet.playerStatsSheetInstance.movementSpeedMultiplier;

    }

    // update
    void Update()
    {
        var lastPosition = transform.position;
		if (inRoll)
        {
            UpdateRoll();
        }
        else
        {
            UpdateCrouch();
            UpdateMove();

            if (moveVelocity.magnitude > 0 && cc.isGrounded)
            {
                distanceToWalkSoundLeft -= moveVelocity.magnitude * Time.deltaTime;
                if (distanceToWalkSoundLeft <= 0)
                {
                    AudioManager.instance.PlayOneShot(walkSound, transform.position);
                    distanceToWalkSoundLeft = distanceForWalkSound;
                }
            }
        }

            
        UpdateGravity();

        var moveVector = new Vector3(moveVelocity.x, gravityVelocity, moveVelocity.z);
        
        if (!inSlide)
        {
			cc.Move(moveVector * Time.deltaTime);
			OnMoveUpdated?.Invoke(moveVector);
		}
        else
        {
			OnMoveUpdated?.Invoke(playerSlide3D.GetVelocity());
		}






        if (cc.isGrounded)
        {
            lastGroundTouch = Time.time;
        }

        distanceWentThisFrame = transform.position - lastPosition;

        if (gravityVelocity> 0 && distanceWentThisFrame.y <= 0)
        {
			// raycast up to check if we are hitting the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.up, out hit, 2f, GroundLayer))
            {
                // if we are hitting the ground, reset the gravity velocity
                gravityVelocity = 0;
			}



		}

	}

    public void CancelSlide(Vector3 rigidbodySlideForce)
    {
        if (inSlide)
        {
            
            inSlide = false;

			moveVelocity = new Vector3(rigidbodySlideForce.x, moveVelocity.y, rigidbodySlideForce.z)* slideCancelMultiplier;

            gravityVelocity = rigidbodySlideForce.y;

			if (inCrouch)
			{
				ToggleCrouch();
			}

			if (gravityVelocity > 0)
            {
				AudioManager.instance.PlayOneShot(jumpSound, transform.position);
				jumpCooldownTimer = jumpCooldown;
				OnJump?.Invoke();
				inSlideJump = true;


			}
            if (rigidbodySlideForce.magnitude > 3f)
            {
				inPushedState = true; // set the player in a pushed state
				pushedTimer = minPushTime; // set the pushed timer to the minimum push time
			}
			
		}
	}
    


    private void UpdateCrouch()
    {

        
        if ( inCrouch)
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, head_crouchPosition.position, crouchSpeed * Time.deltaTime);

            if (cc.height != playerCrouchingHitbox.Height || cc.center.y != playerCrouchingHitbox.Offset)
            {
                cc.height = playerCrouchingHitbox.Height;
                cc.center = new Vector3(0, playerCrouchingHitbox.Offset, 0);
            }
            if (canSlide && playerSlide3D.CanStartSlide() && !inSlide)
            {
                playerSlide3D.StartSlide(new Vector3(moveVelocity.x, gravityVelocity, moveVelocity.z));
                inSlide = true;
			}


        }
        else
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, head_normalPosition.position, crouchSpeed * Time.deltaTime);

            if (cc.height != playerStandingHitbox.Height || cc.center.y != playerStandingHitbox.Offset)
            {
                cc.height = playerStandingHitbox.Height;
                cc.center = new Vector3(0, playerStandingHitbox.Offset, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        

    }
    bool slopeGravityApplied = false;

	private void UpdateMove()
    {
        Vector2 input = this.moveInput;//controller.Player.Move.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(input.x, 0, input.y);
        Vector3 camForward = head.transform.forward;
        Vector3 camRight = head.transform.right;



        if (ignoreAimDirection)
        {
            camForward = Vector3.forward;
            camRight = Vector3.right;
        }
       

        camForward.y = 0;
        camForward.Normalize();
        
        Vector3 move = camForward * moveInput.z + camRight * moveInput.x;

		var acceleration = cc.isGrounded ? acceleration_ground : acceleration_air;

		if (inPushedState)
		{
			acceleration = acceleration_pushed;
            
			pushedTimer -= Time.deltaTime;
			if (pushedTimer <= 0 && (cc.isGrounded || moveVelocity.magnitude < maxMoveSpeed))
			{
				inPushedState = false; // reset the pushed state
			}
		}
        else if (meleeAttacker.InLaunch)
        {
			moveVelocity = Vector3.MoveTowards(moveVelocity, Vector2.zero, deceleration_ground * Time.deltaTime);
            return;
		}
		if (inSlideJump)
        {
            Debug.Log("SlideJump");
            acceleration = acceleration_slideJump;

            moveVelocity += move.normalized * (slideJumpPlayerInputStrength + slideJumpPlayerInputStrenghtScaledByMoveSpeed * moveVelocity.magnitude) * Time.deltaTime;

		}


		if (move.magnitude == 0)
        {
            if (!inPushedState && !inSlideJump)
                acceleration = cc.isGrounded ? deceleration_ground : deceleration_air;


            moveVelocity = Vector3.MoveTowards(moveVelocity, Vector2.zero, acceleration * Time.deltaTime);
        }
        else
        {


            var moveSpeedMultiplier = 1f;
            if (arms.IsDualWielding)
            {
                moveSpeedMultiplier = arms.MovementSpeedMultiplier;
            }
            else
            {
                var weapon = arms.RightArm.CurrentWeapon;
                if (weapon != null)
                {
                    moveSpeedMultiplier = weapon.MoveSpeedMultiplier;
                }
            }





            var speedMultiplier = MaxMoveSpeed * moveSpeedMultiplier;
            if (inCrouch)
            {
                speedMultiplier *= moveSpeedCrouchMultiplier;
            }
            else if (arms.RightArm.IsInZoom)
            {
                speedMultiplier *= arms.RightArm.CurrentWeapon.Data.ZoomMoveSpeed;
            }
            else if (arms.LeftArm.IsInZoom)
            {
                speedMultiplier *= arms.LeftArm.CurrentWeapon.Data.ZoomMoveSpeed;
            }

            var ideal = move * speedMultiplier * moveSpeedStatSheetMultiplier * weaponMoveSpeedMultiplier;
            if (aura_moveSpeedReduction != 0)
            {
                ideal *= 1 - aura_moveSpeedReduction;
            }


            moveVelocity = Vector3.MoveTowards(moveVelocity, ideal, acceleration * Time.deltaTime);
        }

        slopeGravityApplied = false;
		if (isGrounded && jumpCooldownTimer <=0)
        {
            inSlideJump = false;

			float slopeDot = CheckIfGoingDownASlope(moveVelocity);

			if (slopeDot > 0f)
			{
				slopeGravityApplied = true;
				// Apply additional downward pull proportional to how much we are moving down the slope
				gravityVelocity = Mathf.Lerp(gravityVelocity, Physics.gravity.y, slopeDot);
			}
		}

        
    }


	public float CheckIfGoingDownASlope(Vector3 moveVelocity)
	{
		if (cc.isGrounded)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, cc.height / 2 + 0.5f, GroundLayer))
			{
				// Project the ground normal onto the XZ plane to get slope direction
				Vector3 slopeDir = Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal), hit.normal);
				slopeDir.Normalize();

				// Flatten move velocity to XZ plane (ignore vertical)
				Vector3 moveDir = new Vector3(moveVelocity.x, 0f, moveVelocity.z).normalized;

				// Check alignment (dot product) between movement and downslope
				float dot = Vector3.Dot(moveDir, slopeDir);

				// Optional: return dot only if positive (means moving down the slope)
				return dot > 0f ? dot : 0f;
			}
		}

		return 0f;
	}

	public void SetMovementSpeedMultiplier(float multiplier)
    {
        maxMoveSpeedMultiplier = multiplier;
    }

    private void UpdateGravity()
    {
        ignoreGravityResetTimer -= Time.deltaTime;

		if (cc.isGrounded && jumpCooldownTimer <= 0 && ignoreGravityResetTimer <= 0 && !inPushedState)
        {
            if (!slopeGravityApplied)
				gravityVelocity = -0.1f;

		}
        else
        {
            gravityVelocity -= gravity * gravityMultiplier * Time.deltaTime;
        }

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    public bool CanJump()
    {
        if (inSlide)
        {
            return playerSlide3D.IsGrounded();
		}

        return isGrounded && !inRoll && jumpCooldownTimer <= 0;
	}

	public void TryJump()
    {
		if(inSlide && CanJump())
        {
            playerSlide3D.TryJump();
			
			return;
		}



		if (isGrounded && !inRoll && jumpCooldownTimer <= 0)
        {
            
            gravityVelocity = jumpPower;

			AudioManager.instance.PlayOneShot(jumpSound, transform.position);
			jumpCooldownTimer = jumpCooldown;
            OnJump?.Invoke();
            if (inCrouch)
            {
                ToggleCrouch();
            }
            
        }
    }

    public void UpdateRoll()
    {
        rollTimer -= Time.deltaTime;
        var rollSpeed = rollCurve.Evaluate(1 - (rollTimer / rollTime)); // evaluate the curve based on the remaining time

        head.transform.position = Vector3.MoveTowards(head.transform.position, head_crouchPosition.position, crouchSpeed * Time.deltaTime);

        var ideal = rollSpeed * rollDirection * MaxMoveSpeed * moveSpeedRoolMultiplier;
		if (aura_moveSpeedReduction != 0)
		{
			ideal *= 1 - aura_moveSpeedReduction;
		}

		moveVelocity = Vector3.MoveTowards(moveVelocity, ideal, acceleration_roll * Time.deltaTime);

        if (cc.height != playerCrouchingHitbox.Height || cc.center.y != playerCrouchingHitbox.Offset)
        {
            cc.height = playerCrouchingHitbox.Height;
            cc.center = new Vector3(0, playerCrouchingHitbox.Offset, 0);
        }

        if (rollTimer <= 0)
        {
            EndRoll();
        }
    }

    public bool CanRoll()
    {
        return isGrounded && jumpCooldownTimer <= 0 && moveInput != Vector2.zero && !inRoll && (moveInput.normalized).magnitude > 0.9f;
    }

    public void TryRoll()
    {
        if (isGrounded && jumpCooldownTimer <= 0 && moveInput != Vector2.zero && !inRoll && (moveInput.normalized).magnitude > 0.9f)
        {
            TryStandUp();
            var forward = transform.forward;
            // use input and forward direction to determine roll direction
            var rollDirection = (forward * moveInput.y + transform.right * moveInput.x).normalized;
            rollDirection.y = 0; // ensure we are rolling on the ground plane
            inRoll = true;
            
            this.rollDirection = rollDirection;
            rollTimer = rollTime;
            OnRollStarted?.Invoke(rollDirection, rollTime);

        }
    }

    public void EndRoll()
    {
        inRoll = false;
        OnRollEnded?.Invoke();
        moveVelocity = Vector3.zero; // reset move velocity after roll
    }

    // player input funtion
    public void UpdateMoveInput(Vector2 input)
    {
        moveInput = input;

    }



    public void ToggleCrouch()
    {
        if (inSlide)
        {
            playerSlide3D.StopSlide();
			inCrouch = false;
			OnStandUp?.Invoke();
			return;
		}

        
        if (inCrouch)
        {
            inCrouch = false;
            OnStandUp?.Invoke();
            
        }
        else 
        {
            // do raycast down 
            bool raycastHit = Physics.Raycast(transform.position, Vector3.down, 1, GroundLayer);

			if (cc.isGrounded || raycastHit)
			{
				inCrouch = true;
				OnCrouch?.Invoke();
			}
				
        }
    }

    public void TryCrouch()
    {

        if (!inCrouch&&cc.isGrounded)
        {
            inCrouch = true;
            OnCrouch?.Invoke();
        }
    }

    public void TryStandUp()
    {
        if (inCrouch)
        {
            inCrouch = false;
            OnStandUp?.Invoke();
        }
    }


    public void AddHeight(float amount, float centerOffset)
    {
        playerStandingHitbox.Height += amount;
        playerCrouchingHitbox.Height += amount;
        playerStandingHitbox.Offset += centerOffset;
        playerCrouchingHitbox.Offset += centerOffset;
	}

}


[Serializable]
public struct PlayerHitBoxSize
{
    public float Height;
    public float Offset;
}