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
                EquipWeaponFromInventory(rightArm);
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
            if (TrySwitchWeapon(rightArm))
            {
                armsInput.ResetSwitchInput();
            }
        }


        if (InAction) return;


        if (rightArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmoInReserve(rightArm.CurrentWeapon.Data.WeaponTypeIndex))
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
                if (!inventory.HasAmmoInReserve(rightArm.CurrentWeapon.Data.WeaponTypeIndex))
                {
                    TrySwitchWeapon(rightArm);
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

        if ((armsInput.PickUp2 || armsInput.Reload2))
        {

            if (pickUpScan.IsWeaponInRange() && 
                pickUpScan.CanPickUpWeapon(leftArm)&&
                (canDualWield2HandedWeapons ||  
                (rightArm.CurrentWeapon.WeaponType == WeaponType.oneHanded && pickUpScan.IsClosesPickUpOneHanded())))
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
            else if (canDualWield2HandedWeapons || 
                (rightArm.CurrentWeapon.WeaponType == WeaponType.oneHanded && ItemIndexList.Instance.GetWeaponViaIndex(inventory.WeaponInInventory).WeaponType == WeaponType.oneHanded))
            {
                leftArm.EquipWeapon(inventory.RemoveWeapon());
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

        if (InSwitchOut)
        {
            switchOutTimer -= Runner.DeltaTime;
            if (switchOutTimer <= 0f)
            {
                MoveWeaponToInventory(leftArm);
                SetState(ArmsState.OneWeapon);
            }
            return;
        }

        if (leftArm.CurrentWeapon == null)
        {
            SetState(ArmsState.OneWeapon);
        }
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
            if (inventory.HasWeaponInInventory)
            {
                leftArm.DropWeapon(weaponDropForce);
                armsInput.ResetSwitchInput();
                SetState(ArmsState.OneWeapon);
            }
            else
            {
                if (TrySwitchWeapon(leftArm))
                {
                    armsInput.ResetSwitchInput();
                }
            }


            
            return;
        }


        if (rightArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmoInReserve(rightArm.CurrentWeapon.Data.WeaponTypeIndex))
                rightArm.TryReload();

        }
        if (leftArm.CurrentWeapon.Magazine == 0)
        {
            if (inventory.HasAmmoInReserve(leftArm.CurrentWeapon.Data.WeaponTypeIndex))
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

    

    public bool TrySwitchWeapon(Arm arm)
    {
        if (InAction || (inventory.WeaponInInventory == -1) && arm != leftArm) return false;



        arm.CancelTimers();
        if (arm.InReload)
        {
            arm.CancelReload();
        }

        if (arm.CurrentWeapon != null && arm.CurrentWeapon.CanNotBeInInventory)
        {
            arm.DropWeapon();
        }

        if (arm.CurrentWeapon == null)
        {
            EquipWeaponFromInventory(arm);
        }
        else
        {
            var weaponToSwitchOut = arm.CurrentWeapon;
            switchOutTimer = weaponToSwitchOut.SwitchOutTime;
            arm.OnWeaponUnequipStarted?.Invoke(weaponToSwitchOut, weaponToSwitchOut.SwitchOutTime);
            weaponToSwitchOut.SwitchOutStart(switchOutTimer);
        }

        return true;
    }

    public void EquipWeaponFromInventory(Arm arm)
    {
        arm.EquipWeapon(inventory.RemoveWeapon());

    }

    public void MoveWeaponToInventory(Arm arm)
    {
        var weaponStruct = new WeaponNetworkStruct()
        {
            weaponTypeIndex = arm.WeaponIndex,
            ammoInMagazine = arm.CurrentWeapon.Magazine,
            ammoInReserve = 0
        };
        inventory.SetWeaponInInventory(weaponStruct);
        arm.DeleteWeapon();
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
            TrySwitchWeapon(rightArm);
        }
        if (leftArm.CurrentWeapon != null && leftArm.CurrentWeapon.Data == data)
        {
            leftArm.DeleteWeapon();
            SetState(ArmsState.OneWeapon);
        }

    }

    public bool HasWeapon(int weaponIndex)
    {
        if (weaponIndex == -1) return false;





        return rightArm.WeaponIndex == weaponIndex ||
            leftArm.WeaponIndex == weaponIndex ||
            inventory.WeaponInInventory == weaponIndex;
    }

    public bool HasMultipleOfTheSameWeapon(int weaponIndexToCheck)
    {
        int count = 0;
        if(rightArm.WeaponIndex == weaponIndexToCheck)
        {
            count++;
        }

        if (leftArm.WeaponIndex == weaponIndexToCheck)
        {
            count++;
        }

        if (inventory.WeaponInInventory == weaponIndexToCheck)
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