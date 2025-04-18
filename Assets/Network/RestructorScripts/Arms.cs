using Fusion;
using System;
using UnityEngine;

public class Arms : NetworkBehaviour
{
    public Action<Weapon_Arms> OnRightWeaponEquiped;
    public Action<Weapon_Arms> OnRightWeaponRemoved;
    public Action<Weapon_Arms> OnRightWeaponStoringStarted;
    


    [SerializeField] WeaponInventory weaponInventory;



    Weapon_Arms weapon_LeftHand;
    Weapon_Arms weapon_RightHand;

    
    [Networked] public TickTimer GetReadyTimer_RightWeapon {  get; private set; }
    [Networked] public TickTimer StoreTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer ReloadWeapon_RightWeapon { get; private set; }

    [Networked] public TickTimer GetReadyTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer StoreTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer ReloadWeapon_LeftWeapon { get; private set; }

    


    




    public override void FixedUpdateNetwork()
    {
        if (StoreTimer_RightWeapon.Expired(Runner))
        {
            StoreTimer_RightWeapon = TickTimer.None;
            weaponInventory.Switch_RightWithBackWeapon();
        }



        if (!HasWeaponIndex(weapon_RightHand, weaponInventory.RightWeapon))
        {
            RemoveWeaponFromRightHand();
            if (weaponInventory.RightWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(weaponInventory.RightWeapon);
                AssignWeaponToRightHand(newWeapon);
            }
            
        }
        if (!HasWeaponIndex(weapon_LeftHand, weaponInventory.LeftWeapon))
        {
            RemoveWeaponFromLeftHand();
            if (weaponInventory.LeftWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(weaponInventory.LeftWeapon);
                AssignWeaponToLeftHand(newWeapon);
            }
        }
    }

    bool HasWeaponIndex(Weapon_Arms weapon, WeaponNetworkStruct weaponStruct)
    {
        if (weapon == null)
            return false;
        if (weaponStruct.weaponTypeIndex == weapon.GetWeaponNetworkStruct().weaponTypeIndex)
            return true;
        return false;
    }

    Weapon_Arms CreateWeaponArms(WeaponNetworkStruct weaponStruct)
    {
        Weapon_Arms weapon = new Weapon_Arms(ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex), weaponStruct.ammoInMagazine, weaponStruct.weaponTypeIndex);
        return weapon;
    }

    void AssignWeaponToRightHand(Weapon_Arms weapon)
    {
        weapon_RightHand = weapon;
        GetReadyTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, weapon_RightHand.SwitchInTime);
        OnRightWeaponEquiped?.Invoke(weapon);

    }

    void AssignWeaponToLeftHand(Weapon_Arms weapon)
    {

    }

    void RemoveWeaponFromRightHand()
    {
        var weapon = weapon_RightHand;
        weapon_RightHand = null;
        OnRightWeaponRemoved?.Invoke(weapon);
    }

    void RemoveWeaponFromLeftHand()
    {
        
    }

    void InitiateRightWeaponSwitch()
    {
        StoreTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, weapon_RightHand.SwitchOutTime);
        OnRightWeaponStoringStarted?.Invoke(weapon_RightHand);
    }

    public bool TryRightWeaponSwitch()
    {
        if (
            GetReadyTimer_RightWeapon.ExpiredOrNotRunning(Runner) 
            && StoreTimer_RightWeapon.ExpiredOrNotRunning(Runner))
        {
            InitiateRightWeaponSwitch();
            return true;
        }
        return false;
    }



}
