using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartEquipment : MonoBehaviour
{
    [Header("equpment")]
    [SerializeField] List<Weapon_Data> startingWeapons = new List<Weapon_Data>();
    [SerializeField] int startingGranades = 0;

    [Header("References")]
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerInventory playerInventory;


    private void Start()
    {
        if (startingWeapons.Count <= 0)
            return;

        var firstWeapon = SpawnWeapon(startingWeapons[0]);
        playerArms.PickUpWeapon(firstWeapon);

        for (int i = 1; i < startingWeapons.Count; i++)
        {
            var weapon = SpawnWeapon(startingWeapons[i]);
            playerInventory.AddWeapon(weapon);
        }

        for (int i = 0; i < startingGranades; i++)
        {
            playerInventory.AddGranade();
        }





    }

    public Weapon_Arms SpawnWeapon(Weapon_Data data)
    {
        var weapon = new Weapon_Arms(data);
        weapon.FillMagazine();
        weapon.FillReserve();
        return weapon;
    }



}


[Serializable]
public class Equipment
{
    [SerializeField] bool hasShild = true;
    [SerializeField] bool headShotOneShot = true;


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



}