using UnityEngine;

public class ArmsExtended : ArmsEvents
{
    public bool TryRightWeaponSwitch()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && WeaponInventory.BackWeapon.weaponTypeIndex != -1
            )
        {
            InitiateRightWeaponSwitch();
            return true;
        }
        return false;
    }

    public bool TryReloadRightWeapon()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && ReloadWeapon_RightWeapon.ExpiredOrNotRunning(Runner)
            && WeaponInventory.AmmoReserve[WeaponInventory.RightWeapon.weaponTypeIndex] > 0
            && WeaponInventory.RightWeapon.ammoInMagazine < Weapon_RightHand.MagazineSize
            )
        {
            InitiateReloadRightWeapon();
            return true;
        }
        return false;
    }
}
