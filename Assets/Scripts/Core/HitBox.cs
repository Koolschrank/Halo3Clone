using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] float damage = 10f;

    [SerializeField] Collider collider;


    // on trigger enter
    private void OnTriggerEnter(Collider other)
    {
        // if health component is found
        if (other.TryGetComponent(out Health health))
        {
            // deal damage
            //health.TakeDamage(damage, transform.position);
        }
    }

    // enable collider
    public void EnableCollider()
    {
        collider.enabled = true;
    }

    // disable collider
    public void DisableCollider()
    {
        collider.enabled = false;
    }


}
