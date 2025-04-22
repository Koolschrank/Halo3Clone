using Fusion;
using System;
using UnityEngine;

public class ArmsEvents : Arms
{
    public Action<Weapon_Arms> OnRightWeaponEquiped;
    public Action<Weapon_Arms> OnRightWeaponRemoved;
    public Action<Weapon_Arms> OnRightWeaponStoringStarted;

    public Action<Weapon_Arms> OnLeftWeaponEquiped;
    public Action<Weapon_Arms> OnLeftWeaponRemoved;
    public Action<Weapon_Arms> OnLeftWeaponStoringStarted;

    protected override void AssignWeaponToRightHand(Weapon_Arms weapon)
    {
        base.AssignWeaponToRightHand(weapon);
        OnRightWeaponEquiped?.Invoke(weapon);

    }

    protected override void AssignWeaponToLeftHand(Weapon_Arms weapon)
    {
        base.AssignWeaponToLeftHand(weapon);
        OnLeftWeaponEquiped?.Invoke(weapon);
    }

    protected override void RemoveWeaponFromRightHand()
    {
        var weapon = Weapon_RightHand;
        base.RemoveWeaponFromRightHand();
        OnRightWeaponRemoved?.Invoke(weapon);
    }

    protected override void RemoveWeaponFromLeftHand()
    {
        var weapon = Weapon_LeftHand;
        base.RemoveWeaponFromLeftHand();
        OnLeftWeaponRemoved?.Invoke(weapon);
    }

    protected override void InitiateRightWeaponSwitch()
    {
        base.InitiateRightWeaponSwitch();
        OnRightWeaponStoringStarted?.Invoke(Weapon_RightHand);
    }

    protected override void InitiateLeftWeaponSwitch()
    {
        base.InitiateLeftWeaponSwitch();
        OnLeftWeaponStoringStarted?.Invoke(Weapon_LeftHand);
    }
}
