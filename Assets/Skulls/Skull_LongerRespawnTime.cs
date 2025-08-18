using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/longerRespawnTime")]
public class Skull_LongerRespawnTime : Skull
{
	public float respawnTimeMultiplier = 1.5f;
	public override void Activate()
	{
		PlayerManager.instance.respawnMultiplier = respawnTimeMultiplier;
	}

	public override void Deactivate()
	{
		PlayerManager.instance.respawnMultiplier = 1f;
	}

}
