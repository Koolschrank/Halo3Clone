using UnityEngine;

public class AutoPickUp_Buff : AutoPickUp
{
	[SerializeField] Buff[] buffs;


	public override void PickUp(GameObject player)
	{
		var playerBuffs = player.GetComponent<PlayerBuffs>();


		var randomIndex = UnityEngine.Random.Range(0, buffs.Length);
		var buff = buffs[randomIndex];
		playerBuffs.ApplyBuff(buff);

		base.PickUp(player);
	}
}
