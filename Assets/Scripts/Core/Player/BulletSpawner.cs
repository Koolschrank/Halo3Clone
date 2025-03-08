using System;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public Action<Transform> OnTargetAcquired;
    public Action<Transform> OnTargetLost;


    Transform target;


    [SerializeField] Transform mainTransform;
    [SerializeField] LayerMask autoAimLayerMask;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] LayerMask wallCheck;


    AutoAim autoAimOfCurrentWeapon;

    public void Start()
    {
        playerArms.OnWeaponEquipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = weapon.AutoAim;
        };

        if (playerArms.CurrentWeapon != null)
        {
            autoAimOfCurrentWeapon = playerArms.CurrentWeapon.AutoAim;
        }

        playerArms.OnWeaponUnequipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = null;
        };

        playerArms.OnWeaponDroped += (weapon) =>
        {
            autoAimOfCurrentWeapon = null;
        };

    }

    public void Update()
    {
        if (autoAimOfCurrentWeapon == null)
        {
            return;
        }

        var newTarget = GetAutoAimTarget(autoAimOfCurrentWeapon.Radius, autoAimOfCurrentWeapon.RaycastLenght);
        if (newTarget != target)
        {
            if (newTarget)
            {
                OnTargetLost?.Invoke(target);
                OnTargetAcquired?.Invoke(newTarget);
            }
            else
            {
                OnTargetLost?.Invoke(target);
            }
            target = newTarget;
        }
    }



    public Transform GetAutoAimTarget(float radius, float lenght)
    {

        //sphere cast
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, lenght, autoAimLayerMask))
        {
            // make a ray cast to check if there is a wall between player and target
            if (Physics.Raycast(transform.position, (hit.point - transform.position), out RaycastHit wallHit, Vector3.Distance(transform.position, hit.point), wallCheck))
            {
                return null;
            }



            if (hit.collider.TryGetComponent<PlayerTeam>(out PlayerTeam pt))
            {
                if (pt.TeamIndex == playerTeam.TeamIndex)
                {
                    
                    return null;
                }

            }

            if (hit.collider.TryGetComponent<CharacterHealth>(out CharacterHealth ch))
            {
                if (ch.IsHeadAreaCloserThanMainBody(hit.point))
                {
                    return ch.GetHead();
                }
            }

            return hit.transform;
        }

        return null;
    }


    public GameObject[] ShootGranade(Weapon_Arms weapon)
    {
        var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;

        Weapon_Bullet_Granade granade_data = weapon.Bullet as Weapon_Bullet_Granade;
        var forward = transform.forward;
        var target = GetAutoAimTarget(autoAimRadius, autoAimRaycastLenght);
        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - transform.position), autoAimLerp).normalized;
        }

        int bulletCount = weapon.BulletsPerShot;
        GameObject[] granades = new GameObject[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            //var forwardForThisBullet = forward + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;
            var inaccuracy = UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;
            // spawn projectile at transform position and rotate it to forward
            granades[i] = granadeThrower.ThrowGranadeWithWeapon(granade_data.GranadeStats, inaccuracy);

            
        }

        return granades;



    }

    public Vector3[] ShootHitScan(Weapon_Arms weapon)
    {
        var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;


        Weapon_Bullet_Hitscan bullet = weapon.Bullet as Weapon_Bullet_Hitscan;
        float range = bullet.Range;
        LayerMask hitLayer = bullet.HitLayer;

        DamagePackage damagePackage = new DamagePackage(bullet.Damage);
        damagePackage.origin = mainTransform.position;
        damagePackage.owner = mainTransform.gameObject;
        damagePackage.headShotMultiplier = bullet.HeadShotMultiplier;
        damagePackage.shildDamageMultiplier = bullet.ShildDamageMultiplier;

        Transform cameraTransform = transform;
        RaycastHit hit;
        var forward = cameraTransform.forward;
        var target = GetAutoAimTarget(autoAimRadius, autoAimRaycastLenght);




        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - cameraTransform.position), autoAimLerp).normalized;
        }

        int bulletCount = weapon.BulletsPerShot;
        Vector3 shotDirection = forward;
        Vector3[] hitPoints = new Vector3[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 shotDirectionForThisBullet = shotDirection + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;

            if (Physics.Raycast(cameraTransform.position, shotDirectionForThisBullet, out hit, range, hitLayer))
            {

                damagePackage.forceVector = shotDirectionForThisBullet.normalized * bullet.Force;
                damagePackage.hitPoint = hit.point;

                damagePackage.damageAmount = bullet.Damage * bullet.GetDamageFalloff(hit.distance);

                // if hit health
                if (hit.collider.TryGetComponent<Health>(out Health health))
                {
                    health.TakeDamage(damagePackage);
                    AudioManager.instance.PlayOneShot(bullet.BodyHitSound, hit.point);
                }
                else
                {
                    AudioManager.instance.PlayOneShot(bullet.GroundHitSound, hit.point);
                }
                if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
                }

                GameObject impact = Instantiate(bullet.Impact, hit.point, Quaternion.identity);
                // get normal of hit point
                impact.transform.forward = hit.normal;
                hitPoints[i] = hit.point;

               
            }
            else
            {
                hitPoints[i] = cameraTransform.position + shotDirectionForThisBullet * range;
            }
        }

        return hitPoints;


    }

    public GameObject[] ShootProjectile(Weapon_Arms weapon)
    {
        var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;

        Weapon_Bullet_Projectile bullet_data = weapon.Bullet as Weapon_Bullet_Projectile;
        var forward = transform.forward;
        var target = GetAutoAimTarget(autoAimRadius, autoAimRaycastLenght);
        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - transform.position), autoAimLerp).normalized;
        }

        int bulletCount = weapon.BulletsPerShot;
        GameObject[] bullets = new GameObject[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            var forwardForThisBullet = forward + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;
            // spawn projectile at transform position and rotate it to forward
            GameObject projectile = Instantiate(bullet_data.BulletPrefab, transform.position, Quaternion.LookRotation(forwardForThisBullet));

            if (projectile.TryGetComponent<Bullet>(out Bullet bullet))
            {
                bullet.SetUp(mainTransform.gameObject);

            }
            bullets[i] = projectile;
        }

        return bullets;

    }
}
