using UnityEngine;

public class RightArm : Arm
{
    

    public override void TryPickUpWeapon()
    {

		if (cannotDropSwapOrPickUpWeapons) return;
		Debug.Log("interact");
        if (playerInteractableTrigger.CanInteract())
        {
            Debug.Log("interact2");
            playerInteractableTrigger.Interact();
            return;
        }



        if (playerArms.LeftArm.CurrentWeapon == null)
        {
            
			if (armState == ArmState.SwitchingOut) return;

			if (pickUpScan.CanPickUpWeapon())
			{
				IfZoomedInExitZoom();

				var newWeapon = pickUpScan.PickUpWeapon();
				if (newWeapon.Data.CanOnlyBeInLeftHand)
				{
					EquipWeapon(newWeapon);
					return;

				}
				OnWeaponPickedUp?.Invoke(newWeapon);



				if (inventory.Full)
				{
					DropWeapon();
					PickUpWeapon(newWeapon);
				}
				else
				{
					inventory.AddWeapon(newWeapon);
					TrySwitchWeapon();
				}

                if (newWeapon.WeaponType == WeaponType.oneHanded)
                {
                    playerArms.LeftArm.TryPickUpWeapon();
				}
			}
		}
        else
        {
            if (armState == ArmState.SwitchingOut) return;

            if (pickUpScan.CanPickUpWeapon())
            {
                IfZoomedInExitZoom();
                var newWeapon = pickUpScan.PickUpWeapon();
				if (newWeapon.Data.CanOnlyBeInLeftHand)
                {
                    EquipWeapon(newWeapon);
                    return;

				}

				OnWeaponPickedUp?.Invoke(newWeapon);
                DropWeapon();
                PickUpWeapon(newWeapon);
            }
        }


    }

    protected override void EquipWeapon(Weapon_Arms weapon)
    {
		if (weapon.Data.CanOnlyBeInLeftHand)
		{
            if (CurrentWeapon != null && CurrentWeapon.WeaponType != WeaponType.oneHanded)
            {
                DropWeapon();
            }

			playerArms.LeftArm.DropWeapon();
            playerArms.LeftArm.PickUpWeapon(weapon);

			if (weapon != null)
			{
				weapon.SetIsBeingDualWielded(playerArms.IsDualWielding);
			}

            return;
		}


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
        if (CurrentWeapon == null )
        {
            if (playerArms.LeftArm.CurrentWeapon != null)
            {
				playerArms.LeftArm.TryMeleeAttack();

			}
            return;
        }

        base.TryMeleeAttack();
        //playerArms.LeftArm.DropWeapon();


    }

    public override void TryThrowGranade()
    {
		if (cannotDropSwapOrPickUpWeapons) return;
		base.TryThrowGranade();
        //playerArms.LeftArm.DropWeapon();
    }


}
