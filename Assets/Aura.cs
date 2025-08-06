using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Aura : MonoBehaviour
{
    List<GameObject> playersInAura = new List<GameObject>();

    [SerializeField] float stayTime = 8f;
    float timer = 0f;
    float timerInstandHeal = 0f;

	[SerializeField] float instantHealTime = 0.1f;
    [SerializeField] float instantHeal = 50f;
	[SerializeField] float shildRegenMultiplier = 4f;

	[SerializeField] float armorHeal = 4f;
	[SerializeField] float damageReductionMultiplier = 0.5f;

    [SerializeField] float poisonDamage = 0f;
	[SerializeField] float moveSpeedHandicap = 0f;
    [SerializeField] GameObject forceShild;
    [SerializeField] bool ignoreAICharacters = false;
    [SerializeField] bool reviveBody = false;
    [SerializeField] LayerMask reviveLayer;

	private void Start()
	{
		timer = stayTime;
        timerInstandHeal = instantHealTime;
		// Set the initial values for players in aura


		// sperecast for revive body
        if (reviveBody)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, reviveLayer);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<ReviveBody>(out ReviveBody body))
                {
                    body.Revive();
                }
            }
		}

	}

	// on trigger enter and exit
	private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<BodyMindConnection>(out BodyMindConnection body))
        {
            if (ignoreAICharacters && body.Mind == null)
            {
                return;
            }


            AddPlayerToAura(body);
		}
        
	}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<BodyMindConnection>(out BodyMindConnection body))
        {
            RemovePlayerFromAura(body);
		}
	}

	private void Update()
	{
        timer -= Time.deltaTime;
		timerInstandHeal -= Time.deltaTime;

		if (timer <= 0f)
        {
			Destroy(forceShild);
			RemoveAura();
			// disable collider
            GetComponent<Collider>().enabled = false;
            
			return;
		}

        if (shildRegenMultiplier != 0)
        {
            foreach (var player in playersInAura)
            {
                if (player.TryGetComponent<CharacterHealth>(out CharacterHealth health))
                {
                    health.InShild();

				}
			}
		}

	}

	void AddPlayerToAura(BodyMindConnection body)
	{
		if (!playersInAura.Contains(body.gameObject))
		{
			playersInAura.Add(body.gameObject);

            var health = body.GetComponent<CharacterHealth>();
            if (damageReductionMultiplier != 0)
                health.aura_DamageReduction = damageReductionMultiplier;
            if (shildRegenMultiplier != 0)
				health.aura_shildRegenDelay = shildRegenMultiplier;
            if (poisonDamage != 0f)
                health.aura_poisonDamage = poisonDamage;
            if (moveSpeedHandicap != 0f)
            {
                body.GetComponent<PlayerMovement>().aura_moveSpeedReduction = moveSpeedHandicap;
			}

            if (armorHeal != 0)
                health.aura_armorHeal = armorHeal;

			if (timerInstandHeal > 0f)
            {
                health.GainShild(instantHeal);
                timerInstandHeal = instantHealTime; // Reset the instant heal timer
            }
           

		}
	}

    void RemovePlayerFromAura(BodyMindConnection body)
    {
        if (playersInAura.Contains(body.gameObject))
        {
            playersInAura.Remove(body.gameObject);
            var health = body.GetComponent<CharacterHealth>();
            if (damageReductionMultiplier != 0)
				health.aura_DamageReduction = 0f; // Reset to default value
            if (shildRegenMultiplier != 0)
				health.aura_shildRegenDelay = 0f; // Reset to default value
            if (poisonDamage != 0f)
                health.aura_poisonDamage = 0f; // Reset to default value

			if (armorHeal != 0)
				health.aura_armorHeal = 0f;
			if (moveSpeedHandicap != 0f)
            {
                body.GetComponent<PlayerMovement>().aura_moveSpeedReduction = 0f; // Reset to default value
            }
		}
	}

	void RemoveAura()
    {
        foreach (var player in playersInAura)
        {
            if (player.TryGetComponent<BodyMindConnection>(out BodyMindConnection body))
            {
                RemovePlayerFromAura(body);
            }
		}

        playersInAura.Clear(); 
	}
    
}
