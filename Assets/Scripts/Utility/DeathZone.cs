using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // on trigger enter if have health take 1000000 damage
    public void OnTriggerEnter(Collider other)
    {
        DamagePackage damage = new DamagePackage(1000000);

		CharacterHealth health = other.GetComponent<CharacterHealth>();
        if (health != null)
        {
            var owner = health.ownerOfLastDamage;
            if (owner == null)
            {
                owner = other.gameObject;
			}

            damage.owner = owner;

			health.TakeDamage(damage);
        }
    }
}
