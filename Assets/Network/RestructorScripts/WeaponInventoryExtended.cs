using System;
using UnityEngine;

public class WeaponInventoryExtended : WeaponInventory
{
    public Action<WeaponNetworkStruct> OnBackWeaponEquipped;
    public Action<WeaponNetworkStruct> OnLeftWeaponEquipped;
    public Action<WeaponNetworkStruct> OnRightWeaponEquipped;


    public Action<WeaponNetworkStruct> OnBackWeaponRemoved;
    public Action<WeaponNetworkStruct> OnLeftWeaponRemoved;
    public Action<WeaponNetworkStruct> OnRightWeaponRemoved;

    public Action<int> OnBackWeaponMagazinChanged;
    public Action<int> OnLeftWeaponMagazinChanged;
    public Action<int> OnRightWeaponMagazinChanged;

    public Action<int> OnBackWeaponReserveAmmoChanged;
    public Action<int> OnLeftWeaponReserveAmmoChanged;
    public Action<int> OnRightWeaponReserveAmmoChanged;

    public override void Equip_BackWeapon(WeaponNetworkStruct newWeapon)
    {
        base.Equip_BackWeapon(newWeapon);
        OnBackWeaponEquipped?.Invoke(newWeapon);
    }

    public override void Equip_LeftWeapon(WeaponNetworkStruct newWeapon)
    {
        base.Equip_LeftWeapon(newWeapon);
        OnLeftWeaponEquipped?.Invoke(newWeapon);
    }

    public override void Equip_RightWeapon(WeaponNetworkStruct newWeapon)
    {
        base.Equip_RightWeapon(newWeapon);
        OnRightWeaponEquipped?.Invoke(newWeapon);
    }

    public override WeaponNetworkStruct Remove_BackWeapon()
    {
        var weaponToRemove = base.Remove_BackWeapon();

        OnBackWeaponRemoved?.Invoke(weaponToRemove);
        return weaponToRemove;
    }

    public override WeaponNetworkStruct Remove_LeftWeapon()
    {
        var weaponToRemove = base.Remove_LeftWeapon();
        OnLeftWeaponRemoved?.Invoke(weaponToRemove);
        return weaponToRemove;
    }

    public override WeaponNetworkStruct Remove_RightWeapon()
    {
        var weaponToRemove = base.Remove_RightWeapon();
        OnRightWeaponRemoved?.Invoke(weaponToRemove);
        return weaponToRemove;

    }

    public override void SetAmmo(int index, int amount)
    {
        base.SetAmmo(index, amount);
        if (index == RightWeapon.weaponTypeIndex)
        {
            OnRightWeaponReserveAmmoChanged?.Invoke(amount);
        }
        if (index == LeftWeapon.weaponTypeIndex)
        {
            OnLeftWeaponReserveAmmoChanged?.Invoke(amount);
        }
        if (index == BackWeapon.weaponTypeIndex)
        {
            OnBackWeaponReserveAmmoChanged?.Invoke(amount);
        }
    }

    public override void SetRightWeapon(WeaponNetworkStruct weapon)
    {
        base.SetRightWeapon(weapon);
        OnRightWeaponMagazinChanged?.Invoke(weapon.ammoInMagazine);
    }

    public override void SetLeftWeapon(WeaponNetworkStruct weapon)
    {
        base.SetLeftWeapon(weapon);
        OnLeftWeaponMagazinChanged?.Invoke(weapon.ammoInMagazine);
    }

    public override void SetBackWeapon(WeaponNetworkStruct weapon)
    {
        base.SetBackWeapon(weapon);
        OnBackWeaponMagazinChanged?.Invoke(weapon.ammoInMagazine);
    }

    public int GetReserveAmmoLeftWeapon()
    {
        return AmmoReserve[LeftWeapon.weaponTypeIndex];
    }

    public int GetReserveAmmoRightWeapon()
    {
        return AmmoReserve[RightWeapon.weaponTypeIndex];
    }

    public int GetReserveAmmoBackWeapon()
    {
        return AmmoReserve[BackWeapon.weaponTypeIndex];
    }
}
