using UnityEngine;

public class TimedExplosionOnGround : TimedExplosion
{
    [SerializeField] float raycastDistance = 0.5f;
    [SerializeField] LayerMask groundLayer;


    protected override void OnCollisionEnter(Collision collision)
    {
        // check if below is ground
        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance, groundLayer))
        {
            base.OnCollisionEnter(collision);
        }
        // if collision has health
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            base.OnCollisionEnter(collision);
        }
    }
}
