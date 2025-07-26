using UnityEngine;


[CreateAssetMenu(fileName = "Buff_Armor", menuName = "Buffs/Armor")]
public class Buff_Armor : Buff
{
	public override void ApplyBuff(GameObject player)
	{
		if (player.TryGetComponent<CharacterHealth>(out CharacterHealth health))
		{
			health.FillArmor();
		}
	}

	public override void RemoveBuff(GameObject player)
	{
		if (player.TryGetComponent<CharacterHealth>(out CharacterHealth health))
		{
			health.RemoveArmor();
		}
	}
}
