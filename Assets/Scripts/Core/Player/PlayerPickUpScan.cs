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

    public Action<Weapon_PickUp> OnWeaponPickUpUpdate;
    public Action OnWeaponPickUp;


    // todo: this is not a good bug fix, need to find the root cause
    private void Update()
    {
        // check every 10 onweaponPickupUpdate
        if (Time.frameCount % 10 ==0)
        {
            TrySendPickUpUpdate();
        }
    }

    public void TrySendPickUpUpdate()
    {
        if (Time.time - lastPickUpTime > pickUpCooldown)
        {
            OnWeaponPickUpUpdate?.Invoke(GetClosesPickUp());
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
            }
            else
            {
                pickUpsInRange.Add(pickUp);
                TrySendPickUpUpdate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pickUp = other.GetComponent<Weapon_PickUp>();
        if (pickUp != null)
        {
            pickUpsInRange.Remove(pickUp);
            TrySendPickUpUpdate();
        }
    }

    public bool CheckIfPlayerOwnsWeapon(Weapon_PickUp pickup)
    {
        var weaponData = pickup.WeaponData;

        var weaponInHand = playerArms.GetWeaponInHand();
        if (weaponInHand != null && weaponInHand.IsSameWeapon(weaponData))
            return true;
        var weaponInInventory = playerInventory.GetWeapon();
        if (weaponInInventory != null && weaponInInventory.IsSameWeapon(weaponData))
            return true;
        return false;

    }

    public void TransferAmmoFromWeaponOnGroundToPlayer(Weapon_PickUp pickUp)
    {
        var weaponInHand = playerArms.GetWeaponInHand();
        if (weaponInHand != null && weaponInHand.IsSameWeapon(pickUp.WeaponData))
        {
            weaponInHand.TransferAmmo(pickUp);
        }
        else
        {
            var weaponInInventory = playerInventory.GetWeapon();
            if (weaponInInventory != null && weaponInInventory.IsSameWeapon(pickUp.WeaponData))
            {
                weaponInInventory.TransferAmmo(pickUp);
            }
        }
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

    public Weapon_Arms PickUpWeapon()
    {
        var pickUp = GetClosesPickUp();
        pickUpsInRange.Remove(pickUp);
        TrySendPickUpUpdate();
        lastPickUpTime = Time.time;
        if (pickUp == null)
            return null;
        return pickUp.PickUp();
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
