using Fusion;
using System;
using UnityEngine;

public class Arms : NetworkBehaviour
{
    public Action<Weapon_Arms> OnRightWeaponEquiped;
    public Action<Weapon_Arms> OnRightWeaponRemoved;
    public Action<Weapon_Arms> OnRightWeaponStoringStarted;

    public Action<Weapon_Arms> OnLeftWeaponEquiped;
    public Action<Weapon_Arms> OnLeftWeaponRemoved;
    public Action<Weapon_Arms> OnLeftWeaponStoringStarted;



    [SerializeField] protected WeaponInventory weaponInventory;



    protected Weapon_Arms weapon_LeftHand;
    protected Weapon_Arms weapon_RightHand;

    
    [Networked] public TickTimer GetReadyTimer_RightWeapon {  get; private set; }
    [Networked] public TickTimer StoreTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer ReloadWeapon_RightWeapon { get; private set; }

    

    [Networked] public TickTimer GetReadyTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer StoreTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer ReloadWeapon_LeftWeapon { get; private set; }

    


    




    public override void FixedUpdateNetwork()
    {
        // timer updates
        if (StoreTimer_RightWeapon.Expired(Runner))
        {
            StoreTimer_RightWeapon = TickTimer.None;
            weaponInventory.Switch_RightWithBackWeapon();
        }
        if (ReloadWeapon_RightWeapon.Expired(Runner))
        {
            ReloadWeapon_RightWeapon = TickTimer.None;
            ReloadRightWeapon();
        }
        if (StoreTimer_LeftWeapon.Expired(Runner))
        {
            StoreTimer_LeftWeapon = TickTimer.None;
            weaponInventory.Switch_LeftWithBackWeapon();
        }
        if (ReloadWeapon_LeftWeapon.Expired(Runner))
        {
            ReloadWeapon_LeftWeapon = TickTimer.None;
            ReloadLeftWeapon();
        }



        // check for new weapon equiped
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
        weapon_LeftHand = weapon;
        GetReadyTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, weapon_LeftHand.SwitchInTime);
        OnLeftWeaponEquiped?.Invoke(weapon);

    }

    void RemoveWeaponFromRightHand()
    {
        var weapon = weapon_RightHand;
        weapon_RightHand = null;
        OnRightWeaponRemoved?.Invoke(weapon);
    }

    void RemoveWeaponFromLeftHand()
    {
        var weapon = weapon_LeftHand;
        weapon_LeftHand = null;
        OnLeftWeaponRemoved?.Invoke(weapon);

    }

    protected void InitiateRightWeaponSwitch()
    {
        ReloadWeapon_RightWeapon = TickTimer.None;

        StoreTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, weapon_RightHand.SwitchOutTime);
        OnRightWeaponStoringStarted?.Invoke(weapon_RightHand);
    }

    protected void InitiateLeftWeaponSwitch()
    {
        ReloadWeapon_LeftWeapon = TickTimer.None;

        StoreTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, weapon_LeftHand.SwitchOutTime);
        OnLeftWeaponStoringStarted?.Invoke(weapon_LeftHand);
    }

    protected void InitiateReloadRightWeapon()
    {
        ReloadWeapon_RightWeapon = TickTimer.CreateFromSeconds(Runner, weapon_RightHand.ReloadTime);
    }

    protected void InitiateReloadLeftWeapon()
    {
        ReloadWeapon_LeftWeapon = TickTimer.CreateFromSeconds(Runner, weapon_LeftHand.ReloadTime);
    }

    void ReloadRightWeapon()
    {
        int ammoInMagazine = weaponInventory.RightWeapon.ammoInMagazine;
        int magazineSize = weapon_RightHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        weaponInventory.TransferReserveAmmo_RightWeapon(ammoNeeded);

    }

    void ReloadLeftWeapon()
    {
        int ammoInMagazine = weaponInventory.LeftWeapon.ammoInMagazine;
        int magazineSize = weapon_LeftHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        weaponInventory.TransferReserveAmmo_LeftWeapon(ammoNeeded);
    }





}
