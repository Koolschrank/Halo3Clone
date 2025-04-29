using Fusion;
using System;
using UnityEngine;

public class BulletSpawner_HitScan : NetworkBehaviour
{
    public Action OnShoot;
    public Action<Vector3> OnBulletShot;


    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform bulletSpawnTransform;


    [Networked] private int _fireCount { get; set; }


    [Networked, Capacity(32)] private NetworkArray<HitScanData> _projectileData { get; }

    private int _visibleFireCount;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    public void FireProjectile(Weapon_Arms weapon)
    {
        var projectileData = weapon.Data.WeaponBullet as Weapon_Bullet_Hitscan;

        var target = bulletSpawner.Target; 
        var hitPosition = Vector3.zero;
        var spawnPosition = bulletSpawnTransform.position;
        var forward = bulletSpawnTransform.forward;
        if (target != null)
        {
            forward = Vector3.Lerp(forward, (bulletSpawnTransform.position - bulletSpawnTransform.position), weapon.Data.AutoAim.AimLerp).normalized;
        }


        float damageMultiplier = 1f;
        DamagePackage damagePackage = new DamagePackage(projectileData.Damage * damageMultiplier);
        damagePackage.origin = spawnPosition;
        damagePackage.owner = gameObject;
        damagePackage.headShotMultiplier = projectileData.HeadShotMultiplier;
        damagePackage.shildDamageMultiplier = projectileData.ShildDamageMultiplier;
        damagePackage.canHeadShotShild = projectileData.CanHeadShotShild;




        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;
        Vector3 shotDirection = forward;
        Vector3 shotDirectionForThisBullet = shotDirection + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;


        if (Runner.LagCompensation.Raycast(spawnPosition, shotDirectionForThisBullet, projectileData.Range, Object.InputAuthority, out var hit, projectileData.HitLayer))
        {
            damagePackage.forceVector = shotDirectionForThisBullet.normalized * projectileData.Force;
            damagePackage.hitPoint = hit.Point;


            bool bodyHit = false;


            if ( hit.Hitbox.Root != null &&hit.Hitbox.Root.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damagePackage);

                bodyHit = true;
            }
            else
            {
                // if layer is dead player layer
                if (hit.Hitbox.Root != null && hit.Hitbox.Root.gameObject.layer == PlayerManager.instance.GetDeadPlayerLayer())
                {
                    bodyHit = true;
                }
            }
            if (bodyHit)
            {
                AudioManager.instance.PlayOneShot(projectileData.BodyHitSound, hit.Point);
                GameObject impact = Instantiate(projectileData.ImpactBody, hit.Point, Quaternion.identity);
                // get normal of hit point
                impact.transform.forward = hit.Normal;

            }
            else
            {
                AudioManager.instance.PlayOneShot(projectileData.GroundHitSound, hit.Point);
                GameObject impact = Instantiate(projectileData.ImpactGround, hit.Point, Quaternion.identity);

                impact.transform.forward = hit.Normal;
            }



            if (hit.Hitbox.Root != null && hit.Hitbox.Root.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForceAtPosition(damagePackage.forceVector, hit.Point, ForceMode.Impulse);
            }



            damagePackage.damageAmount = projectileData.Damage * damageMultiplier * projectileData.GetDamageFalloff(hit.Distance);





            hitPosition = hit.Point;
        }
        else
        {
            hitPosition = spawnPosition + forward * projectileData.Range;
        }

            _projectileData.Set(_fireCount % _projectileData.Length, new HitScanData()
            {
                HitPosition = hitPosition,
            });

        _fireCount++;
    }

    public override void Render()
    {
        if (_visibleFireCount < _fireCount)
        {
            OnShoot?.Invoke();
        }

        for (int i = _visibleFireCount; i < _fireCount; i++)
        {
            var data = _projectileData[i % _projectileData.Length];

            if (data.HitPosition != Vector3.zero)
            {
                OnBulletShot?.Invoke(data.HitPosition);
            }
        }

        _visibleFireCount = _fireCount;
    }

    private struct HitScanData : INetworkStruct
    {
        public Vector3 HitPosition;
    }
}
