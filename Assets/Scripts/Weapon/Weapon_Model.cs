using UnityEngine;
using ZakhanSpellsPack;

public class Weapon_Model : MonoBehaviour
{
    protected Weapon_Arms weapon;
    [SerializeField] protected Transform bulletSpawnPoint;
    [SerializeField] protected Transform muzzleFlashSpawnPoint;
    [SerializeField] protected GameObject muzzleFlashPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public virtual void SetUp(Weapon_Arms weapon)
    {
        this.weapon = weapon;
        weapon.OnProjectileShot += SpawnProjectileClone;
        weapon.OnHitscanShot += SpawnBulletLine;
        weapon.OnGranadeShot += SpawnGranadeClone;
    }

    public virtual void OnDestroy()
    {

        if (weapon == null) return;
        weapon.OnProjectileShot -= SpawnProjectileClone;
        weapon.OnHitscanShot -= SpawnBulletLine;
        weapon.OnGranadeShot -= SpawnGranadeClone;
        
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

        SpawnMuzzleFlash();
    }

    public void SpawnBulletLine(Vector3 target)
    {
        var bulletData = weapon.Bullet as Weapon_Bullet_Hitscan;
        var bulletRay = Instantiate(bulletData.Trail, bulletSpawnPoint.position, Quaternion.identity) as GameObject;
        var bulletScript = bulletRay.GetComponent<BulletTrail>();

        bulletRay.layer = gameObject.layer;

        bulletScript.ShowTrail(target - bulletSpawnPoint.position);
        bulletRay.layer = gameObject.layer;
        SpawnMuzzleFlash();
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

        SpawnMuzzleFlash();
    }

    public void SpawnMuzzleFlash()
    {
        var muzzle =Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation) as GameObject;
        muzzle.transform.localScale = muzzleFlashSpawnPoint.localScale;
        muzzle.layer = gameObject.layer;
    }
}
