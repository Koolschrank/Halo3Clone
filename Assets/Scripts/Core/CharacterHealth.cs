using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;
using NUnit.Framework.Internal;

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
    float shildRegenTimer;

    [Header("References")]
    [SerializeField] HeadShotArea headShotArea;
    [SerializeField] RagdollTrigger ragdollTrigger;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerBodyStatSheet statSheet;
    [SerializeField] GameObject shildBreakParticle;
    [SerializeField] GameObject deathParticle;





    [Header("Sound")]
    [SerializeField] EventReference shildEmptySound;
    EventInstance shildEmptySoundInstance;
    [SerializeField] EventReference shildRechargeSound;
    EventInstance shildRechargeSoundInstance;
    [SerializeField] EventReference shildPopSound;


    public Action<float> OnShildChanged;
    public Action<float> OnMaxShildChanged;
    public Action OnShildDepleted;
    public UnityEvent OnDamageTakenUnityEvent;
    public Action OnShildEnabled;
    public Action OnShildDisabled;
    public Action OnShildDamageTaken;
    public Action OnHealthDamageTaken;
    public Action OnShildRechargeStarted;

    public Action OnShildHealStarted;
    


    float maxShildMultiplier = 1;

    public float MaxShild => maxShild * maxShildMultiplier;

    public RagdollTrigger RagdollTrigger => ragdollTrigger;

	public void MultiplyHealth(float multiplier)
    {
        maxHeath *= multiplier;
        currentHeath = maxHeath;

        OnMaxHealthChanged?.Invoke(maxHeath);
    }

    public void MultiplyShild(float multiplier)
    {
        maxShild *= multiplier;
        currentShild = maxShild;

        OnMaxShildChanged?.Invoke(MaxShild);
    }

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

    public void IncreaseMaxHealth(float amount)
    {
        maxHeath += amount;
        currentHeath = Mathf.Clamp(currentHeath + amount, 0, maxHeath);
        OnHealthChanged?.Invoke(HealthPercentage);
        OnMaxHealthChanged?.Invoke(maxHeath);
    }

    public void IncreaseHealthRegen(float amount)
    {
        healthRegenAmountPerSecond += amount;
        hasHealthRegen = healthRegenAmountPerSecond > 0;
    }

    public void ReduceShildRegenTime(float amountOfReduction)
    {
        shildRegenDelay -= amountOfReduction;
    }

    public void SetHeadShotOneShot(bool headShotOneShot)
    {
        this.headShotOneShot = headShotOneShot;
    }

    private void Awake()
    {
        if (statSheet != null)
        {
            statSheet.OnStatSheetUpdated += SetStatSheet;
        }
    }

    public void SetStatSheet()
    {
        if (!statSheet.useStatSheet) return;

        var statSheetInstance = statSheet.playerStatsSheetInstance;
        var maxHealthChange = statSheetInstance.maxHealth - maxHeath;

        maxHeath = statSheetInstance.maxHealth;
        currentHeath = Mathf.Clamp(currentHeath + maxHealthChange, 0, maxHeath);

        healthRegenAmountPerSecond = statSheetInstance.healthRegenPerSecond;
        healthRegenDelay = statSheetInstance.healthRegenDelay;
        maxShild = statSheetInstance.maxShild;
        currentShild = Mathf.Clamp(currentShild, 0, maxShild);
        hasShild = maxShild > 0;
        shildRegenAmountPerSecond = statSheetInstance.shieldRegenPerSecond;
        shildRegenDelay = statSheetInstance.shieldRegenDelay;

        OnMaxShildChanged?.Invoke(MaxShild);
        OnMaxHealthChanged?.Invoke(maxHeath);
    }


    protected override void Start()
    {
        base.Start();
        if (setMaxHeathOnStart)
            currentShild = MaxShild;

        shildEmptySoundInstance = RuntimeManager.CreateInstance(shildEmptySound);
        shildRechargeSoundInstance = RuntimeManager.CreateInstance(shildRechargeSound);

        if (MapLoader.instance != null)
            damageMultiplier = MapLoader.instance.GetDamageMultiplier();
    }

    // update
    public override void Update()
    {
        if (dead)
            return;

        base.Update();
        if (shildRegenTimer > 0 && hasShild)
        {
            shildRegenTimer -= Time.deltaTime;
            shildEmptySoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            if (shildRegenTimer <= 0)
            {
                shildRechargeSoundInstance.start();
                OnShildHealStarted?.Invoke();

			}
        }
        else if (currentShild < MaxShild && hasShild)
        {

            if (currentShild == 0)
            {
                shildEmptySoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                OnShildRechargeStarted?.Invoke();



            }
            shildRechargeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            currentShild += shildRegenAmountPerSecond * Time.deltaTime;
            currentShild = Mathf.Clamp(currentShild, 0, MaxShild);
            OnShildChanged?.Invoke(ShildPercentage);

        }

        
    }

    public void SetHealthOverride(HealthOverride newHealth)
    {
        if (newHealth == null ||!newHealth.hasHealthOverride) return;
        maxHeath = newHealth.health;
        maxShild = newHealth.shild;
        currentHeath = maxHeath;
        currentShild = maxShild;
        hasShild = maxShild > 0;
        healthRegenAmountPerSecond = newHealth.healthRegen;
        hasHealthRegen = healthRegenAmountPerSecond > 0;
        healthRegenDelay = newHealth.healthRegenStartTime;

        shildRegenAmountPerSecond = newHealth.shildRegen;
        shildRegenDelay = newHealth.shildRegenStartTime;

        spawnInvulnerabilityTime = newHealth.spawnInvulnerabilityTime;
        shildPopDamageNegation = newHealth.shildPopDamageNegation;

        if (newHealth.showHealthBar)
        {
            OnShowHealthBar?.Invoke();
            Debug.Log("Health bar shown for " + gameObject.name);
        }

        OnMaxShildChanged?.Invoke(MaxShild);


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
        if ((currentShild <= 0 ||(  damagePackage.canHeadShotShild && currentShild < damage)) && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint))
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
                damageAgainstShild -= currentShild + shildPopDamageNegation;
                damage = damageAgainstShild / damagePackage.shildDamageMultiplier;
                currentShild = 0;
                OnShildChanged?.Invoke(0);
                OnShildDamageTaken?.Invoke();
                OnShildDepleted?.Invoke();
                shildBreakParticle.SetActive(true);
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
        bool hasHealthDamage = false;
        if (damage > 0)
        {
            hasHealthDamage = true;
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
            if (hasHealthRegen && hasHealthDamage)
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


        deathParticle.SetActive(true);
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
