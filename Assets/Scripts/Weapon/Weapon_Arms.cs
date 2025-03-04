using FMODUnity;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;
using static PlayerArms;
using static UnityEngine.Rendering.DebugUI;


/*
public class Weapon_Arms2 : MonoBehaviour
{
    PlayerArms playerArms;
    
    [SerializeField] Weapon_Data weaponData;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] Transform muzzleFlashSpawnPoint;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] int magazine;
    [SerializeField] int reserve;

   

    public Action OnShoot;
    public Action<float> OnReloadStart;
    public Action<float> OnSwitchInStart;
    public Action<float> OnSwitchOutStart;

    public Action<Weapon_Arms> OnAmmoChange;
    public Action<Weapon_Arms> OnSwitchOutFinished;

    float cooldown;
    

    bool isTriggerPressed;
    WeaponState weaponState;

    


    public int Reserve => reserve;

    public int MagazineSize => weaponData.MagazineSize;

    public GameObject WeaponModel => weaponData.WeaponModel;



    public void Equip(PlayerArms player)
    {
        playerArms = player;
        SwitchIn();
    }

    public void PressTrigger()
    {
        isTriggerPressed = true;
        if (CanShoot())
        {
            Shoot();
        }
        else if (magazine == 0)
        {
            TryReload();
        }


    }

    public void ReleaseTrigger()
    {
        isTriggerPressed = false;

    }

    public bool TryReload()
    {
        if (CanReload())
        {
            Reload();
            return true;
        }
        return false;
    }

    public void GainAmmo(int count)
    {
        reserve = Math.Min(reserve + count, weaponData.MaxAmmoInReserve);
        OnAmmoChange?.Invoke(this);
    }

    public void FillMagazin()
    {
        magazine = weaponData.MagazineSize;
        OnAmmoChange?.Invoke(this);
    }

    public void FillReserve()
    {
        reserve = weaponData.MaxAmmoInReserve;
        OnAmmoChange?.Invoke(this);
    }

    // set magazine and reserve
    public void SetAmmo(int magazine, int reserve)
    {
        this.magazine = Mathf.Min(weaponData.MagazineSize, magazine);
        this.reserve = Mathf.Min(weaponData.MaxAmmoInReserve, reserve);
        OnAmmoChange?.Invoke(this);
    }


    private void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        if (weaponState == WeaponState.Reloading && cooldown <= 0)
        {
            ReloadFinished();
        }
        else if (weaponState == WeaponState.SwitchingIn && cooldown <= 0)
        {
            weaponState = WeaponState.Idle;
        }
        else if (weaponState == WeaponState.SwitchingOut && cooldown <= 0)
        {
            OnSwitchOutFinished?.Invoke(this);
        }
        else if (isTriggerPressed && weaponData.ShootType == ShootType.Auto && CanShoot())
        {
            Shoot();
        }
    }

    private bool CanReload()
    {
        return weaponState == WeaponState.Idle && 
            magazine < weaponData.MagazineSize && 
            reserve > 0;
    }
    private void Reload()
    {
        weaponState = WeaponState.Reloading;
        cooldown = weaponData.ReloadTime;
        Debug.Log("Reloading...");
        OnReloadStart?.Invoke(cooldown);
    }

    private void ReloadFinished()
    {
        weaponState = WeaponState.Idle;
        int missingAmmo = weaponData.MagazineSize - magazine;
        int ammoToReload = Math.Min(missingAmmo, reserve);
        magazine += ammoToReload;
        reserve -= ammoToReload;
        OnAmmoChange?.Invoke(this);

        Debug.Log("Reloaded!");

    }

    private bool CanShoot()
    {
        return cooldown <= 0 && magazine > 0 && weaponState == WeaponState.Idle ;
    }

    private void Shoot()
    {
        magazine--;
        OnAmmoChange?.Invoke(this);
        OnShoot?.Invoke();

        cooldown = weaponData.FireRate;
        Debug.Log("Bang!");
        // debug ammo and max ammo
        Debug.Log($"Magazine: {magazine}/{weaponData.MagazineSize}");


        // check if bullet is bullet_hitscan
        if (weaponData.WeaponBullet is Weapon_Bullet_Hitscan)
        {
            ShootHitScan();
        }
        else if (weaponData.WeaponBullet is Weapon_Bullet_Projectile)
        {
            ShootProjectile();
        }
        else
        {
            Debug.LogError("Unknown bullet type");
        }

        if (muzzleFlash != null)
        {
            var mf =Instantiate(muzzleFlash, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation) as GameObject;
            mf.transform.parent = muzzleFlashSpawnPoint;

            mf.transform.localScale = muzzleFlashSpawnPoint.localScale;
        }

        if (magazine == 0)
        {
            TryReload();
        }
    }

    private void ShootHitScan()
    {
        Weapon_Bullet_Hitscan bullet = weaponData.WeaponBullet as Weapon_Bullet_Hitscan;
        float range = bullet.Range;
        LayerMask hitLayer = bullet.HitLayer;
        float damage = bullet.Damage;
        Transform cameraTransform = playerArms.PlayerCamera.transform;
        RaycastHit hit;
        GameObject trail = Instantiate(bullet.Trail, bulletSpawnPoint.position, quaternion.identity);
        Vector3 shotDirection = cameraTransform.forward + UnityEngine.Random.insideUnitSphere * weaponData.Inaccuracy;

        if (Physics.Raycast(cameraTransform.position, shotDirection, out hit, range, hitLayer))
        {
            Debug.Log($"Hit {hit.collider.name}");
            Debug.DrawLine(cameraTransform.position, hit.point, Color.red, 1f);
            Debug.Log(hit.point);

            // if hit health
            if (hit.collider.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }


            // spawn trail

            if (trail.TryGetComponent<BulletTrail>(out BulletTrail trailEffect))
            {
                trailEffect.ShowTrail(hit.point - bulletSpawnPoint.position);
            }

            GameObject impact = Instantiate(bullet.Impact, hit.point, quaternion.identity);
            // get normal of hit point
            impact.transform.forward = hit.normal;
        }
        else
        {
            // spawn trail

            if (trail.TryGetComponent<BulletTrail>(out BulletTrail trailEffect))
            {
                trailEffect.ShowTrail(shotDirection * range);
            }
        }
    }

    public void ShootProjectile()
    {
        Weapon_Bullet_Projectile bullet_data = weaponData.WeaponBullet as Weapon_Bullet_Projectile;
        var cameraTransform = playerArms.PlayerCamera.transform;
        // use camera rotation and inaccuracy
        var rotation = cameraTransform.rotation * Quaternion.Euler(UnityEngine.Random.insideUnitSphere * weaponData.Inaccuracy);
        GameObject projectile = Instantiate(bullet_data.BulletPrefab, cameraTransform.position, rotation);
        GameObject bulletCopy = Instantiate(bullet_data.BulletVisual, bulletSpawnPoint.position, rotation);
        if (projectile.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.SetBulletCopy(bulletCopy.transform);
            bullet.SetOwner(playerArms.gameObject);
        }


    }

    public void SwitchIn()
    {
        weaponState = WeaponState.SwitchingIn;
        cooldown = weaponData.SwitchInTime;
        OnSwitchInStart?.Invoke(cooldown);
    }

    public void SwitchOut()
    {
        
        weaponState = WeaponState.SwitchingOut;
        cooldown = weaponData.SwitchOutTime;
        CancelCurrentAction();
        OnSwitchOutStart?.Invoke(cooldown);

    }

    public Weapon_PickUp GetPickUp(Transform transform)
    {
        // spawn weapon pickup
        Weapon_PickUp weaponPickUp = Instantiate(weaponData.WeaponPickUp, transform.position, transform.rotation);
        weaponPickUp.SetAmmo(magazine, reserve);
        return weaponPickUp;
    }

    public void PutInInventory()
    {
        weaponState = WeaponState.InInventory;
    }

    public void CancelCurrentAction()
    {
        isTriggerPressed = false;
    }

    public bool CanSwitch()
    {
        return weaponState != WeaponState.SwitchingOut;
    }

    public Transform BulletSpawnPoint => bulletSpawnPoint;

}

public enum WeaponState
{
    Idle,
    Reloading,
    SwitchingIn,
    SwitchingOut,
    InInventory
}*/


public class Weapon_Arms
{
    public Action<float> OnReloadStart;
    public Action<float> OnSwitchOutStart;
    public Action<float> OnSwitchInStart;
    public Action<float> OnMeleeStart;
    public Action OnShot;

    public Action<GameObject> OnProjectileShot;
    public Action<Vector3> OnHitscanShot;


    [SerializeField] Weapon_Data weaponData;
    [SerializeField] int magazine;
    [SerializeField] int reserve;

    BulletSpawner bulletSpawner;
    float shootCooldown = 0;
    float shotsLeftInBurst = 0;
    float shootCooldownInBurst = 0;


    public Action<int, int> OnAmmoChange;

    public Weapon_Arms(Weapon_Data weaponData)
    {
        this.weaponData = weaponData;
    }

    public Weapon_Arms(Weapon_Data weaponData, int magazine, int reserve)
    {
        this.weaponData = weaponData;
        SetAmmo(magazine, reserve);
    }

    public int Magazine
    {
        get => magazine;
        set
        {
            if (magazine != value) 
            {
                magazine = value;
                OnAmmoChange?.Invoke(magazine,reserve); 
            }
        }
    }

    public int Reserve
    {
        get => reserve;
        set
        {
            if (reserve != value)
            {
                reserve = value;
                OnAmmoChange?.Invoke(magazine, reserve);
            }
        }
    }

    public int MagazineSize => weaponData.MagazineSize;


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
    }

    public bool UpdateBurstShot()
    {
        if (shotsLeftInBurst > 0 && shootCooldownInBurst > 0)
        {
            shootCooldownInBurst -= Time.deltaTime;
            if (shootCooldownInBurst <= 0)
            {
                shotsLeftInBurst--;
                shootCooldownInBurst = weaponData.BurstFireRate;
                Shoot();
                return true;
            }
        }
        return false;
    }

    public bool IsInBurst()
    {
        return shotsLeftInBurst > 0;
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
            shootCooldownInBurst = weaponData.BurstFireRate;
            shotsLeftInBurst--;
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        OnShot?.Invoke();
        shootCooldown = weaponData.FireRate;
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
        else
        {
            Debug.LogError("Unknown bullet type");
        }
    }
   

    public bool IsInShootCooldown()
    {
        return shootCooldown > 0;
    }

    public bool CanShoot()
    {
        return shootCooldown <= 0 && Magazine > 0;
    }

    public bool CanReload()
    {
        return Magazine < weaponData.MagazineSize && reserve > 0;
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

    public void ReloadFinished()
    {
        int missingAmmo = weaponData.MagazineSize - Magazine;
        int ammoToReload = System.Math.Min(missingAmmo, reserve);
        Magazine += ammoToReload;
        Reserve -= ammoToReload;

    }

    public float ReloadTime => weaponData.ReloadTime;

    public ShootType ShootType => weaponData.ShootType;

    public float SwitchInTime => weaponData.SwitchInTime;

    public float SwitchOutTime => weaponData.SwitchOutTime;

    public Weapon_PickUp PickUpVersion => weaponData.WeaponPickUp;

    public GameObject WeaponModel => weaponData.Weapon3rdPersonModel;

    public void SetAmmo(int magazine, int reserve)
    {
        Magazine = Mathf.Min(weaponData.MagazineSize, magazine);
        Reserve = Mathf.Min(weaponData.MaxAmmoInReserve, reserve);
    }

    public void GainMagazins(int magazins)
    {
        var bulletsAmount = magazins * weaponData.MagazineSize;

        if (bulletsAmount >= Magazine)
        {
            bulletsAmount -= Magazine;
            Magazine = weaponData.MagazineSize;
        }
        else
        {
            Magazine += bulletsAmount;
            return;
        }
        Reserve += bulletsAmount;
        OnAmmoChange?.Invoke(Magazine, Reserve);
    }

    public void FillMagazine()
    {
        Magazine = weaponData.MagazineSize;
    }

    public void FillReserve()
    {
        Reserve = weaponData.MaxAmmoInReserve;
    }

    public void AddAmmo(int amount)
    {
        Reserve = Mathf.Min(Reserve + amount, weaponData.MaxAmmoInReserve);
    }

    public Weapon_Bullet Bullet => weaponData.WeaponBullet;

    public float Inaccuracy => weaponData.Inaccuracy;

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

    public void TransferAmmo(Weapon_PickUp weaponAmmoToTransfer)
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
}