using Fusion;
using System;
using UnityEngine;

public class Arms : NetworkBehaviour
{
    [SerializeField] public WeaponInventory WeaponInventory { get; private set; }

    public Weapon_Arms Weapon_LeftHand { get; private set; }
    public Weapon_Arms Weapon_RightHand { get; private set; }

    
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
            WeaponInventory.Switch_RightWithBackWeapon();
        }
        if (ReloadWeapon_RightWeapon.Expired(Runner))
        {
            ReloadWeapon_RightWeapon = TickTimer.None;
            ReloadRightWeapon();
        }
        if (StoreTimer_LeftWeapon.Expired(Runner))
        {
            StoreTimer_LeftWeapon = TickTimer.None;
            WeaponInventory.Switch_LeftWithBackWeapon();
        }
        if (ReloadWeapon_LeftWeapon.Expired(Runner))
        {
            ReloadWeapon_LeftWeapon = TickTimer.None;
            ReloadLeftWeapon();
        }



        // check for new weapon equiped
        if (!HasWeaponIndex(Weapon_RightHand, WeaponInventory.RightWeapon))
        {
            RemoveWeaponFromRightHand();
            if (WeaponInventory.RightWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(WeaponInventory.RightWeapon);
                AssignWeaponToRightHand(newWeapon);
            }
            
        }
        if (!HasWeaponIndex(Weapon_LeftHand, WeaponInventory.LeftWeapon))
        {
            RemoveWeaponFromLeftHand();
            if (WeaponInventory.LeftWeapon.weaponTypeIndex != -1)
            {
                var newWeapon = CreateWeaponArms(WeaponInventory.LeftWeapon);
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

    protected virtual void AssignWeaponToRightHand(Weapon_Arms weapon)
    {
        Weapon_RightHand = weapon;
        GetReadyTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.SwitchInTime);
    }

    protected virtual void AssignWeaponToLeftHand(Weapon_Arms weapon)
    {
        Weapon_LeftHand = weapon;
        GetReadyTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.SwitchInTime);
    }

    protected virtual void RemoveWeaponFromRightHand()
    {
        var weapon = Weapon_RightHand;
        Weapon_RightHand = null;
    }

    protected virtual void RemoveWeaponFromLeftHand()
    {
        var weapon = Weapon_LeftHand;
        Weapon_LeftHand = null;

    }

    protected virtual void InitiateRightWeaponSwitch()
    {
        ReloadWeapon_RightWeapon = TickTimer.None;
        StoreTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.SwitchOutTime);
    }

    protected virtual void InitiateLeftWeaponSwitch()
    {
        ReloadWeapon_LeftWeapon = TickTimer.None;
        StoreTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.SwitchOutTime);
    }

    protected void InitiateReloadRightWeapon()
    {
        ReloadWeapon_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.ReloadTime);
    }

    protected void InitiateReloadLeftWeapon()
    {
        ReloadWeapon_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.ReloadTime);
    }

    void ReloadRightWeapon()
    {
        int ammoInMagazine = WeaponInventory.RightWeapon.ammoInMagazine;
        int magazineSize = Weapon_RightHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        WeaponInventory.TransferReserveAmmo_RightWeapon(ammoNeeded);

    }

    void ReloadLeftWeapon()
    {
        int ammoInMagazine = WeaponInventory.LeftWeapon.ammoInMagazine;
        int magazineSize = Weapon_LeftHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        WeaponInventory.TransferReserveAmmo_LeftWeapon(ammoNeeded);
    }





}
