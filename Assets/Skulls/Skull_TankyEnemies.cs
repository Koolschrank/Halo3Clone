using UnityEngine;

[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/extraHealth")]
public class Skull_TankyEnemies : Skull
{
	public float damageReduction = 0.6f;


	public override void Activate()
	{
		
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.enemyDamageReduction *= damageReduction;


	}

	public override void Deactivate()
	{
		var enemySpawner = EnemySpawner.instance;
		enemySpawner.enemyDamageReduction /= damageReduction;

	}
}
