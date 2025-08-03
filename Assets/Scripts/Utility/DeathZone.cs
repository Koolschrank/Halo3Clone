using System.Collections;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] float spawnSafeTime = 2f;

    // on trigger enter if have health take 1000000 damage
    public void OnTriggerEnter(Collider other)
    {
        DamagePackage damage = new DamagePackage(1000000);

		CharacterHealth health = other.GetComponent<CharacterHealth>();
        if (health != null)
        {
            if (health.spawnTime + spawnSafeTime > Time.timeSinceLevelLoad)
            {
                CheckIfOutOfBounds(health);
				return;
            }


            var owner = health.ownerOfLastDamage;
            if (owner == null)
            {
                owner = other.gameObject;
			}

            damage.owner = owner;

			health.TakeDamage(damage);
        }
    }


    IEnumerator CheckIfOutOfBounds(CharacterHealth character)
    {
        yield return new WaitForSeconds(spawnSafeTime * 2);
        if (character.transform.position.y < -10f)
        {
            DamagePackage damage = new DamagePackage(1000000);
            damage.owner = character.gameObject;
            character.TakeDamage(damage);
		}
	}
}
