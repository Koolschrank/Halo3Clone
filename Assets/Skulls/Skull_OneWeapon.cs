using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/NoBackWeapon")]
public class Skull_OneWeapon : Skull
{
	public override void Activate()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();

		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			var inventory = player.PlayerBody.GetComponent<PlayerInventory>();
			inventory.EnterNoInventoryMode();

		}



	}

	public override void Deactivate()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();

		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			var inventory = player.PlayerBody.GetComponent<PlayerInventory>();
			inventory.ExitNoInventoryMode();

		}

	}

	public override void PlayerSpawned(PlayerMind player)
	{
		var inventory = player.PlayerBody.GetComponent<PlayerInventory>();
		inventory.EnterNoInventoryMode();
		

	}
}
