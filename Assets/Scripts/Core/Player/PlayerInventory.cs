using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Action<Weapon_Arms> OnWeaponAddedToInventory;
    public Action<Weapon_Arms> OnWeaponDrop;
    public Action<int> OnGranadeCountChanged;
    public Action<float> OnGranadeChargeChanged;
    public Action OnMiniMapDisabled;
    public Action OnMiniMapEnabled;


    [SerializeField] int weaponInvetorySize = 1;
    List<Weapon_Arms> weapons = new List<Weapon_Arms>();

    [SerializeField] GranadeStats granadeStats = null;
    [SerializeField] int granadeInventorySize = 1;
    [SerializeField] bool rechargeGranades = false;
    [SerializeField] float rechargeGranadeTime = 5;
    [SerializeField] Transform weaponDropPoint;
    [SerializeField] CharacterHealth characterHealth;
    float rechargeGranadeTimer;
    int granadeCount = 0;

    public bool HasWeapon => weapons.Count > 0;

    // start
    public void Start()
    {
        characterHealth.OnDeath += DropWeapon;
    }

    public void Clear()
        { weapons.Clear(); }

    public void DropWeapon()
    {
        if (weapons.Count > 0)
        {
            var weapon = weapons[0];
            if (weapon == null || (weapon.Magazine == 0 && weapon.Reserve ==0))
            {
                return;
            }

            Weapon_PickUp pickUp = Instantiate(weapon.PickUpVersion, weaponDropPoint.position, weaponDropPoint.rotation);
            pickUp.SetAmmo(weapon.Magazine, weapon.Reserve);
            OnWeaponDrop?.Invoke(weapons[0]);
            weapons.RemoveAt(0);
            
        }
    }

    public void Update()
    {
        foreach (var weapon in weapons)
        {
            weapon.UpdateWeapon();
        }

        if (rechargeGranades && granadeCount < granadeInventorySize)
        {
            if (rechargeGranadeTimer > 0)
            {
                rechargeGranadeTimer -= Time.deltaTime;
                OnGranadeChargeChanged?.Invoke(1-(rechargeGranadeTimer / rechargeGranadeTime));
            }
            else
            {
                AddGranade();
                rechargeGranadeTimer = rechargeGranadeTime;
                OnGranadeChargeChanged?.Invoke(1);

            }
        }
    }

    public void AddWeapon(Weapon_Arms weapon)
    {
        if (!Full)
        {
            weapons.Add(weapon);
            OnWeaponAddedToInventory?.Invoke(weapon);

        }
    }

    public Weapon_Arms RemoveWeapon()
    {
        if (weapons.Count > 0)
        {
            var weapon = weapons[0];
            weapons.RemoveAt(0);
            return weapon;
        }
        return null;
    }


    // shoot be obsolete
    public Weapon_Arms SwitchWeapon(Weapon_Arms weapon)
    {
        var weaponToReturn = weapons[0];
        weapons.RemoveAt(0);

        if (weapon != null)
        {
            AddWeapon(weapon);
        }
        return weaponToReturn;
    }

    public Weapon_Arms GetWeapon()
    {
        if (weapons.Count > 0)
        {
            return weapons[0];
        }
        return null;
    }

    public bool Full => weapons.Count >= weaponInvetorySize;

    // add granade

    public void ChangeGranade(GranadeStats granade)
    {
        granadeStats = granade;
    }
    public void AddGranades(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddGranade();
        }
    }

    public void AddGranade()
    {
        if (granadeCount < granadeInventorySize)
        {
            granadeCount++;
            OnGranadeCountChanged?.Invoke(granadeCount);
            if (granadeCount == granadeInventorySize)
            {
                OnGranadeChargeChanged?.Invoke(1);
            }
        }
    }

    // has granade
    public bool HasGranades => granadeStats != null && granadeCount > 0;

    // reduce granade
    public void UseGranade()
    {
        if (granadeCount > 0)
        {
            granadeCount--;
            OnGranadeCountChanged?.Invoke(granadeCount);

            if (rechargeGranades)
            {
                rechargeGranadeTimer = rechargeGranadeTime;
                OnGranadeChargeChanged?.Invoke(0);
            }
        }
    }

    public GranadeStats GranadeStats => granadeStats;





}

