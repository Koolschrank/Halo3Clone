using UnityEngine;
using static Arm;

public class LeftArm : Arm
{
    [SerializeField] bool noInvectoryInteraction = false;

    public bool NoInvectoryInteraction => noInvectoryInteraction;


    public override void TryPickUpWeapon()
    {
        
        if (armState == ArmState.SwitchingOut) return;

        if (pickUpScan.CanPickUpWeapon())
        {
            IfZoomedInExitZoom();
            var newWeapon = pickUpScan.PickUpWeapon();

            DropWeapon();
            PickUpWeapon(newWeapon);
            OnWeaponPickedUp?.Invoke(newWeapon);

            if (inventory.Full && !noInvectoryInteraction)
            {
                inventory.DropWeapon();
            }
            
        }
    }
    public override void TrySwitchWeapon()
    {
        if (noInvectoryInteraction)
        {
            playerArms.RightArm.TrySwitchWeapon();
            DropWeapon();
            return;
        }

        Debug.Log("Switching weapon start");
        if (armState != ArmState.Ready && armState != ArmState.Reloading && armState != ArmState.Empty) return;
        switchInputBufferTimer = 0;
        IfZoomedInExitZoom();
        if (weaponInHand == null)
        {
            if (inventory.FirstWeaponInInventory.WeaponType == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons)

            SwitchWeapon();

            return;
        }

        armState = ArmState.SwitchingOut;
        switchOutTimer = weaponInHand.SwitchOutTime;
        OnWeaponUnequipStarted?.Invoke(weaponInHand, weaponInHand.SwitchOutTime);
        weaponInHand.SwitchOutStart(switchOutTimer);
    }

    public override bool CanPickUpWeapon()
    {
        var weaponToPickUp = pickUpScan.GetClosesPickUp();
        if (weaponToPickUp == null) return false;

        if (weaponToPickUp.WeaponData.WeaponType == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons)
        {
            return true;
        }
        return false;
    }

    protected override void SwitchWeapon()
    {


        
        switchInputBufferTimer = 0;
        if (weaponInHand == null)
        {
            Debug.Log("Switching weapon");
            var weaponToSwitchInto = inventory.RemoveWeapon();
            EquipWeapon(weaponToSwitchInto);
        }
        else
        {
            Debug.Log("Switching weapon out");
            OnWeaponUnequipFinished?.Invoke(weaponInHand);
            inventory.AddWeapon(weaponInHand);
            
            weaponInHand = null;
            armState = ArmState.Empty;
            
        }

        
        
    }


    public override void DropWeapon()
    {

        base.DropWeapon();
        
    }

    public void ForceWeaponToInventory()
    {
        if (weaponInHand != null)
        {
            armState = ArmState.Ready;
            TrySwitchWeapon();
        }
    }

    

    protected override void EquipWeapon(Weapon_Arms weapon)
    {
        base.EquipWeapon(weapon);
        if (weapon != null)
        {
            weapon.SetIsBeingDualWielded(true);
        }
    }
}
