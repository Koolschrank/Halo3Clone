using System;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{

    public Action<GameObject> OnGranadeThrow;


    float throwDelay = 0f;
    GranadeStats granadeStats = null;
    [SerializeField] Transform mainTransform;

    public void ThrowGranadeStart(GranadeStats granadeStats)
    {
        this.granadeStats = granadeStats;
        throwDelay = granadeStats.ThrowDelay;
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

    public void ThrowGranade(GranadeStats granadeStats)
    {
        if (granadeStats == null) return;

        GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation);
        Rigidbody rb = granade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * granadeStats.ThrowForce, ForceMode.Impulse);
        rb.AddForce(transform.up * granadeStats.ThrowForce * granadeStats.ThrowAngle, ForceMode.Impulse);
        OnGranadeThrow?.Invoke(granade);

        if (granade.TryGetComponent<Granade>(out Granade granadeScript))
        {
            granadeScript.SetOwner(mainTransform.gameObject);

        }
    }





}
