using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // unity event on death
    public Action OnDeath;
    public Action OnShowHealthBar;
    public Action OnPreDeath;




    [SerializeField] protected float maxHeath;
    [SerializeField] protected float currentHeath;
    [SerializeField] protected bool setMaxHeathOnStart = true;
    

    [SerializeField] protected bool hasHealthRegen;
    [SerializeField] protected float healthRegenDelay;
    protected float healthRegenTimer;
    [SerializeField] protected float healthRegenAmountPerSecond;
    [SerializeField] protected float spawnInvulnerabilityTime = 0.5f; // time in seconds to be invulnerable after spawn
    float spawnTime;


    public float MaxHeath => maxHeath;

    
    public float CurrentHeath => currentHeath;

    // action health change
    public Action<float> OnHealthChanged;
    public Action<float> OnMaxHealthChanged;
    public Action<DamagePackage> OnDamageTaken;

    public bool IsDead => currentHeath <= 0;


	[NonSerialized]
	public bool weakBody = false;
	public float weakBody_HealthRegenMultiplier = 0.2f;
	public float weakBody_ShildDamageMultiplier = 1.2f;
	public float weakBody_ShildRegenMultiplier = 0.75f;

	protected virtual void Start()
    {
        spawnTime = Time.time;
        if (setMaxHeathOnStart)
        {
            currentHeath = maxHeath;
            OnHealthChanged?.Invoke(HealthPercentage);
        }
    }

    public virtual void Update()
    {
        if (hasHealthRegen)
        {
            if (healthRegenTimer > 0)
            {
				var regenTimerReduction = 1f;
				if (weakBody)
				{
					regenTimerReduction *= weakBody_HealthRegenMultiplier;
				}
				healthRegenTimer -= regenTimerReduction * Time.deltaTime;
            }
            else
            {
                var regenAmount = healthRegenAmountPerSecond;
                if (weakBody)
                {
                    regenAmount *= weakBody_HealthRegenMultiplier;
				}
				Heal(regenAmount * Time.deltaTime);
            }
        }
    }



    
    public virtual void TakeDamage(DamagePackage damagePackage)
    {


        if (currentHeath <= 0)
        {
            return;
        }

        if (Time.time - spawnTime < spawnInvulnerabilityTime)
        {
            // if we are in the spawn invulnerability time, ignore the damage
            return;
        }

        currentHeath -= damagePackage.damageAmount;
        if (currentHeath <= 0)
        {
            currentHeath = 0;
            Die();
        }
        else
        {
            if (hasHealthRegen)
            {
                healthRegenTimer = healthRegenDelay;
            }
        }
        OnHealthChanged?.Invoke(HealthPercentage);
        OnDamageTaken?.Invoke(damagePackage);
    }

    public void Heal(float healAmount)
    {
        if (currentHeath <= 0)
        {
            return;
        }

        currentHeath += healAmount;
        if (currentHeath > maxHeath)
        {
            currentHeath = maxHeath;
        }

        OnHealthChanged?.Invoke(HealthPercentage);
    }
    protected virtual void Die()
    {
        OnPreDeath?.Invoke();
        OnDeath?.Invoke();
        

    }

    public float HealthPercentage => currentHeath / maxHeath;

}
