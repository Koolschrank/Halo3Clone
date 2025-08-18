using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/ExtraGuys")]
public class Skull_MoreAI : Skull
{
	public int extraTeamMates;
	public int extraEnemiesMates;


	public override void Activate()
	{
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.extraTeammates = extraTeamMates;
		enemySpawner.extraEnemies = extraEnemiesMates;
	}

	public override void Deactivate()
	{
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.extraTeammates = 0;
		enemySpawner.extraEnemies = 0;
	}

}
