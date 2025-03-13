using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class CharacterHealth : Health
{

    [SerializeField] float damageMultiplier = 1; 
    [SerializeField] bool hasShild = true;
    [SerializeField] bool headShotOneShot = true;

    [SerializeField] float maxShild = 100;
    [SerializeField] float currentShild = 100;
    [SerializeField] float shildPopDamageNegation = 25;
    [SerializeField] float shildRegenDelay = 5;
    [SerializeField] float shildRegenAmountPerSecond = 20;
    [SerializeField] GameObject shildPopParticalPrefab;
    float shildRegenTimer;

    [SerializeField] HeadShotArea headShotArea;
    [SerializeField] RagdollTrigger ragdollTrigger;


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


    float maxShildMultiplier = 1;

    float MaxShild => maxShild * maxShildMultiplier;

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

        }else
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


    protected override void Start()
    {
        base.Start();
        if (setMaxHeathOnStart)
            currentShild = MaxShild;

        shildEmptySoundInstance = RuntimeManager.CreateInstance(shildEmptySound);
        shildRechargeSoundInstance = RuntimeManager.CreateInstance(shildRechargeSound);
    }

    // update
    public override void Update()
    {
        if (dead)
            return;

        base.Update();
        if(shildRegenTimer > 0 && hasShild)
        {
            shildRegenTimer -= Time.deltaTime;
            shildEmptySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            if (shildRegenTimer <= 0)
            {
                shildRechargeSoundInstance.start();
            }
        }
        else if (currentShild < MaxShild && hasShild)
        {

            if (currentShild == 0)
            {
                shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                
                

            }
            shildRechargeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            currentShild += shildRegenAmountPerSecond * Time.deltaTime;
            currentShild = Mathf.Clamp(currentShild, 0, MaxShild);
            OnShildChanged?.Invoke(ShildPercentage);

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

    public override void TakeDamage(DamagePackage damagePackage)
    {
        

        float damage = damagePackage.damageAmount * damageMultiplier;

        TargetHitCollector damageDealer = null;
        if (damagePackage.owner != null)
        {
            damageDealer = damagePackage.owner.GetComponent<TargetHitCollector>();
        }
        if ( (currentShild <= 0 || damagePackage.canHeadShotShild) && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint) )
        {
            damage *= damagePackage.headShotMultiplier;
            if (headShotOneShot)
            {
                damage *= 100f;
            }
        }

        OnDamageTakenUnityEvent?.Invoke();


        shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (hasShild && currentShild > 0) {

            var damageAgainstShild = damage * damagePackage.shildDamageMultiplier;

            if (damageAgainstShild >= currentShild)
            {
                damageAgainstShild -= currentShild + shildPopDamageNegation;
                damage = damageAgainstShild / damagePackage.shildDamageMultiplier;
                currentShild = 0;
                OnShildChanged?.Invoke(0);
                OnShildDepleted?.Invoke();
                Instantiate(shildPopParticalPrefab, transform.position, Quaternion.identity);
                AudioManager.instance.PlayOneShot(shildPopSound, transform.position);

                shildEmptySoundInstance.start();

            }
            else
            {
                
                currentShild -= damageAgainstShild;
                damage = 0;
                OnShildChanged?.Invoke(ShildPercentage);
            }
        }
        if (damage > 0)
        {
            currentHeath -= damage;
            OnHealthChanged?.Invoke(HealthPercentage);
        }
        


        if (currentHeath <= 0)
        {
            currentHeath = 0;
            if (damageDealer != null)
                damageDealer.CharacterKill(damagePackage,gameObject);
            Die(damagePackage);
        }
        else
        {
            if (damageDealer != null)
                damageDealer.CharacterHit(damagePackage, gameObject);
            if (hasHealthRegen)
            {
                healthRegenTimer = healthRegenDelay;
            }
            shildRegenTimer = shildRegenDelay;

            
        }
       
        OnDamageTaken?.Invoke(damagePackage);

    }

    bool dead = false;
    protected void Die(DamagePackage damagePackage)
    {
        base.Die();
        ragdollTrigger.Activate(damagePackage);
        
        dead = true;

        shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    public float ShildPercentage
    {
        get
        {
            return currentShild / maxShild;
        }
    }
}
