using UnityEngine;

[CreateAssetMenu(fileName = "Buff_DualWield", menuName = "Buffs/DualWield")]
public class Buff_DualWield : Buff
{

	[SerializeField] Weapon_Data[] leftHandWeapons;
	[SerializeField] int magazins;

	public override void ApplyBuff(GameObject player)
    {
        if (player.TryGetComponent<PlayerArms>(out PlayerArms arms))
		{
			arms.SetCanDualWield2HandedWeapons(true);

			if (arms.LeftArm.CurrentWeapon == null)
			{
				var randomWeapon = leftHandWeapons[UnityEngine.Random.Range(0, leftHandWeapons.Length)];
				var ammo = randomWeapon.MagazineSize * (magazins -1 );


				arms.LeftArm.PickUpWeapon(new Weapon_Arms(randomWeapon, 100));
				player.GetComponent<PlayerInventory>().AddAmmo(randomWeapon, ammo);
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
