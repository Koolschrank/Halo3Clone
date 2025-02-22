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

    public Action<Weapon_PickUp> OnWeaponPickUpUpdate;
    public Action OnWeaponPickUp;


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
            pickUpsInRange.Add(pickUp);
            TrySendPickUpUpdate();


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
