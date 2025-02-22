using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
// rigging

/*
public class PlayerArms2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] TwoBoneIKConstraint leftHandWeaponGrip;
    [SerializeField] MultiAimConstraint rightHandWeaponGrip;


    [SerializeField] Controller controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Transform weaponPosition;
    [SerializeField] PlayerPickUpScan pickUpScan;
    [SerializeField] Transform dropPosition;
    [SerializeField] Transform weaponSocket;
    GameObject weaponSocketObject;

    [Header("Settings")]
    [SerializeField] float weaponDropForce;


    // action for equiping a weapon
    public Action<Weapon_Arms> OnWeaponEquiped;
    public Action<Weapon_Arms> OnWeaponUnequiped;


    Weapon_Arms weapon;


    // start with a weapon
    private void Start()
    {
        controller = new Controller();
        controller.Enable();
        controller.Player.ShootWeapon.performed += ctx => PressTrigger();
        controller.Player.ShootWeapon.canceled += ctx => ReleaseTrigger();
        controller.Player.Reload.performed += ctx => TryReload();
        controller.Player.SwitchWeapon.performed += ctx => TrySwitchWeapon();

        controller.Player.DropWeapon.performed += ctx => TryPickUpWeapon();
    }

    private void PressTrigger()
    {
        weapon?.PressTrigger();
    }

    private void ReleaseTrigger()
    {
        weapon?.ReleaseTrigger();
    }

    private void TryReload()
    {
        weapon?.TryReload();
        
        animator.SetTrigger("Reload");
        StartCoroutine(WeaponGripRegain());
    }

    IEnumerator WeaponGripRegain()
    {
        leftHandWeaponGrip.weight = 0;
        rightHandWeaponGrip.weight = 0;
        yield return new WaitForSeconds(2f);
        leftHandWeaponGrip.weight = 1;
        rightHandWeaponGrip.weight = 1;
    }


    private void TrySwitchWeapon()
    {
        if (inventory.HasWeapon && (weapon == null || weapon.CanSwitch()))
        {
            SwitchWeaponStart();
            animator.SetTrigger("SwitchWeapon");
            StartCoroutine(WeaponGripRegain());
        }
    }

    public void TryPickUpWeapon()
    {
        if (weapon != null &&!weapon.CanSwitch()) return;


        if (pickUpScan.CanPickUpWeapon())
        {
            var newWeapon = pickUpScan.PickUpWeapon();
            if (inventory.Full)
            {
                DropCurrentWeapon();
                EquipWeapon(newWeapon);
            }
            else
            {
                inventory.AddWeapon(newWeapon);
                TrySwitchWeapon();

            }
        }
    }

    private void SwitchWeaponStart()
    {
        if (weapon == null)
        {
            SwitchWeapon(null);
            return;
        }
        weapon.OnSwitchOutFinished += SwitchWeapon;
        weapon.SwitchOut();
    }
        

    private void SwitchWeapon(Weapon_Arms weapon)
    {
        if (weapon != null)
        {
            weapon.OnSwitchOutFinished -= SwitchWeapon;
        }
        
        var weaponToSwitch = inventory.SwitchWeapon(weapon);
        EquipWeapon(weaponToSwitch);
    }

    public void DropCurrentWeapon()
    {
        if (weapon == null) return;
        var pickUp = weapon.GetPickUp(dropPosition);
        pickUp.AddImpulse(dropPosition.forward, weaponDropForce);
        Destroy(weapon.gameObject);
    }




    public void EquipWeapon(Weapon_Arms weapon)
    {
        if (this.weapon != null) {
            OnWeaponUnequiped?.Invoke(this.weapon);
        }
        
        this.weapon = weapon;
        weapon.gameObject.SetActive(true);
        weapon.Equip(this);
        OnWeaponEquiped?.Invoke(weapon);
        weapon.transform.SetParent(weaponPosition);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        // if trigger is pressed down, press trigger
        if ( controller != null && controller.Player.ShootWeapon.ReadValue<float>() != 0)
        {
            PressTrigger();
        }


        // destroy weapon socket object
        if (weaponSocketObject != null)
        {
            Destroy(weaponSocketObject);
        }

        // add model to weapon socket
        var weaponModel = Instantiate(weapon.WeaponModel, weaponSocket);
        weaponSocketObject = weaponModel;



    }

    public void SetWeapon(Weapon_Arms weapon)
    {
        this.weapon = weapon;
    }

    public Camera PlayerCamera
    {
        get
        {
            return playerCamera;
        }
    }

    private void Update()
    {
        
    }





}
*/

public class PlayerArms : MonoBehaviour
{
    public Action<Weapon_Arms, float> OnWeaponEquipStarted;
    public Action<Weapon_Arms, float> OnWeaponUnequipStarted;
    public Action<Weapon_Arms, float> OnWeaponReloadStarted;
    public Action<Weapon_Arms, float> OnMeleeWithWeaponStarted;
    public Action<Weapon_Arms> OnWeaponUnequipFinished;
    public Action<Weapon_Arms> OnWeaponDroped;
    public Action<GranadeStats> OnGranadeThrowStarted;
    public Action<GameObject, GranadeStats> OnGranadeThrow;
    public Action<Weapon_Arms> OnZoomIn;
    public Action<Weapon_Arms> OnZoomOut;


    [Header("References")]
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

            switch (weaponInHand.ShootType)
            {
                case ShootType.Single :

                    if (!wasTriggerPressed && isTriggerPressed)
                    {
                        if (weaponInHand.CanShoot())
                        {
                            weaponInHand.TryShoot();
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
                            weaponInHand.TryShoot();
                        }
                        else // if try to shoot but cannot because magazine is empty reload
                        {
                            TryReload();
                        }
                    }
                    break;
            }

            if (weaponInHand.IsInShootCooldown())
            {
                armState = ArmState.Shooting;
            } 
                
        }
        wasTriggerPressed = isTriggerPressed;

        if (armState == ArmState.Shooting && !weaponInHand.IsInShootCooldown())
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

    public void PressZoomButton()
    {
        TryToggleZoom();
    }

    public void TryToggleZoom()
    {
        if (armState != ArmState.Ready && armState != ArmState.Shooting) return;
        if (weaponInHand != null && weaponInHand.CanZoom)
        {
            inZoom = !inZoom;
            // zoom event
            if (inZoom)
            {
                OnZoomIn?.Invoke(weaponInHand);
                
            }
            else
            {
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

    void DropWeapon()
    {
        if (weaponInHand == null) return;
        IfZoomedInExitZoom();
        OnWeaponDroped?.Invoke(weaponInHand);

        var pickUpVersion = weaponInHand.PickUpVersion;
        var pickUp = Instantiate(pickUpVersion, dropPosition.position, dropPosition.rotation);
        pickUp.AddImpulse(dropPosition.forward, weaponDropForce);
        weaponInHand = null;

        

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

        

        //weaponInHand.transform.SetParent(weaponSocket);
        //weaponInHand.transform.localPosition = Vector3.zero;
        //weaponInHand.transform.localRotation = Quaternion.identity;
        //weaponInHand.gameObject.SetActive(true);

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
        Reloading,
        SwitchingIn,
        SwitchingOut,
        InGranadeThrow,
        InMeleeAttack
    }

}