using UnityEngine;

public class AutoPickUp_Buff : AutoPickUp
{
	[SerializeField] Buff buff;


	public override void PickUp(GameObject player)
	{
		var playerBuffs = player.GetComponent<PlayerBuffs>();
		playerBuffs.ApplyBuff(buff);

		base.PickUp(player);
	}
}
