using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;
using NUnit.Framework.Internal;
using Fusion;

public class CharacterHealth : Health
{

    [SerializeField] float damageMultiplier = 1;
    [SerializeField] [Networked] bool hasShild { get; set; } = true;
    [SerializeField][Networked] bool headShotOneShot { get; set; } = true;

    [SerializeField][Networked] float maxShild { get; set; } = 100;
    [SerializeField][Networked, OnChangedRender(nameof(UpdateShildVisual))] float currentShild { get; set; } = 100;
    [SerializeField][Networked] float shildRegenDelay { get; set; } = 5;
    [SerializeField][Networked] float shildRegenAmountPerSecond { get; set; } = 20;

    [Networked] TickTimer shildRegenDelayTimer { get; set; }

    [SerializeField] HeadShotArea headShotArea;
    [SerializeField] RagdollTrigger ragdollTrigger;
    [SerializeField] ArmsExtended playerArms;



    [Header("Sound")]
    [SerializeField] EventReference shildEmptySound;
    EventInstance shildEmptySoundInstance;
    [SerializeField] EventReference shildRechargeSound;
    EventInstance shildRechargeSoundInstance;
    [SerializeField] EventReference shildPopSound;


    public Action<float> OnShildChanged;
    public Action OnShildDepleted;
    public UnityEvent OnDamageTakenUnityEvent;
    public Action OnShildEnabled;
    public Action OnShildDisabled;
    public Action OnShildDamageTaken;
    public Action OnHealthDamageTaken;
    public Action OnShildRechargeStarted;


    [Networked] float maxShildMultiplier { get; set; } = 1;

    public float MaxShild => maxShild * maxShildMultiplier;

    public void SetHasShild(bool hasShild)
    {
        if (this.hasShild == hasShild)
            return;

        this.hasShild = hasShild;
        if (!hasShild)
        {
            currentShild = 0;
            maxShildMultiplier = 0;
            OnShildDisabled?.Invoke();

        }
        else
        {
            currentShild = maxShild;
            maxShildMultiplier = 1;
            OnShildEnabled?.Invoke();
        }


    }

    public void SetHeadShotOneShot(bool headShotOneShot)
    {
        this.headShotOneShot = headShotOneShot;
    }

    public override void Spawned()
    {
        base.Spawned();
        if (setMaxHeathOnStart)
            currentShild = MaxShild;

        shildEmptySoundInstance = RuntimeManager.CreateInstance(shildEmptySound);
        shildRechargeSoundInstance = RuntimeManager.CreateInstance(shildRechargeSound);
    }

    public void UpdateShildVisual()
    {
        OnShildChanged?.Invoke(ShildPercentage);
    }

    // update 
    public override void FixedUpdateNetwork()
    {
        if (dead)
            return;

        base.FixedUpdateNetwork();

        if (hasShild)
        {
            if (currentShild == 0)
            {
                shildEmptySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

                if (shildRegenDelayTimer.Expired(Runner))
                {
                    shildRegenDelayTimer = TickTimer.None;
                    shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    shildRechargeSoundInstance.start();
                    currentShild += shildRegenAmountPerSecond * Runner.DeltaTime;
                }
            }
            else if (currentShild < maxShild)
            {
                shildRechargeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                if (shildRegenDelayTimer.Expired(Runner))
                {
                    shildRegenDelayTimer = TickTimer.None;
                    shildRechargeSoundInstance.start();
                }

                if (shildRegenDelayTimer.ExpiredOrNotRunning(Runner))
                {
                    currentShild += shildRegenAmountPerSecond * Runner.DeltaTime;
                    currentShild = Mathf.Clamp(currentShild, 0, MaxShild);
                    OnShildChanged?.Invoke(ShildPercentage);
                }

                
            }
        }


        

        
    }



    public bool IsHeadAreaCloserThanMainBody(Vector3 hitPoint)
    {
        var headPosition = headShotArea.transform.position;
        var bodyPosition = transform.position;

        var headDistance = Vector3.Distance(headPosition, hitPoint);
        var bodyDistance = Vector3.Distance(bodyPosition, hitPoint);
        return headDistance < bodyDistance;
    }

    public Transform GetHead()
    {
        return headShotArea.transform;
    }

    float firstShotTime = 0;

    public override void TakeDamage(DamagePackage damagePackage)
    {
        if (currentShild == maxShild)
        {
            firstShotTime = Time.time;
        }

        float damageReduction = playerArms.DamageReduction;
        float damage = damagePackage.damageAmount * damageMultiplier *(1 - damageReduction);

        TargetHitCollector damageDealer = null;
        if (damagePackage.owner != null)
        {
            damageDealer = damagePackage.owner.GetComponent<TargetHitCollector>();
        }
        if ((currentShild <= 0 || damagePackage.canHeadShotShild) && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint))
        {
            damage *= damagePackage.headShotMultiplier;
            if (headShotOneShot)
            {
                damage *= 100f;
            }
        }

        OnDamageTakenUnityEvent?.Invoke();


        shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (hasShild && currentShild > 0)
        {

            var damageAgainstShild = damage * damagePackage.shildDamageMultiplier;

            if (damageAgainstShild >= currentShild)
            {
                damageAgainstShild -= currentShild;
                damage = damageAgainstShild / damagePackage.shildDamageMultiplier;
                currentShild = 0;
                OnShildChanged?.Invoke(0);
                OnShildDamageTaken?.Invoke();
                OnShildDepleted?.Invoke();
                AudioManager.instance.PlayOneShot(shildPopSound, transform.position);

                shildEmptySoundInstance.start();

            }
            else
            {
                OnShildDamageTaken?.Invoke();
                currentShild -= damageAgainstShild;
                damage = 0;
                OnShildChanged?.Invoke(ShildPercentage);

            }
        }
        if (damage > 0)
        {
            currentHeath -= damage;
            OnHealthChanged?.Invoke(HealthPercentage);
            OnHealthDamageTaken?.Invoke();
        }



        if (currentHeath <= 0)
        {
            currentHeath = 0;
            if (damageDealer != null)
                damageDealer.CharacterKill(damagePackage, gameObject);
            Die(damagePackage);
        }
        else
        {
            if (damageDealer != null)
                damageDealer.CharacterHit(damagePackage, gameObject);
            if (hasHealthRegen)
            {
                healthRegenDelayTimer = TickTimer.CreateFromSeconds(Runner, healthRegenDelay);
            }
            if (hasShild)
            {
                shildRegenDelayTimer = TickTimer.CreateFromSeconds(Runner, shildRegenDelay);
            }



        }

        OnDamageTaken?.Invoke(damagePackage.origin);

    }

    bool dead = false;
    protected void Die(DamagePackage damagePackage)
    {
        base.Die();
        ragdollTrigger.Activate(damagePackage);

        dead = true;

        shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        float timeToKill = Time.time - firstShotTime;
        Debug.Log("Time to kill: " + timeToKill);
    }

    public float ShildPercentage
    {
        get
        {
            return currentShild / maxShild;
        }
    }

    
}
