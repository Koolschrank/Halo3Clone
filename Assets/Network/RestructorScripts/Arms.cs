using Fusion;
using System;
using UnityEngine;

public abstract class Arms : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] protected WeaponInventory weaponInventory;
    [SerializeField] protected AbilityInventory abilityInventory;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] protected PlayerPickUpScan pickUpScan;
    [SerializeField] Transform rightWeaponDropPosition;
    [SerializeField] Transform leftWeaponDropPosition;

    [Header("Stats")]
    [SerializeField] PlayerMeleeAttack defaultMelee;
    [SerializeField] float dropForce = 5.0f;

    public WeaponInventory WeaponInventory => weaponInventory;

    public Weapon_Arms Weapon_LeftHand { get; private set; }
    public Weapon_Arms Weapon_RightHand { get; private set; }

    bool inZoom = false;
    public virtual bool InZoom
    {
        get => inZoom;
        protected set => inZoom = value;
    }

    public bool CanDualWield2HandedWeapons { get; private set; } = false;

    


    // Right Weapon
    [Networked] public TickTimer GetReadyTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer StoreTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer ReloadTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer MeleeHitTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer MeleeRecoveryTimer_RightWeapon { get; private set; }
    [Networked] public TickTimer ShootCooldownTimer_RightWeapon { get; private set; }

    public float RemainingReloadTime_RightWeapon => ReloadTimer_RightWeapon.RemainingTime(Runner) ?? 0f;
    public float RemainingStoreTime_RightWeapon => StoreTimer_RightWeapon.RemainingTime(Runner) ?? 0f;
    public float RemainingGetReadyTime_RightWeapon => GetReadyTimer_RightWeapon.RemainingTime(Runner) ?? 0f;

    public float RemainingMeleeTime_RightWeapon => MeleeRecoveryTimer_RightWeapon.RemainingTime(Runner) ?? 0f;

    


    // Left Weapon
    [Networked] public TickTimer GetReadyTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer StoreTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer ReloadTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer MeleeHitTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer MeleeRecoveryTimer_LeftWeapon { get; private set; }
    [Networked] public TickTimer ShootCooldownTimer_LeftWeapon { get; private set; }


    public float RemainingReloadTime_LeftWeapon => ReloadTimer_LeftWeapon.RemainingTime(Runner) ?? 0f;
    public float RemainingStoreTime_LeftWeapon => StoreTimer_LeftWeapon.RemainingTime(Runner) ?? 0f;
    public float RemainingGetReadyTime_LeftWeapon => GetReadyTimer_LeftWeapon.RemainingTime(Runner) ?? 0f;

    public float RemainingMeleeTime_LeftWeapon => MeleeRecoveryTimer_LeftWeapon.RemainingTime(Runner) ?? 0f;

    // ability
    [Networked] public TickTimer AbilityUseTimer { get; private set; }

    [Networked] public TickTimer AbilityEndLagTimer { get; private set; }

    public override void FixedUpdateNetwork()
    {
        // timer updates
        if (StoreTimer_RightWeapon.Expired(Runner))
        {
            StoreTimer_RightWeapon = TickTimer.None;
            WeaponInventory.Switch_RightWithBackWeapon();
        }
        if (ReloadTimer_RightWeapon.Expired(Runner))
        {
            ReloadTimer_RightWeapon = TickTimer.None;
            ReloadRightWeapon();
        }
        if (StoreTimer_LeftWeapon.Expired(Runner))
        {
            StoreTimer_LeftWeapon = TickTimer.None;
            WeaponInventory.Switch_LeftWithBackWeapon();
        }
        if (ReloadTimer_LeftWeapon.Expired(Runner))
        {
            ReloadTimer_LeftWeapon = TickTimer.None;
            ReloadLeftWeapon();
        }
        if (GetReadyTimer_RightWeapon.Expired(Runner))
        {
            GetReadyTimer_RightWeapon = TickTimer.None;
        }
        if (GetReadyTimer_LeftWeapon.Expired(Runner))
        {
            GetReadyTimer_LeftWeapon = TickTimer.None;
        }
        if (ShootCooldownTimer_RightWeapon.Expired(Runner))
        {
            ShootCooldownTimer_RightWeapon = TickTimer.None;
        }
        if (ShootCooldownTimer_LeftWeapon.Expired(Runner))
        {
            ShootCooldownTimer_LeftWeapon = TickTimer.None;
        }
        if (AbilityUseTimer.Expired(Runner))
        {
            AbilityUseTimer = TickTimer.None;
            UseAbility();
        }
        if (MeleeHitTimer_RightWeapon.Expired(Runner))
        {
            MeleeHitTimer_RightWeapon = TickTimer.None;
            MeleeWithRightWeapon();
        }
        if (MeleeHitTimer_LeftWeapon.Expired(Runner))
        {
            MeleeHitTimer_LeftWeapon = TickTimer.None;
            MeleeWithLeftWeapon();
        }
        if (MeleeRecoveryTimer_RightWeapon.Expired(Runner))
        {
            MeleeRecoveryTimer_RightWeapon= TickTimer.None;
        }
        if (MeleeRecoveryTimer_LeftWeapon.Expired(Runner))
        {
            MeleeRecoveryTimer_LeftWeapon = TickTimer.None;
        }
        if (AbilityEndLagTimer.Expired(Runner))
        {
            AbilityEndLagTimer = TickTimer.None;
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
        if (weaponStruct.index == weapon.Index)
            return true;
        return false;
    }

    Weapon_Arms CreateWeaponArms(WeaponNetworkStruct weaponStruct)
    {
        Weapon_Arms weapon = new Weapon_Arms(ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex), weaponStruct.index);
        return weapon;
    }

    protected virtual void AssignWeaponToRightHand(Weapon_Arms weapon)
    {
        ReloadTimer_RightWeapon = TickTimer.None;
        ShootCooldownTimer_RightWeapon = TickTimer.None;
        Weapon_RightHand = weapon;
        GetReadyTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.SwitchInTime);
    }

    protected virtual void AssignWeaponToLeftHand(Weapon_Arms weapon)
    {
        ReloadTimer_LeftWeapon = TickTimer.None;
        ShootCooldownTimer_LeftWeapon = TickTimer.None;
        Weapon_LeftHand = weapon;
        GetReadyTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.SwitchInTime);
    }

    protected virtual void RemoveWeaponFromRightHand()
    {
        Weapon_RightHand = null;
    }

    protected virtual void RemoveWeaponFromLeftHand()
    {
        Weapon_LeftHand = null;

    }

    protected virtual void InitiateRightWeaponSwitch()
    {
        ReloadTimer_RightWeapon = TickTimer.None;
        StoreTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.SwitchOutTime);
    }

    protected virtual void InitiateLeftWeaponSwitch()
    {
        ReloadTimer_LeftWeapon = TickTimer.None;
        StoreTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.SwitchOutTime);
    }

    protected virtual void InitiateReloadRightWeapon()
    {
        ReloadTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.ReloadTime);
    }

    protected virtual void InitiateReloadLeftWeapon()
    {
        ReloadTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.ReloadTime);
    }

    protected virtual void ReloadRightWeapon()
    {
        int ammoInMagazine = WeaponInventory.RightWeapon.ammoInMagazine;
        int magazineSize = Weapon_RightHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        WeaponInventory.TransferReserveAmmo_RightWeapon(ammoNeeded);

    }

    protected virtual void ReloadLeftWeapon()
    {
        if (Weapon_LeftHand == null)
            return;

        int ammoInMagazine = WeaponInventory.LeftWeapon.ammoInMagazine;
        int magazineSize = Weapon_LeftHand.MagazineSize;
        int ammoNeeded = magazineSize - ammoInMagazine;

        WeaponInventory.TransferReserveAmmo_LeftWeapon(ammoNeeded);
    }


    protected virtual void InitiateAbilityUse()
    {
        ReloadTimer_RightWeapon = TickTimer.None;
        var ability = ItemIndexList.Instance.GetAbilityViaIndex(abilityInventory.AbilityIndex);

        if (ability is Ability_Data_Granade)
        {
            var stats = (Ability_Data_Granade)ability;
            var granadeStats = stats.Granade;
            float timeMultiplier = 1;

            granadeThrower.ThrowGranadeStart(granadeStats, timeMultiplier);
            AbilityUseTimer = TickTimer.CreateFromSeconds(Runner, granadeStats.ThrowDelay);
            AbilityEndLagTimer = TickTimer.CreateFromSeconds(Runner, granadeStats.ThrowTime);
        }
    }

    protected virtual void UseAbility()
    {
        abilityInventory.UseAbility();
        var ability = ItemIndexList.Instance.GetAbilityViaIndex(abilityInventory.AbilityIndex);
        if (ability is Ability_Data_Granade)
        {
            var stats = (Ability_Data_Granade)ability;
            var granadeStats = stats.Granade;
            granadeThrower.ThrowGranade(granadeStats);
        }
    }

    protected virtual void InitiateMeleeWithRightWeapon()
    {
        var attackStats = Weapon_RightHand.MeleeAttack;
        if (attackStats == null)
        {
            attackStats = defaultMelee;
        }
        float multiplier = 1f;
        ReloadTimer_RightWeapon = TickTimer.None;

        MeleeHitTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, attackStats.Delay * multiplier);
        MeleeRecoveryTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, attackStats.MeleeTime * multiplier);
        meleeAttacker.AttackStart(attackStats);
    }

    void MeleeWithRightWeapon()
    {
        var attackStats = Weapon_RightHand.MeleeAttack;
        if (attackStats == null)
        {
            attackStats = defaultMelee;
        }
        meleeAttacker.Attack(attackStats);
    }

    protected virtual void InitiateMeleeWithLeftWeapon()
    {
        var attackStats = Weapon_LeftHand.MeleeAttack;
        if (attackStats == null)
        {
            attackStats = defaultMelee;
        }
        MeleeHitTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, attackStats.Delay);
        MeleeRecoveryTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, attackStats.MeleeTime);
        meleeAttacker.AttackStart(attackStats);
    }

    void MeleeWithLeftWeapon()
    {
        var attackStats = Weapon_LeftHand.MeleeAttack;
        if (attackStats == null)
        {
            attackStats = defaultMelee;
        }
        meleeAttacker.Attack(attackStats);
    }

    protected virtual void InitiateShootRightWeapon()
    {
        ShootCooldownTimer_RightWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_RightHand.Data.FireRate);
        ShootWithWeapon(Weapon_RightHand);
        weaponInventory.ReduceRightWeaponMagazin(1);
    }

    protected virtual void InitiateShootLeftWeapon()
    {
        ShootCooldownTimer_LeftWeapon = TickTimer.CreateFromSeconds(Runner, Weapon_LeftHand.Data.FireRate);
        ShootWithWeapon(Weapon_LeftHand);
        weaponInventory.ReduceLeftWeaponMagazin(1);
    }

    void ShootWithWeapon(Weapon_Arms weapon)
    {
        var projectileData = weapon.Data.WeaponBullet;

        if (projectileData is Weapon_Bullet_Hitscan)
        {
            var hitscan = bulletSpawner.ShootHitScan(weapon);
            HitScanHit(weapon, hitscan);
        }
        else if (projectileData is Weapon_Bullet_Projectile)
        {
            var projectiles = bulletSpawner.ShootProjectile(weapon);
            ProjectilesShot(weapon, projectiles);
        }
        else if (projectileData is Weapon_Bullet_Granade)
        {
            var granades = bulletSpawner.ShootGranade(weapon);
            GranadesShot(weapon, granades);
        }
    }

    public void InitiateWeaponDropRight(WeaponNetworkStruct weaponStruct)
    {

        var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex);

        var pickUp = Runner.Spawn(weaponData.WeaponPickUp, rightWeaponDropPosition.position, rightWeaponDropPosition.rotation).GetComponent<Weapon_PickUp>();
        pickUp.Index = weaponStruct.index;
        pickUp.SetAmmoInMagazin(weaponStruct.ammoInMagazine);
        pickUp.SetAmmoInReserve(weaponStruct.ammoInReserve);
        pickUp.AddImpulse(rightWeaponDropPosition.forward, dropForce);
    }

    public void InitiateWeaponDropLeft(WeaponNetworkStruct weaponStruct)
    {
        var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex);

        var pickUp = Runner.Spawn(weaponData.WeaponPickUp, leftWeaponDropPosition.position, leftWeaponDropPosition.rotation).GetComponent<Weapon_PickUp>();
        pickUp.Index = weaponStruct.index;
        pickUp.SetAmmoInMagazin(weaponStruct.ammoInMagazine);
        pickUp.SetAmmoInReserve(weaponStruct.ammoInReserve);
        pickUp.AddImpulse(leftWeaponDropPosition.forward, dropForce);
    }

    protected abstract void HitScanHit(Weapon_Arms weapon,Vector3[] hits);

    protected abstract void ProjectilesShot(Weapon_Arms weapon, GameObject[] projectiles);

    protected abstract void GranadesShot(Weapon_Arms weapon, GameObject[] granades);





}
