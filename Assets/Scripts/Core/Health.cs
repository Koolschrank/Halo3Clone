using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // unity event on death
    public Action OnDeath;

   

    [SerializeField] protected float maxHeath;
    [SerializeField] protected float currentHeath;
    [SerializeField] protected bool setMaxHeathOnStart = true;
    

    [SerializeField] protected bool hasHealthRegen;
    [SerializeField] protected float healthRegenDelay;
    protected float healthRegenTimer;
    [SerializeField] protected float healthRegenAmountPerSecond;


    public float MaxHeath => maxHeath;
    public float CurrentHeath => currentHeath;

    // action health change
    public Action<float> OnHealthChanged;
    public Action<DamagePackage> OnDamageTaken;

    public bool IsDead => currentHeath <= 0;

    protected virtual void Start()
    {
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
                healthRegenTimer -= Time.deltaTime;
            }
            else
            {
                Heal(healthRegenAmountPerSecond * Time.deltaTime);
            }
        }
    }



    
    public virtual void TakeDamage(DamagePackage damagePackage)
    {
        if (currentHeath <= 0)
        {
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
        OnDeath?.Invoke();
        

    }

    public float HealthPercentage => currentHeath / maxHeath;

}
