using UnityEngine;
using System;

public class MeleeAttacker : MonoBehaviour
{
    public Action<PlayerMeleeAttack> OnMeleeStart;
    public Action<PlayerMeleeAttack> OnMeleeHit;

    [SerializeField] GameObject self;
    [SerializeField] float velocityYOffset = 0.5f;

    public void AttackStart(PlayerMeleeAttack attackData)
    {
        OnMeleeStart?.Invoke(attackData);
    }

    // attack
    public void Attack(PlayerMeleeAttack attackData)
    {
        var hitPoint = transform.position + transform.forward * attackData.MeleeDistance;
        var radius = attackData.MeleeRadius;
        var colliders = Physics.OverlapSphere(hitPoint, radius, attackData.EnemyLayer);

        if (colliders.Length == 0)
        {
            return;
        }
        int hits = 0;

        foreach (var collider in colliders)
        {
            DamagePackage damagePackage = new DamagePackage(attackData.Damage);
            damagePackage.origin = hitPoint;
            // direction of self move 
            var direction = transform.forward;
            damagePackage.forceVector = direction * attackData.Force;
            damagePackage.owner = self;
            damagePackage.hitPoint = hitPoint;
            damagePackage.impactType = ImpactType.wholeBody;


            if (collider.gameObject == self)
            {
                continue;
            }
            hits++;

            OnMeleeHit?.Invoke(attackData);

            if (collider.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damagePackage);
                
            }

            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(transform.forward * attackData.Force, ForceMode.Impulse);
            }
        }

    }


}
