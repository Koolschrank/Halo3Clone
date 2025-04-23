using System;
using UnityEngine;

public class ArmsExtended : ArmsEvents
{
    [SerializeField] PlayerArmsInput input;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (weaponInventory.LeftWeapon.weaponTypeIndex != -1 
            && weaponInventory.RightWeapon.weaponTypeIndex != -1)
        {

        }
        else
        {
            OneWeaponUpdate();
        }
        ZoomUpdate();
    }

    void OneWeaponUpdate()
    {
        if (input.PickUp1)
        {
            if (TryPickUpRightWeapon())
            {
                input.ResetPickUp1Input();
            }
        }

        if (input.SwitchWeapon)
        {
            if (TryRightWeaponSwitch())
            {
                input.ResetSwitchInput();
            }
        }
        if (input.Reload1)
        {
            if (TryReloadRightWeapon())
            {
                input.ResetReload1Input();
            }
        }
        if (input.Melee)
        {
            if (TryMeleeRightWeapon())
            {
                input.ResetMeleeInput();
            }
        }

        if (weaponInventory.RightWeapon.ammoInMagazine == 0 && weaponInventory.AmmoReserve[weaponInventory.RightWeapon.weaponTypeIndex] > 0)
        {
            if (TryReloadRightWeapon())
            {
                input.ResetReload1Input();
            }
        }



        RightWeaponTriggerUpdate();
    }

    

    void TwoWeaponUpdate()
    {
        
    }
    
    bool triggerHeld = false;
    void RightWeaponTriggerUpdate()
    {
        bool triggerPressed = input.Weapon1;
        bool bulletsInMagzin = weaponInventory.RightWeapon.ammoInMagazine > 0;
        bool triggerPressedDown = triggerPressed && !triggerHeld;

        if (triggerPressedDown && !bulletsInMagzin && weaponInventory.AmmoReserve[weaponInventory.RightWeapon.weaponTypeIndex] == 0)
        {
            TryRightWeaponSwitch();
            return;
        }


        
        bool inCooldown = ShootCooldownTimer_RightWeapon.IsRunning;
        
        bool inAction =
            ReloadTimer_RightWeapon.IsRunning
            || GetReadyTimer_RightWeapon.IsRunning
            || StoreTimer_RightWeapon.IsRunning
            || MeleeRecoveryTimer_RightWeapon.IsRunning
            || AbilityEndLagTimer.IsRunning;

        Weapon_Arms weapon = Weapon_RightHand;

        if (inCooldown || !bulletsInMagzin || inAction) return;

        switch (weapon.ShootType)
        {
            case ShootType.Single:
                if (triggerPressedDown)
                {
                    InitiateShootRightWeapon();
                }
                break;
            case ShootType.Auto:
                if (triggerPressed)
                {
                    InitiateShootRightWeapon();
                }
                break;
            case ShootType.Melee:
                if (triggerPressedDown)
                {
                    InitiateMeleeWithRightWeapon();
                }
                break;
            case ShootType.Burst:
                if (triggerPressedDown)
                {
                    InitiateShootRightWeapon();
                }
                break;

        }
        triggerHeld = triggerPressed;
    }

    void ZoomUpdate()
    {
        bool noZoom =
            !input.Weapon2
            || weaponInventory.LeftWeapon.weaponTypeIndex != -1
            || Weapon_RightHand == null
            || !Weapon_RightHand.CanZoom
            || ReloadTimer_RightWeapon.IsRunning
            || GetReadyTimer_RightWeapon.IsRunning
            || StoreTimer_RightWeapon.IsRunning
            || MeleeRecoveryTimer_RightWeapon.IsRunning
            || AbilityEndLagTimer.IsRunning;

        InZoom = !noZoom;
    }


    public bool TryRightWeaponSwitch()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_RightWeapon.ExpiredOrNotRunning(Runner)
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
            && ShootCooldownTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && ReloadTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && WeaponInventory.AmmoReserve[WeaponInventory.RightWeapon.weaponTypeIndex] > 0
            && WeaponInventory.RightWeapon.ammoInMagazine < Weapon_RightHand.MagazineSize
            
            )
        {
            InitiateReloadRightWeapon();
            return true;
        }
        return false;
    }

    public bool TryMeleeRightWeapon()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            InitiateMeleeWithRightWeapon();
            return true;
        }
        return false;
    }

    private bool TryPickUpRightWeapon()
    {
        if (
            pickUpScan.IndexOfClosestPickUp != -1 
            && pickUpScan.CanPickUpWeapon()
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            var pickUp = pickUpScan.PickUpWeapon();
            if (pickUp.weaponTypeIndex != -1)
            {
                weaponInventory.PickUp_RightWeapon(pickUp);
            }
        }

        return false;
    }





    public void DeleteRightWeapon()
    {
        RemoveWeaponFromRightHand();
    }

    public void DeleteLeftWeapon()
    {
        RemoveWeaponFromLeftHand();
    }

    public float DamageReduction
    {
        get
        {
            float damageReduction = 0;
            if (Weapon_RightHand != null)
            {
                damageReduction = Weapon_RightHand.DamageReduction;
            }
            if (Weapon_LeftHand != null && Weapon_LeftHand.DamageReduction > damageReduction)
            {
                damageReduction = Weapon_LeftHand.DamageReduction;
            }
            return damageReduction;
        }
    }
}
