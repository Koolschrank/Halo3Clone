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
    public Action<Weapon_Arms> OnWeaponDroped;
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
    [SerializeField] protected PlayerPickUpScan pickUpScan;
    [SerializeField] Transform dropPosition;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] PlayerMeleeAttack basicMeleeAttack;

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

    private void Start()
    {
        granadeThrower.OnGranadeThrow += SendGranadeThrowSignal;
        characterHealth.OnDeath += DropWeaponWithNoForce;
        inventory.OnAmmoChanged += TrySendEventToUpdateReserve;

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
        if (switchInputBufferTimer > 0)
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
                            }
                        }
                        else // if try to shoot but cannot because magazine is empty reload
                        {
                            TryReload();
                        }
                    }
                    break;
                case ShootType.Burst:

                    if (!wasTriggerPressed && isTriggerPressed)
                    {
                        if (weaponInHand.CanShoot())
                        {
                            if (weaponInHand.TryBurstShoot())
                            {
                                armState = ArmState.InBurstShooting;
                                OnWeaponShoot?.Invoke(weaponInHand);
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
            }
        }




        wasTriggerPressed = isTriggerPressed;


        if ((armState == ArmState.Shooting) && !weaponInHand.IsInShootCooldown())
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
            reloadTimer = weaponInHand.ReloadTime;
            OnWeaponReloadStarted?.Invoke(weaponInHand, weaponInHand.ReloadTime);
            weaponInHand.ReloadStart(reloadTimer);
        }
    }

    void ReloadFinished()
    {
        armState = ArmState.Ready;
        if (weaponInHand != null)
        {
            int ammoNeeded = weaponInHand.Data.MagazineSize - weaponInHand.Magazine;
            int ammoAdded = inventory.TakeAmmo(weaponInHand.Data, ammoNeeded);
            weaponInHand.ReloadFinished(ammoAdded);
        }
            
    }


    bool zoomButtonPressed = false;
    public void PressZoomButton()
    {
        zoomButtonPressed = true;

    }

    public void ReleaseZoomButton()
    {

        zoomButtonPressed = false;
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
        OnWeaponDroped?.Invoke(weaponInHand);
        // if weapon is empty return null
        if (weaponInHand.Magazine == 0 && inventory.GetAmmo(weaponInHand.Data) == 0)
        {
            return null;
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
        if (inventory.HasGranades)
        {
            IfZoomedInExitZoom();
            var granade = inventory.GranadeStats;
            float timeMultiplier = 1;
            if (playerArms.IsDualWielding)
            {
                timeMultiplier = granadeThrowTimeMultiplierInDualWielding;
            }
            granadeThrower.ThrowGranadeStart(granade, timeMultiplier);
            inventory.UseGranade();
            armState = ArmState.InGranadeThrow;
            granadeThrowTimer = granade.ThrowTime * timeMultiplier;
            OnGranadeThrowStarted?.Invoke(granade, granadeThrowTimer);
        }
    }

    void SendGranadeThrowSignal(GameObject granade)
    {
        OnGranadeThrow?.Invoke(granade, inventory.GranadeStats);
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
        switchInTimer = weaponInHand.SwitchInTime;
        armState = ArmState.SwitchingIn;
        OnWeaponEquipStarted?.Invoke(weaponInHand, weaponInHand.SwitchInTime);

        SetUpWeapon(weaponInHand);
        weaponInHand.SwitchInStart(switchInTimer);
    }

    public void SetWeaponToIfDualWielding(bool isDualWielding)
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.SetIsBeingDualWielded(isDualWielding);
        }
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
