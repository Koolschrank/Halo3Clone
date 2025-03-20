using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damageAmount = 10f;
    [SerializeField] float force = 10f;
    [SerializeField] float shildDamageMultiplier = 1f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] float radius = 0.1f;

    [SerializeField] float copyTransitionSpeed;

    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject[] destroyObjects;

    [Header("Audio")]
    [SerializeField] EventReference bodyHitSound;
    [SerializeField] EventReference groundHitSound;




    Vector3 lastPosition;
    
    List<Transform> bulletCopys = new List<Transform>();

    GameObject owner;
    DamagePackage damagePackage;


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

            if (hit.collider.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damagePackage);
                AudioManager.instance.PlayOneShot(bodyHitSound, hit.point);
            }
            else
            {
                AudioManager.instance.PlayOneShot(groundHitSound, hit.point);
            }
            // if hit has rigidbody
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
            }

            if (hitParticle != null)
            {
                Instantiate(hitParticle, hit.point, Quaternion.identity);
                // rotate the particle to the normal of the hit point
                hitParticle.transform.forward = hit.normal;

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
            Destroy(gameObject);
            if (bulletCopys != null)
            {
                foreach (var bulletCopys in bulletCopys)
                {
                    Destroy(bulletCopys.gameObject);
                }
            }
        }


        lastPosition = currentPos;


    }

}
