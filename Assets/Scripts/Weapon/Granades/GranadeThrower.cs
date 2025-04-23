using System;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{

    public Action<GameObject,GranadeStats> OnGranadeThrow;
    public Action<GranadeStats, float> OnGranadeThrowStart;


    float throwDelay = 0f;
    GranadeStats granadeStats = null;
    [SerializeField] Transform mainTransform;

    public void ThrowGranadeStart(GranadeStats granadeStats , float timeMultiplier)
    {
        this.granadeStats = granadeStats;
        throwDelay = granadeStats.ThrowDelay * timeMultiplier;
    }

    public void Update()
    {
        if (throwDelay > 0)
        {
            throwDelay -= Time.deltaTime;

            if (throwDelay <= 0)
            {
                ThrowGranade(granadeStats);
            }
        }

    }

    // todo: a lot of redundant code here
    public GameObject ThrowGranadeWithWeapon(GranadeStats granadeStats, Vector3 inaccuracy)
    {
        if (granadeStats == null) return null;

        GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation);
        Rigidbody rb = granade.GetComponent<Rigidbody>();
        rb.AddForce((transform.forward + inaccuracy) * granadeStats.ThrowForce, ForceMode.Impulse);
        rb.AddForce((transform.up + inaccuracy) * granadeStats.ThrowForce * granadeStats.ThrowAngle, ForceMode.Impulse);

        if (granade.TryGetComponent<Granade>(out Granade granadeScript))
        {
            granadeScript.SetOwner(mainTransform.gameObject);

        }

        return granade;
    }

    public GameObject ThrowGranade(GranadeStats granadeStats)
    {
        if (granadeStats == null) return null;

        GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation);
        Rigidbody rb = granade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * granadeStats.ThrowForce, ForceMode.Impulse);
        rb.AddForce(transform.up * granadeStats.ThrowForce * granadeStats.ThrowAngle, ForceMode.Impulse);
        OnGranadeThrow?.Invoke(granade, granadeStats);

        if (granade.TryGetComponent<Granade>(out Granade granadeScript))
        {
            granadeScript.SetOwner(mainTransform.gameObject);

        }

        return granade;
    }





}
