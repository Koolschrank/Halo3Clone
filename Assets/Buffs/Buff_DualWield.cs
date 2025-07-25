using UnityEngine;

[CreateAssetMenu(fileName = "Buff_DualWield", menuName = "Buffs/DualWield")]
public class Buff_DualWield : Buff
{

	[SerializeField] Weapon_Data leftHandWeapon;
	[SerializeField] int leftHandWeaponAmmo;

	public override void ApplyBuff(GameObject player)
    {
        if (player.TryGetComponent<PlayerArms>(out PlayerArms arms))
		{
			arms.SetCanDualWield2HandedWeapons(true);

			if (arms.LeftArm.CurrentWeapon == null)
			{
				arms.LeftArm.PickUpWeapon(new Weapon_Arms(leftHandWeapon, leftHandWeaponAmmo));
			}
			
		}

		
	}

	public override void RemoveBuff(GameObject player)
	{
		if (player.TryGetComponent<PlayerArms>(out PlayerArms arms))
		{
			arms.SetCanDualWield2HandedWeapons(false);

		}
	}
}
