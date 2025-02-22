using NUnit.Framework;
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
