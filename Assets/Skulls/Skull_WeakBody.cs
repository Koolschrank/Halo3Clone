using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/weakBody")]
public class Skull_WeakBody : Skull
{
	public override void Activate()
	{
		base.Activate();

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			var health = player.PlayerBody.GetComponent<CharacterHealth>();
			health.weakBody = true;
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
			health.weakBody = false;
		}

	}

	public override void PlayerSpawned(PlayerMind player)
	{
		base.PlayerSpawned(player);
		var health = player.PlayerBody.GetComponent<CharacterHealth>();
		if (health == null) return;
		health.weakBody = true;
	}
}
