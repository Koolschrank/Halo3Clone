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
            Debug.Log($"Hit that {hit.collider.name}");

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


                // if hit health
                if (hit.collider.TryGetComponent<Health>(out Health health))
                {
                    health.TakeDamage(damagePackage);
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
                bullet.SetUp(gameObject);

            }
            bullets[i] = projectile;
        }

        return bullets;

    }
}
