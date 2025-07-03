using FMODUnity;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;
using static PlayerArms;
using static UnityEngine.Rendering.DebugUI;

public class Weapon_Arms
{
    public Action<float> OnReloadStart;
    public Action<float> OnSwitchOutStart;
    public Action<float> OnSwitchInStart;
    public Action<float> OnMeleeStart;
    public Action<float> OnRollStart;
    public Action OnShot;

    public Action<GameObject> OnProjectileShot;
    public Action<Vector3> OnHitscanShot;
    public Action<GameObject> OnGranadeShot;
    public Action OnWeaponDroped;


    [SerializeField] Weapon_Data weaponData;
    [SerializeField] int magazine;

    public Action<int> OnMagazineChange;

    BulletSpawner bulletSpawner;
    float shootCooldown = 0;
    float shotsLeftInBurst = 0;
    float shootCooldownInBurst = 0;

    bool isBeingDualWielded = false;

    int extraBulletsInMagazine = 0;

    bool reloadOnPickup = false;

    float fireRateMultiplier = 1f;


    float fireRateMultiplierStat = 1;
    float reloadTimeMultiplierStat = 1;
    float switchTimeMultiplierStat = 1;


    public void SetStatSheet(PlayerBodyStatSheet statSheet)
    {
        if (!statSheet.useStatSheet) return;

        fireRateMultiplierStat = statSheet.playerStatsSheetInstance.fireRateMultiplier;
        reloadTimeMultiplierStat = statSheet.playerStatsSheetInstance.reloadSpeedMultiplier;
        switchTimeMultiplierStat = statSheet.playerStatsSheetInstance.weaponSwitchSpeedMultiplier;

    }
    


    public void SetFireRateMultiplier(float fireRateMultiplier)
    {
        this.fireRateMultiplier = fireRateMultiplier;
    }

    public bool ReloadOnPickup
    {
        get => reloadOnPickup;
        set => reloadOnPickup = value;
    }


    public void SetExtraBulletsInMagazine(int extraBulletsInMagazine)
    {
        this.extraBulletsInMagazine = extraBulletsInMagazine;
    }

    public int GetExtraBulletsInMagazine()
    {
        return extraBulletsInMagazine;
    }

    public void SetIsBeingDualWielded(bool isBeingDualWielded)
    {
        this.isBeingDualWielded = isBeingDualWielded;
    }

    public bool IsBeingDualWielded => isBeingDualWielded;

    public float DamageMultiplierWhenDualWielded => weaponData.DualWieldDamageMultiplier;

    public void DropWeapon()
    {
        OnWeaponDroped?.Invoke();
    }



    public Weapon_Arms(Weapon_Data weaponData)
    {
        this.weaponData = weaponData;
    }

    public Weapon_Arms(Weapon_Data weaponData, int magazine)
    {
        this.weaponData = weaponData;
        SetAmmo(magazine);
    }

    public int Magazine
    {
        get => magazine;
        set
        {
            if (magazine != value) 
            {
                magazine = value;
                OnMagazineChange?.Invoke(magazine); 
            }
        }
    }

    public int MagazineSize => weaponData.MagazineSize + extraBulletsInMagazine;


    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    public void UpdateWeapon()
    {
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }
        else if (shootCooldown <0)
        {
            shootCooldown = 0;
        }
    }

    public bool UpdateBurstShot()
    {
        if (shotsLeftInBurst > 0)
        {
            shootCooldownInBurst -= Time.deltaTime;
            bool shot = false;
            while (shootCooldownInBurst <= 0)
            {
                shotsLeftInBurst--;
                shootCooldownInBurst += weaponData.BurstFireRate;
                shootCooldown = 0;
                Shoot();
                shot = true;
            }
            return shot;
        }
        return false;
    }

    public bool IsInBurst()
    {
        return shotsLeftInBurst > 0 && magazine > 0;
    }

    public bool TryShoot()
    {
        if (CanShoot())
        {
            Shoot();
            return true;
        }
        return false;

    }

    public bool TryBurstShoot()
    {
        if (CanShoot())
        {
            shotsLeftInBurst = weaponData.BulletsInBurst;
            Shoot();
            shootCooldownInBurst = weaponData.BurstFireRate / fireRateMultiplier / fireRateMultiplierStat;
            shotsLeftInBurst--;
            return true;
        }
        return false;
    }

    public void ResetShootCooldown()
    {
        shootCooldown = weaponData.GetFireRate(isBeingDualWielded) / fireRateMultiplier / fireRateMultiplierStat;
    }

    private void Shoot()
    {
        while(shootCooldown <= 0 && magazine > 0)
        {
            OnShot?.Invoke();

            shootCooldown += weaponData.GetFireRate(isBeingDualWielded) / fireRateMultiplier / fireRateMultiplierStat; 
            Magazine--;

            if (weaponData.WeaponBullet is Weapon_Bullet_Hitscan)
            {
                var hitscan = bulletSpawner.ShootHitScan(this);
                foreach (var hit in hitscan)
                {
                    OnHitscanShot?.Invoke(hit);
                }
            }
            else if (weaponData.WeaponBullet is Weapon_Bullet_Projectile)
            {
                var projectiles = bulletSpawner.ShootProjectile(this);
                foreach (var projectile in projectiles)
                {
                    OnProjectileShot?.Invoke(projectile);
                }

            }
            else if (weaponData.WeaponBullet is Weapon_Bullet_Granade)
            {
                var granades = bulletSpawner.ShootGranade(this);
                foreach (var granade in granades)
                {
                    OnGranadeShot?.Invoke(granade);
                }
            }
            else
            {
                Debug.LogError("Unknown bullet type");
            }
        }

        
    }
   

    public bool IsInShootCooldown()
    {
        return shootCooldown > 0;
    }

    public bool IsShootCooldownLessThanHalf()
    {
        return shootCooldown < weaponData.GetFireRate(isBeingDualWielded) / fireRateMultiplier / fireRateMultiplierStat / 2;
	}

	public float ShootCooldown => shootCooldown;

	public bool CanShoot()
    {
        return shootCooldown <= 0 && Magazine > 0;
    }

    public bool CanReload()
    {
        return Magazine < MagazineSize;
    }

    public void ReloadStart(float reloadTime)
    {
        OnReloadStart?.Invoke(reloadTime);
    }

    public void SwitchOutStart(float switchOutTime)
    {
        OnSwitchOutStart?.Invoke(switchOutTime);
    }

    public void SwitchInStart(float switchInTime)
    {
        OnSwitchInStart?.Invoke(switchInTime);
    }

    public void MeleeStart(float meleeTime)
    {
        OnMeleeStart?.Invoke(meleeTime);
    }

    public void RollStart(float rollTime)
    {
        OnRollStart?.Invoke(rollTime);
    }

    public void ReloadFinished(int ammoAdded)
    {
        int missingAmmo = MagazineSize - Magazine;
        Magazine += ammoAdded;

    }

    public float ReloadTime => weaponData.GetReloadTime(isBeingDualWielded) / reloadTimeMultiplierStat;

    public ShootType ShootType => weaponData.ShootType;

    public float SwitchInTime => weaponData.SwitchInTime / switchTimeMultiplierStat;

    public float SwitchOutTime => weaponData.SwitchOutTime / switchTimeMultiplierStat;

    public Weapon_PickUp PickUpVersion => weaponData.WeaponPickUp;

    public GameObject WeaponModel => weaponData.Weapon3rdPersonModel;

    public void SetAmmo(int magazine)
    {
        Magazine = Mathf.Min(MagazineSize, magazine);
        //Reserve = Mathf.Min(weaponData.MaxAmmoInReserve, reserve);
    }

    //public void GainMagazins(int magazins)
    //{
    //    var bulletsAmount = magazins * weaponData.MagazineSize;

    //    if (bulletsAmount >= Magazine)
    //    {
    //        bulletsAmount -= Magazine;
    //        Magazine = weaponData.MagazineSize;
    //    }
    //    else
    //    {
    //        Magazine += bulletsAmount;
    //        return;
    //    }
    //    Reserve += bulletsAmount;
    //    OnMagazineChange?.Invoke(Magazine, Reserve);
    //}

    public void FillMagazine()
    {
        Magazine = weaponData.MagazineSize;
    }

    

    //public void FillReserve()
    //{
    //    Reserve = weaponData.MaxAmmoInReserve;
    //}

    //public void AddAmmo(int amount)
    //{
    //    Reserve = Mathf.Min(Reserve + amount, weaponData.MaxAmmoInReserve);
    //}

    public Weapon_Bullet Bullet => weaponData.WeaponBullet;

    public float Inaccuracy => weaponData.GetInaccuracy(isBeingDualWielded);

    public Weapon_Visual WeaponFPSModel => weaponData.WeaponFPSModel;

    public PlayerMeleeAttack MeleeAttack => weaponData.MeleeData;

    public bool CanZoom => weaponData.CanZoom;

    public float ZoomFOV => weaponData.ZoomFOV;

    public int BulletsPerShot => weaponData.BulletsPerShoot;

    public AutoAim AutoAim => weaponData.AutoAim;


    public EventReference ShootSound => weaponData.ShootSound;

    public TimedSoundList SwitchInSound => weaponData.SwitchInSound;

    public TimedSoundList ReloadSounds => weaponData.ReloadSounds;

    public bool IsSameWeapon(Weapon_Data otherWeapon)
    {
        return weaponData == otherWeapon;
    }

    public float MoveSpeedMultiplier => weaponData.MoveSpeedMultiplier;

    public WeaponType WeaponType => weaponData.WeaponType;

    public Weapon_Data Data => weaponData;

    public float DamageReduction => weaponData.DamageReduction;

    public bool CanNotBeInInventory => weaponData.CanNotBePutInInventory;

    public bool ShowAmmoUI => weaponData.ShowAmmoUI;

    public Sprite GunSpriteUI => weaponData.GunSpriteUI;

    public Sprite BulletSpriteUI => weaponData.BulletSpriteUI;

    public int BulletsPerRowUI => weaponData.BulletsPerRowUI;

    public Vector2 BulletSizeUI => weaponData.BulletSizeUI;

    public Sprite CrosshairUI => weaponData.CrosshairsUI;

    public Vector2 CrosshairSizeUI => weaponData.CrosshairsSizeUI;

    public bool HasKnockback => weaponData.HasKnockback;

    public GunKnockback GunKnockback => weaponData.GunKnockback;

    /*public void TransferAmmo(Weapon_PickUp weaponAmmoToTransfer)
    {
        var missingAmmo = weaponData.ReserveSize - Reserve;
        // first use up ammo from other reserve
        if (missingAmmo > 0)
        {
            var ammoInPickUpReserve = weaponAmmoToTransfer.AmmoInReserve;
            if (ammoInPickUpReserve > missingAmmo)
            {
                Reserve += missingAmmo;
                ammoInPickUpReserve -= missingAmmo;
                missingAmmo = 0;
                weaponAmmoToTransfer.SetAmmoInReserve(ammoInPickUpReserve);
            }
            else
            {
                Reserve += ammoInPickUpReserve;
                missingAmmo -= ammoInPickUpReserve;
                ammoInPickUpReserve = 0;
                weaponAmmoToTransfer.SetAmmoInReserve(ammoInPickUpReserve);

                var ammoInPickUpMagazine = weaponAmmoToTransfer.AmmoInMagazine;
                if (missingAmmo > 0)
                {
                    if (ammoInPickUpMagazine > missingAmmo)
                    {
                        Reserve += missingAmmo;
                        ammoInPickUpMagazine -= missingAmmo;
                        missingAmmo = 0;
                        weaponAmmoToTransfer.SetAmmoInMagazin(ammoInPickUpMagazine);
                    }
                    else
                    {
                        Reserve += ammoInPickUpMagazine;
                        missingAmmo -= ammoInPickUpMagazine;
                        ammoInPickUpMagazine = 0;
                        weaponAmmoToTransfer.SetAmmoInMagazin(ammoInPickUpMagazine);
                        weaponAmmoToTransfer.DestroyObject();
                    }
                }
            }
        }

        
    }
    */
}