using System;
using UnityEngine;
using static PlayerArms;

public class Arm : MonoBehaviour
{
    [SerializeField] protected PlayerArms playerArms;





    public Action<Weapon_Arms, float> OnWeaponEquipStarted;
    public Action<Weapon_Arms, float> OnWeaponUnequipStarted;
    public Action<Weapon_Arms, float> OnWeaponReloadStarted;
    public Action<Weapon_Arms, float> OnMeleeWithWeaponStarted;
    public Action<Weapon_Arms> OnWeaponShoot;
    public Action<Weapon_Arms> OnWeaponUnequipFinished;
    public Action<Weapon_Arms, Weapon_PickUp> OnWeaponDroped;
    public Action<Weapon_Arms> OnWeaponPickedUp;
    public Action<GranadeStats, float> OnGranadeThrowStarted;
    public Action<GameObject, GranadeStats> OnGranadeThrow;
    public Action<Weapon_Arms> OnZoomIn;
    public Action<Weapon_Arms> OnZoomOut;
    public Action<int> OnReserveAmmoChanged;


    [Header("References")]
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] Controller controller;
    [SerializeField] protected PlayerInventory inventory;
    [SerializeField] protected AbilityInventory abilityInventory;
    [SerializeField] protected PlayerPickUpScan pickUpScan;
    [SerializeField] Transform dropPosition;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] PlayerMeleeAttack basicMeleeAttack;
    [SerializeField] PlayerAim playerAim;

     bool isTriggerPressed;
     bool wasTriggerPressed;
     float reloadTimer;
    protected float switchOutTimer;
     float switchInTimer;
     float granadeThrowTimer;
     float meleeAttackTimer;


    protected Weapon_Arms weaponInHand;
    protected ArmState armState = ArmState.Ready;
    bool inZoom = false;

    [Header("Settings")]
    [SerializeField]  float weaponDropForce;
    [SerializeField]  float reloadInputBuffer = 0.4f;
    float reloadInputBufferTimer;
    [SerializeField] float switchInputBuffer = 0.4f;
    protected float switchInputBufferTimer;
    [SerializeField] float granadeThrowInputBuffer = 0.4f;
    float granadeThrowInputBufferTimer;
    [SerializeField] float meleeAttackTimeMultiplierInDualWielding = 1.5f;
    [SerializeField] float granadeThrowTimeMultiplierInDualWielding = 1.5f;
    [SerializeField] bool reloadWeaponWhenDroped = false;


    int extraBulletsInMagazine = 0;

    float bulletRecoveryChance = 0;
    float reloadWeaponSpeedMultiplier = 1;
    float fireRateMultiplier = 1;

    public void AddToFireRateMultiplier(float value)
    {
        fireRateMultiplier += value;
        if (weaponInHand != null)
        {
            weaponInHand.SetFireRateMultiplier(fireRateMultiplier);
        }
        if (inventory.HasWeapon)
        {
            inventory.GetWeapon().SetFireRateMultiplier(fireRateMultiplier);
        }
    }

    public void SetReloadWeaponSpeedMultiplier(float multiplier)
    {
        reloadWeaponSpeedMultiplier = multiplier;
    }

    public void SetBulletRecoveryChance(float chance)
    {
        bulletRecoveryChance = chance;
    }


    public int ExtraBulletsInMagazine
    {
        get
        {
            return extraBulletsInMagazine;
        }
        set
        {
            extraBulletsInMagazine = value;

            if (weaponInHand != null)
            {
                weaponInHand.SetExtraBulletsInMagazine(extraBulletsInMagazine);
            }

            if (inventory.HasWeapon)
            {
                inventory.GetWeapon().SetExtraBulletsInMagazine(extraBulletsInMagazine);
            }
        }
    }

    public void AddExtraBullets(int amount)
    {
        extraBulletsInMagazine += amount;
    }

    private void Start()
    {
        granadeThrower.OnGranadeThrow += SendGranadeThrowSignal;
        characterHealth.OnDeath += DropWeaponWithNoForce;
        inventory.OnAmmoChanged += TrySendEventToUpdateReserve;

        OnWeaponShoot += (Weapon_Arms) => TryBulletRecovery();

    }

    public void TrySendEventToUpdateReserve(Weapon_Data weaponAmmoChanged, int ammo)
    {
        if (weaponInHand != null && weaponInHand.Data == weaponAmmoChanged)
        {
            OnReserveAmmoChanged?.Invoke(ammo);
        }
    }

    public int AmmoOfWeaponInReserve
    {
        get
        {
            if (weaponInHand == null) return 0;
            return inventory.GetAmmo(weaponInHand.Data);
        }
    }

    void Update()
    {
        weaponInHand?.UpdateWeapon();

        // input buffers
        if (switchInputBufferTimer > 0 ||( (isTriggerPressed&&CurrentWeapon != null && !playerArms.IsDualWielding &&CurrentWeapon.Magazine == 0 && inventory.GetAmmo(CurrentWeapon.Data) <= 0 && inventory.HasWeapon && (inventory.GetWeapon().Magazine !=0||inventory.GetAmmo(inventory.GetWeapon().Data) != 0)) ))
        {
            switchInputBufferTimer -= Time.deltaTime;
            TrySwitchWeapon();
        }
        else if (reloadInputBufferTimer > 0)
        {
            reloadInputBufferTimer -= Time.deltaTime;
            TryReload();
        }
        else if (granadeThrowInputBufferTimer > 0)
        {
            granadeThrowInputBufferTimer -= Time.deltaTime;
            TryThrowGranade();
        }

        TryToggleZoom();



        if (armState == ArmState.Reloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                ReloadFinished();
            }
        }
        if (armState == ArmState.SwitchingOut)
        {
            switchOutTimer -= Time.deltaTime;
            if (switchOutTimer <= 0)
            {
                SwitchWeapon();
            }
        }
        if (armState == ArmState.SwitchingIn)
        {
            switchInTimer -= Time.deltaTime;
            if (switchInTimer <= 0)
            {
                SwitchInFinished();
            }
        }
        if (armState == ArmState.InGranadeThrow)
        {
            granadeThrowTimer -= Time.deltaTime;
            if (granadeThrowTimer <= 0)
            {
                armState = ArmState.Ready;
            }
        }
        if (armState == ArmState.InMeleeAttack)
        {
            meleeAttackTimer -= Time.deltaTime;
            if (meleeAttackTimer <= 0)
            {
                armState = ArmState.Ready;
            }
        }

        if (armState == ArmState.Ready && weaponInHand != null)
        {
            if (weaponInHand.Magazine == 0)
            {
                TryReload();
            }




            switch (weaponInHand.ShootType)
            {
                case ShootType.Single:

                    if (!wasTriggerPressed && isTriggerPressed)
                    {
                        if (weaponInHand.CanShoot())
                        {
                            if (weaponInHand.TryShoot())
                            {
                                armState = ArmState.Shooting;
                                OnWeaponShoot?.Invoke(weaponInHand);
                                ApplyWeaponKnockback();
                            }
                        }
                        else // if try to shoot but cannot because magazine is empty reload
                        {
                            TryReload();
                        }
                    }
                    break;
                case ShootType.Burst:

                    if (/*!wasTriggerPressed &&*/ isTriggerPressed)
                    {
                        if (weaponInHand.CanShoot())
                        {
                            if (weaponInHand.TryBurstShoot())
                            {
                                armState = ArmState.InBurstShooting;
                                OnWeaponShoot?.Invoke(weaponInHand);
                                ApplyWeaponKnockback();
                            }
                        }
                        else // if try to shoot but cannot because magazine is empty reload
                        {
                            TryReload();
                        }
                    }
                    break;
                case ShootType.Auto:
                    if (isTriggerPressed)
                    {
                        if (weaponInHand.CanShoot())
                        {
                            if (weaponInHand.TryShoot())
                            {
                                armState = ArmState.Shooting;
                                OnWeaponShoot?.Invoke(weaponInHand);
                                ApplyWeaponKnockback();
                            }
                        }
                        else // if try to shoot but cannot because magazine is empty reload
                        {
                            TryReload();
                        }
                    }
                    break;
                case ShootType.Melee
                    :
                    if (!wasTriggerPressed && isTriggerPressed)
                    {
                        TryMeleeAttack();
                    }
                    break;
            }
        }
        if (armState == ArmState.InBurstShooting && weaponInHand != null)
        {
            if (weaponInHand.UpdateBurstShot())
            {
                armState = ArmState.InBurstShooting;
                OnWeaponShoot?.Invoke(weaponInHand);
            }

            if (!weaponInHand.IsInBurst())
            {
                armState = ArmState.Shooting;
                weaponInHand.ResetShootCooldown();
            }
        }




        wasTriggerPressed = isTriggerPressed;


        if ((armState == ArmState.Shooting) && weaponInHand != null && !weaponInHand.IsInShootCooldown())
        {
            armState = ArmState.Ready;
        }

    }


    public bool PressReloadButtonIfNothingToPickUp()
    {
        if (pickUpScan.CanPickUpWeapon())
        {
            return false;
        }
        PressReloadButton();
        return true;
    }

    public void TryBulletRecovery()
    {
        if (bulletRecoveryChance == 0) return;

        if (UnityEngine.Random.Range(0f, 1f) < bulletRecoveryChance)
        {
            if (weaponInHand != null)
            {
                inventory.AddAmmo(weaponInHand.Data, 1);
            }
        }
    }

    public void PressReloadButton()
    {
        reloadInputBufferTimer = reloadInputBuffer;
        switchInputBufferTimer = 0;
    }

    void TryReload()
    {
        if (armState != ArmState.Ready) return;
        reloadInputBufferTimer = 0;

        if (weaponInHand != null && weaponInHand.CanReload() && inventory.HasAmmo(weaponInHand.Data))
        {
            IfZoomedInExitZoom();
            armState = ArmState.Reloading;
            reloadTimer = weaponInHand.ReloadTime * reloadWeaponSpeedMultiplier;
            OnWeaponReloadStarted?.Invoke(weaponInHand, reloadTimer);
            weaponInHand.ReloadStart(reloadTimer);
        }
    }

    void ReloadFinished()
    {
        armState = ArmState.Ready;
        if (weaponInHand != null)
        {
            int ammoNeeded = weaponInHand.MagazineSize - weaponInHand.Magazine;
            int ammoAdded = inventory.TakeAmmo(weaponInHand.Data, ammoNeeded);
            weaponInHand.ReloadFinished(ammoAdded);
        }
            
    }


    bool zoomButtonPressed = false;
    public void PressZoomButton()
    {
        zoomButtonPressed = true;

    }

    public void ApplyWeaponKnockback()
    {
        if (weaponInHand != null && weaponInHand.HasKnockback)
        {
            playerAim.AddGunKnockback(weaponInHand.GunKnockback);
        }
    }

    public void ReleaseZoomButton()
    {

        zoomButtonPressed = false;
    }

    public void DeleteWeapon()
    {
        if (weaponInHand != null)
        {
            OnWeaponDroped?.Invoke(weaponInHand, null);
            weaponInHand = null;
            TrySwitchWeapon();

        }
    }

    // TODO: this function can be cleared up

    public void TryToggleZoom()
    {
        if (zoomButtonPressed == inZoom) return;


        if (armState != ArmState.Ready && armState != ArmState.Shooting)
        {
            OnZoomOut?.Invoke(weaponInHand);
            return;
        }



        if (weaponInHand != null && weaponInHand.CanZoom)
        {
            if (zoomButtonPressed)
            {
                inZoom = true;
                OnZoomIn?.Invoke(weaponInHand);
            }
            else
            {
                inZoom = false;
                OnZoomOut?.Invoke(weaponInHand);
            }
        }
    }



    public void IfZoomedInExitZoom()
    {
        if (inZoom)
        {
            inZoom = false;
            OnZoomOut?.Invoke(weaponInHand);
        }
    }

    public bool PressSwitchButtonIfNothingToPickUp()
    {
        if (pickUpScan.CanPickUpWeapon())
        {
            return false;
        }
        PressSwitchButton();
        return true;
    }

    public void PressSwitchButton()
    {
       
        switchInputBufferTimer = switchInputBuffer;
        reloadInputBufferTimer = 0;
    }

    public virtual void TrySwitchWeapon()
    {
        Debug.Log("Switching weapon try");

        if (armState != ArmState.Ready && armState != ArmState.Reloading) return;

        if (weaponInHand != null && weaponInHand.CanNotBeInInventory)
        {
            DropWeapon();
        }
            


        switchInputBufferTimer = 0;
        if (inventory.HasWeapon)
        {
            IfZoomedInExitZoom();
            if (weaponInHand == null)
            {
                SwitchWeapon();
                return;
            }

            armState = ArmState.SwitchingOut;
            switchOutTimer = weaponInHand.SwitchOutTime;
            OnWeaponUnequipStarted?.Invoke(weaponInHand, weaponInHand.SwitchOutTime);
            weaponInHand.SwitchOutStart(switchOutTimer);

        }
    }

    public virtual bool CanPickUpWeapon()
    {
        return pickUpScan.CanPickUpWeapon();
    }

    public virtual void TryPickUpWeapon()
    {
        if (armState == ArmState.SwitchingOut) return;

        if (pickUpScan.CanPickUpWeapon())
        {
            IfZoomedInExitZoom();
            
            var newWeapon = pickUpScan.PickUpWeapon();
            OnWeaponPickedUp?.Invoke(newWeapon);



            if (inventory.Full)
            {
                DropWeapon();
                PickUpWeapon(newWeapon);
            }
            else
            {
                inventory.AddWeapon(newWeapon);
                TrySwitchWeapon();
            }
        }
    }

    void DropWeaponWithNoForce()
    {
        if (weaponInHand == null) return;
        var pickUp = LetGoOfWeapon();
    }

    public virtual void DropWeapon()
    {

        if (weaponInHand == null) return;
        var pickUp = LetGoOfWeapon();
        if (pickUp == null) return;

        pickUp.AddImpulse(dropPosition.forward, weaponDropForce);
    }

    Weapon_PickUp LetGoOfWeapon()
    {
        if (weaponInHand == null) return null;

        IfZoomedInExitZoom();
        weaponInHand.SetExtraBulletsInMagazine(0);

        // if weapon is empty return null
        if (weaponInHand.Magazine == 0 && inventory.GetAmmo(weaponInHand.Data) == 0)
        {
            return null;
        }

        if (reloadWeaponWhenDroped)
        {
            int ammoNeeded = weaponInHand.MagazineSize - weaponInHand.Magazine;
            int ammoAdded = inventory.TakeAmmo(weaponInHand.Data, ammoNeeded);
            weaponInHand.ReloadFinished(ammoAdded);
            
        }

        var pickUpVersion = weaponInHand.PickUpVersion;
        var pickUp = Instantiate(pickUpVersion, dropPosition.position, dropPosition.rotation);


        if (playerArms.HasMultipleOfTheSameWeapon(weaponInHand.Data))
        {
            pickUp.SetAmmo(weaponInHand.Magazine, 0); 
        }
        else
        {
            pickUp.SetAmmo(weaponInHand.Magazine, inventory.TakeAllAmmo(weaponInHand.Data));
        }
        if (reloadWeaponWhenDroped)
        {
            pickUp.ReloadOnPickup = true;
        }

            pickUp.EnterDeleteTime();
        OnWeaponDroped?.Invoke(weaponInHand, pickUp);
        weaponInHand.DropWeapon();
        weaponInHand = null;
        return pickUp;
    }

    public void PickUpWeapon(Weapon_Arms weapon)
    {


        EquipWeapon(weapon);
    }

    public void PressGranadeButton()
    {
        granadeThrowInputBufferTimer = granadeThrowInputBuffer;
    }


    
    public virtual void TryThrowGranade()
    {
        if (armState != ArmState.Ready) return;
        if (abilityInventory.CanUseCurrentAbility() && abilityInventory.IsCurrentAbilityAGranade())
        {
            IfZoomedInExitZoom();
            var ability = abilityInventory.GetCurrentAbility().abilityData as AbilityData_Granade;
            if (ability == null)
            {
                Debug.LogError("Ability is not a granade");
                return;
            }


            float timeMultiplier = 1;
            if (playerArms.IsDualWielding)
            {
                timeMultiplier = granadeThrowTimeMultiplierInDualWielding;
            }
            granadeThrower.ThrowGranadeStart(ability.granadeStats, timeMultiplier);
            
            armState = ArmState.InGranadeThrow;
            granadeThrowTimer = ability.granadeStats.ThrowTime * timeMultiplier;
            OnGranadeThrowStarted?.Invoke(ability.granadeStats, granadeThrowTimer);
            
           
            abilityInventory.UseSelectedIndex();
        }
    }

    void SendGranadeThrowSignal(GameObject granade)
    {
       
        Debug.Log(granade.name);
        var ability = abilityInventory.GetLastAbility();
        Debug.Log(ability);
        var granadeAbility = (ability.abilityData as AbilityData_Granade);
        Debug.Log(granadeAbility);

        var granadestats = granadeAbility.granadeStats;
        Debug.Log(granadestats);

        OnGranadeThrow?.Invoke(granade, granadestats);
    }

    public void PressMeleeButton()
    {
        TryMeleeAttack();
    }

    public virtual void TryMeleeAttack()
    {
        if (armState != ArmState.Ready && armState != ArmState.Shooting && armState != ArmState.Reloading) return;
        IfZoomedInExitZoom();
        var meleeAttack = weaponInHand.MeleeAttack;
        if (meleeAttack == null)
        {
            meleeAttack = basicMeleeAttack;
        }
        armState = ArmState.InMeleeAttack;
        float timeMultiplier = 1;
        if (playerArms.IsDualWielding)
        {
            timeMultiplier = meleeAttackTimeMultiplierInDualWielding;
        }


        meleeAttackTimer = meleeAttack.MeleeTime * timeMultiplier;
        meleeAttacker.AttackStart(meleeAttack, timeMultiplier);
        weaponInHand.MeleeStart(meleeAttackTimer);
        OnMeleeWithWeaponStarted?.Invoke(weaponInHand, meleeAttackTimer);


    }

    protected virtual void SwitchWeapon()
    {
        var weaponToSwitchInto = inventory.RemoveWeapon();
        if (weaponInHand != null)
        {
            inventory.AddWeapon(weaponInHand);
        }
        EquipWeapon(weaponToSwitchInto);
    }

    protected virtual void EquipWeapon(Weapon_Arms weapon)
    {
        if (weapon == null)
        {
            armState = ArmState.Ready;
            return;
        }

        if (weaponInHand != null)
        {
            OnWeaponUnequipFinished?.Invoke(weaponInHand);
        }

        weaponInHand = weapon;
        weaponInHand.SetExtraBulletsInMagazine(extraBulletsInMagazine);
        weaponInHand.SetFireRateMultiplier(fireRateMultiplier);
        switchInTimer = weaponInHand.SwitchInTime;
        armState = ArmState.SwitchingIn;
        OnWeaponEquipStarted?.Invoke(weaponInHand, weaponInHand.SwitchInTime);

        SetUpWeapon(weaponInHand);

        if (weapon.ReloadOnPickup)
        {
            ReloadFinished();
            weapon.ReloadOnPickup = false;
        }

       


        weaponInHand.SwitchInStart(switchInTimer);
    }

    public void SetWeaponToIfDualWielding(bool isDualWielding)
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.SetIsBeingDualWielded(isDualWielding);
        }
    }

    public BulletSpawner GetBulletSpawner()
    {
        return bulletSpawner;
    }

    public Weapon_Arms GetWeaponInHand()
    {
        return weaponInHand;
    }

    public float GetWeaponInHandSwitchInTime()
    {
        if (weaponInHand == null) return 0;

        return weaponInHand.SwitchInTime;
    }

    void SwitchInFinished()
    {
        armState = ArmState.Ready;
    }

    void SetUpWeapon(Weapon_Arms weapon)
    {
        weapon.SetBulletSpawner(bulletSpawner);

    }

    public void UpdateWeaponTrigger(bool value)
    {
        isTriggerPressed = value;
    }

    public bool IsInZoom
    {
        get
        {
            return inZoom;
        }
    }

    public Weapon_Arms CurrentWeapon
    {
        get
        {
            return weaponInHand;
        }
    }

    public enum ArmState
    {
        Ready,
        Shooting,
        InBurstShooting,
        Reloading,
        SwitchingIn,
        SwitchingOut,
        InGranadeThrow,
        InMeleeAttack,
        Empty,
    }


}
