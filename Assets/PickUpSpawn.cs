using UnityEngine;

public class PickUpSpawn : MonoBehaviour
{
    [SerializeField] AutoPickUp pickUpPrefab;
    [SerializeField] float spawnTime = 20f;
    float timer = 0f;
	bool isPickUpActive = false;

	private void Update()
	{
		if (isPickUpActive)
		{
			return; // Exit if a pick-up is already active
		}

		timer -= Time.deltaTime; // Decrease the timer by the time since last frame
		if (timer <= 0f)
		{
			SpawnPickUp(); // Spawn a new pick-up when the timer reaches zero
		}
		



	}

	private void SpawnPickUp()
	{
		if (pickUpPrefab != null)
		{
			AutoPickUp pickUp = Instantiate(pickUpPrefab, transform.position, Quaternion.identity);
			pickUp.OnPickUp += OnPickUp;
			isPickUpActive = true;
			timer = spawnTime; // Reset the timer after spawning a pick-up
		}
		else
		{
			Debug.LogWarning("PickUp prefab is not assigned in the inspector.");
		}
	}

	private void OnPickUp()
	{
		// Handle the pick-up logic here, if needed
		Debug.Log("PickUp has been collected!");
		isPickUpActive = false;
		timer = spawnTime; // Reset the timer after spawning a pick-up
	}


	// draw gizmo sphere in editor
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 0.5f); // Adjust the radius as needed
	}



}
