using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using System;


public class Bullet : MonoBehaviour
{
    [SerializeField] float damageAmount = 10f;
    [SerializeField] float force = 10f;
    [SerializeField] float shildDamageMultiplier = 1f;
    [SerializeField] float shildDamagMultiplierVSAI = 1.5f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] float radius = 0.1f;

    [SerializeField] float copyTransitionSpeed;
    [SerializeField] bool isNedler = false;
    [SerializeField] bool ignoreShilds = false;

	[SerializeField] GameObject bodyHitPartical;
    [SerializeField] GameObject groundHitPartical;
    [SerializeField] GameObject[] destroyObjects;

    [SerializeField] bool stickToSurfaces = false;
    [NonSerialized]
	public bool inStick = false;

	[SerializeField] NedlerDelete nedlerDelet;



	[Header("Audio")]
    [SerializeField] EventReference bodyHitSound;
    [SerializeField] EventReference groundHitSound;




    Vector3 lastPosition;
    
    List<Transform> bulletCopys = new List<Transform>();

    GameObject owner;
    DamagePackage damagePackage;

    public GameObject Owner => owner;

	// set bullet copy
	public void AddBulletCopy(Transform bulletCopy)
    {

        bulletCopys.Add(bulletCopy);
    }

    // set owner
    public void SetUp(GameObject owner)
    {
        this.owner = owner;
        damagePackage = new DamagePackage(damageAmount);
        damagePackage.owner = owner;
        damagePackage.origin = transform.position;
        damagePackage.headShotMultiplier = headShotMultiplier;
        damagePackage.shildDamageMultiplier = shildDamageMultiplier;
        damagePackage.isNedlerDamage = isNedler;
        lastPosition = transform.position;

		

	}

    public void ApplyDamageMultiplier(float multiplier)
    {
        damagePackage.damageAmount *= multiplier;
    }

    void Update()
    {
        var currentPos = transform.position;
        var direction = transform.position - lastPosition;


		if (inStick)
		{
            
			if (bulletCopys != null)
			{
				foreach (var bulletCopy in bulletCopys)
				{
					bulletCopy.position = transform.position;
				}
			}
			return;
		}

		if (bulletCopys != null)
        {
            foreach (var bulletCopy in bulletCopys)
            {
                bulletCopy.position += direction;
                bulletCopy.position = Vector3.MoveTowards(bulletCopy.position, currentPos, copyTransitionSpeed * Time.deltaTime);
            }
        }


        
        
        RaycastHit hit;
        if (Physics.SphereCast(lastPosition, radius, direction, out hit, Vector3.Distance(currentPos, lastPosition), hitLayer))
        {
            if (hit.collider.gameObject == owner)
            {
                return;
            }

            damagePackage.forceVector = direction.normalized * force;
            damagePackage.hitPoint = hit.point;
            damagePackage.ignoreShild = ignoreShilds;
			bool bodyHit = false;
            if (hit.collider.TryGetComponent<Health>(out Health health))
            {
				if (GameModeSelector.gameModeManager.GameModeStats.team2LoosesScoreWhenTeam1scores && owner.GetComponent<BodyMindConnection>().Mind != null)
				{
					damagePackage.shildDamageMultiplier *= shildDamagMultiplierVSAI;
				}

				health.TakeDamage(damagePackage);
                
                bodyHit = true;
            }
            else
            {

                // if layer is dead player layer
                if (hit.collider.gameObject.layer == PlayerManager.instance.GetDeadPlayerLayer())
                {
                    bodyHit = true;
                }

            }
            // if hit has rigidbody
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
            }

            // spawn the hit particle


            if (bodyHit)
            {
                AudioManager.instance.PlayOneShot(bodyHitSound, hit.point);
                if (bodyHitPartical != null)
                {
                    Instantiate(bodyHitPartical, hit.point, Quaternion.identity);
                    bodyHitPartical.transform.forward = hit.normal;
                }
            }
            else
            {
                AudioManager.instance.PlayOneShot(groundHitSound, hit.point);
                if (groundHitPartical != null)
                {
                    Instantiate(groundHitPartical, hit.point, Quaternion.identity);
                    groundHitPartical.transform.forward = hit.normal;
                }
            }

            for (int i = 0; i < destroyObjects.Length; i++)
            {
                // spawn here the destroy objects
                var obj =Instantiate(destroyObjects[i], hit.point, Quaternion.identity) as GameObject;

                if (obj.TryGetComponent<Explosion>(out Explosion expo))
                {
                    expo.Activate(owner);
                }
            }
            if (stickToSurfaces)
            {
				inStick = true;
				nedlerDelet.enabled = true;
                // try get fly forward
                if (gameObject.TryGetComponent<FlyForward>(out FlyForward flyForward))
                {
                    flyForward.enabled = false;
				}

				if (hit.collider.gameObject.TryGetComponent<CharacterHealth>(out CharacterHealth body))
				{
                    if (!body.IsDead)
                    {
						
						nedlerDelet.Activate(body);

					}
                    else
                    {
                        DestroySelf();
					}
					


				}
				else
				{
					// stick to the object
                    var startPosition = transform.position - transform.forward * 3f;

					// make raycast from startposition to current position
                    if (Physics.Raycast(startPosition, (hit.point - startPosition).normalized, out RaycastHit hit2, Vector3.Distance(startPosition, hit.point) +2f, nedlerDelet.rigidBodyLayer))
                    {
                        transform.position = hit2.point + hit2.normal * 0.1f;
                        foreach (var bulletCopy in bulletCopys)
                        {
							bulletCopy.transform.localScale *= 1.5f;
						}
						
                       
                    }
                    else
                    {
                        
					}


					transform.SetParent(hit.collider.transform);
                    nedlerDelet.Activate();

				}
				

				if (bulletCopys != null)
				{
					foreach (var bulletCopy in bulletCopys)
					{
						bulletCopy.position = transform.position;
						
					}
				}
			}
            else
            {
                DestroySelf();
			}


                
        }


        lastPosition = currentPos;


    }

    public void DestroySelf()
    {
		Destroy(gameObject);
		if (bulletCopys != null)
		{
			foreach (var bulletCopys in bulletCopys)
			{
				Destroy(bulletCopys.gameObject);
			}
		}
	}

}
