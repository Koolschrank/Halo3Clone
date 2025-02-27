using UnityEngine;

public class FireDamageOverTime : MonoBehaviour
{
    
    [SerializeField] float radius = 5f;
    [SerializeField] float damage = 1f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float damageRate = 0.3f;
    float damageTimer = 0f;

    [SerializeField] float height = 1f;
    [SerializeField] float depth = 1f;
    [SerializeField] float aliveTime = 5f;
    float aliveTimer = 0f;

    GameObject owner;

    // setOwner
    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public void Start()
    {
        aliveTimer = aliveTime;

    }

    public void Update()
    {
        aliveTimer -= Time.deltaTime;
        if (aliveTimer <= 0)
        {
            return;
        }
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0)
        {
            DamagePackage damagePackage = new DamagePackage(damage);
            damagePackage.origin = transform.position;
            damagePackage.owner = owner;




            damageTimer = damageRate;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
            foreach (Collider collider in colliders)
            {
                // check if the collider is within the height and depth of the fire
                Vector3 closestPoint = collider.ClosestPoint(transform.position);
                float yDistance = closestPoint.y - transform.position.y;
                if (yDistance <height && yDistance > -depth)
                {
                    damagePackage.hitPoint = closestPoint;
                    // apply damage
                    Health health = collider.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(damagePackage);
                    }
                }
            }
        }
    }


}
