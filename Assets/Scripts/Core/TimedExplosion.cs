using UnityEngine;
using FMODUnity;

public class TimedExplosion : MonoBehaviour
{
    [SerializeField] bool startTimerOnFirstCollision = true;
    [SerializeField] bool stickOnCollision = false;
	[SerializeField] float collisionCooldown = 0.05f;
    float spawnTime;

    bool timerActive = false;
    [SerializeField] float explosionTime = 3f;
    [SerializeField] GameObject explosionEffect;

    [SerializeField] Granade mainGranade;
    [SerializeField] GameObject[] gameObjectsToEnableOnTimerStart;

    float timer;

    [Header("Charge")]
    [SerializeField] bool hasChargeObject = false;
    [SerializeField] float timeUntilExplosionToSpawnChargeObject = 1f;
    [SerializeField] GameObject chargeObjectPrefab;

	[SerializeField] EventReference chargeSound;
	bool chargeObjectSpawned = false;


	[Header("Sound")]
    [SerializeField] protected EventReference bounceSound;
    



    // on collision enter

    void Start()
    {
        spawnTime = Time.time;
        timer = explosionTime;
        if (!startTimerOnFirstCollision)
        {
            timerActive = true;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // bounce sound
        

        if (Time.time - spawnTime < collisionCooldown)
        {
            return;
        }
        RuntimeManager.PlayOneShot(bounceSound, transform.position);
        if (startTimerOnFirstCollision)
        {
            if (!timerActive)
            {
                foreach (var obj in gameObjectsToEnableOnTimerStart)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
            }


            timerActive = true;

        }

        if (stickOnCollision)
        {
			GetComponent<Rigidbody>().isKinematic = true;
			if (collision.gameObject.TryGetComponent<CharacterHealth>(out CharacterHealth body))
            {
               var bodyPart = body.RagdollTrigger.GetClosesRigidbody(transform.position);
                transform.SetParent(bodyPart.transform);

			}
            else
            {
				// stick to the object
				transform.SetParent(collision.transform);
				
			}
                
        }
        
	}

    void Update()
    {
        if (!timerActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Explode();
        }

        if (hasChargeObject && !chargeObjectSpawned && timer <= timeUntilExplosionToSpawnChargeObject)
        {
            SpawnChargeObject();
		}
	}

    void SpawnChargeObject()
    {
        if (chargeObjectPrefab != null)
        {
            var charge =Instantiate(chargeObjectPrefab, transform);
            chargeObjectSpawned = true;
			RuntimeManager.PlayOneShot(chargeSound, transform.position);
		}
		
	}

	void Explode()
    {
        var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity) as GameObject;
        if (explosion.TryGetComponent<Explosion>(out Explosion expo))
        {
            expo.Activate(mainGranade.GetOwner());
        }


        Destroy(gameObject);
    }

}
