using UnityEngine;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - spawnTime < collisionCooldown)
        {
            return;
        }

        Debug.Log(collision.gameObject.name);
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
        var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        if (explosion.TryGetComponent<Explosion>(out Explosion expo))
        {
            expo.Activate(mainGranade.GetOwner());
        }


        Destroy(gameObject);
    }

}
