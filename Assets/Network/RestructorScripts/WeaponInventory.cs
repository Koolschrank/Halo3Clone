using Fusion;
using UnityEngine;

public class WeaponInventory : NetworkBehaviour
{
    [Networked] public WeaponNetworkStruct RightWeapon { get; private set; }
    [Networked] public WeaponNetworkStruct LeftWeapon { get; private set; }
    [Networked] public WeaponNetworkStruct BackWeapon { get; private set; }
    [Networked, Capacity(30)] NetworkArray<int> AmmoReserve { get; }

    public void Equip_RightWeapon(WeaponNetworkStruct newWeapon)
    {
        if (RightWeapon.weaponTypeIndex != -1)
        {
            Remove_RightWeapon();
        }

        AmmoReserve.Set(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
        newWeapon.ammoInReserve = 0;
        RightWeapon = newWeapon;
    }

    public void Equip_LeftWeapon(WeaponNetworkStruct newWeapon)
    {
        if (LeftWeapon.weaponTypeIndex != -1)
        {
            Remove_LeftWeapon();
        }

        AmmoReserve.Set(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
        newWeapon.ammoInReserve = 0;
        LeftWeapon = newWeapon;
    }

    public void Equip_BackWeapon(WeaponNetworkStruct newWeapon)
    {
        if (BackWeapon.weaponTypeIndex != -1)
        {
            Remove_BackWeapon();
        }

        AmmoReserve.Set(newWeapon.weaponTypeIndex, AmmoReserve[newWeapon.weaponTypeIndex] + newWeapon.ammoInReserve);
        newWeapon.ammoInReserve = 0;
        BackWeapon = newWeapon;
    }

    public WeaponNetworkStruct Remove_RightWeapon()
    {
        // remove the weapon from the right hand
        var weaponToRemove = RightWeapon;
        RightWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };

        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (BackWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex 
            && LeftWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex )
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            AmmoReserve.Set(weaponToRemove.weaponTypeIndex, 0);
        }
        return weaponToRemove;
    }

    public WeaponNetworkStruct Remove_LeftWeapon()
    {
        // remove the weapon from the left hand
        var weaponToRemove = LeftWeapon;
        LeftWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };
        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (BackWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex
            && RightWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex)
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            AmmoReserve.Set(weaponToRemove.weaponTypeIndex, 0);
        }
        return weaponToRemove;
    }

    public WeaponNetworkStruct Remove_BackWeapon()
    {
        // remove the weapon from the back
        var weaponToRemove = BackWeapon;
        BackWeapon = new WeaponNetworkStruct()
        {
            weaponTypeIndex = -1,
        };
        // remove ammo from reserve and add it to the weapon if this was the only weapon of this type
        if (LeftWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex
            && RightWeapon.weaponTypeIndex != weaponToRemove.weaponTypeIndex)
        {
            weaponToRemove.ammoInReserve = AmmoReserve[weaponToRemove.weaponTypeIndex];
            AmmoReserve.Set(weaponToRemove.weaponTypeIndex, 0);
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


    public void Drop_RightWeapon()
    {
        // remove the weapon from the right hand
        var weaponToDrop = Remove_RightWeapon();
        DropWeapon(weaponToDrop);
    }

    public void Drop_LeftWeapon()
    {
        // remove the weapon from the left hand
        var weaponToDrop = Remove_LeftWeapon();
        DropWeapon(weaponToDrop);
    }

    public void Drop_BackWeapon()
    {
        // remove the weapon from the back
        var weaponToDrop = Remove_BackWeapon();
        DropWeapon(weaponToDrop);
    }

    public void DropWeapon(WeaponNetworkStruct weapon)
    {

    }
}
