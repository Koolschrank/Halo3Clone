using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartEquipment : MonoBehaviour
{
    

    
    [Header("References")]
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] AbilityInventory abilityInventory;
    [SerializeField] CharacterHealth health;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] bool isNonPlayer = false;



    public void GetEquipment(Equipment equipment)
    {
        var weaponInHand = equipment.WeaponInHand;
        var sideArm = equipment.SideArm;
        var weaponInLeftHand = equipment.WeaponInLeftHand;

        if (isNonPlayer)
        {
            if (weaponInHand != null &&weaponInHand.HasEnemyAiWeaponData)
            {
                weaponInHand = weaponInHand.EnemyAiWeaponData;
            }
            if (sideArm != null && sideArm.HasEnemyAiWeaponData)
            {
                sideArm = sideArm.EnemyAiWeaponData;
            }
            if (weaponInLeftHand != null && weaponInLeftHand.HasEnemyAiWeaponData)
            {
                weaponInLeftHand = weaponInLeftHand.EnemyAiWeaponData;
            }
        }


        if (weaponInHand != null)
        {
            playerArms.RightArm.PickUpWeapon(
            SpawnWeapon(
                weaponInHand,
                equipment.MagazinsOfWeaponInHand));
        }

        if (equipment.WeaponInLeftHand != null)
        {
            playerArms.LeftArm.PickUpWeapon(
            SpawnWeapon(
                weaponInLeftHand,
                equipment.MagazinsOfWeaponInLeftHand));
        }





        if (equipment.SideArm != null)
        {
            playerInventory.Clear();

            playerInventory.AddWeapon(
            SpawnWeapon(
                sideArm,
                equipment.MagazinsOfSideArm));
        }

        if (equipment.Ability != null)
        {
            abilityInventory.AddAbility(equipment.Ability);
        }
        if (equipment.Ability2 != null)
        {

            abilityInventory.AddAbility(equipment.Ability2);
        }
        if (equipment.Ability3 != null)
        {
            abilityInventory.AddAbility(equipment.Ability3);
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

        health.SetHealthOverride(equipment.HealthOverride);

        playerArms.SetCanDualWield(equipment.CanDualWield);
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
        if (magazins != 0)
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

    [SerializeField] bool canDualWield = true;
    [SerializeField] bool canDualWieldEverything = false;
    [SerializeField] float movementSpeedMultiplier = 1;


    [SerializeField] Weapon_Data weaponInHand;
    [SerializeField] int magazinsOfWeaponInHand = 3;
    [SerializeField] Weapon_Data weaponInLeftHand;
    [SerializeField] int magazinsOfWeaponInLeftHand = 3;
    [SerializeField] Weapon_Data sideArm;
    [SerializeField] int magazinsOfSideArm = 5;

    [SerializeField] AbilityData ability;
    [SerializeField] AbilityData ability2;
    [SerializeField] AbilityData ability3;

    [SerializeField] HealthOverride healthOverride;


    public HealthOverride HealthOverride => healthOverride;

    public bool HasShild => hasShild;
    public bool HeadShotOneShot => headShotOneShot;

    public bool CanDualWield => canDualWield;

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

    public AbilityData Ability => ability;
    public AbilityData Ability2 => ability2;
    public AbilityData Ability3 => ability3;

    public bool HasMiniMap => hasMiniMap;

    public float MovementSpeedMultiplier => movementSpeedMultiplier;

    public bool CanDualWieldEverything => canDualWieldEverything;


    public Equipment(Equipment equipmentToCopy)
    {
        this.hasShild = equipmentToCopy.hasShild;
        this.headShotOneShot = equipmentToCopy.headShotOneShot;
        this.hasMiniMap = equipmentToCopy.hasMiniMap;
        this.canDualWield = equipmentToCopy.canDualWield;
        this.canDualWieldEverything = equipmentToCopy.canDualWieldEverything;
        this.movementSpeedMultiplier = equipmentToCopy.movementSpeedMultiplier;
        this.weaponInHand = equipmentToCopy.weaponInHand;
        this.magazinsOfWeaponInHand = equipmentToCopy.magazinsOfWeaponInHand;
        this.weaponInLeftHand = equipmentToCopy.weaponInLeftHand;
        this.magazinsOfWeaponInLeftHand = equipmentToCopy.magazinsOfWeaponInLeftHand;
        this.sideArm = equipmentToCopy.sideArm;
        this.magazinsOfSideArm = equipmentToCopy.magazinsOfSideArm;
        this.ability = equipmentToCopy.ability;
        this.ability2 = equipmentToCopy.ability2;
        this.ability3 = equipmentToCopy.ability3;
        this.healthOverride = equipmentToCopy.healthOverride;
    }


}

[Serializable]
public class HealthOverride
{
    public bool hasHealthOverride = false;
    public bool showHealthBar = false;
    public float health;
    public float shild;
    public float healthRegenStartTime = 0.5f;
    public float shildRegenStartTime = 0.5f;
    public float healthRegen;
    public float shildRegen;
    public float spawnInvulnerabilityTime = 5f; // time in seconds to be invulnerable after spawn

    public float shildPopDamageNegation = 30f;
}