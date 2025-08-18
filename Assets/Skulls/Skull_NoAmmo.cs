using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/noEnemyWeapons")]
public class Skull_NoAmmo : Skull
{
	public override void Activate()
	{
		base.Activate();
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.enemiesDoNotDropLoot = true;

	}

	public override void Deactivate()
	{
		base.Deactivate();
		var enemySpawner = EnemySpawner.instance;
		if (enemySpawner != null)
		{
			enemySpawner.enemiesDoNotDropLoot = false;
		}


	}
}
