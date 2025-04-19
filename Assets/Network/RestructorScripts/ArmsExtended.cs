using UnityEngine;

public class ArmsExtended : Arms
{
    public bool TryRightWeaponSwitch()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && weaponInventory.BackWeapon.weaponTypeIndex != -1
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
            && weaponInventory.AmmoReserve[weaponInventory.RightWeapon.weaponTypeIndex] > 0
            && weaponInventory.RightWeapon.ammoInMagazine < weapon_RightHand.MagazineSize
            )
        {
            InitiateReloadRightWeapon();
            return true;
        }
        return false;
    }
}
