using UnityEngine;

public class BouncePad : MonoBehaviour
{
	public float forceOnPlayer = 10f;
	public float forceOnRigidbodies = 5f;
	public bool resetVelocityOnPlayer = true;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
		{
			// Apply force to rigidbodies
			Vector3 forceDirection = transform.up; 
			rb.AddForce(forceDirection * forceOnRigidbodies, ForceMode.Impulse);
		}
		else if (other.gameObject.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse playerImpulse))
		{
			// Apply force to players
			Vector3 forceDirection = transform.up;
			var impact = new PlayerImpactStruct
			{
				impactForce = forceDirection * forceOnPlayer,
				resetGravity = resetVelocityOnPlayer
			};

			playerImpulse.AddImpulse(impact);
		}
		else
		{
			

		}

	}
}
