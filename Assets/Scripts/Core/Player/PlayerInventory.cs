using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public Action<int, int> OnAmmoChanged;
    public Action<int> OnInventoryWeaponAmmoChanged;
    public Action<int> OnWeaponAddedToInventory;
    public Action<int> OnWeaponRemovedFromInventory;


    [SerializeField] PlayerArms playerArms;

    [Networked] public int WeaponInInventory { get; private set; } = -1;

    [Networked] public int AmmoInInventoryMagazine { get; private set; } = 0;
    [Networked, Capacity(30)] public NetworkArray<int> AmmoReserve { get; }

    [Networked] public int AbilityInInventory { get; private set; } = -1;
    [Networked] public int AbilityUses { get; private set; } = 0;

    public bool HasWeaponInInventory => WeaponInInventory != -1;

    public void SetWeaponInInventory(WeaponNetworkStruct weapon)
    {
        if (WeaponInInventory != -1)
        {
            RemoveWeapon();
        }


        WeaponInInventory = weapon.weaponTypeIndex;
        AmmoInInventoryMagazine = weapon.ammoInMagazine;
        OnWeaponAddedToInventory?.Invoke(weapon.weaponTypeIndex);
    }

    public WeaponNetworkStruct GetWeaponInInventory()
    {
        return new WeaponNetworkStruct()
        {
            weaponTypeIndex = WeaponInInventory,
            ammoInMagazine = AmmoInInventoryMagazine,
            ammoInReserve = AmmoReserve[WeaponInInventory]
        };
    }

    public WeaponNetworkStruct RemoveWeapon()
    {
        if (WeaponInInventory == -1)
        {
            return new WeaponNetworkStruct()
            {
                weaponTypeIndex = -1,
                ammoInMagazine = 0,
                ammoInReserve = 0
            };
             
        }
            

        var weapon = GetWeaponInInventory();
        WeaponInInventory = -1;
        if (true) //(!playerArms.HasWeaponOfTypeInHand(weapon))
        {
            SetAmmoInReserve(weapon.weaponTypeIndex, 0);
        }
        OnWeaponRemovedFromInventory?.Invoke(weapon.weaponTypeIndex);
        return weapon;
    }

    public void SetAmmoInReserve(int weaponID, int ammo)
    {
        if (weaponID < 0|| ammo < 0)
        {
            return;
        }

        AmmoReserve.Set(weaponID, ammo);
        OnAmmoChanged?.Invoke(weaponID, ammo);

        if (weaponID == WeaponInInventory)
            OnInventoryWeaponAmmoChanged?.Invoke(ammo + AmmoInInventoryMagazine);
    }



    public int TakeAmmoFromReserve(int weaponID, int ammoToTake)
    {
        int ammoInReserve = AmmoReserve[weaponID];
        if (ammoInReserve >= ammoToTake)
        {
            SetAmmoInReserve(weaponID, ammoInReserve - ammoToTake);
            return ammoToTake;
        }
        else
        {
            SetAmmoInReserve(weaponID, 0);
            return ammoInReserve;
        }
    }

    public int GetAmmoInReserve(int weaponID)
    {
        return AmmoReserve[weaponID];
    }

    public int TakeAllAmmo(int weaponID)
    {
        int ammoInReserve = AmmoReserve[weaponID];
        SetAmmoInReserve(weaponID, 0);
        return ammoInReserve;
    }


    public bool HasAmmoInReserve(int weaponID)
    {
        return AmmoReserve[weaponID] > 0;
    }










    //public Action<Weapon_Arms, int> OnWeaponAddedToInventoryOld;
    //public Action<Weapon_Arms> OnWeaponDrop;
    public Action<int> OnGranadeCountChanged;

    public Action<int> OnMaxGranadeCountChanged;
    public Action<float> OnGranadeChargeChanged;
    public Action OnMiniMapDisabled;
    public Action OnMiniMapEnabled;
    //public Action<Weapon_Data,int> OnAmmoChangedOld;
    //public Action<int> OnAmmoOfWeaponInInventoryChanged;





    Weapon_Arms weapon { get; set; }
    // make an ammo dictionary weapondata -> ammo
    

    //Dictionary<Weapon_Data,int> ammo = new Dictionary<Weapon_Data, int>();






    [SerializeField] GranadeStats granadeStats = null;
    [SerializeField] int granadeInventorySize = 1;
    [SerializeField] bool rechargeGranades = false;
    [SerializeField] float rechargeGranadeTime = 5;
    [SerializeField] Transform weaponDropPoint;
    [SerializeField] CharacterHealth characterHealth;
    float rechargeGranadeTimer;
    int granadeCount = 0;

   

    public Weapon_Arms FirstWeaponInInventory => weapon;


    public float GranadeCharge => 1 - (rechargeGranadeTimer / rechargeGranadeTime);
    // start
    public void Start()
    {
        characterHealth.OnDeath += DropWeapon;
        //OnAmmoChangedOld += TryInvokeAmmoChangeOfInventoryWeapon;
    }


    public void DropWeapon()
    {
    //    if (weapon != null)
    //    {
    //        if (weapon == null || (weapon.Magazine == 0 && GetAmmo(weapon.Data) == 0))
    //        {
    //            return;
    //        }

    //        Weapon_PickUp pickUp = Instantiate(weapon.PickUpVersion, weaponDropPoint.position, weaponDropPoint.rotation);
    //        pickUp.SetAmmo(weapon.Magazine, TakeAllAmmo(weapon.Data));
    //        OnWeaponDrop?.Invoke(weapon);
    //        weapon.DropWeapon();
    //        weapon = null;


    //    }
    }

    public void Update()
    {
        /*
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
        }*/
    }




    // shoot be obsolete


  

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

















}



