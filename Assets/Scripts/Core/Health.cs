using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    // unity event on death
    public Action OnDeath;



    
    [SerializeField] [Networked] protected float maxHeath { get; set; } = 100f;
    [SerializeField][Networked] protected float currentHeath { get; set; } = 100f;
    [SerializeField] protected bool setMaxHeathOnStart = true;
    

    [SerializeField][Networked] protected bool hasHealthRegen { get; set; } = false;
    [SerializeField][Networked] protected float healthRegenDelay { get; set; } = 4.5f;
    [Networked] TickTimer healthRegenDelayTimer { get; set; }


    protected float healthRegenTimer;
    [SerializeField][Networked] protected float healthRegenAmountPerSecond { get; set; } = 20f;


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


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    void RPC_InitiateRightWeaponSwitch()
    {
        
    }

}
