using UnityEngine;

public class ArmsRender : ArmsEvents
{
    public override void Render()
    {
        base.Render();

        if (!HasStateAuthority && !HasInputAuthority)
        {
            ProxyOnlyRender();
        }
    }

    void ProxyOnlyRender()
    {
        if (!HasWeaponIndex(Weapon_RightHand, WeaponInventory.RightWeapon))
        {
            RemoveWeaponFromRightHand();
            if (WeaponInventory.RightWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(WeaponInventory.RightWeapon);
                AssignWeaponToRightHand(newWeapon);
            }

        }
        if (!HasWeaponIndex(Weapon_LeftHand, WeaponInventory.LeftWeapon))
        {
            RemoveWeaponFromLeftHand();
            if (WeaponInventory.LeftWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(WeaponInventory.LeftWeapon);
                AssignWeaponToLeftHand(newWeapon);
            }
        }
    }
}
