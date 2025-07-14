using System;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{

    public Action<GameObject> OnGranadeThrow;


    float throwDelay = 0f;
    GranadeStats granadeStats = null;
    [SerializeField] Transform mainTransform;
    [SerializeField] AbilityInventory abilityInventory;
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] PlayerMovement playermovement;

    float soundTriggerDelay = 0f;

	private void Awake()
	{
        characterHealth.OnDeath += TryDropGranade;
	}
    public bool InGranadeThrow => throwDelay > 0;

	public void ThrowGranadeStart(GranadeStats granadeStats , float timeMultiplier)
    {
        this.granadeStats = granadeStats;
        throwDelay = granadeStats.ThrowDelay * timeMultiplier;

        soundTriggerDelay = granadeStats.ThrowSoundDelay * timeMultiplier;
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

        if (soundTriggerDelay > 0)
        {
            soundTriggerDelay -= Time.deltaTime;
            if (soundTriggerDelay <= 0)
            {
				AudioManager.instance.PlayOneShot(granadeStats.ThrowSound, transform.position);
			}
		}

	}

    // todo: a lot of redundant code here
    public GameObject ThrowGranadeWithWeapon(GranadeStats granadeStats, Vector3 inaccuracy)
    {
        if (granadeStats == null) return null;

        GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation);
        Rigidbody rb = granade.GetComponent<Rigidbody>();

        var angle = granadeStats.ThrowAngle;
        if (playermovement.gravityMultiplier != 1)
        {
            angle = 0;
        }

		rb.AddForce((transform.forward + inaccuracy) * granadeStats.ThrowForce, ForceMode.Impulse);
        rb.AddForce((transform.up + inaccuracy) * granadeStats.ThrowForce * angle, ForceMode.Impulse);

        if (granade.TryGetComponent<Granade>(out Granade granadeScript))
        {
            granadeScript.SetOwner(mainTransform.gameObject);

        }

        return granade;
    }

    public GameObject ThrowGranade(GranadeStats granadeStats)
    {
        if (granadeStats == null) return null;

		Debug.Log(granadeStats.name);
		GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation) as GameObject;

        Rigidbody rb = granade.GetComponent<Rigidbody>();

		var angle = granadeStats.ThrowAngle;
		if (playermovement.gravityMultiplier != 1)
		{
			angle = 0;
		}

		rb.AddForce(transform.forward * granadeStats.ThrowForce, ForceMode.Impulse);
        rb.AddForce(transform.up * granadeStats.ThrowForce * angle, ForceMode.Impulse);
        Debug.Log(granade.name);
        OnGranadeThrow?.Invoke(granade);

        if (granade.TryGetComponent<Granade>(out Granade granadeScript))
        {
            granadeScript.SetOwner(mainTransform.gameObject);

        }
		abilityInventory.UseSelectedIndex();
		return granade;
    }

    public void TryDropGranade()
    {
		if (throwDelay > 0)
		{
			DropGranade(granadeStats);
            throwDelay = 0f;
		}
	}

    public GameObject DropGranade(GranadeStats granadeStats)
    {
		if (granadeStats == null) return null;

		Debug.Log(granadeStats.name);
		GameObject granade = Instantiate(granadeStats.GranadePrefab, transform.position, transform.rotation) as GameObject;

		Rigidbody rb = granade.GetComponent<Rigidbody>();
		OnGranadeThrow?.Invoke(granade);

		rb.AddForce(-transform.up * 0.5f, ForceMode.Impulse);

		if (granade.TryGetComponent<Granade>(out Granade granadeScript))
		{
			granadeScript.SetOwner(mainTransform.gameObject);

		}
		abilityInventory.UseSelectedIndex();
		return granade;
	}




}
