using Fusion;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStartEquipment : NetworkBehaviour // is only networked for testin
{
    [SerializeField] Equipment testEquipment;


    [Header("References")]
    [SerializeField] WeaponInventory weaponInventory;
    [SerializeField] AbilityInventory abilityInventory;
    [SerializeField] CharacterHealth health;
    [SerializeField] PlayerMovement playerMovement;


    public override void Spawned()
    {
        GetEquipment(testEquipment);
    }


    public void GetEquipment(Equipment equipment)
    {
        if (!HasStateAuthority) return;

        if (equipment.WeaponInHand != null)
        {
            weaponInventory.Equip_RightWeapon(
                CreateWeapon(
                equipment.WeaponInHand.WeaponTypeIndex,
                equipment.MagazinsOfWeaponInHand));
        }
        else
        {
            weaponInventory.Equip_RightWeapon(
                new WeaponNetworkStruct
                {
                    index = -1,
                    weaponTypeIndex = -1,
                    ammoInMagazine = 0,
                    ammoInReserve = 0
                });
        }

        if (equipment.WeaponInLeftHand != null)
        {
            weaponInventory.Equip_LeftWeapon(
                CreateWeapon(
                equipment.WeaponInLeftHand.WeaponTypeIndex,
                equipment.MagazinsOfWeaponInLeftHand));
        }
        else
        {
            weaponInventory.Equip_LeftWeapon(
                new WeaponNetworkStruct
                {
                    index = -1,
                    weaponTypeIndex = -1,
                    ammoInMagazine = 0,
                    ammoInReserve = 0
                });
        }






        if (equipment.SideArm != null)
        {
            weaponInventory.Equip_BackWeapon(
                CreateWeapon(
                equipment.SideArm.WeaponTypeIndex,
                equipment.MagazinsOfSideArm));
        }
        else
        {
            weaponInventory.Equip_BackWeapon(
                new WeaponNetworkStruct
                {
                    index = -1,
                    weaponTypeIndex = -1,
                    ammoInMagazine = 0,
                    ammoInReserve = 0
                });
        }


        if (equipment.Ability != null)
        {
            var ability = equipment.Ability;
            abilityInventory.SetAbility(
                ability.AbilityTypeIndex,
                ability.maxUses,
                ability.RechargeTime);
        }
        

            //if (equipment.Granade != null)
            //{
            //    playerInventory.ChangeGranade(equipment.Granade);
            //    playerInventory.AddGranades(equipment.GranadeCount);
            //}
            //else
            //{
            //    playerInventory.ChangeGranade(null);
            //}

            //playerMovement.SetMovementSpeedMultiplier(equipment.MovementSpeedMultiplier);






            //if (!equipment.HasMiniMap)
            //{
            //    playerInventory.OnMiniMapDisabled?.Invoke();
            //}
            //else
            //{
            //    playerInventory.OnMiniMapEnabled?.Invoke();
            //}

            health.SetHasShild(equipment.HasShild);
        health.SetHeadShotOneShot(equipment.HeadShotOneShot);

        //playerArms.SetCanDualWield2HandedWeapons(equipment.CanDualWieldEverything);

    }

    public WeaponNetworkStruct CreateWeapon(int weaponIndex, int magazines)
    {
        var data = ItemIndexList.Instance.GetWeaponViaIndex(weaponIndex);
        int magazineSize = data.MagazineSize;

        int bulletsInMagazine = magazineSize;
        int bulletsInReserve = (magazines - 1) * magazineSize;

        return new WeaponNetworkStruct
        {
            index = ItemIndexList.Instance.GetNextIndex(),
            weaponTypeIndex = weaponIndex,
            ammoInMagazine = bulletsInMagazine,
            ammoInReserve = bulletsInReserve
        };
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

    [SerializeField] Ability_Data ability;

    public bool HasShild => hasShild;
    public bool HeadShotOneShot => headShotOneShot;

    public Weapon_Data WeaponInHand => weaponInHand;
    public int MagazinsOfWeaponInHand => magazinsOfWeaponInHand;

    public Weapon_Data WeaponInLeftHand => weaponInLeftHand;
    public int MagazinsOfWeaponInLeftHand => magazinsOfWeaponInLeftHand;

    public Weapon_Data SideArm => sideArm;
    public int MagazinsOfSideArm => magazinsOfSideArm;

    public Ability_Data Ability => ability;

    public bool HasMiniMap => hasMiniMap;

    public float MovementSpeedMultiplier => movementSpeedMultiplier;

    public bool CanDualWieldEverything => canDualWieldEverything;


}