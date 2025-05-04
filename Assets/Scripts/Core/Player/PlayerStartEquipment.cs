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
            playerArms.RightArm.PickUpWeapon(
            SpawnWeapon(
                equipment.WeaponInHand,
                equipment.MagazinsOfWeaponInHand));
        }

        if (equipment.WeaponInLeftHand != null)
        {
            playerArms.LeftArm.PickUpWeapon(
            SpawnWeapon(
                equipment.WeaponInLeftHand,
                equipment.MagazinsOfWeaponInLeftHand));
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

        playerArms.SetCanDualWield2HandedWeapons(equipment.CanDualWieldEverything);

    }

    public Weapon_Arms SpawnWeapon(Weapon_Data data)
    {
        var weapon = new Weapon_Arms(data);
        weapon.FillMagazine();
        playerInventory.AddAmmo(data, data.ReserveSize);
        return weapon;
    }

    public Weapon_Arms SpawnWeapon(Weapon_Data data, int magazins)
    {
        var weapon = new Weapon_Arms(data);
        weapon.FillMagazine();
        playerInventory.AddAmmo(data, data.MagazineSize * (magazins -1));
        return weapon;
    }
}


[Serializable]
public class Equipment
{
    [SerializeField] bool hasShild = true;
    [SerializeField] bool headShotOneShot = true;
    [SerializeField] bool hasMiniMap = true;
    [SerializeField] bool canDualWieldEverything = false;
    [SerializeField] float movementSpeedMultiplier = 1;


    [SerializeField] Weapon_Data weaponInHand;
    [SerializeField] int magazinsOfWeaponInHand = 3;
    [SerializeField] Weapon_Data weaponInLeftHand;
    [SerializeField] int magazinsOfWeaponInLeftHand = 3;
    [SerializeField] Weapon_Data sideArm;
    [SerializeField] int magazinsOfSideArm = 5;

    [SerializeField] GranadeStats granade;
    [SerializeField] int granadeCount = 0;

    public bool HasShild => hasShild;
    public bool HeadShotOneShot => headShotOneShot;

    public void SetHasShild(bool hasShild)
    {
        this.hasShild = hasShild;
    }

    public void SetHeadShotOneShot(bool headShotOneShot)
    {
        this.headShotOneShot = headShotOneShot;
    }

    public void SetHasMiniMap(bool hasMiniMap)
    {
        this.hasMiniMap = hasMiniMap;
    }

    public void SetCanDualWieldEverything(bool canDualWieldEverything)
    {
        this.canDualWieldEverything = canDualWieldEverything;
    }

    public void SetWeapons(Weapon_Data weaponInHand, Weapon_Data weaponInLeftHand, Weapon_Data sideArm)
    {
        this.weaponInHand = weaponInHand;
        this.weaponInLeftHand = weaponInLeftHand;
        this.sideArm = sideArm;
    }

    public void SetMagazins(int magazinsOfWeaponInHand, int magazinsOfWeaponInLeftHand, int magazinsOfSideArm)
    {
        this.magazinsOfWeaponInHand = magazinsOfWeaponInHand;
        this.magazinsOfWeaponInLeftHand = magazinsOfWeaponInLeftHand;
        this.magazinsOfSideArm = magazinsOfSideArm;
    }

    public void SetMovementSpeedMultiplier(float movementSpeedMultiplier)
    {
        this.movementSpeedMultiplier = movementSpeedMultiplier;
    }



    public Weapon_Data WeaponInHand => weaponInHand;
    public int MagazinsOfWeaponInHand => magazinsOfWeaponInHand;

    public Weapon_Data WeaponInLeftHand => weaponInLeftHand;
    public int MagazinsOfWeaponInLeftHand => magazinsOfWeaponInLeftHand;

    public Weapon_Data SideArm => sideArm;
    public int MagazinsOfSideArm => magazinsOfSideArm;

    public GranadeStats Granade => granade;
    public int GranadeCount => granadeCount;

    public bool HasMiniMap => hasMiniMap;

    public float MovementSpeedMultiplier => movementSpeedMultiplier;

    public bool CanDualWieldEverything => canDualWieldEverything;


    public Equipment(Equipment equipmentToCopy)
    {
        this.hasShild = equipmentToCopy.hasShild;
        this.headShotOneShot = equipmentToCopy.headShotOneShot;
        this.hasMiniMap = equipmentToCopy.hasMiniMap;
        this.canDualWieldEverything = equipmentToCopy.canDualWieldEverything;
        this.movementSpeedMultiplier = equipmentToCopy.movementSpeedMultiplier;
        this.weaponInHand = equipmentToCopy.weaponInHand;
        this.magazinsOfWeaponInHand = equipmentToCopy.magazinsOfWeaponInHand;
        this.weaponInLeftHand = equipmentToCopy.weaponInLeftHand;
        this.magazinsOfWeaponInLeftHand = equipmentToCopy.magazinsOfWeaponInLeftHand;
        this.sideArm = equipmentToCopy.sideArm;
        this.magazinsOfSideArm = equipmentToCopy.magazinsOfSideArm;
        this.granade = equipmentToCopy.granade;
        this.granadeCount = equipmentToCopy.granadeCount;
    }


}