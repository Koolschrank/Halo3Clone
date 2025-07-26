using UnityEngine;


[CreateAssetMenu(fileName = "Buff_DualWield", menuName = "Buffs/GranadeSpam")]
public class Buff_GranadeSpam : Buff
{
	[SerializeField] float cooldownMultiplier = 1;


	public override void ApplyBuff(GameObject player)
	{
		if (player.TryGetComponent<AbilityInventory>(out AbilityInventory abilityInventory))
		{
			abilityInventory.cooldownMultiplier = cooldownMultiplier;
		}

	}

	public override void RemoveBuff(GameObject player)
	{
		if (player.TryGetComponent<AbilityInventory>(out AbilityInventory abilityInventory))
		{
			abilityInventory.cooldownMultiplier = 1;
		}
	}

	
}
