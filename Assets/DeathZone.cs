using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // on trigger enter if have health take 1000000 damage
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("DeathZone OnTriggerEnter");
        DamagePackage damage = new DamagePackage(1000000);
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            Debug.Log("DeathZone OnTriggerEnter Health");
            health.TakeDamage(damage);
        }
    }
}
