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
        if (weapon.WeaponType != WeaponType.oneHanded && !playerArms.CanDualWield2HandedWeapons )
        {
            if (playerArms.LeftArm.NoInvectoryInteraction)
            {
                playerArms.LeftArm.DropWeapon();
            }
            else
            {
                playerArms.LeftArm.ForceWeaponToInventory();
            }
        }

        base.EquipWeapon(weapon);

        if (weapon != null)
        {
            weapon.SetIsBeingDualWielded(playerArms.IsDualWielding);
        }
    }

    public override void TryMeleeAttack()
    {
        base.TryMeleeAttack();
        //playerArms.LeftArm.DropWeapon();


    }

    public override void TryThrowGranade()
    {
        base.TryThrowGranade();
        playerArms.LeftArm.DropWeapon();
    }
}
