using Fusion;
using System;
using UnityEngine;

public class ArmsEvents : Arms
{
    public Action<Weapon_Arms> OnRightWeaponEquiped;
    public Action<Weapon_Arms> OnRightWeaponRemoved;
    public Action<int> OnRightWeaponStoringStarted;
    public Action<int> OnRightWeaponReloadStarted;
    public Action<int> OnRightWeaponMeleeStarted;
    public Action<Weapon_Arms> OnRightWeaponShot;

    public Action<Weapon_Arms> OnLeftWeaponEquiped;
    public Action<Weapon_Arms> OnLeftWeaponRemoved;
    public Action<int> OnLeftWeaponStoringStarted;
    public Action<int> OnLeftWeaponReloadStarted;
    public Action<int> OnLeftWeaponMeleeStarted;
    public Action<Weapon_Arms> OnLeftWeaponShot;

    public Action<bool> OnZoomedChanged;
    public Action<int> OnAbilityStarted;
    public Action<int> OnAbilityUsed;

    

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
        if (HasStateAuthority)
        {
            RPC_InitiateRightWeaponSwitch();
        }
    }

    // rpc
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_InitiateRightWeaponSwitch()
    {
        OnRightWeaponStoringStarted?.Invoke(Weapon_RightHand.Data.WeaponTypeIndex);
        Weapon_RightHand.OnSwitchOutStart?.Invoke(RemainingStoreTime_RightWeapon);
    }

    protected override void InitiateLeftWeaponSwitch()
    {
        base.InitiateLeftWeaponSwitch();
        if (HasStateAuthority)
        {
            RPC_InitiateLeftWeaponSwitch();
        }
    }

    // rpc
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_InitiateLeftWeaponSwitch()
    {
        OnLeftWeaponStoringStarted?.Invoke(Weapon_LeftHand.Data.WeaponTypeIndex);
        Weapon_LeftHand.OnSwitchOutStart?.Invoke(RemainingStoreTime_LeftWeapon);
    }



    protected override void InitiateReloadRightWeapon()
    {
        base.InitiateReloadRightWeapon();
        

        if (HasStateAuthority)
        {
            RPC_ReloadRightWeapon();
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_ReloadRightWeapon()
    {
        OnRightWeaponReloadStarted?.Invoke(Weapon_RightHand.Data.WeaponTypeIndex);
        Weapon_RightHand.OnReloadStart?.Invoke(RemainingReloadTime_RightWeapon);
    }

    protected override void InitiateReloadLeftWeapon()
    {
        base.InitiateReloadLeftWeapon();
        if (HasStateAuthority)
        {
            RPC_ReloadLeftWeapon();
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_ReloadLeftWeapon()
    {
        OnLeftWeaponReloadStarted?.Invoke(Weapon_LeftHand.Data.WeaponTypeIndex);
        Weapon_LeftHand.OnReloadStart?.Invoke(RemainingReloadTime_LeftWeapon);
    }

    protected override void InitiateMeleeWithRightWeapon()
    {
        base.InitiateMeleeWithRightWeapon();
        if (HasStateAuthority)
        {
            RPC_InitiateMeleeWithRightWeapon();

        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_InitiateMeleeWithRightWeapon()
    {
        OnRightWeaponMeleeStarted?.Invoke(Weapon_RightHand.Data.WeaponTypeIndex);
        Weapon_RightHand.OnMeleeStart?.Invoke(RemainingMeleeTime_RightWeapon);
    }

    protected override void InitiateMeleeWithLeftWeapon()
    {
        base.InitiateMeleeWithLeftWeapon();
        if (HasStateAuthority)
        {
            RPC_InitiateMeleeWithLeftWeapon();

        }
    }


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_InitiateMeleeWithLeftWeapon()
    {
        OnLeftWeaponMeleeStarted?.Invoke(Weapon_LeftHand.Data.WeaponTypeIndex);
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

    protected override void InitiateAbilityUse()
    {
        base.InitiateAbilityUse();
        var ability = ItemIndexList.Instance.GetAbilityViaIndex(abilityInventory.AbilityIndex);
        OnAbilityStarted?.Invoke(ability.AbilityTypeIndex);
    }

    protected override void UseAbility()
    {
        base.UseAbility();
        var ability = ItemIndexList.Instance.GetAbilityViaIndex(abilityInventory.AbilityIndex);
        OnAbilityUsed?.Invoke(ability.AbilityTypeIndex);
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
