using System;
using System.Collections;
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
    public Action<Weapon_Data,int> OnAmmoChanged;




    [SerializeField] int weaponInvetorySize = 1;
    List<Weapon_Arms> weapons = new List<Weapon_Arms>();
    // make an ammo dictionary weapondata -> ammo
    Dictionary<Weapon_Data,int> ammo = new Dictionary<Weapon_Data, int>();






    [SerializeField] GranadeStats granadeStats = null;
    [SerializeField] int granadeInventorySize = 1;
    [SerializeField] bool rechargeGranades = false;
    [SerializeField] float rechargeGranadeTime = 5;
    [SerializeField] Transform weaponDropPoint;
    [SerializeField] CharacterHealth characterHealth;
    float rechargeGranadeTimer;
    int granadeCount = 0;

    public bool HasWeapon => weapons.Count > 0;

    public Weapon_Arms FirstWeaponInInventory => weapons.Count > 0 ? weapons[0] : null;

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
            if (weapon == null || (weapon.Magazine == 0 && GetAmmo(weapon.Data) == 0))
            {
                return;
            }

            Weapon_PickUp pickUp = Instantiate(weapon.PickUpVersion, weaponDropPoint.position, weaponDropPoint.rotation);
            pickUp.SetAmmo(weapon.Magazine, TakeAllAmmo(weapon.Data));
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

    public void AddAmmo(Weapon_Data weaponType, int ammo)
    {
        if (!this.ammo.ContainsKey(weaponType))
        {
            this.ammo.Add(weaponType,Mathf.Min(weaponType.ReserveSize,  ammo));

        }
        else
        {
            this.ammo[weaponType] = Mathf.Min(this.ammo[weaponType] + ammo, weaponType.ReserveSize);
        }
        OnAmmoChanged?.Invoke(weaponType, this.ammo[weaponType]);
    }

    public int GetAmmo(Weapon_Data weaponType)
    {
        if (this.ammo.ContainsKey(weaponType))
        {
            return this.ammo[weaponType];
        }
        return 0;
    }

    public void RemoveAmmo(Weapon_Data weaponType, int ammo)
    {
        if (this.ammo.ContainsKey(weaponType))
        {
            this.ammo[weaponType] -= ammo;
            if (this.ammo[weaponType] <= 0)
            {
                this.ammo.Remove(weaponType);
            }
            OnAmmoChanged?.Invoke(weaponType, this.ammo[weaponType]);
        }
    }

    public void RemoveAllAmmoOfWeapon(Weapon_Data weapon)
    {
        if (this.ammo.ContainsKey(weapon))
        {
            this.ammo.Remove(weapon);
            OnAmmoChanged?.Invoke(weapon, 0);
        }
    }

    public int TakeAllAmmo(Weapon_Data weaponType)
    {
        if (this.ammo.ContainsKey(weaponType))
        {
            int ammo = this.ammo[weaponType];
            this.ammo.Remove(weaponType);
            OnAmmoChanged?.Invoke(weaponType, 0);
            return ammo;
        }
        return 0;
    }

    public int TakeAmmo(Weapon_Data weaponType, int desiredAmount)
    {
        if (this.ammo.ContainsKey(weaponType))
        {
            int ammo = this.ammo[weaponType];
            if (ammo >= desiredAmount)
            {
                this.ammo[weaponType] -= desiredAmount;
                OnAmmoChanged?.Invoke(weaponType, this.ammo[weaponType]);
                return desiredAmount;
            }
            else
            {
                this.ammo.Remove(weaponType);
                OnAmmoChanged?.Invoke(weaponType, 0);
                return ammo;
            }
        }
        return 0;

    }

    public bool HasAmmo(Weapon_Data weaponType)
    {
        return this.ammo.ContainsKey(weaponType) && this.ammo[weaponType] != 0;
    }

    public int TakeMagazin(Weapon_Data weaponType)
    {
        if (ammo.ContainsKey(weaponType))
        {
            int ammoInInventory = this.ammo[weaponType];
            int ammoToTake = Mathf.Min(ammoInInventory, weaponType.MagazineSize);
            this.ammo[weaponType] -= ammoToTake;
            if (this.ammo[weaponType] <= 0)
            {
                this.ammo.Remove(weaponType);
            }
            OnAmmoChanged?.Invoke(weaponType, this.ammo[weaponType]);
            return ammoToTake;

        }
        return 0;
    }


    public void TransferAmmoFromPickUp(Weapon_PickUp pickUp)
    {
        int maxAmmo = pickUp.WeaponData.ReserveSize;
        int ammoOfWeaponType = GetAmmo(pickUp.WeaponData);
        int missingAmmo = maxAmmo - ammoOfWeaponType;
        int ammoInPickUpReserve = pickUp.AmmoInReserve;
        int ammoInPickUpMagazine = pickUp.AmmoInMagazine;

        if (missingAmmo > 0)
        {
            // first use up ammo from pick reserve then from magazine
            if (ammoInPickUpReserve > missingAmmo)
            {
                AddAmmo(pickUp.WeaponData, missingAmmo);
                ammoInPickUpReserve -= missingAmmo;
                missingAmmo = 0;
                pickUp.SetAmmo(ammoInPickUpMagazine, ammoInPickUpReserve);
            }
            else
            {
                AddAmmo(pickUp.WeaponData, ammoInPickUpReserve);
                missingAmmo -= ammoInPickUpReserve;
                ammoInPickUpReserve = 0;
                pickUp.SetAmmo(ammoInPickUpMagazine, ammoInPickUpReserve);
                if (missingAmmo > 0)
                {
                    if (ammoInPickUpMagazine > missingAmmo)
                    {
                        AddAmmo(pickUp.WeaponData, missingAmmo);
                        ammoInPickUpMagazine -= missingAmmo;
                        missingAmmo = 0;
                        pickUp.SetAmmo(ammoInPickUpMagazine, ammoInPickUpReserve);
                    }
                    else
                    {
                        AddAmmo(pickUp.WeaponData, ammoInPickUpMagazine);
                        missingAmmo -= ammoInPickUpMagazine;
                        ammoInPickUpMagazine = 0;
                        pickUp.SetAmmo(ammoInPickUpMagazine, ammoInPickUpReserve);
                    }
                }
            }

        }
        OnAmmoChanged?.Invoke(pickUp.WeaponData, GetAmmo(pickUp.WeaponData));

    }




}



