using UnityEngine;

[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/Shield Gain Only On Melee")]
public class Skull_ShildGainOnlyOnMelee : Skull
{
	public override void Activate()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();

		foreach (var player in allPlayers)
		{
			if (player == null ||player.IsDead) continue;
			SetUpPlayerBody(player.PlayerBody.GetComponent<BodyMindConnection>(), true);
		}



	}

	public override void Deactivate()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();

		foreach (var player in allPlayers)
		{
			if (player == null || player.IsDead) continue;
			SetUpPlayerBody(player.PlayerBody.GetComponent<BodyMindConnection>(), false);
		}

	}

	public override void PlayerSpawned(PlayerMind player)
	{
		SetUpPlayerBody(player.PlayerBody.GetComponent<BodyMindConnection>(), true);
	}

	public void SetUpPlayerBody(BodyMindConnection body,bool value)
	{
		body.GetHealth().shildGainOnMelee = value;
		body.GetMeleeAttacker().shildGainOnMelee = value;
	}
}
