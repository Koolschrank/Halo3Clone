using Fusion;
using UnityEngine;

public class WeaponInventory : NetworkBehaviour
{
    
    [Networked] public WeaponNetworkStruct RightWeapon { get; private set; } = new WeaponNetworkStruct(){weaponTypeIndex = -1};
    [Networked] public WeaponNetworkStruct LeftWeapon { get; private set; } = new WeaponNetworkStruct() { weaponTypeIndex = -1 };

    [Networked] public WeaponNetworkStruct BackWeapon { get; private set; } = new WeaponNetworkStruct() { weaponTypeIndex = -1 };
    
    [Networked, Capacity(30)] public NetworkArray<int> AmmoReserve { get; }

    public bool IsDualWielding => LeftWeapon.weaponTypeIndex != -1 && RightWeapon.weaponTypeIndex != -1;

    public override void Spawned()
    {
        
    }

    public virtual void Equip_RightWeapon(WeaponNetworkStruct newWeapon)
    {
        if (RightWeapon.weaponTypeIndex != -1)
        {
            Remove_RightWeapon();
        }

        if (newWeapon.weaponTypeIndex != -1)
        {
            SetAmmo(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
            
        }
        
        newWeapon.ammoInReserve = 0;
        SetRightWeapon(newWeapon);
    }

    public virtual void Equip_LeftWeapon(WeaponNetworkStruct newWeapon)
    {
        

        if (LeftWeapon.weaponTypeIndex != -1)
        {
            Remove_LeftWeapon();
        }

        if (newWeapon.weaponTypeIndex != -1)
        {
            SetAmmo(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
        }
        
        newWeapon.ammoInReserve = 0;
        SetLeftWeapon(newWeapon);
    }

    public virtual void Equip_BackWeapon(WeaponNetworkStruct newWeapon)
    {
        if (BackWeapon.weaponTypeIndex != -1)
        {
            Remove_BackWeapon();
        }
        if (newWeapon.weaponTypeIndex != -1)
        {
            SetAmmo(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
        }
        newWeapon.ammoInReserve = 0;
        SetBackWeapon(newWeapon);
    }

    public virtual WeaponNetworkStruct Remove_RightWeapon()
    {
        // remove the weapon from the right hand
        var weaponToRemove = RightWeapon;
        RightWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };

        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (weaponToRemove.weaponTypeIndex != -1
            && BackWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex 
            && LeftWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex )
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            SetAmmo(weaponToRemove.weaponTypeIndex, 0);
        }
        return weaponToRemove;
    }

    public virtual WeaponNetworkStruct Remove_LeftWeapon()
    {
        // remove the weapon from the left hand
        var weaponToRemove = LeftWeapon;
        LeftWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };
        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (weaponToRemove.weaponTypeIndex != -1
            && BackWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex
            && RightWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex)
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            SetAmmo(weaponToRemove.weaponTypeIndex, 0);
        }
        return weaponToRemove;
    }

    public virtual WeaponNetworkStruct Remove_BackWeapon()
    {
        // remove the weapon from the back
        var weaponToRemove = BackWeapon;
        BackWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };
        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (weaponToRemove.weaponTypeIndex != -1
            && LeftWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex
            && RightWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex)
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            SetAmmo(weaponToRemove.weaponTypeIndex, 0);
        }
        return weaponToRemove;
    }

    public void Switch_RightWithBackWeapon()
    {
        var rightWeapon = Remove_RightWeapon();
        var backWeapon = Remove_BackWeapon();

        Equip_RightWeapon(backWeapon);
        Equip_BackWeapon(rightWeapon);
    }

    public void Switch_LeftWithBackWeapon()
    {
        var leftWeapon = Remove_LeftWeapon();
        var backWeapon = Remove_BackWeapon();

        Equip_LeftWeapon(backWeapon);
        Equip_BackWeapon(leftWeapon);
    }

    

    

    

    

    public void TransferReserveAmmo_RightWeapon(int ammoToTransfer)
    {
        var rightWeaponTemp = RightWeapon;
        int ammoInReserve = AmmoReserve[RightWeapon.weaponTypeIndex];
        if (ammoInReserve >= ammoToTransfer)
        {
            SetAmmo(RightWeapon.weaponTypeIndex, ammoInReserve - ammoToTransfer);
            rightWeaponTemp.ammoInMagazine += ammoToTransfer;
            SetRightWeapon(rightWeaponTemp);
        }
        else
        {
            SetAmmo(RightWeapon.weaponTypeIndex, 0);
            rightWeaponTemp.ammoInMagazine += ammoInReserve;
            SetRightWeapon(rightWeaponTemp);
        }
    }

    public void TransferReserveAmmo_LeftWeapon(int ammoToTransfer)
    {
        var leftWeaponTemp = LeftWeapon;
        int ammoInReserve = AmmoReserve[LeftWeapon.weaponTypeIndex];
        if (ammoInReserve >= ammoToTransfer)
        {
            SetAmmo(LeftWeapon.weaponTypeIndex, ammoInReserve - ammoToTransfer);
            leftWeaponTemp.ammoInMagazine += ammoToTransfer;
            SetLeftWeapon(leftWeaponTemp);
        }
        else
        {
            SetAmmo(LeftWeapon.weaponTypeIndex, 0);
            leftWeaponTemp.ammoInMagazine += ammoInReserve;
            SetLeftWeapon(leftWeaponTemp);
        }
    }

    public virtual void SetAmmo(int index, int amount)
    {
        if (index < 0)
        {
            Debug.LogError("Invalid ammo index: " + index);
            return;
        }
        AmmoReserve.Set(index, amount);
    }

    public virtual void SetRightWeapon(WeaponNetworkStruct weapon)
    {
        RightWeapon = weapon;
    }

    public virtual void SetLeftWeapon(WeaponNetworkStruct weapon)
    {
        LeftWeapon = weapon;
    }

    public virtual void SetBackWeapon(WeaponNetworkStruct weapon)
    {
        BackWeapon = weapon;
    }

    public void ReduceLeftWeaponMagazin(int count)
    {
        var leftWeaponTemp = LeftWeapon;
        leftWeaponTemp.ammoInMagazine = Mathf.Max(0, leftWeaponTemp.ammoInMagazine - count);
        SetLeftWeapon(leftWeaponTemp);
    }

    public void ReduceRightWeaponMagazin(int count)
    {
        var rightWeaponTemp = RightWeapon;
        rightWeaponTemp.ammoInMagazine = Mathf.Max(0, rightWeaponTemp.ammoInMagazine - count);
        SetRightWeapon(rightWeaponTemp);
    }

    public void ReduceBackWeaponMagazin(int count)
    {
        var backWeaponTemp = BackWeapon;
        backWeaponTemp.ammoInMagazine = Mathf.Max(0, backWeaponTemp.ammoInMagazine - count);
        SetBackWeapon(backWeaponTemp);
    }


}
