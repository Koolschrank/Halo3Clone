using System.Collections.Generic;
using UnityEngine;

public class PhysicsForceArea : MonoBehaviour
{
	public float forceOnRigidbodyiesMagnitude = 10f;
	public float playerGravityCap = 50f;
	public float forceOnPlayersMagnitude = 10f;
	public float forceMultiplierIfYVelocityIsNegative = 2f;


	public bool hasPlayerGravity = false;
	public float playerGravityChange = 0.5f;

	List<Rigidbody> affectedRigidbodies = new List<Rigidbody>();
    List<PlayerPhysicsImpulse> affectedPlayers = new List<PlayerPhysicsImpulse>();

	private void FixedUpdate()
	{
		var deltaTime = Time.deltaTime;
		foreach (var rb in affectedRigidbodies)
		{
			if (rb != null)
			{
				var vector = transform.up * forceOnRigidbodyiesMagnitude;
				if (rb.linearVelocity.y < 0)
				{
					vector *= forceMultiplierIfYVelocityIsNegative;
				}

				rb.AddForce(vector, ForceMode.Acceleration);
			}
		}

		if (forceOnPlayersMagnitude != 0)
		{
			foreach (var player in affectedPlayers)
			{
				if (player != null)
				{
					// Cap the player's gravity force
					var gravityForce = player.GetPlayerGravityForce();
					if (gravityForce > playerGravityCap)
					{
						continue;
					}
					var vector = transform.up * forceOnPlayersMagnitude * deltaTime;
					if (gravityForce <= 0)
					{
						vector.y *= forceMultiplierIfYVelocityIsNegative;

					}
					player.AddImpulse(new PlayerImpactStruct
					{
						impactForce = vector,
						resetGravity = false
					});
				}
			}
		}
		

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
		{
			if (!affectedRigidbodies.Contains(rb))
			{
				affectedRigidbodies.Add(rb);
			}
		}
		else if (other.gameObject.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse player))
		{
			if (!affectedPlayers.Contains(player))
			{
				affectedPlayers.Add(player);
				if (hasPlayerGravity)
				{
					player.ChangeGravity(playerGravityChange);
				}
			}
		}

	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
		{
			if (affectedRigidbodies.Contains(rb))
			{
				affectedRigidbodies.Remove(rb);
			}
		}
		else if (other.gameObject.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse player))
		{
			if (affectedPlayers.Contains(player))
			{
				affectedPlayers.Remove(player);
				if (hasPlayerGravity)
				{
					float baseGravity = 1;
					var gravityOverride = GravityOverrider.Instance;
					if (gravityOverride != null)
					{
						baseGravity = gravityOverride.playerGravityMultiplier;
					}
					player.ChangeGravity(baseGravity);
				}
			}

		}
	}
}
