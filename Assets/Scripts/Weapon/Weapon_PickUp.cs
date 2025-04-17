using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;
using Fusion;

public class Weapon_PickUp : NetworkBehaviour
{
    public Action<Weapon_PickUp> OnPickUp;

    [SerializeField] Rigidbody rb;
    [SerializeField] Weapon_Data weapon_Data;
    [SerializeField] int ammoInMagazine = 0;
    [SerializeField] int ammoInReserve = 0;

    List<int> teamsBlockedFromPickUpThis = new List<int>();
    bool pickedUp = false;


    public Weapon_Arms PickUp()
    {
        if (pickedUp) return null;
        var weapon = new Weapon_Arms(weapon_Data, ammoInMagazine);
        pickedUp = true;
        OnPickUp?.Invoke(this);
        StartCoroutine(DestroyAfter(0.01f)); // Destroy the pickup object after 0.01 seconds to avoid multiple pickups
        
        return weapon;

    }

    public IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (HasStateAuthority)
            Runner.Despawn(Object);
    }

    public Weapon_Data WeaponData => weapon_Data;

    public string WeaponName => weapon_Data.WeaponName;

    public void SetAmmo(int magazine, int reserve)
    {
        ammoInMagazine = Mathf.Min(weapon_Data.MagazineSize, magazine);
        ammoInReserve = Mathf.Min(weapon_Data.ReserveSize, reserve);
    }

    public void AddImpulse(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public int AmmoInMagazine => ammoInMagazine;

    public int AmmoInReserve => ammoInReserve;

    public void SetAmmoInMagazin(int ammo)
    {
        ammoInMagazine = ammo;
    }

    public void SetAmmoInReserve(int ammo)
    {
        ammoInReserve = ammo;
    }

    public void SetAmmoWithMagazines(int magazines)
    {
        ammoInMagazine = weapon_Data.MagazineSize;
        ammoInReserve = weapon_Data.MagazineSize * (magazines -1);
    }

    // destroy object
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public WeaponType WeaponType => weapon_Data.WeaponType;

    public void BlockPickUpForTeam(int teamIndex)
    {
        teamsBlockedFromPickUpThis.Add(teamIndex);
    }

    public void UnblockPickUpForTeam(int teamIndex)
    {
        teamsBlockedFromPickUpThis.Remove(teamIndex);
    }

    public bool CanBePickedUpByTeam(int teamIndex)
    {
        return !teamsBlockedFromPickUpThis.Contains(teamIndex);
    }

    public Sprite GunSpriteUI => weapon_Data.GunSpriteUI;

}


