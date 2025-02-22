using System;
using UnityEngine;

[Serializable]
public class Damage
{
    [SerializeField] float damageAmount = 10f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] float force = 10f;
    [SerializeField] bool originIsObjectThatSpawnsThis = false;
    [SerializeField] Vector3 origin;

    public float DamageAmount => damageAmount;

    public float Force => force;

    public Vector3 Origin => origin;

    public void SetDamageAmount(float damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    public bool OriginIsObjectThatSpawnsThis => originIsObjectThatSpawnsThis;

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public float HeadShotMultiplier => headShotMultiplier;




}
