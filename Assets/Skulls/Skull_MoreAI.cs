using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/ExtraGuys")]
public class Skull_MoreAI : Skull
{
	public int extraTeamMates;
	public int extraEnemiesMates;
	public int extraEnemiesPerPlayer;


	public override void Activate()
	{
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.extraTeammates = extraTeamMates;

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		int totalPlayers = allPlayers.Count;
		enemySpawner.extraEnemies = extraEnemiesMates +  extraEnemiesPerPlayer * totalPlayers;
	}

	public override void Deactivate()
	{
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.extraTeammates = 0;
		enemySpawner.extraEnemies = 0;
	}

}
