using UnityEngine;

public class PlayerJumpHeightChanger : MonoBehaviour
{
	public float jumpHeightMultiplier = 0.5f;
	PlayerManager playerManager;
	private void Start()
	{
		playerManager = FindAnyObjectByType<PlayerManager>();

		playerManager.OnPlayerSpawned += ChangeJumpHeight;
	}

	public void ChangeJumpHeight(PlayerMind player)
	{
		player.PlayerBody.GetComponent<PlayerMovement>().MultiplyJumpForce(jumpHeightMultiplier);
	}
}
