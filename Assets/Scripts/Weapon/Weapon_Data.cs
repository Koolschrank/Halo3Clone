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
    [Range(0, 1)]
    [SerializeField] float reloadGainAmmoTrigger = 0.9f;
    [SerializeField] float inaccuracy;
    [SerializeField] float switchOutTime;
    [SerializeField] float switchInTime;
    [SerializeField] bool canZoom;
    [SerializeField] float zoomFOV;

	[Range(0, 1)]
	[SerializeField] float zoomMoveSpeed = 1f;
	[SerializeField] float moveSpeedMultiplier = 1f;
    [SerializeField] float damageReduction = 0f;
    [SerializeField] PlayerMeleeAttack meleeData;
    [SerializeField] WeaponType weaponType;
    [SerializeField] bool canNotBeInInventory = false;



	[Header("Aim Support")]
	[SerializeField] AutoAim autoAim;
    [SerializeField] AutoAimType autoAimType = AutoAimType.followBody;


	[Header("Burst Values")]
    [SerializeField] int burstAmount;
    [SerializeField] float burstDelay;

    [Header("CameraImpact")]
    [SerializeField] bool hasKnockback;
    [SerializeField] GunKnockback gunKnockback;

    [Header("dual Wielding")]
    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float fireRateMultiplier = 1f;
    [SerializeField] float inaccuracyMultiplier = 1f;
    [SerializeField] float reloadTimeMultiplier = 1f;


    [Header("Block")]
    [SerializeField] bool hasBlock;
    [SerializeField] Block block;

	[Header("Sound")]
    [SerializeField] EventReference shootSound;
    [SerializeField] TimedSoundList switchInSound;
    [SerializeField] TimedSoundList reloadSounds;

    [Header("UI")]
    [SerializeField] bool showAmmo = true;
    [SerializeField] Sprite gunSprite;
    [SerializeField] Sprite bulletSprite;
    [SerializeField] Vector2 bulletSize = Vector2.one;
    [SerializeField] int bulletsPerRow;
    [SerializeField] Sprite crosshairs = null;
    [SerializeField] Vector2 crosshairsSize = Vector2.one;

    [Header("AI")]
    [SerializeField] Weapon_Data enemyAiWeaponData;
    [SerializeField] GunAiBehaviour gunAiBehaviour;

    [Header("Upgrade")]
    [SerializeField] Weapon_Data upgradedWeaponData = null;
    


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

    public float ZoomMoveSpeed => zoomMoveSpeed;

    public float GetReloadTime(bool isBeingDualWielded)
    {
        if (isBeingDualWielded)
        {
            return reloadTime * reloadTimeMultiplier;
        }
        return reloadTime;
    }

    public AutoAimType AutoAimType => autoAimType;

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

    public bool HasBlock => hasBlock;

    public Block DamageBlock => block;

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

    public Vector2 BulletSizeUI => bulletSize;

    public Sprite CrosshairsUI => crosshairs;

    public Vector2 CrosshairsSizeUI => crosshairsSize;

    public bool HasKnockback => hasKnockback;

    public GunKnockback GunKnockback => gunKnockback;

    public bool HasEnemyAiWeaponData => enemyAiWeaponData != null;
    public Weapon_Data EnemyAiWeaponData => enemyAiWeaponData;

    public GunAiBehaviour GunAiBehaviour => gunAiBehaviour;

    public Weapon_Data UpgradedWeaponData => upgradedWeaponData;

    public float ReloadGainAmmoTrigger => reloadGainAmmoTrigger;
}



public enum ShootType
{
    Single,
    Auto,
    Burst,
    Melee,
    Zoom
}

public enum WeaponType
{
    oneHanded,
    twoHanded,
    massive

}

[Serializable]
public class GunAiBehaviour
{
    public float IdealRange = 10;

    public float focusGainWhenOnTarget = 1f;
    public float focusLossWhenNotOnTarget = 0.5f;
    public float moveSpeedWithGun = 1f;
    public float crouchDistance = 1f;
    public float minDistanceToDogeWhenTakingDamage = 20;

}


public enum AutoAimType
{
    followBody,
    followHead,
    none
}