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
    [SerializeField] PlayerMeleeAttack meleeData;
    [SerializeField] AutoAim autoAim;

    [Header("Burst Values")]
    [SerializeField] int burstAmount;
    [SerializeField] float burstDelay;

    [Header("Sound")]
    [SerializeField] EventReference shootSound;
    [SerializeField] TimedSoundList switchInSound;
    [SerializeField] TimedSoundList reloadSounds;



    public ShootType ShootType => shootType;
    public float FireRate => fireRate;
    public int MagazineSize => magazineSize;
    public int MaxAmmoInReserve => maxAmmoInReserve;

    public float ReloadTime => reloadTime;

    public Weapon_Bullet WeaponBullet => weaponBullet;

    public float Inaccuracy => inaccuracy;

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

}



public enum ShootType
{
    Single,
    Auto,
    Burst,
    Melee
}

