using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PickUp : MonoBehaviour
{
    public Action<Weapon_PickUp> OnPickUp;

    [SerializeField] Rigidbody rb;
    [SerializeField] Weapon_Data weapon_Data;
    [SerializeField] GameObject destroyParticle;
    [SerializeField] GameObject weaponDeleteParticle;
    [SerializeField] int ammoInMagazine = 0;
    [SerializeField] int ammoInReserve = 0;

    [SerializeField] float deleteTime = 30f;
    [SerializeField] float fastDeletTime = 7f;
    bool inDeleteState = false;
    float deleteTimer = 0f;

    List<int> teamsBlockedFromPickUpThis = new List<int>();
    bool pickedUp = false;


    public void EnterDeleteTime()
    {
        if (inDeleteState) return;
        inDeleteState = true;

        if (deleteTimer == 0)
            deleteTimer = deleteTime;

    }

    public void EnterFastDeleteTime()
    {
        inDeleteState = true;
        deleteTimer = fastDeletTime;
        destroyParticle.SetActive(true);
    }

    private void Update()
    {
        if (inDeleteState)
        {
            if (deleteTimer > fastDeletTime && deleteTimer - Time.deltaTime <= fastDeletTime)
            {
                EnterFastDeleteTime();
            }

            deleteTimer -= Time.deltaTime;
            if (deleteTimer <= 0)
            {
                OnPickUp?.Invoke(this);

                Instantiate(weaponDeleteParticle, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }





    public Weapon_Arms PickUp()
    {
        if (pickedUp) return null;
        var weapon = new Weapon_Arms(weapon_Data, ammoInMagazine);
        pickedUp = true;
        OnPickUp?.Invoke(this);
        Destroy(gameObject, 0.01f); // Destroy the pickup object after 0.01 seconds to avoid multiple pickups
        return weapon;

    }

    public Weapon_Data WeaponData => weapon_Data;

    public string WeaponName => weapon_Data.WeaponName;

    public void SetAmmo(int magazine, int reserve)
    {
        ammoInMagazine = Mathf.Min(weapon_Data.MagazineSize, magazine);
        ammoInReserve = Mathf.Min(weapon_Data.ReserveSize, reserve);


        if (ammoInReserve == 0 && ammoInMagazine == 0)
        {
            EnterFastDeleteTime();
        }
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


