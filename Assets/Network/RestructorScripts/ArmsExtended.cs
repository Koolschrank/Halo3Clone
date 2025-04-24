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
            TwoWeaponUpdate();
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
            var hasBackWeapon = weaponInventory.BackWeapon.weaponTypeIndex != -1;

            if (hasBackWeapon)
            {
                if (TryPickUpRightWeapon())
                {
                    input.ResetPickUp1Input();
                    input.ResetReload1Input();
                }
            }
            else
            {
                if (TryPickUpBackWeapon())
                {
                    input.ResetPickUp1Input();
                    input.ResetReload1Input();
                }
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
        if (input.Ability1)
        {
            if (TryUseAbility())
            {
                
            }
        }
        if (input.Reload2 || input.PickUp2)
        {
            if (TryPickUpLeftWeapon())
            {
                input.ResetReload2Input();
                input.ResetReload1Input();
                input.ResetPickUp2Input();
                return;
            }
            else if (TryEquipBackWeaponToLeftHand())
            {
                input.ResetReload2Input();
                input.ResetReload1Input();
                input.ResetPickUp2Input();
                return;
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
        if (input.PickUp1)
        {

            var weaponIndex = pickUpScan.IndexOfClosestPickUp;
            if (weaponIndex != -1)
            {
                var weaponToPickUp = ItemIndexList.Instance.GetWeaponViaIndex(weaponIndex);
                if (CanDualWield2HandedWeapons || (Weapon_RightHand.WeaponType == WeaponType.oneHanded && weaponToPickUp.WeaponType == WeaponType.oneHanded))
                {
                    if (TryPickUpLeftWeapon())
                    {
                        input.ResetPickUp1Input();
                        input.ResetReload1Input();
                        input.ResetReload2Input();
                    }
                }
                else
                {
                    var hasBackWeapon = weaponInventory.BackWeapon.weaponTypeIndex != -1;

                    if (hasBackWeapon)
                    {
                        if (TryPickUpRightWeapon())
                        {
                            ForceDropLeftWeapon();
                            input.ResetPickUp2Input();
                            input.ResetReload1Input();
                            input.ResetReload2Input();
                            return;
                        }
                    }
                    else
                    {
                        if (TryPickUpBackWeapon())
                        {
                            ForceDropLeftWeapon();
                            input.ResetPickUp2Input();
                            input.ResetReload1Input();
                            input.ResetReload2Input();
                            return;
                        }
                    }


                   
                }
            }

        }
        if (input.PickUp2)
        {
            var weaponIndex = pickUpScan.IndexOfClosestPickUp;
            if (weaponIndex != -1)
            {
                var weaponToPickUp = ItemIndexList.Instance.GetWeaponViaIndex(weaponIndex);
                if ( CanDualWield2HandedWeapons || (Weapon_RightHand.WeaponType == WeaponType.oneHanded && weaponToPickUp.WeaponType == WeaponType.oneHanded))
                {
                    if (TryPickUpRightWeapon())
                    {
                        input.ResetPickUp2Input();
                    }
                }
                else
                {

                    var hasBackWeapon = weaponInventory.BackWeapon.weaponTypeIndex != -1;

                    if (hasBackWeapon)
                    {
                        if (TryPickUpRightWeapon())
                        {
                            ForceDropLeftWeapon();
                            input.ResetPickUp2Input();
                            input.ResetReload1Input();
                            input.ResetReload2Input();
                            return;
                        }
                    }
                    else
                    {
                        if (TryPickUpBackWeapon())
                        {
                            ForceDropLeftWeapon();
                            input.ResetPickUp2Input();
                            input.ResetReload1Input();
                            input.ResetReload2Input();
                            return;
                        }
                    }
                }
            }
            
        }
        if (input.Melee)
        {
            if (TryMeleeRightWeapon())
            {
                input.ResetMeleeInput();
            }
        }
        if (input.Reload1)
        {
            if (TryReloadLeftWeapon())
            {
                input.ResetReload1Input();
            }
        }
        if (input.Reload2)
        {
            if (TryReloadRightWeapon())
            {
                input.ResetReload2Input();
            }
        }
        if (input.Ability1)
        {
            if (TryUseAbility())
            {

            }
        }
        if ( input.SwitchWeapon)
        {
            if (TryPutLeftWeaponIntoBack())
            {
                input.ResetSwitchInput();
            }
            else if (TryDropLeftWeapon())
            {
                input.ResetSwitchInput();
            }
            
        }

        if (weaponInventory.RightWeapon.ammoInMagazine == 0 && weaponInventory.AmmoReserve[weaponInventory.RightWeapon.weaponTypeIndex] > 0)
        {
            if (TryReloadRightWeapon())
            {
                input.ResetReload1Input();
            }
        }

        if (weaponInventory.LeftWeapon.ammoInMagazine == 0 && weaponInventory.AmmoReserve[weaponInventory.LeftWeapon.weaponTypeIndex] > 0)
        {
            if (TryReloadLeftWeapon())
            {
                input.ResetReload2Input();
            }
        }


        RightWeaponTriggerUpdate();
        LeftWeaponTriggerUpdate();
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

    void LeftWeaponTriggerUpdate()
    {
        bool triggerPressed = input.Weapon2;
        bool bulletsInMagzin = weaponInventory.LeftWeapon.ammoInMagazine > 0;
        bool triggerPressedDown = triggerPressed && !triggerHeld;
        if (triggerPressedDown && !bulletsInMagzin && weaponInventory.AmmoReserve[weaponInventory.LeftWeapon.weaponTypeIndex] == 0)
        {
            TryRightWeaponSwitch();
            return;
        }
        bool inCooldown = ShootCooldownTimer_LeftWeapon.IsRunning;
        bool inAction =
            ReloadTimer_LeftWeapon.IsRunning
            || GetReadyTimer_LeftWeapon.IsRunning
            || StoreTimer_LeftWeapon.IsRunning
            || MeleeRecoveryTimer_LeftWeapon.IsRunning
            || AbilityEndLagTimer.IsRunning;
        Weapon_Arms weapon = Weapon_LeftHand;
        if (inCooldown || !bulletsInMagzin || inAction) return;
        switch (weapon.ShootType)
        {
            case ShootType.Single:
                if (triggerPressedDown)
                {
                    InitiateShootLeftWeapon();
                }
                break;
            case ShootType.Auto:
                if (triggerPressed)
                {
                    InitiateShootLeftWeapon();
                }
                break;
            case ShootType.Melee:
                if (triggerPressedDown)
                {
                    InitiateMeleeWithLeftWeapon();
                }
                break;
            case ShootType.Burst:
                if (triggerPressedDown)
                {
                    InitiateShootLeftWeapon();
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

    public bool TryReloadLeftWeapon()
    {
        if (
            GetReadyTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && ShootCooldownTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && ReloadTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && WeaponInventory.AmmoReserve[WeaponInventory.LeftWeapon.weaponTypeIndex] > 0
            && WeaponInventory.LeftWeapon.ammoInMagazine < Weapon_LeftHand.MagazineSize
            )
        {
            InitiateReloadLeftWeapon();
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
                var weaponToDrop = weaponInventory.Remove_RightWeapon();
                weaponInventory.Equip_RightWeapon(pickUp);
                if (weaponToDrop.weaponTypeIndex != -1)
                    InitiateWeaponDropRight(weaponToDrop);
                return true;
            }
        }

        return false;
    }

    private bool TryPickUpBackWeapon()
    {
        if (
            pickUpScan.IndexOfClosestPickUp != -1
            && pickUpScan.CanPickUpWeapon()
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            var pickUp = pickUpScan.PickUpWeapon();
            if (pickUp.weaponTypeIndex != -1)
            {
                weaponInventory.Equip_BackWeapon(pickUp);
                InitiateRightWeaponSwitch();
                return true;
            }
        }
        return false;
    }

    private bool TryPickUpLeftWeapon()
    {
        if (
            pickUpScan.IndexOfClosestPickUp != -1
            && pickUpScan.CanPickUpWeapon()
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && Weapon_RightHand != null
            && (CanDualWield2HandedWeapons 
            || (Weapon_RightHand.WeaponType == WeaponType.oneHanded 
            && ItemIndexList.Instance.GetWeaponViaIndex(pickUpScan.IndexOfClosestPickUp).WeaponType == WeaponType.oneHanded))
            )
        {
            var pickUp = pickUpScan.PickUpWeapon();
            if (pickUp.weaponTypeIndex != -1)
            {
                var weaponToDrop = weaponInventory.Remove_LeftWeapon();
                weaponInventory.Equip_LeftWeapon(pickUp);
                if (weaponToDrop.weaponTypeIndex != -1)
                    InitiateWeaponDropLeft(weaponToDrop);
                return true;
            }

        }

        return false;
    }

    private bool TryEquipBackWeaponToLeftHand()
    {
        if (
            weaponInventory.BackWeapon.weaponTypeIndex != -1
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && (CanDualWield2HandedWeapons
            || (Weapon_RightHand.WeaponType == WeaponType.oneHanded
            && ItemIndexList.Instance.GetWeaponViaIndex(weaponInventory.BackWeapon.weaponTypeIndex).WeaponType == WeaponType.oneHanded))
            )
        {
            weaponInventory.Switch_LeftWithBackWeapon();
            return true;
        }
        return false;
    }

    private bool TryPutLeftWeaponIntoBack()
    {
        if (
            weaponInventory.LeftWeapon.weaponTypeIndex != -1
            && weaponInventory.BackWeapon.weaponTypeIndex == -1
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && AbilityEndLagTimer.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            InitiateLeftWeaponSwitch();
            return true;
        }
        return false;
    }

    public bool TryDropLeftWeapon()
    {
        if (
            weaponInventory.LeftWeapon.weaponTypeIndex != -1
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            var weaponToDrop = weaponInventory.Remove_LeftWeapon();
            if (weaponToDrop.weaponTypeIndex != -1)
                InitiateWeaponDropLeft(weaponToDrop);
            return true;
        }
        return false;

    }

    public void ForceDropLeftWeapon()
    {
        var weaponToDrop = weaponInventory.Remove_LeftWeapon();
        if (weaponToDrop.weaponTypeIndex != -1)
            InitiateWeaponDropLeft(weaponToDrop);
    }

    public bool TryUseAbility()
    {
        if (
            abilityInventory.UsesLeft > 0
            && GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && GetReadyTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && StoreTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            && MeleeRecoveryTimer_LeftWeapon.ExpiredOrNotRunning(Runner)
            && AbilityEndLagTimer.ExpiredOrNotRunning(Runner)
            && ShootCooldownTimer_RightWeapon.ExpiredOrNotRunning(Runner)
            )
        {
            InitiateAbilityUse();
            return true;
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
