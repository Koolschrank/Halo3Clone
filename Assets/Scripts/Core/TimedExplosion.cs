using UnityEngine;
using FMODUnity;

public class TimedExplosion : MonoBehaviour
{
    [SerializeField] bool startTimerOnFirstCollision = true;
    [SerializeField] float collisionCooldown = 0.05f;
    float spawnTime;

    bool timerActive = false;
    [SerializeField] float explosionTime = 3f;
    [SerializeField] GameObject explosionEffect;

    [SerializeField] Granade mainGranade;

    float timer;



    [Header("Sound")]
    [SerializeField] EventReference bounceSound;
    



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
            timerActive = true;
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
