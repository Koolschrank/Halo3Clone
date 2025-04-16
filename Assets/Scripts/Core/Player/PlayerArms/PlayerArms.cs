using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using static Arm;
// rigging


public class PlayerArms : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] RightArm rightArm;
    [SerializeField] LeftArm leftArm;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerArmsInput armsInput;
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] PlayerPickUpScan pickUpScan;
    
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] PlayerMeleeAttack basicMeleeAttack;

    [Header("Settings")]
    [SerializeField] float weaponDropForce;
    [SerializeField] bool canDualWield2HandedWeapons = false;
    [SerializeField] float meleeAttackTimeMultiplierInDualWielding = 1.5f;
    [SerializeField] float granadeThrowTimeMultiplierInDualWielding = 1.5f;


    public float MeleeAttackTimeMultiplierInDualWielding => meleeAttackTimeMultiplierInDualWielding;
    public PlayerInventory Inventory => inventory;
    public PlayerPickUpScan PickUpScan => pickUpScan;
    public BulletSpawner BulletSpawner => bulletSpawner;

    public PlayerMeleeAttack BasicMeleeAttack => basicMeleeAttack;

    public MeleeAttacker MeleeAttacker => meleeAttacker;

    public GranadeThrower GranadeThrower => granadeThrower;
    public float WeaponDropForce => weaponDropForce;


    ArmsState armsState = ArmsState.OneWeapon;
    public RightArm RightArm
    {
        get
        {
            return rightArm;
        }
    }

    public LeftArm LeftArm
    {
        get
        {
            return leftArm;
        }
    }

    public Action<bool> OnZoomUpdated;

    bool inZoom;
    public bool InZoom 
    {
        set
        {
            if (value != inZoom)
            {
                OnZoomUpdated?.Invoke(value);
            }

            inZoom = value;
        }
        get
        {
            return inZoom;
        }
    }

    float switchOutTimer = 0f;
    float meleeAttackTimer = 0f;
    float granadeThrowTimer = 0f;

    public bool InSwitchOut => switchOutTimer > 0f;
    public bool InMeleeAttack => meleeAttackTimer > 0f;
    public bool InGranadeThrow => granadeThrowTimer > 0f;

    public bool InAction => InSwitchOut || InMeleeAttack || InGranadeThrow;

    public bool IsDualWielding => armsState == ArmsState.TwoWeapons;


    public void SetState(ArmsState state)
    {
        if (armsState == state) return;

        armsState = state;

        switch (armsState)
        {
            case ArmsState.TwoWeapons:
                EnterDualWielding();
                break;
            case ArmsState.OneWeapon:
                ExitDualWielding();
                break;
        }
    }

    public override void FixedUpdateNetwork()
    {
        rightArm.ArmUpdate();
        leftArm.ArmUpdate();
        switch (armsState)
        {
            case ArmsState.OneWeapon:
                FixedUpdate_OneWeapon();
                break;
            case ArmsState.TwoWeapons:
                FixedUpdate_TwoWeapons();
                break;

        }


    }


    private void FixedUpdate_OneWeapon()
    {
        if (rightArm.CurrentWeapon == null)
            return;


        InZoom = armsInput.Weapon2 && rightArm.CurrentWeapon != null && rightArm.CurrentWeapon.CanZoom && rightArm.InIdle  && !InAction;
        if (InSwitchOut)
        {
            switchOutTimer -= Runner.DeltaTime;
            if (switchOutTimer <= 0f)
            {
                EquipWeaponFromInventory();
            }
            return;
        }
        if (InGranadeThrow)
        {
            granadeThrowTimer -= Runner.DeltaTime;
            if (granadeThrowTimer <= 0f)
            {
                
            }
            return;
        }

        if (rightArm.InMeleeAttack)
        {
            return;
        }



        if (armsInput.Melee)
        {
            if (rightArm.TryMelee())
            {
                return;
            }
        }
        if (armsInput.Ability1)
        {
            if (TryUseAbility())
            {
                return;
            }
        }
        if (armsInput.SwitchWeapon)
        {
            if (TrySwitchWeapon())
            {
                armsInput.ResetSwitchInput();
            }
        }


        if (InAction) return;


        if (rightArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmo(rightArm.CurrentWeapon.Data))
                rightArm.TryReload();
            
        }

        if (armsInput.Reload1)
        {
            bool reloadStarted = rightArm.TryReload();
            if (reloadStarted)
            {
                armsInput.ResetReload1Input();
            }
        }


        if (armsInput.Weapon1)
        {
            if (rightArm.CurrentWeapon.Magazine == 0)
            {
                if (!inventory.HasAmmo(rightArm.CurrentWeapon.Data))
                {
                    TrySwitchWeapon();
                    if (InSwitchOut) return;
                }
            }
            else
                rightArm.TriggerHeld(); 
        }
        else
        {
            rightArm.TriggerReleased();
        }

        

        if (armsInput.PickUp1)
        {
            bool pickUpStarted = rightArm.TryPickUpWeapon();
            if (pickUpStarted)
            {
                armsInput.ResetPickUp1Input();
            }
        }

        if ((armsInput.PickUp2 || armsInput.Reload2) &&(canDualWield2HandedWeapons || (rightArm.CurrentWeapon.WeaponType == WeaponType.oneHanded && pickUpScan.IsClosesPickUpOneHanded())))
        {


            bool pickUpStarted = leftArm.TryPickUpWeapon();
            if (pickUpStarted)
            {
                armsInput.ResetPickUp2Input();
                armsInput.ResetPickUp1Input();
                armsInput.ResetReload1Input();

                armsInput.ResetReload2Input();
                SetState(ArmsState.TwoWeapons);
                
            }
        }

    }

    public void FixedUpdate_TwoWeapons()
    {

        if (leftArm.CurrentWeapon == null)
            return;
        InZoom = false;
        
        if (InGranadeThrow)
        {
            granadeThrowTimer -= Runner.DeltaTime;
            if (granadeThrowTimer <= 0f)
            {

            }
            return;
        }

        if (rightArm.InMeleeAttack && rightArm.CurrentWeapon.ShootType != ShootType.Melee) // only if its a melee weapon can the player still use the other weapon
        {
            return;
        }



        if (armsInput.Melee)
        {
            if (rightArm.TryMelee())
            {
                return;
            }
        }
        if (armsInput.Ability1)
        {
            if (TryUseAbility())
            {
                return;
            }
        }
        


        if (InAction) return;

        if (armsInput.SwitchWeapon)
        {
            leftArm.DropWeapon(weaponDropForce);
            armsInput.ResetSwitchInput();
            SetState(ArmsState.OneWeapon);
            return;
        }


        if (rightArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmo(rightArm.CurrentWeapon.Data))
                rightArm.TryReload();

        }
        if (leftArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmo(leftArm.CurrentWeapon.Data))
                leftArm.TryReload();

        }

        if (armsInput.Reload2)
        {
            bool reloadStarted = rightArm.TryReload();
            if (reloadStarted)
            {
                armsInput.ResetReload2Input();
            }
        }
        if (armsInput.Reload1)
        {
            bool reloadStarted = leftArm.TryReload();
            if (reloadStarted)
            {
                armsInput.ResetReload1Input();
            }
        }


        if (armsInput.Weapon1)
        {
            rightArm.TriggerHeld();
        }
        else
        {
            rightArm.TriggerReleased();
        }

        if (armsInput.Weapon2)
        {
            leftArm.TriggerHeld();
        }
        else
        {
            leftArm.TriggerReleased();
        }



        if (armsInput.PickUp2)
        {
            if (canDualWield2HandedWeapons || pickUpScan.IsClosesPickUpOneHanded())
            {
                bool pickUpStarted = rightArm.TryPickUpWeapon();
                if (pickUpStarted)
                {
                    armsInput.ResetPickUp2Input();
                }
            }
            else
            {
                bool pickUpStarted = rightArm.TryPickUpWeapon();
                if (pickUpStarted)
                {
                    armsInput.ResetPickUp2Input();
                    leftArm.DropWeapon(weaponDropForce);
                    SetState(ArmsState.OneWeapon);
                    return;
                }
            }
            
        }

        if (armsInput.PickUp1)
        {
            if (canDualWield2HandedWeapons || pickUpScan.IsClosesPickUpOneHanded())
            {
                bool pickUpStarted = leftArm.TryPickUpWeapon();
                if (pickUpStarted)
                {
                    armsInput.ResetPickUp1Input();
                }
            }
            else
            {
                bool pickUpStarted = rightArm.TryPickUpWeapon();
                if (pickUpStarted)
                {
                    armsInput.ResetPickUp1Input();
                    leftArm.DropWeapon(weaponDropForce);
                    SetState(ArmsState.OneWeapon);
                    return;
                }
            }

            
        }

    }

    public bool TryUseAbility()
    {
        if (InAction || !inventory.HasGranades) return false;

        var granadeStats = inventory.GranadeStats;
        float timeMultiplier = 1;
        if (isDualWielding)
        {
            timeMultiplier = granadeThrowTimeMultiplierInDualWielding;
        }

        granadeThrowTimer = granadeStats.ThrowTime * timeMultiplier;
        granadeThrower.ThrowGranadeStart(granadeStats, timeMultiplier);
        inventory.UseGranade();
        rightArm.OnGranadeThrowStarted(granadeStats, timeMultiplier);
        leftArm.OnGranadeThrowStarted(granadeStats, timeMultiplier);
        return true;
        
    }

    

    public bool TrySwitchWeapon()
    {
        if (InAction || !inventory.HasWeapon) return false;



        rightArm.CancelTimers();
        if (rightArm.InReload)
        {
            rightArm.CancelReload();
        }

        if ( rightArm.CurrentWeapon != null && rightArm.CurrentWeapon.CanNotBeInInventory)
        {
            rightArm.DropWeapon();
        }

        if (rightArm.CurrentWeapon == null)
        {
            EquipWeaponFromInventory();
        }
        else
        {
            var weaponToSwitchOut = rightArm.CurrentWeapon;
            switchOutTimer = weaponToSwitchOut.SwitchOutTime;
            rightArm.OnWeaponUnequipStarted?.Invoke(weaponToSwitchOut, weaponToSwitchOut.SwitchOutTime);
            weaponToSwitchOut.SwitchOutStart(switchOutTimer);
        }

        return true;
    }

    public void EquipWeaponFromInventory()
    {
        rightArm.EquipWeapon(inventory.RemoveWeapon());

    }








    public Action OnDualWieldingEntered;
    public Action OnDualWieldingExited;

    
    

    bool isDualWielding = false;

    float movementSpeedMultiplier = 1;

    public void SetCanDualWield2HandedWeapons(bool value)
    {
        canDualWield2HandedWeapons = value;
    }

    public float MovementSpeedMultiplier
    {
        get
        {
            return movementSpeedMultiplier;
        }
    }

    private void Awake()
    {
        rightArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        rightArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        leftArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        leftArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };
    }


    

    public void EnterDualWielding()
    {
        OnDualWieldingEntered?.Invoke();
        isDualWielding = true;

        rightArm.SetWeaponToIfDualWielding(true);
        leftArm.SetWeaponToIfDualWielding(true);



    }

    public void ExitDualWielding()
    {
        OnDualWieldingExited?.Invoke();
        isDualWielding = false;

        rightArm.SetWeaponToIfDualWielding(false);
    }


    public bool CanDualWield2HandedWeapons
    {
        get
        {
            return canDualWield2HandedWeapons;
        }
    }


    public void DeleteWeapon(Weapon_Data data)
    {
        
        if (rightArm.CurrentWeapon != null && rightArm.CurrentWeapon.Data == data)
        {
            rightArm.DeleteWeapon();
        }
        if (leftArm.CurrentWeapon != null && leftArm.CurrentWeapon.Data == data)
        {
            leftArm.DeleteWeapon();
            SetState(ArmsState.OneWeapon);
        }

    }

    public bool HasWeapon(Weapon_Data weapon)
    {
        return rightArm.CurrentWeapon != null && rightArm.CurrentWeapon.Data == weapon ||
            leftArm.CurrentWeapon != null && leftArm.CurrentWeapon.Data == weapon ||
            inventory.GetWeapon() != null && inventory.GetWeapon().Data == weapon;
    }

    public bool HasMultipleOfTheSameWeapon(Weapon_Data weaponToCheck)
    {
        Weapon_Data rightWeapon = null;
        Weapon_Data leftWeapon = null;
        Weapon_Data weaponInInventory = null;

        if (rightArm.CurrentWeapon != null)
        {
            rightWeapon = rightArm.CurrentWeapon.Data;
        }

        if (leftArm.CurrentWeapon != null)
        {
            leftWeapon = leftArm.CurrentWeapon.Data;
        }

        if (inventory.GetWeapon() != null)
        {
            weaponInInventory = inventory.GetWeapon().Data;
        }


        int count = 0;
        if (rightWeapon != null && rightWeapon == weaponToCheck)
        {
            count++;
        }

        if (leftWeapon != null && leftWeapon == weaponToCheck)
        {
            count++;
        }

        if (weaponInInventory != null && weaponInInventory == weaponToCheck)
        {
            count++;
        }

        return count > 1;



    }


    float damageReduction = 0;
    public float DamageReduction
    {
        get
        {
            return damageReduction;
        }
    }

    public void SetDamageReduction()
    {
        damageReduction = 0;
        Weapon_Arms rightWeapon = rightArm.CurrentWeapon;
        Weapon_Arms leftWeapon = leftArm.CurrentWeapon;
        if (rightWeapon != null)
        {
            damageReduction = rightWeapon.Data.DamageReduction;
        }

        if (leftWeapon != null)
        {
            float leftWeaponDamageReduction = leftWeapon.Data.DamageReduction;
            if (leftWeaponDamageReduction > damageReduction)
            {
                damageReduction = leftWeaponDamageReduction;
            }
        }
    }

    public void SetMovementSpeedMultiplier()
    {
        movementSpeedMultiplier = 1;
        Weapon_Arms rightWeapon = rightArm.CurrentWeapon;
        Weapon_Arms leftWeapon = leftArm.CurrentWeapon;
        if (rightWeapon != null)
        {
            movementSpeedMultiplier = rightWeapon.MoveSpeedMultiplier;
        }
        if (leftWeapon != null)
        {
            float leftWeaponMovementSpeedMultiplier = leftWeapon.MoveSpeedMultiplier;
            if (leftWeaponMovementSpeedMultiplier != 1)
            {
                movementSpeedMultiplier *= leftWeaponMovementSpeedMultiplier;
            }
        }
    }
}


public enum ArmsState
{
    OneWeapon,
    TwoWeapons,
}