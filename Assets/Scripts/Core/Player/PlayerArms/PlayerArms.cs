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
    public Action OnDualWieldingEntered;
    public Action OnDualWieldingExited;

    [SerializeField] RightArm rightArm;
    [SerializeField] LeftArm leftArm;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] bool canDualWield2HandedWeapons = false;

    bool isDualWielding = false;

    float movementSpeedMultiplier = 1;

    public float MovementSpeedMultiplier
    {
        get
        {
            return movementSpeedMultiplier;
        }
    }

    private void Awake()
    {
        // enter dualwielding
        leftArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            if (rightArm.CurrentWeapon != null)
            {
                EnterDualWielding();
            }
        };

        LeftArm.OnWeaponUnequipFinished += (weapon) =>
        {
            ExitDualWielding();
        };

        LeftArm.OnWeaponDroped += (weapon) =>
        {
            ExitDualWielding();
        };


        rightArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        rightArm.OnWeaponDroped += (weapon) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        leftArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };

        leftArm.OnWeaponDroped += (weapon) =>
        {
            SetDamageReduction();
            SetMovementSpeedMultiplier();
        };
    }


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


    public bool IsDualWielding
    {
        get
        {
            return isDualWielding;
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
            if (leftWeaponMovementSpeedMultiplier > movementSpeedMultiplier)
            {
                movementSpeedMultiplier = leftWeaponMovementSpeedMultiplier;
            }
        }
    }
}