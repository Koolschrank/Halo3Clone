using System;
using UnityEngine;

public class PlayerGrappleHook : MonoBehaviour
{
	public PlayerMovement playerMovement;
	public PlayerSlide3D playerSlide3D;
	public Rigidbody playerRigidbody;
	public Transform grappleSpawner;
	public float grappleRange = 50f;
	public float grapplePower = 10f;
	public float grapplePowerWhenExtended = 10f;

	public Vector3 offsetVector = new Vector3(0.3f, -0.3f, 0.5f);

	Vector3 grapplePoint;
	[NonSerialized]
	public bool isGrappling = false;
	public LayerMask grappleLayerMask;

	public GameObject grappleVisualPrefab;
	private GameObject currentGrappleVisual;

	float startGrappleDistance;

	public bool isAccelerating = false;

	private void Update()
	{
		if (isGrappling)
		{
			float currentDistance = Vector3.Distance(transform.position, grapplePoint);
			float currentGrapplePower = grapplePower;
			if (currentDistance > startGrappleDistance)
			{
				currentGrapplePower = grapplePowerWhenExtended;
			}

			ForceMode forceMode = ForceMode.Acceleration;
			if (isAccelerating)
			{
				forceMode = ForceMode.VelocityChange;
			}

			Vector3 direction = (grapplePoint - transform.position).normalized;
			playerRigidbody.AddForce(direction * currentGrapplePower, forceMode);
			UpdateLine(); 
		}
	}


	

	public void ToggleGrappel(Transform visualSpawnPoint)
	{
		if (isGrappling)
		{
			DisableGrapple();
		}
		else
		{
			EnableGrapple(visualSpawnPoint);
		}
	}

	public void EnableGrapple(Transform visualSpawnPoint)
	{
		playerMovement.inSlide = true;


		RaycastHit hit;
		if (Physics.Raycast(grappleSpawner.position, grappleSpawner.forward, out hit, grappleRange, grappleLayerMask))
		{
			grapplePoint = hit.point;
			startGrappleDistance = Vector3.Distance(transform.position, grapplePoint);
		}
		else
		{
			return;
		}


		isGrappling = true;

		Vector3 vector = new Vector3(playerMovement.moveVelocity.x, playerMovement.gravityVelocity, playerMovement.moveVelocity.z);
		playerSlide3D.StartSlide(vector);


		if (grappleVisualPrefab != null)
		{
			currentGrappleVisual = Instantiate(grappleVisualPrefab, Vector3.zero, Quaternion.identity);
			

			UpdateLine();
		}
	}

	public void UpdateLine()
	{
		if (currentGrappleVisual != null)
		{
			LineRenderer lineRenderer = currentGrappleVisual.GetComponent<LineRenderer>();
			if (lineRenderer != null)
			{
				Vector3 offsetPosition = grappleSpawner.transform.TransformVector(offsetVector);


				lineRenderer.SetPosition(0, grappleSpawner.transform.position + offsetPosition);
				lineRenderer.SetPosition(1, grapplePoint);
			}
		}
	}

	public void DisableGrapple()
	{
		Debug.Log("Disable Grapple");
		isGrappling = false;

		if (currentGrappleVisual != null)
		{
			Destroy(currentGrappleVisual);
		}

		if (!playerSlide3D.IsGrounded())
		{
			playerSlide3D.StopSlide();
		}
	}


}
