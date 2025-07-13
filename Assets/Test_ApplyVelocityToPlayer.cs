using UnityEngine;

public class Test_ApplyVelocityToPlayer : MonoBehaviour
{
    [SerializeField] float power = 10f; // Power of the velocity to apply


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse player))
		{
			var direction = (other.transform.position - transform.position).normalized; // Get the direction from the trigger to the player

			var impact = new PlayerImpactStruct
			{
				impactForce = direction * power, // Set the impact force in the direction towards the player
				resetGravity = true // Reset the player's velocity before applying the new one
			};

			player.AddImpulse(impact);
		}
	}
}
