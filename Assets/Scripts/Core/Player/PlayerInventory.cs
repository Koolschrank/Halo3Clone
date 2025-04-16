using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public Action<Weapon_Arms, int> OnWeaponAddedToInventory;
    public Action<Weapon_Arms> OnWeaponDrop;
    public Action<int> OnGranadeCountChanged;

    public Action<int> OnMaxGranadeCountChanged;
    public Action<float> OnGranadeChargeChanged;
    public Action OnMiniMapDisabled;
    public Action OnMiniMapEnabled;
    public Action<Weapon_Data,int> OnAmmoChanged;
    public Action<int> OnAmmoOfWeaponInInventoryChanged;





    Weapon_Arms weapon { get; set; }
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

    public bool HasWeapon => weapon != null;

    public Weapon_Arms FirstWeaponInInventory => weapon;


    public float GranadeCharge => 1 - (rechargeGranadeTimer / rechargeGranadeTime);
    // start
    public void Start()
    {
        characterHealth.OnDeath += DropWeapon;
        OnAmmoChanged += TryInvokeAmmoChangeOfInventoryWeapon;
    }

    public void Clear()
        { weapon = null; }

    public void DropWeapon()
    {
        if (weapon != null)
        {
            if (weapon == null || (weapon.Magazine == 0 && GetAmmo(weapon.Data) == 0))
            {
                return;
            }

            Weapon_PickUp pickUp = Instantiate(weapon.PickUpVersion, weaponDropPoint.position, weaponDropPoint.rotation);
            pickUp.SetAmmo(weapon.Magazine, TakeAllAmmo(weapon.Data));
            OnWeaponDrop?.Invoke(weapon);
            weapon.DropWeapon();
            weapon = null;


        }
    }

    public void Update()
    {
        if (weapon != null)
        {
            weapon.UpdateWeapon(Runner.DeltaTime);
        }

        if (rechargeGranades && granadeCount < granadeInventorySize)
        {
            if (rechargeGranadeTimer > 0)
            {
                rechargeGranadeTimer -= Time.deltaTime;
                OnGranadeChargeChanged?.Invoke(GranadeCharge);
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
            this.weapon = weapon;
            if (ammo.TryGetValue(weapon.Data, out int ammuntion))
            {
                OnWeaponAddedToInventory?.Invoke(weapon, ammo[weapon.Data] + weapon.Magazine);
            }
            else
            {
                OnWeaponAddedToInventory?.Invoke(weapon, weapon.Magazine);
            }

            

        }
    }

    public Weapon_Arms RemoveWeapon()
    {
        var weaponToReturn = weapon;
        weapon = null;
        return weaponToReturn;
    }


    // shoot be obsolete
    public Weapon_Arms SwitchWeapon(Weapon_Arms weapon)
    {
        var weaponToReturn = weapon;
        weapon = null;

        if (weapon != null)
        {
            AddWeapon(weapon);
        }
        return weaponToReturn;
    }

    public Weapon_Arms GetWeapon()
    {
        return weapon;
    }

    public bool Full => weapon != null;

    // add granade

    public void ChangeGranade(GranadeStats granade)
    {
        granadeStats = granade;
        if (granadeStats != null)
        {
            OnMaxGranadeCountChanged?.Invoke(granadeInventorySize);
        }
        else
        {
            OnMaxGranadeCountChanged?.Invoke(0);
        }
    }

    public int GranadeInventorySize => granadeStats == null ? 0 : granadeInventorySize;


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

    public void TryInvokeAmmoChangeOfInventoryWeapon(Weapon_Data weaponType, int amount)
    {
        Debug.Log("TryInvokeAmmoChangeOfInventoryWeapon");
        if (weapon != null && weapon.Data == weaponType)
        {
            Debug.Log("TryInvokeAmmoChangeOfInventoryWeapon 2");
            OnAmmoOfWeaponInInventoryChanged?.Invoke(amount + weapon.Magazine);
        }
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
        if (ammoInPickUpReserve > 0 || ammoInPickUpMagazine > 0)
        {
            pickUp.OnPickUp?.Invoke(pickUp);
            if (pickUp.WeaponType != WeaponType.oneHanded)
                Destroy(pickUp.gameObject);
        }
        OnAmmoChanged?.Invoke(pickUp.WeaponData, GetAmmo(pickUp.WeaponData));

    }




}



