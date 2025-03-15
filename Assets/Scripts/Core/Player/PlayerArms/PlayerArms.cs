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
    [SerializeField] bool canDualWield2HandedWeapons = false;
    [SerializeField] float dualWieldMoveSpeedMultiplier = 1;

    bool isDualWielding = false;

    public float DualWieldMoveSpeedMultiplier
    {
        get
        {
            return dualWieldMoveSpeedMultiplier;
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




}