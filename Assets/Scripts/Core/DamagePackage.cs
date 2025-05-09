using System;
using UnityEngine;

public struct DamagePackage
{
    public Action<Transform> OnDamageApplied;
    public Action<Transform> OnKill;

    public float damageAmount;

    public Vector3 hitPoint;
    public Vector3 origin;
    public Vector3 forceVector;
    public ImpactType impactType;
    public GameObject owner;
    public bool canHeadShot;
    public bool canHeadShotShild;
    public float headShotMultiplier;
    public float shildDamageMultiplier;
    public bool hasHitMarkerEffect;

    // construtor
    public DamagePackage(float damageAmount)
    {
        this.damageAmount = damageAmount;
        headShotMultiplier = 1f;
        forceVector = Vector3.zero;
        hitPoint = Vector3.zero;
        origin = Vector3.zero;
        impactType = ImpactType.singleBodyPart;
        owner = null;
        canHeadShot = false;
        shildDamageMultiplier = 1f;

        OnDamageApplied = null;
        OnKill = null;
        hasHitMarkerEffect = true;
        canHeadShotShild = false;
    }

    public DamagePackage(float damageAmount, Vector3 hitPoint, Vector3 origin, Vector3 force, ImpactType impactType, GameObject owner, bool canHeadShot, float headShotMultiplier, bool hasHitMarkerEffect,bool canHeadShotShild)
    {
        this.damageAmount = damageAmount;
        this.hitPoint = hitPoint;
        this.origin = origin;
        this.forceVector = force;
        this.impactType = impactType;
        this.owner = owner;
        this.canHeadShot = canHeadShot;
        this.headShotMultiplier = headShotMultiplier;
        this.hasHitMarkerEffect = hasHitMarkerEffect;
        this.canHeadShotShild = canHeadShotShild;
        shildDamageMultiplier = 1f;

        OnDamageApplied = null;
        OnKill = null;
    }
}

public enum ImpactType
{
    singleBodyPart,
    wholeBody
}
