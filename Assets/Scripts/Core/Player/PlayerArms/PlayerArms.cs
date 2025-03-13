using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
// rigging


public class PlayerArms : MonoBehaviour
{
    public Action<Weapon_Arms, float> OnWeaponEquipStarted;
    public Action<Weapon_Arms, float> OnWeaponUnequipStarted;
    public Action<Weapon_Arms, float> OnWeaponReloadStarted;
    public Action<Weapon_Arms, float> OnMeleeWithWeaponStarted;
    public Action<Weapon_Arms> OnWeaponShoot;
    public Action<Weapon_Arms> OnWeaponUnequipFinished;
    public Action<Weapon_Arms> OnWeaponDroped;
    public Action<GranadeStats> OnGranadeThrowStarted;
    public Action<GameObject, GranadeStats> OnGranadeThrow;
    public Action<Weapon_Arms> OnZoomIn;
    public Action<Weapon_Arms> OnZoomOut;


    [Header("References")]
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] Controller controller;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerPickUpScan pickUpScan;
    [SerializeField] Transform dropPosition;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] PlayerMeleeAttack basicMeleeAttack;

    bool isTriggerPressed;
    bool wasTriggerPressed;
    float reloadTimer;
    float switchOutTimer;
    float switchInTimer;
    float granadeThrowTimer;
    float meleeAttackTimer;


    Weapon_Arms weaponInHand;
    ArmState armState = ArmState.Ready;
    bool inZoom = false;

    [Header("Settings")]
    [SerializeField] float weaponDropForce;
    [SerializeField] float reloadInputBuffer = 0.4f;
    float reloadInputBufferTimer;
    [SerializeField] float switchInputBuffer = 0.4f;
    float switchInputBufferTimer;
    [SerializeField] float granadeThrowInputBuffer = 0.4f;
    float granadeThrowInputBufferTimer;

    private void Start()
    {
        granadeThrower.OnGranadeThrow += SendGranadeThrowSignal;
        characterHealth.OnDeath += DropWeaponWithNoForce;
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


        if ((armState == ArmState.Shooting)&& !weaponInHand.IsInShootCooldown())
        {
            armState = ArmState.Ready;
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

        if (weaponInHand != null && weaponInHand.CanReload())
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
        weaponInHand.ReloadFinished();
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

    public void PressSwitchButton()
    {
        switchInputBufferTimer = switchInputBuffer;
        reloadInputBufferTimer = 0;
    }

    void TrySwitchWeapon()
    {
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

    public void TryPickUpWeapon()
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

    void DropWeapon()
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
        if (weaponInHand.Magazine == 0 && weaponInHand.Reserve == 0)
        {
            
            return null;
        }

        var pickUpVersion = weaponInHand.PickUpVersion;
        var pickUp = Instantiate(pickUpVersion, dropPosition.position, dropPosition.rotation);
        pickUp.SetAmmo(weaponInHand.Magazine, weaponInHand.Reserve);

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

    public void TryThrowGranade()
    {
        if (armState != ArmState.Ready) return;
        if (inventory.HasGranades)
        {
            IfZoomedInExitZoom();
            var granade = inventory.GranadeStats;
            granadeThrower.ThrowGranadeStart(granade);
            inventory.UseGranade();
            armState = ArmState.InGranadeThrow;
            granadeThrowTimer = granade.ThrowTime;
            OnGranadeThrowStarted?.Invoke(granade);
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

    public void TryMeleeAttack()
    {
        if (armState != ArmState.Ready && armState != ArmState.Shooting && armState != ArmState.Reloading) return;
        IfZoomedInExitZoom();
        var meleeAttack = weaponInHand.MeleeAttack;
        if (meleeAttack == null)
        {
            meleeAttack = basicMeleeAttack;
        }
        armState = ArmState.InMeleeAttack;
        meleeAttackTimer = meleeAttack.MeleeTime;
        meleeAttacker.AttackStart(meleeAttack);
        weaponInHand.MeleeStart(meleeAttackTimer);
        OnMeleeWithWeaponStarted?.Invoke(weaponInHand, meleeAttackTimer);


    }

    void SwitchWeapon()
    {
        var weaponToSwitchInto = inventory.RemoveWeapon();
        if (weaponInHand != null)
        {
            inventory.AddWeapon(weaponInHand);
        }
        EquipWeapon(weaponToSwitchInto);
    }

    void EquipWeapon(Weapon_Arms weapon)
    {
        weaponInHand = weapon;
        switchInTimer = weaponInHand.SwitchInTime;
        armState = ArmState.SwitchingIn;
        OnWeaponEquipStarted?.Invoke(weaponInHand, weaponInHand.SwitchInTime);

        SetUpWeapon(weaponInHand);
        weaponInHand.SwitchInStart(switchInTimer);
    }

    public Weapon_Arms GetWeaponInHand()
    {
        return weaponInHand;
    }

    public float GetWeaponInHandSwitchInTime()
    {
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