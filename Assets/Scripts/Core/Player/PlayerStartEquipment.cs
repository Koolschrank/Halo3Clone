using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartEquipment : MonoBehaviour
{
    

    
    [Header("References")]
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] CharacterHealth health;
    [SerializeField] PlayerMovement playerMovement;



    public void GetEquipment(Equipment equipment)
    {

        if (equipment.WeaponInHand != null)
        {
            playerArms.PickUpWeapon(
            SpawnWeapon(
                equipment.WeaponInHand,
                equipment.MagazinsOfWeaponInHand));
        }

            

        if (equipment.SideArm != null)
        {
            playerInventory.Clear();

            playerInventory.AddWeapon(
            SpawnWeapon(
                equipment.SideArm,
                equipment.MagazinsOfSideArm));
        }

        if (equipment.Granade != null)
        {
            playerInventory.ChangeGranade(equipment.Granade);
            playerInventory.AddGranades(equipment.GranadeCount);
        }
        else
        {
            playerInventory.ChangeGranade(null);
        }

        playerMovement.SetMovementSpeedMultiplier(equipment.MovementSpeedMultiplier);






        if (!equipment.HasMiniMap)
        {
            playerInventory.OnMiniMapDisabled?.Invoke();
        }
        else
        {
            playerInventory.OnMiniMapEnabled?.Invoke();
        }

        health.SetHasShild(equipment.HasShild);
        health.SetHeadShotOneShot(equipment.HeadShotOneShot);

    }

    public Weapon_Arms SpawnWeapon(Weapon_Data data)
    {
        var weapon = new Weapon_Arms(data);
        weapon.FillMagazine();
        weapon.FillReserve();
        return weapon;
    }

    public Weapon_Arms SpawnWeapon(Weapon_Data data, int magazin)
    {
        var weapon = new Weapon_Arms(data);
        weapon.GainMagazins(magazin);
        return weapon;
    }
}


[Serializable]
public class Equipment
{
    [SerializeField] bool hasShild = true;
    [SerializeField] bool headShotOneShot = true;
    [SerializeField] bool hasMiniMap = true;
    [SerializeField] float movementSpeedMultiplier = 1;


    [SerializeField] Weapon_Data weaponInHand;
    [SerializeField] int magazinsOfWeaponInHand = 3;
    [SerializeField] Weapon_Data sideArm;
    [SerializeField] int magazinsOfSideArm = 5;

    [SerializeField] GranadeStats granade;
    [SerializeField] int granadeCount = 0;

    public bool HasShild => hasShild;
    public bool HeadShotOneShot => headShotOneShot;

    public Weapon_Data WeaponInHand => weaponInHand;
    public int MagazinsOfWeaponInHand => magazinsOfWeaponInHand;

    public Weapon_Data SideArm => sideArm;
    public int MagazinsOfSideArm => magazinsOfSideArm;

    public GranadeStats Granade => granade;
    public int GranadeCount => granadeCount;

    public bool HasMiniMap => hasMiniMap;

    public float MovementSpeedMultiplier => movementSpeedMultiplier;


}