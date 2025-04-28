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


        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        if (Runner.LagCompensation.Raycast(spawnPosition, forward, projectileData.Range, Object.InputAuthority, out var hit, projectileData.HitLayer, hitOptions))
        {
            

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
