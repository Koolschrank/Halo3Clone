using System;
using UnityEngine;
using static PlayerArms;

public class Arm : MonoBehaviour
{
    public Action<Weapon_Arms, float> OnWeaponEquipStarted;
    public Action<Weapon_Arms, float> OnWeaponUnequipStarted;
    public Action<Weapon_Arms, float> OnWeaponReloadStarted;
    public Action<Weapon_Arms, float> OnMeleeWithWeaponStarted;
    public Action<Weapon_Arms> OnWeaponShoot;
    public Action<Weapon_Arms> OnWeaponUnequipFinished;
    public Action<Weapon_Arms> OnWeaponDroped;

    bool isTriggerPressed;
    bool wasTriggerPressed;
    float reloadTimer;
    float switchOutTimer;
    float switchInTimer;


    Weapon_Arms weaponInHand;
    ArmState armState = ArmState.Ready;


}
