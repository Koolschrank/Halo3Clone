using UnityEngine;
// fmod
using FMODUnity;
using System;
using JetBrains.Annotations;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class Weapon_Data : ScriptableObject
{
    [SerializeField] Weapon_PickUp weaponPickUp;
    [SerializeField] Weapon_Bullet weaponBullet;
    [SerializeField] Weapon_Visual weaponFPSModel;
    [SerializeField] GameObject weapon3rdPersonModel;


    [Header("WeaponStats")]
    [SerializeField] string weaponName;
    [SerializeField] ShootType shootType;
    [SerializeField] float fireRate;
    [SerializeField] int bulletsPerShot = 1;
    
    [SerializeField] int magazineSize;
    [SerializeField] int maxAmmoInReserve;
    [SerializeField] float reloadTime;
    [SerializeField] float inaccuracy;
    [SerializeField] float switchOutTime;
    [SerializeField] float switchInTime;
    [SerializeField] bool canZoom;
    [SerializeField] float zoomFOV;
    [SerializeField] float moveSpeedMultiplier = 1f;
    [SerializeField] float damageReduction = 0f;
    [SerializeField] PlayerMeleeAttack meleeData;
    [SerializeField] AutoAim autoAim;
    [SerializeField] WeaponType weaponType;
    [SerializeField] bool canNotBeInInventory = false;


    [Header("Burst Values")]
    [SerializeField] int burstAmount;
    [SerializeField] float burstDelay;

    [Header("dual Wielding")]
    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float fireRateMultiplier = 1f;
    [SerializeField] float inaccuracyMultiplier = 1f;
    [SerializeField] float reloadTimeMultiplier = 1f;

    [Header("Sound")]
    [SerializeField] EventReference shootSound;
    [SerializeField] TimedSoundList switchInSound;
    [SerializeField] TimedSoundList reloadSounds;

    [Header("UI")]
    [SerializeField] bool showAmmo = true;
    [SerializeField] Sprite gunSprite;
    [SerializeField] Sprite bulletSprite;
    [SerializeField] float bulletSize;
    [SerializeField] int bulletsPerRow;



    public ShootType ShootType => shootType;
    public float FireRate => fireRate;

    public float GetFireRate(bool isBeingDualWielded)
    {
        if (isBeingDualWielded)
        {
            return fireRate * fireRateMultiplier;
        }
        return fireRate;
    }

    public int MagazineSize => magazineSize;
    public int MaxAmmoInReserve => maxAmmoInReserve;

    public float ReloadTime => reloadTime;

    public float GetReloadTime(bool isBeingDualWielded)
    {
        if (isBeingDualWielded)
        {
            return reloadTime * reloadTimeMultiplier;
        }
        return reloadTime;
    }

    public Weapon_Bullet WeaponBullet => weaponBullet;

    public float Inaccuracy => inaccuracy;

    public float GetInaccuracy(bool isBeingDualWielded)
    {
        if (isBeingDualWielded)
        {
            return inaccuracy * inaccuracyMultiplier;
        }
        return inaccuracy;
    }

    public float DualWieldDamageMultiplier => damageMultiplier;

    public Weapon_PickUp WeaponPickUp => weaponPickUp;

    public float SwitchOutTime => switchOutTime;
    public float SwitchInTime => switchInTime;

    public string WeaponName => weaponName;

    public int ReserveSize => maxAmmoInReserve;

    public bool CanZoom => canZoom;

    public float ZoomFOV => zoomFOV;

    public int BulletsPerShoot => Mathf.Max(bulletsPerShot,1);

    public Weapon_Visual WeaponFPSModel => weaponFPSModel;
    public GameObject Weapon3rdPersonModel => weapon3rdPersonModel;

    public PlayerMeleeAttack MeleeData => meleeData;

    public AutoAim AutoAim => autoAim;

    public EventReference ShootSound => shootSound;

    public TimedSoundList SwitchInSound => switchInSound;

    public TimedSoundList ReloadSounds => reloadSounds;

    public int BulletsInBurst => burstAmount;

    public float BurstFireRate => burstDelay;


    public float MoveSpeedMultiplier => moveSpeedMultiplier;

    public WeaponType WeaponType => weaponType;

    public float DamageReduction => damageReduction;

    public bool CanNotBePutInInventory => canNotBeInInventory;

    public bool ShowAmmoUI => showAmmo;

    public Sprite GunSpriteUI => gunSprite;

    public Sprite BulletSpriteUI => bulletSprite;

    public int BulletsPerRowUI => bulletsPerRow;

    public float BulletSizeUI => bulletSize;
}



public enum ShootType
{
    Single,
    Auto,
    Burst,
    Melee
}

public enum WeaponType
{
    oneHanded,
    twoHanded,
    massive

}

