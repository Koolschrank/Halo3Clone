using FMODUnity;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;
using static PlayerArms;
using static UnityEngine.Rendering.DebugUI;

public class Weapon_Arms
{

    public int Index { get; set; } = -1;


    public Action<float> OnReloadStart;
    public Action<float> OnSwitchOutStart;
    public Action<float> OnSwitchInStart;
    public Action<float> OnMeleeStart;
    public Action OnShot;

    public Action<GameObject> OnProjectileShot;
    public Action<Vector3> OnHitscanShot;
    public Action<GameObject> OnGranadeShot;
    public Action OnWeaponDroped;


    [SerializeField] Weapon_Data weaponData;
    [SerializeField] int magazine;

    //public Action<int> OnMagazineChange;

    BulletSpawner bulletSpawner;
    float shootCooldown = 0;
    float shotsLeftInBurst = 0;
    float shootCooldownInBurst = 0;

    bool isBeingDualWielded = false;

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



    public Weapon_Arms(Weapon_Data weaponData, int index)
    {
        this.weaponData = weaponData;
        this.Index = index;
        //SetAmmo(magazine);
    }

    public WeaponNetworkStruct GetWeaponNetworkStruct()
    {
        return new WeaponNetworkStruct()
        {
            weaponTypeIndex = weaponData.WeaponTypeIndex,
            ammoInMagazine = magazine,
            ammoInReserve = 0
        };
    }




    public int MagazineSize => weaponData.MagazineSize;


    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    public void UpdateWeapon(float delta)
    {
        if (shootCooldown > 0)
        {
            shootCooldown -= delta;
        }
        else if (shootCooldown <0)
        {
            shootCooldown = 0;
        }

        if (IsInBurst())
        {
            UpdateBurstShot(delta);
        }
    }

    public bool UpdateBurstShot(float delta)
    {
        if (shotsLeftInBurst > 0)
        {
            shootCooldownInBurst -= delta;
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



    public bool TryBurstShoot()
    {
        if (true)//(CanShoot())
        {
            shotsLeftInBurst = weaponData.BulletsInBurst;
            Shoot();
            shootCooldownInBurst = weaponData.BurstFireRate;
            shotsLeftInBurst--;
            return true;
        }
        return false;
    }

    public void ResetShootCooldown()
    {
        shootCooldown = weaponData.GetFireRate(isBeingDualWielded);
    }

    private void Shoot()
    {
        Debug.Log("shoot");
        while(shootCooldown <= 0 && magazine > 0)
        {
            OnShot?.Invoke();

            shootCooldown += weaponData.GetFireRate(isBeingDualWielded);
            //Magazine--;

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



    public float ReloadTime => weaponData.GetReloadTime(isBeingDualWielded);

    public ShootType ShootType => weaponData.ShootType;

    public float SwitchInTime => weaponData.SwitchInTime;

    public float SwitchOutTime => weaponData.SwitchOutTime;

    public Weapon_PickUp PickUpVersion => weaponData.WeaponPickUp;

    public GameObject WeaponModel => weaponData.Weapon3rdPersonModel;





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

    public int WeaponTypeIndex => weaponData.WeaponTypeIndex;

}