using UnityEngine;
using System.Collections;

public class PickUpSpawn : MonoBehaviour
{
    [SerializeField] AutoPickUp pickUpPrefab;
    [SerializeField] float spawnTime = 20f;
    float timer = 1f;
	bool isPickUpActive = false;

	bool started = false;
	public void Start()
	{
		StartCoroutine(StartDelay());
	}

	IEnumerator StartDelay()
	{
		yield return new WaitForSeconds(0.1f);
		DelayStart();
	}

	public void DelayStart()
	{
		if (!GameModeSelector.gameModeManager.HasWeaponPickups)
		{
			Destroy(gameObject);
			return;
		}
		started = true; // Set started to true to indicate the spawn has begun
	}


	private void Update()
	{
		if (!started)
		{
			return; // Exit if the spawn has not started yet
		}

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
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 0.5f); // Adjust the radius as needed
	}



}
