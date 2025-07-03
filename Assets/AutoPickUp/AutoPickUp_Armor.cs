using UnityEngine;

public class AutoPickUp_Armor : AutoPickUp
{
	public override void PickUp(GameObject player)
	{
		var health = player.GetComponent<CharacterHealth>();
		if (health != null)
		{
			health.FillArmor();
			health.GainShild(0.1f);
		}

		base.PickUp(player);
	}
}
