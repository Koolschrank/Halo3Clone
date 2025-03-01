using UnityEngine;
using FMODUnity;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    
    [SerializeField] float range = 5f;
    [SerializeField] float damage = 10f;
    [SerializeField] float damageOnShildMultiplier = 1f;
    [SerializeField] AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    
    [SerializeField] float force = 10f;
    [SerializeField] float forceYOffset = -1f;
    // force fall off curve
    [SerializeField] AnimationCurve forceFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    // curve to control the damage falloff

    [SerializeField] LayerMask hitLayer;
    [SerializeField] float timeForSelfDestruction = 5f;

    DamagePackage damagePackage;

    [Header("Sound")]
    [SerializeField] EventReference explosionSound;

    [Header("extra features")]
    [SerializeField] FireDamageOverTime fireDamageOverTime;

    public UnityEvent OnExplosion;


    public void Activate(GameObject owner)
    {
        damagePackage = new DamagePackage(damage);
        damagePackage.owner = owner;
        damagePackage.origin = transform.position;
        Trigger();

        if (fireDamageOverTime != null)
        {
            fireDamageOverTime.SetOwner(owner);
        }

    }


    public void Trigger()
    {
        OnExplosion?.Invoke();
        RuntimeManager.PlayOneShot(explosionSound, transform.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, hitLayer);
        foreach (var collider in colliders)
        {
            var direction = collider.transform.position - transform.position;
            var forceDirection = collider.transform.position - (transform.position + transform.up * forceYOffset);
            var distance = direction.magnitude;
            var falloff = damageFalloff.Evaluate(distance / range);
            var finalDamage = damage * falloff;
            var forceFalloffValue = forceFalloff.Evaluate(distance / range);
            var finalForce = force * forceFalloffValue;
            damagePackage.hitPoint = collider.transform.position;
            damagePackage.damageAmount = finalDamage;
            damagePackage.forceVector = forceDirection.normalized * finalForce;
            damagePackage.impactType = ImpactType.wholeBody;
            damagePackage.shildDamageMultiplier = damageOnShildMultiplier;

            if (collider.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damagePackage);
            }
            // add force to the rigidbody
            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                
                rb.AddForce(damagePackage.forceVector, ForceMode.Impulse);
            }


        }

        Destroy(gameObject, timeForSelfDestruction);

    }


    // gizmo for range also not during runtime
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
