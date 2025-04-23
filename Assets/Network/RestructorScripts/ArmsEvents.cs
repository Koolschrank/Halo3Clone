using Fusion;
using System;
using UnityEngine;

public class ArmsEvents : Arms
{
    public Action<Weapon_Arms> OnRightWeaponEquiped;
    public Action<Weapon_Arms> OnRightWeaponRemoved;
    public Action<Weapon_Arms> OnRightWeaponStoringStarted;
    public Action<Weapon_Arms> OnRightWeaponReloadStarted;
    public Action<Weapon_Arms> OnRightWeaponMeleeStarted;
    public Action<Weapon_Arms> OnRightWeaponShot;

    public Action<Weapon_Arms> OnLeftWeaponEquiped;
    public Action<Weapon_Arms> OnLeftWeaponRemoved;
    public Action<Weapon_Arms> OnLeftWeaponStoringStarted;
    public Action<Weapon_Arms> OnLeftWeaponReloadStarted;
    public Action<Weapon_Arms> OnLeftWeaponMeleeStarted;
    public Action<Weapon_Arms> OnLeftWeaponShot;

    public Action<bool> OnZoomedChanged;

    protected override void AssignWeaponToRightHand(Weapon_Arms weapon)
    {
        base.AssignWeaponToRightHand(weapon);
        OnRightWeaponEquiped?.Invoke(weapon);
        weapon.OnSwitchInStart?.Invoke(RemainingGetReadyTime_RightWeapon);

    }

    protected override void AssignWeaponToLeftHand(Weapon_Arms weapon)
    {
        base.AssignWeaponToLeftHand(weapon);
        OnLeftWeaponEquiped?.Invoke(weapon);
        weapon.OnSwitchInStart?.Invoke(RemainingGetReadyTime_LeftWeapon);
    }

    protected override void RemoveWeaponFromRightHand()
    {
        var weapon = Weapon_RightHand;
        base.RemoveWeaponFromRightHand();
        if (weapon != null)
            OnRightWeaponRemoved?.Invoke(weapon);
    }

    protected override void RemoveWeaponFromLeftHand()
    {
        var weapon = Weapon_LeftHand;
        base.RemoveWeaponFromLeftHand();
        if (weapon != null)
            OnLeftWeaponRemoved?.Invoke(weapon);
    }

    protected override void InitiateRightWeaponSwitch()
    {
        base.InitiateRightWeaponSwitch();
        OnRightWeaponStoringStarted?.Invoke(Weapon_RightHand);
        Weapon_RightHand.OnSwitchOutStart?.Invoke(RemainingStoreTime_RightWeapon);
    }

    protected override void InitiateLeftWeaponSwitch()
    {
        base.InitiateLeftWeaponSwitch();
        OnLeftWeaponStoringStarted?.Invoke(Weapon_LeftHand);
        Weapon_LeftHand.OnSwitchOutStart?.Invoke(RemainingStoreTime_LeftWeapon);
    }

    protected override void InitiateReloadRightWeapon()
    {
        base.InitiateReloadRightWeapon();
        OnRightWeaponReloadStarted?.Invoke(Weapon_RightHand);
        Weapon_RightHand.OnReloadStart?.Invoke(RemainingReloadTime_RightWeapon);
    }

    protected override void InitiateReloadLeftWeapon()
    {
        base.InitiateReloadLeftWeapon();
        OnLeftWeaponReloadStarted?.Invoke(Weapon_LeftHand);
        Weapon_LeftHand.OnReloadStart?.Invoke(RemainingReloadTime_LeftWeapon);
    }

    protected override void InitiateMeleeWithRightWeapon()
    {
        base.InitiateMeleeWithRightWeapon();
        OnRightWeaponMeleeStarted?.Invoke(Weapon_RightHand);
        Weapon_RightHand.OnMeleeStart?.Invoke(RemainingMeleeTime_RightWeapon);
    }

    protected override void InitiateMeleeWithLeftWeapon()
    {
        base.InitiateMeleeWithLeftWeapon();
        OnLeftWeaponMeleeStarted?.Invoke(Weapon_LeftHand);
        Weapon_LeftHand.OnMeleeStart?.Invoke(RemainingMeleeTime_LeftWeapon);
    }

    protected override void ReloadRightWeapon()
    {
        base.ReloadRightWeapon();
        
    }

    protected override void ReloadLeftWeapon()
    {
        base.ReloadLeftWeapon();
    }

    protected override void InitiateShootRightWeapon()
    {
        base.InitiateShootRightWeapon();
        OnRightWeaponShot?.Invoke(Weapon_RightHand);
        Weapon_RightHand.OnShot?.Invoke();
    }

    protected override void InitiateShootLeftWeapon()
    {
        base.InitiateShootLeftWeapon();
        OnLeftWeaponShot?.Invoke(Weapon_LeftHand);
        Weapon_LeftHand.OnShot?.Invoke();
    }

    protected override void HitScanHit(Weapon_Arms weapon,Vector3[] hits)
    {
        
        foreach (var hit in hits)
        {
            weapon.OnHitscanShot?.Invoke(hit);
        }
    }

    protected override void ProjectilesShot(Weapon_Arms weapon, GameObject[] projectiles)
    {
        foreach (var projectile in projectiles)
        {
            weapon.OnProjectileShot?.Invoke(projectile);
        }
    }

    protected override void GranadesShot(Weapon_Arms weapon, GameObject[] granades)
    {
        foreach (var granade in granades)
        {
            weapon.OnGranadeShot?.Invoke(granade);
        }

    }

    public override bool InZoom { get => base.InZoom; protected set
        {
            if ( value != InZoom)
            {
                base.InZoom = value;
                OnZoomedChanged?.Invoke(value);
            }

        } }


}
