using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZakhanSpellsPack;

public class Weapon_Model : MonoBehaviour
{

	public Action OnStartManuelAnimations;
	protected Weapon_Arms weapon;
    [SerializeField] public Transform bulletSpawnPoint;
    [SerializeField] protected GameObject muzzleFlash;
    [SerializeField] int weaponAnimationIndex; // 0 rifle, 1 pistol, 2 Shild 

    public bool IsShild => weaponAnimationIndex == 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject chargeObject;


    [SerializeField] bool activeParticalWhileShooting = false;

	public bool ManualAnimationControl = false;

	public Action OnShootAction;
    public Action OnShootStopAction;
    public Action OnChargeStartAction;
    public Action OnChargeEndAction;
    public Action OnMeleeAttack;
    public Action<bool> OnZoomUpdate;

	public virtual void SetUp(Weapon_Arms weapon)
    {
        this.weapon = weapon;
        weapon.OnProjectileShot += SpawnProjectileClone;
        weapon.OnHitscanShot += SpawnBulletLine;
        weapon.OnAdvancedHitscanShot += SpawnBulletLineAdvanced;
		weapon.OnGranadeShot += SpawnGranadeClone;

        weapon.OnChargeStart += TriggerCharge;
        weapon.OnChargeEnd += CancelCharge;
        weapon.StopHoldingShoot += CancelPartical;
        weapon.OnMeleeStart += OnMeleeStart;
        weapon.UpdateZoom += UpdateZoomLevel;


		if (ManualAnimationControl)
		{
			OnStartManuelAnimations?.Invoke();
		}

	}

    public void UpdateZoomLevel(bool val)
    {
        OnZoomUpdate?.Invoke(val);
    }

	public void OnMeleeStart(float val)
            {
        OnMeleeAttack?.Invoke();
	}

	public virtual void OnDestroy()
    {

        if (weapon == null) return;
        weapon.OnProjectileShot -= SpawnProjectileClone;
        weapon.OnHitscanShot -= SpawnBulletLine;
        weapon.OnGranadeShot -= SpawnGranadeClone;
        weapon.OnAdvancedHitscanShot -= SpawnBulletLineAdvanced;

		weapon.OnChargeStart -= TriggerCharge;
        weapon.OnChargeEnd -= CancelCharge;

        weapon.StopHoldingShoot -= CancelPartical;
        weapon.OnMeleeStart -= OnMeleeStart;
		CancelPartical();

	}

    void CancelPartical()
    {
		if ( !activeParticalWhileShooting)
		{
            return;
		}
		if (muzzleFlash == null) return;
        muzzleFlash.SetActive(false);

        OnShootStopAction?.Invoke();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

    public void SpawnProjectileClone(GameObject projectile)
    {
        var bulletData = weapon.Bullet as Weapon_Bullet_Projectile;
        var bulletClone = Instantiate(bulletData.BulletVisual, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        var projectileScript = projectile.GetComponent<Bullet>();
        bulletClone.layer = gameObject.layer;
        // set children of bullet colone to layer
        foreach (Transform child in bulletClone.transform)
        {
            child.gameObject.layer = gameObject.layer;
        }


        projectileScript.AddBulletCopy(bulletClone.transform);

        bulletClone.layer = gameObject.layer;

        TriggerPartical();
    }

    public void SpawnBulletLine(Vector3 target)
    {
        var bulletData = weapon.Bullet as Weapon_Bullet_Hitscan;
        if (bulletData.Trail != null)
        {
            var bulletRay = Instantiate(bulletData.Trail, bulletSpawnPoint.position, Quaternion.identity) as GameObject;
            var bulletScript = bulletRay.GetComponent<BulletTrail>();

            bulletRay.layer = gameObject.layer;

            bulletScript.ShowTrail(target - bulletSpawnPoint.position);
            bulletRay.layer = gameObject.layer;
        }
        
        
        TriggerPartical();
    }

	public void SpawnBulletLineAdvanced(Vector3[] targets)
	{
		var bulletData = weapon.Bullet as Weapon_Bullet_Hitscan;
		if (bulletData.Trail != null)
		{
			var bulletRay = Instantiate(bulletData.Trail, bulletSpawnPoint.position, Quaternion.identity) as GameObject;
			var bulletScript = bulletRay.GetComponent<BulletTrail>();

			bulletRay.layer = gameObject.layer;
            List<Vector3> points = new List<Vector3>();
            points.Add(Vector3.zero);
            foreach (var point in targets)
            {
                points.Add(point - bulletSpawnPoint.position);
			}

			bulletScript.ShowTrail(points.ToArray());
			bulletRay.layer = gameObject.layer;
		}


		TriggerPartical();
	}



	public void SpawnGranadeClone(GameObject granade)
    {
        var granadeData = weapon.Bullet as Weapon_Bullet_Granade;
        var granadeClone = Instantiate(granadeData.GranadeStats.GranadeClonePrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        var granadeScript = granade.GetComponent<Granade>();
        granadeClone.layer = gameObject.layer;
        // set children of bullet colone to layer
        foreach (Transform child in granadeClone.transform)
        {
            child.gameObject.layer = gameObject.layer;
        }


        granadeScript.AddGranadeCopy(granadeClone.transform);

        granadeClone.layer = gameObject.layer;

        TriggerPartical();
    }

    public void TriggerPartical()
    {
        if (muzzleFlash == null) return;


        if (activeParticalWhileShooting && muzzleFlash.activeSelf) return;

        if (muzzleFlash.activeSelf)
        {
            muzzleFlash.SetActive(false);
        }

        // enable muzzle flash
        muzzleFlash.SetActive(true);

        OnShootAction?.Invoke();
	}

    public void TriggerCharge()
    {
		if (chargeObject == null) return;


		chargeObject.SetActive(true);
        // action
        OnChargeStartAction?.Invoke();
	}

    public void CancelCharge()
    {
		if (chargeObject == null) return;
		chargeObject.SetActive(false);
        // action
        OnChargeEndAction?.Invoke();
	}


    // get animation index
    public int WeaponAnimationIndex { get { return weaponAnimationIndex; } }
}
