using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/noShild")]
public class Skull_NoShild : Skull_TankyEnemies
{

	public override void Activate()
	{
		base.Activate();

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			var health = player.PlayerBody.GetComponent<CharacterHealth>();
			health.RemoveShild();
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			var health = player.PlayerBody.GetComponent<CharacterHealth>();
			health.RestoreShild();
		}

	}

	public override void PlayerSpawned(PlayerMind player)
	{
		base.PlayerSpawned(player);
		var health = player.PlayerBody.GetComponent<CharacterHealth>();
		if (health == null) return;
		health.RemoveShild();
	}
}
