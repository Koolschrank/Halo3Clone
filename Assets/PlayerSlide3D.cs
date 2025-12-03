using System;
using UnityEngine;
using System.Collections;
using static UnityEngine.LightAnchor;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerSlide3D : MonoBehaviour
{
	public Action OnStartSlide;
	public Action<Vector3 > OnStopSlide;

	[Header("Sliding Settings")]
	public float slideStartAngle = 30f; // Minimum angle to start sliding
	public float slideForce = 15f;
	public float slideExitSpeed = 2f; // When to stop sliding
	public float slopeRayLength = 1.5f;
	public float minSlideTime = 0.5f;
	float slideTimer = 0f;

	public float ccVelocityTranslationMultiplier = 0.1f;

	[Header("References")]
	public Transform orientation; // Optional: forward direction for player
	public CapsuleCollider capsuleCollider;
	public PlayerGrappleHook grappleHook;

	private CharacterController controller;
	private Rigidbody rb;
	private bool isSliding = false;
	private Vector3 slopeNormal;

	

	public bool CanStartSlide()
	{
		// get angle of the ground below the player
		float angle = 0f;
		if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength))
		{
			angle = Vector3.Angle(hit.normal, Vector3.up);
		}

		return angle > slideStartAngle;
	}

	public bool CheckIfTooSlowToSlide()
	{
		return rb.linearVelocity.magnitude < slideExitSpeed;
	}

	public Vector3 GetVelocity()
	{
		return rb.linearVelocity;
	}


	void Start()
	{
		controller = GetComponent<CharacterController>();
		rb = GetComponent<Rigidbody>();

		rb.isKinematic = true; // start in character controller mode
	}

	void Update()
	{
		if (!isSliding)
		{
			return;
		}
		if ( Time.time < slideTimer)
		{
			return; // still in minimum slide time
		}
		if (rb.linearVelocity.magnitude < slideExitSpeed)
		{
			StopSlide();
		}
	}

	void FixedUpdate()
	{
		if (isSliding)
		{

			// calculate slope normal
			// raycast down to get slope normal
			bool onSlope = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength);
			if (onSlope)
			{
				slopeNormal = hit.normal;
			}
			else
			{
				slopeNormal = Vector3.up;
			}
			Vector3 slopeDir = Vector3.Cross(Vector3.Cross(slopeNormal, Vector3.down), slopeNormal);
			rb.AddForce(slopeDir.normalized * slideForce, ForceMode.Acceleration);
		}
	}

	//bool OnSteepSlope(out Vector3 hitNormal)
	//{
	//	if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength))
	//	{
	//		float angle = Vector3.Angle(hit.normal, Vector3.up);
	//		hitNormal = hit.normal;
	//		return angle > slopeLimit;
	//	}
	//	hitNormal = Vector3.up;
	//	return false;
	//}

	//bool IsGroundFlat()
	//{
	//	if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength))
	//	{
	//		float angle = Vector3.Angle(hit.normal, Vector3.up);
	//		return angle < slopeLimit * 0.5f;
	//	}
	//	return false;
	//}

	public void StartSlide(Vector3 velocity)
	{
		if (isSliding) return; // already sliding

		isSliding = true;

		controller.enabled = false; // disable CharacterController
		rb.isKinematic = false;
		rb.AddForce(velocity * ccVelocityTranslationMultiplier, ForceMode.VelocityChange);



		    // enable physics
		rb.useGravity = true;

		OnStartSlide?.Invoke();
		slideTimer = Time.time + minSlideTime;

		capsuleCollider.enabled = true;
	}

	public void StopSlide()
	{
		if (grappleHook.isGrappling) return; // cannot stop sliding while grappling

		if (!isSliding) return; // not sliding
		OnStopSlide?.Invoke(rb.linearVelocity);
		isSliding = false;

		rb.isKinematic = true;      // stop physics
		controller.enabled = true;  // re-enable normal control

		

		capsuleCollider.enabled = false;
	}

	public float jumpPower = 5f;

	public float jumpCooldown = 0.2f;
	float lastJumpTime = 0f;

	public void TryJump()
		{
		if (Time.time < lastJumpTime + jumpCooldown)
		{
			return; // still in cooldown
		}
		if (!isSliding)
		{
			return; // can only jump while sliding
		}
		if (!IsGrounded())
		{
			return; // must be grounded
		}
		Vector3 v = rb.linearVelocity;
		if (v.y <0)
		{
			v.y = 0f;
			rb.linearVelocity = v;
		}
		

		Vector3 jumpDirection = Vector3.up;
		rb.AddForce(jumpDirection.normalized * jumpPower, ForceMode.VelocityChange);
		lastJumpTime = Time.time;

		StartCoroutine(JumpRoutine());
	}

	IEnumerator JumpRoutine()
	{
		
		yield return new WaitForFixedUpdate();
		StopSlide();
	}

	public bool IsGrounded()
	{
		// raycast down to check for ground
		bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength);

		return isGrounded;

	}
}
