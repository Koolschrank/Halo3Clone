using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPickUpScan : MonoBehaviour
{


    [SerializeField]List<Weapon_PickUp> pickUpsInRange = new List<Weapon_PickUp>();
    [SerializeField] float pickUpCooldown = 0.5f;
    float lastPickUpTime = -100f;

    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] PlayerTeam playerTeam;

    public Action<Weapon_PickUp> OnWeaponPickUpUpdate;
    public Action<Weapon_PickUp> OnWeaponDualWieldUpdate;
    public Action OnWeaponPickUp;


    // todo: this is not a good bug fix, need to find the root cause
    private void Update()
    {
        // check every 10 onweaponPickupUpdate
        if (Time.frameCount % 10 ==0)
        {
            TrySendUpdates();
        }
    }

    public bool CanBeDualWielded(Weapon_PickUp pickUp)
    {
        var weaponTypeOfWeaponInHand = playerArms.RightArm.GetWeaponInHand().WeaponType;
        bool isDualWieldable = weaponTypeOfWeaponInHand == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons;
        var weaponTypeOfWeaponOnGround = pickUp.WeaponType;
        bool isDualWieldableOnGround = weaponTypeOfWeaponOnGround == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons;
        return isDualWieldable && isDualWieldableOnGround;
    }

    public void TrySendUpdates()
    {
        if (Time.time - lastPickUpTime > pickUpCooldown)
        {
            var closesWeapon = GetClosesPickUp();
            OnWeaponPickUpUpdate?.Invoke(closesWeapon);

            var weaponInRightHand = playerArms.RightArm.GetWeaponInHand();
            if (weaponInRightHand != null && closesWeapon != null)
            {
                if (CanBeDualWielded(closesWeapon))
                {
                    OnWeaponDualWieldUpdate?.Invoke(closesWeapon);
                    return;
                }



            }

            OnWeaponDualWieldUpdate?.Invoke(null);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickUp = other.GetComponent<Weapon_PickUp>();
        if (pickUp != null)
        {
            if (CheckIfPlayerOwnsWeapon(pickUp))
            {
                TransferAmmoFromWeaponOnGroundToPlayer(pickUp);

                if (pickUp != null )
                {
                    pickUpsInRange.Add(pickUp);
                    TrySendUpdates();
                }
            }
            else
            {
                if (!pickUp.CanBePickedUpByTeam(playerTeam.TeamIndex))
                    return;

                pickUpsInRange.Add(pickUp);
                TrySendUpdates();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pickUp = other.GetComponent<Weapon_PickUp>();
        if (pickUp != null)
        {
            pickUpsInRange.Remove(pickUp);
            TrySendUpdates();
        }
    }

    public bool CheckIfPlayerOwnsWeapon(Weapon_PickUp pickup)
    {
        var weaponData = pickup.WeaponData;

        var weaponInHand = playerArms.RightArm.GetWeaponInHand();
        if (weaponInHand != null && weaponInHand.IsSameWeapon(weaponData))
            return true;
        var weaponInLeftHand = playerArms.LeftArm.GetWeaponInHand();
        if (weaponInLeftHand != null && weaponInLeftHand.IsSameWeapon(weaponData))
            return true;
        var weaponInInventory = playerInventory.WeaponInInventory;
        if (weaponInInventory != -1 && weaponInInventory == weaponData.WeaponIndex)
            return true;
        return false;

    }

    public void TransferAmmoFromWeaponOnGroundToPlayer(Weapon_PickUp pickUp)
    {
        /*if (playerArms.HasWeapon(pickUp.WeaponData))
        {
            playerInventory.TransferAmmoFromPickUp(pickUp);


        }*/

        
    }
    public bool CanPickUpWeapon()
    {
        return pickUpsInRange.Count > 0;
    }

    

    public Weapon_PickUp GetClosesPickUp()
    {
        if (pickUpsInRange.Count <= 0)
            return null;
        RemovesNulls();
        SortPriority();
        if (pickUpsInRange.Count <= 0)
            return null;
        return pickUpsInRange[0];
    }

    public bool IsClosesPickUpOneHanded()
    {
        var pickUp = GetClosesPickUp();
        if (pickUp == null)
            return false;
        return pickUp.WeaponType == WeaponType.oneHanded;
    }

    public WeaponNetworkStruct PickUpWeapon()
    {
        var pickUp = GetClosesPickUp();
        pickUpsInRange.Remove(pickUp);
        TrySendUpdates();
        lastPickUpTime = Time.time;
        if (pickUp == null)
            return new WeaponNetworkStruct()
            {
                weaponIndex = -1,
            };

        

        return pickUp.PickUp();
        //playerInventory.AddAmmoOld(pickUp.WeaponData, pickUp.AmmoInReserve);
    }


    public void RemovesNulls()
    {
        pickUpsInRange.RemoveAll(x => x == null);
    }

    // sort based on which one is closest to the player, all 3 vectors
    public void SortPriority()
    {
        pickUpsInRange.Sort((x, y) => Vector3.Distance(x.transform.position, transform.position).CompareTo(Vector3.Distance(y.transform.position, transform.position)));
    }
}
