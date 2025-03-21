using System;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public Action<Transform> OnTargetAcquired;
    public Action<Transform> OnTargetLost;


    Transform target;


    [SerializeField] Transform mainTransform;
    [SerializeField] LayerMask autoAimLayerMask;
    [SerializeField] RightArm rightArm;
    [SerializeField] LeftArm leftArm;
    //[SerializeField] PlayerArms playerArms;
    

    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] LayerMask wallCheck;


    //AutoAim autoAimOfCurrentWeapon;

    /*
    public void Start()
    {
        playerArms.RightArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = weapon.AutoAim;
        };

        if (playerArms.RightArm.CurrentWeapon != null)
        {
            autoAimOfCurrentWeapon = playerArms.RightArm.CurrentWeapon.AutoAim;
        }

        playerArms.RightArm.OnWeaponUnequipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = null;
        };

        playerArms.RightArm.OnWeaponDroped += (weapon) =>
        {
            autoAimOfCurrentWeapon = null;
        };

    }*/

    public void Update()
    {
        Transform newTarget = null;
        var rightArmWeapon = rightArm.CurrentWeapon;
        if (rightArmWeapon != null)
        {
            var rightArmAutoAim = rightArm.CurrentWeapon.AutoAim;
            newTarget = GetAutoAimTarget(rightArmAutoAim.Radius, rightArmAutoAim.RaycastLenght);
        }

        if (newTarget == null)
        {
            var leftArmWeapon = leftArm.CurrentWeapon;
            if (leftArmWeapon != null)
            {
                var leftArmAutoAim = leftArm.CurrentWeapon.AutoAim;
                newTarget = GetAutoAimTarget(leftArmAutoAim.Radius, leftArmAutoAim.RaycastLenght);

            }

        }

           


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

        float damageMultiplier = 1f;
        if (weapon.IsBeingDualWielded)
        {
            damageMultiplier = weapon.DamageMultiplierWhenDualWielded;
        }

        DamagePackage damagePackage = new DamagePackage(bullet.Damage * damageMultiplier);
        damagePackage.origin = mainTransform.position;
        damagePackage.owner = mainTransform.gameObject;
        damagePackage.headShotMultiplier = bullet.HeadShotMultiplier;
        damagePackage.shildDamageMultiplier = bullet.ShildDamageMultiplier;
        damagePackage.canHeadShotShild = bullet.CanHeadShotShild;

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

                damagePackage.damageAmount = bullet.Damage * damageMultiplier * bullet.GetDamageFalloff(hit.distance);


                bool bodyHit = false;
                // if hit health
                if (hit.collider.TryGetComponent<Health>(out Health health))
                {
                    health.TakeDamage(damagePackage);
                    
                    bodyHit = true;
                }
                else
                {
                    // if layer is dead player layer
                    if (hit.collider.gameObject.layer == PlayerManager.instance.GetDeadPlayerLayer())
                    {
                        bodyHit = true;
                    }

                    
                }
                if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
                }

                if (bodyHit)
                {
                    AudioManager.instance.PlayOneShot(bullet.BodyHitSound, hit.point);
                    GameObject impact = Instantiate(bullet.ImpactBody, hit.point, Quaternion.identity);
                    // get normal of hit point
                    impact.transform.forward = hit.normal;
                    
                }
                else
                {
                    AudioManager.instance.PlayOneShot(bullet.GroundHitSound, hit.point);
                    GameObject impact = Instantiate(bullet.ImpactGround, hit.point, Quaternion.identity);
                    
                    impact.transform.forward = hit.normal;
                }
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

                if (weapon.IsBeingDualWielded)
                {
                    bullet.ApplyDamageMultiplier(weapon.DamageMultiplierWhenDualWielded);
                }

            }
            bullets[i] = projectile;
        }

        return bullets;

    }
}
