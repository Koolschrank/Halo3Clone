using UnityEngine;
using FMODUnity;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    
    [SerializeField] float range = 5f;
    [SerializeField] float damage = 10f;
    [SerializeField] float damageDeadzone = 10f;
	[SerializeField] float damageMultiplierVSAI = 1f;
	[SerializeField] float damageOnShildMultiplier = 1f;

	[SerializeField] float damageReductionAgainstBlocking = 0.8f;
	[SerializeField] AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    
    [SerializeField] float force = 10f;
    [SerializeField] float forceOnPlayer = 20f;
	[SerializeField] float forceYOffset = -1f;
    // force fall off curve
    [SerializeField] AnimationCurve forceFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    // curve to control the damage falloff

    [SerializeField] LayerMask hitLayer;
	[SerializeField] LayerMask wallLayers =  1 << 0;
	[SerializeField] float damageReductionIfObstructed = 0.4f;

    [SerializeField] float timeForSelfDestruction = 5f;

    DamagePackage damagePackage;

    [Header("Sound")]
    [SerializeField] EventReference explosionSound;

    [Header("extra features")]
    [SerializeField] FireDamageOverTime fireDamageOverTime;

    public UnityEvent OnExplosion;


    public LayerMask rumbleMask;
    public RumbleData rumbleData;
    public float rumbleReach = 20f;


	public void Activate(GameObject owner)
    {
        damagePackage = new DamagePackage(damage);
        damagePackage.owner = owner;
        damagePackage.origin = transform.position;
        RumbleTrigger();
		Trigger();

        if (fireDamageOverTime != null)
        {
            fireDamageOverTime.SetOwner(owner);
        }

    }

    public void RumbleTrigger()
    {
		// sperecast

		Collider[] colliders = Physics.OverlapSphere(transform.position, rumbleReach, rumbleMask);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<BodyMindConnection>(out BodyMindConnection player) && player.Mind != null)
            {
                var id = player.Mind.playerID;
                var distance = Vector3.Distance(transform.position, player.transform.position);
                var falloff = Mathf.Clamp01(1 - (distance / rumbleReach));
                var tempRumbleData = rumbleData;
                tempRumbleData.intensity *= falloff;
				if (falloff > 0)
                {
                    RumbleManager.Instance.TriggerRumble(tempRumbleData, id);
				}
			}
		}

	}

    public void Trigger()
    {
        OnExplosion?.Invoke();
        RuntimeManager.PlayOneShot(explosionSound, transform.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, hitLayer);
        foreach (var collider in colliders)
        {
            

            

            if (collider.TryGetComponent<Health>(out Health health))
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
                damagePackage.damageReductionAgainstBlock = damageReductionAgainstBlocking;

				

				float margin = 1f;
				if (range < margin)
				{
					margin = range / 2;
				}

                bool obstructed = false;
				// cast a ray to check if the object is obstructed
				if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, wallLayers))
                {
                    if (hit.collider != collider)
                    {
                        obstructed = true;

						damagePackage.damageAmount *= damageReductionIfObstructed;
                        damagePackage.forceVector *= damageReductionIfObstructed;
                    }
                }

				if (collider.gameObject.CompareTag("AIEnemy"))
				{
                    Debug.Log("hit ai");
					damagePackage.damageAmount *= damageMultiplierVSAI;
				}
                if (damagePackage.damageAmount > damageDeadzone)
                    health.TakeDamage(damagePackage);


                if (!health.IsDead && !obstructed)
                {
                    var physicsImpulse = collider.GetComponent<PlayerPhysicsImpulse>();
                    if (physicsImpulse != null)
                    {
                        // apply force to the player
                        var playerDirection = collider.transform.position - (transform.position + transform.up * forceYOffset);
                        var forceOnPlayerFalloff = forceFalloff.Evaluate(playerDirection.magnitude / range);
						var playerForce = playerDirection.normalized * forceOnPlayer * forceOnPlayerFalloff;

                        var playerImpact = new PlayerImpactStruct
                        {
                            impactForce = playerForce,
                            resetGravity = false 
                        };



						physicsImpulse.AddImpulse(playerImpact);
					}
				}
            }
            // add force to the rigidbody
            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
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


                float margin = 1f;
                if (range < margin)
                {
                    margin = range /2;
                }
                if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, wallLayers))
                {
                    if (hit.collider != collider)
                    {
                        damagePackage.damageAmount *= damageReductionIfObstructed;
                        damagePackage.forceVector *= damageReductionIfObstructed;
                    }
                }
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
