using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class CharacterHealth : Health
{
   

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


    protected override void Start()
    {
        base.Start();
        if (setMaxHeathOnStart)
            currentShild = maxShild;

        shildEmptySoundInstance = RuntimeManager.CreateInstance(shildEmptySound);
        shildRechargeSoundInstance = RuntimeManager.CreateInstance(shildRechargeSound);
    }

    // update
    public override void Update()
    {
        if (dead)
            return;

        base.Update();
        if(shildRegenTimer > 0)
        {
            shildRegenTimer -= Time.deltaTime;
            shildEmptySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            if (shildRegenTimer <= 0)
            {
                shildRechargeSoundInstance.start();
            }
        }
        else if (currentShild < maxShild)
        {

            if (currentShild == 0)
            {
                shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                
                

            }
            shildRechargeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            currentShild += shildRegenAmountPerSecond * Time.deltaTime;
            currentShild = Mathf.Clamp(currentShild, 0, maxShild);
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
        float damage = damagePackage.damageAmount;
        var damageDealer = damagePackage.owner.GetComponent<TargetHitCollector>();
        if ( currentShild <= 0 && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint) )
        {
            damage *= damagePackage.headShotMultiplier;
        }

        OnDamageTakenUnityEvent?.Invoke();


        shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if ( currentShild > 0 ) {

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
            damageDealer.CharacterKill(gameObject);
            Die(damagePackage);
        }
        else
        {
            damageDealer.CharacterHit(gameObject);
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
        ragdollTrigger.Activate(damagePackage);
        base.Die();
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
