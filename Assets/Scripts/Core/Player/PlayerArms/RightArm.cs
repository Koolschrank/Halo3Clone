using UnityEngine;

public class RightArm : Arm
{
    public override void TryPickUpWeapon()
    {
        if (playerArms.LeftArm.CurrentWeapon == null)
        {
            base.TryPickUpWeapon();
        }
        else
        {
            if (armState == ArmState.SwitchingOut) return;

            if (pickUpScan.CanPickUpWeapon())
            {
                IfZoomedInExitZoom();
                var newWeapon = pickUpScan.PickUpWeapon();



                DropWeapon();
                PickUpWeapon(newWeapon);
            }
        }


    }

    protected override void EquipWeapon(Weapon_Arms weapon)
    {
        if (weapon.WeaponType != WeaponType.oneHanded && !playerArms.CanDualWield2HandedWeapons)
        {
            playerArms.LeftArm.ForceWeaponToInventory();
        }

        base.EquipWeapon(weapon);

        if (weapon != null)
        {
            weapon.SetIsBeingDualWielded(playerArms.IsDualWielding);
        }
    }
}
