using UnityEngine;

public class AutoPickUp_Granade : AutoPickUp
{
	[SerializeField] AbilityData abilityData;

	public override void PickUp(GameObject player)
	{
		var abilityInventory = player.GetComponent<AbilityInventory>();

		abilityInventory.AddAbility(abilityData);
		base.PickUp(player);
	}
}
