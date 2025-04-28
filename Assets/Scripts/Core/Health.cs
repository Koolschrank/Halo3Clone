using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    // unity event on death
    public Action OnDeath;
    public Action<float> OnHealthChanged;
    public Action<Vector3> OnDamageTaken;




    [SerializeField] [Networked] protected float maxHeath { get; set; } = 100f;
    [SerializeField][Networked] protected float currentHeath { get; set; } = 100f;
    [SerializeField] protected bool setMaxHeathOnStart = true;
    

    [SerializeField][Networked] protected bool hasHealthRegen { get; set; } = false;
    [SerializeField][Networked] protected float healthRegenDelay { get; set; } = 4.5f;
    [Networked] protected TickTimer healthRegenDelayTimer { get; set; }


    
    [SerializeField][Networked] protected float healthRegenAmountPerSecond { get; set; } = 20f;


    public float MaxHeath => maxHeath;
    public float CurrentHeath => currentHeath;

    // action health change
    

    public bool IsDead => currentHeath <= 0;

    public override void Spawned()
    {
        base.Spawned();
        if (hasHealthRegen)
        {
            healthRegenDelayTimer = TickTimer.CreateFromSeconds(Runner, healthRegenDelay);
        }
        if (setMaxHeathOnStart)
        {
            currentHeath = maxHeath;
            OnHealthChanged?.Invoke(HealthPercentage);
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (hasHealthRegen && healthRegenDelayTimer.ExpiredOrNotRunning(Runner))
        {
            Heal(healthRegenAmountPerSecond * Runner.DeltaTime);
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
                healthRegenDelayTimer = TickTimer.CreateFromSeconds(Runner, healthRegenDelay);
            }
            
        }
        OnHealthChanged?.Invoke(HealthPercentage);
        OnDamageTaken?.Invoke(damagePackage.origin);
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
