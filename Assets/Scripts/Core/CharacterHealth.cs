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

	[SerializeField] float shildPopDamageNegationWhenArmored = 25;
	[SerializeField] float shildRegenDelay = 5;
    [SerializeField] float shildRegenAmountPerSecond = 20;
    [SerializeField] float maxArmor = 100;
    [SerializeField] bool armorBeforeShild = false;

	float currentArmor = 0;
	float shildRegenTimer;

    [Header("References")]
    [SerializeField] HeadShotArea headShotArea;
    [SerializeField] RagdollTrigger ragdollTrigger;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerBodyStatSheet statSheet;
    [SerializeField] GameObject shildBreakParticle;
    [SerializeField] GameObject deathParticle;
    [SerializeField] BodyMindConnection body;





    [Header("Sound")]
    [SerializeField] EventReference shildEmptySound;
    EventInstance shildEmptySoundInstance;
    [SerializeField] EventReference shildRechargeSound;
    EventInstance shildRechargeSoundInstance;
    [SerializeField] EventReference shildPopSound;

	[SerializeField] EventReference deathSound;

    public RumbleData rumbleHit;
    public RumbleData rumbleDeath;


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
    public Action OnInHealthAura;
    public Action OnMeleeHit;

    public Action<float> OnArmorChanged;



	float maxShildMultiplier = 1;

    public float MaxShild => maxShild * maxShildMultiplier;

    public RagdollTrigger RagdollTrigger => ragdollTrigger;


    [NonSerialized]
    public float aura_DamageReduction = 0.0f;
	[NonSerialized]
	public float aura_shildRegenDelay = 0.0f;
    [NonSerialized]
    public float aura_poisonDamage = 0.0f;
	[NonSerialized]
	public float aura_armorHeal = 0.0f;

	
    public void RemoveArmor()
    {
        currentArmor = 0;
        OnArmorChanged?.Invoke(0);
	}
	public float ArmorValue     {
        get { return currentArmor / maxArmor; }
	}
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

    public void SetDamageMultiplier(float value)
        {
        damageMultiplier = value;
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

    public void InShild()
    {
        OnInHealthAura?.Invoke();

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

		if (MapLoader.instance != null && body.Mind != null)
            damageMultiplier = MapLoader.instance.GetDamageMultiplier();
    }

    // update
    public override void Update()
    {
        

        base.Update();
        if (shildRegenTimer > 0 && hasShild)
        {
            shildRegenTimer -= Time.deltaTime  * (1 + aura_shildRegenDelay );
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


        if (aura_poisonDamage != 0)
        {
            var damagePackage = new DamagePackage
            {
                damageAmount = aura_poisonDamage * Time.deltaTime,
                owner = ownerOfLastDamage,
                canHeadShotShild = false,
                headShotMultiplier = 1f,
                shildDamageMultiplier = 1f
                
            };
            damagePackage.damageReductionAgainstBlock = 0;
            damagePackage.noScreenShake = true; // no screen shake for poison damage

			TakeDamage(damagePackage);
		}
        if (aura_armorHeal !=0)
        {
            GainArmor(aura_armorHeal * Time.deltaTime);
		}
        
    }

    public void GainArmor(float armorGain)
    {
        currentArmor += armorGain;
		
		if (currentArmor> maxArmor)
            currentArmor = maxArmor;
        else
            OnArmorChanged?.Invoke(maxArmor == 0 ? 0 :  currentArmor / maxArmor);
	}


    public void GainShild(float amount)
    {
        currentShild += amount;
		currentShild = Mathf.Clamp(currentShild, 0, MaxShild);
		OnShildChanged?.Invoke(ShildPercentage);

        shildRegenTimer = 0;
		shildRechargeSoundInstance.start();
		OnShildHealStarted?.Invoke();
	}

    public void SetHealthOverride(HealthOverride newHealth)
    {
        if (newHealth == null ||!newHealth.hasHealthOverride) return;
        maxHeath = newHealth.health;
        maxShild = newHealth.shild;
        maxArmor = newHealth.armor;
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

    [NonSerialized]
    public GameObject ownerOfLastDamage = null;
	float firstShotTime = 0;

    public override void TakeDamage(DamagePackage damagePackage)
    {
        if (currentShild == maxShild)
        {
            firstShotTime = Time.time;
        }

		float damageReduction = playerArms.DamageReduction;
		
		float damage = damagePackage.damageAmount * damageMultiplier *(1 - damageReduction - aura_DamageReduction);
        bool blocked = false;
		if (
            playerArms.RightArm.IsInZoom 
            && playerArms.RightArm.CurrentWeapon != null 
            && playerArms.RightArm.CurrentWeapon.Data.HasBlock 
            && playerArms.RightArm.CurrentWeapon.Data.DamageBlock.IsBlocking(transform, damagePackage.isMeleeDamage ? damagePackage.owner.transform.position : damagePackage.origin))
        {
			damage *= (1 - playerArms.RightArm.CurrentWeapon.Data.DamageBlock.blockPercentage * damagePackage.damageReductionAgainstBlock);
            blocked = true;

		}
        else if (
			playerArms.LeftArm.IsInZoom
			&& playerArms.LeftArm.CurrentWeapon != null
			&& playerArms.LeftArm.CurrentWeapon.Data.HasBlock
			&& playerArms.LeftArm.CurrentWeapon.Data.DamageBlock.IsBlocking(transform, damagePackage.isMeleeDamage ? damagePackage.owner.transform.position : damagePackage.origin))
        {
			damage *= (1 - playerArms.LeftArm.CurrentWeapon.Data.DamageBlock.blockPercentage * damagePackage.damageReductionAgainstBlock);
			blocked = true;
		}

		

			if (damage == 0) return;

			TargetHitCollector damageDealer = null;
        if (damagePackage.owner != null)
        {
            damageDealer = damagePackage.owner.GetComponent<TargetHitCollector>();
        }
        if (!blocked &&((currentShild <= 0 && currentArmor <=0) ||(  damagePackage.canHeadShotShild && currentShild + currentArmor < damage* damagePackage.shildDamageMultiplier)) && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint))
        {
            damage *= damagePackage.headShotMultiplier;
            if (headShotOneShot)
            {
                damage *= 100f;
            }
        }

        if (!damagePackage.noScreenShake)
        {
			OnDamageTakenUnityEvent?.Invoke();

            
        }
		if (playerArms.LeftArm.CurrentWeapon != null && playerArms.LeftArm.CurrentWeapon.Data.BloomOnTakingDamage)
		{
			playerArms.LeftArm.CurrentWeapon.TriggerBloom();
		}
		if (playerArms.RightArm.CurrentWeapon != null && playerArms.RightArm.CurrentWeapon.Data.BloomOnTakingDamage)
		{
			playerArms.RightArm.CurrentWeapon.TriggerBloom();
		}
       


		shildRechargeSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (hasShild && currentShild > 0)
        {

            var damageAgainstShild = damage * damagePackage.shildDamageMultiplier;

			if ( currentArmor > 0 && armorBeforeShild)
			{
				if (damageAgainstShild >= currentArmor)
				{
					damageAgainstShild -= currentArmor;
					currentArmor = 0;
					OnArmorChanged?.Invoke(0);
				}
				else
				{
					currentArmor -= damageAgainstShild;
					damageAgainstShild = 0;
					OnArmorChanged?.Invoke(maxArmor == 0 ? 0 : currentArmor / maxArmor);
				}
			}

            if (damageAgainstShild >0)
            {
				if (damageAgainstShild >= currentShild)
				{
					var damageNegation = shildPopDamageNegation;
					if (currentArmor > 0)
					{
						damageNegation = shildPopDamageNegationWhenArmored;
					}


					damageAgainstShild -= currentShild + damageNegation;
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

			
        }
        bool hasHealthDamage = false;
        if (damage > 0)
		{
            if (currentArmor > 0)
            {
                // if we have armor, reduce the damage by armor
                
                if (damage >= currentArmor)
                {
					damage -= currentArmor;
                    currentArmor = 0;
                    OnArmorChanged?.Invoke(0);
                }
                else
                {
                    currentArmor -= damage;
                    damage = 0;
                    OnArmorChanged?.Invoke(maxArmor == 0 ? 0 : currentArmor / maxArmor);
				}
			}
			if (damage > 0)
			{
				hasHealthDamage = true;
				currentHeath -= damage;
				OnHealthChanged?.Invoke(HealthPercentage);
				OnHealthDamageTaken?.Invoke();
			}


			
        }

		if (damagePackage.isMeleeDamage)
		{
			OnMeleeHit?.Invoke();
		}

		if (currentHeath <= 0)
        {
			if (body.Mind != null)
			{
				RumbleManager.Instance.TriggerRumble(rumbleDeath, body.Mind.playerID);
			}


			currentHeath = 0;
            if (damageDealer != null)
                damageDealer.CharacterKill(damagePackage, gameObject);
            Die(damagePackage);
        }
        else
        {
			if (body.Mind != null)
			{
				RumbleManager.Instance.TriggerRumble(rumbleHit, body.Mind.playerID);
			}

			if (damageDealer != null)
                damageDealer.CharacterHit(damagePackage, gameObject);
            if (hasHealthRegen && hasHealthDamage)
            {
                healthRegenTimer = healthRegenDelay;
            }
            shildRegenTimer = shildRegenDelay;


            

        }

        OnDamageTaken?.Invoke(damagePackage);
		ownerOfLastDamage = damagePackage.owner;


	}

    public void FillArmor()
        {
        currentArmor = maxArmor;
        OnArmorChanged?.Invoke(maxArmor == 0 ? 0 : currentArmor / maxArmor);
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
        if (GetComponent<BodyMindConnection>().Mind != null)
		    AudioManager.instance.PlayOneShot(deathSound, transform.position);

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


[Serializable]
public class Block
{
    [Range(0f,1f)]
    public float blockPercentage = 0.5f;
    [Range(0f,360f)]
    [SerializeField] float blockAngle = 0;
    public bool IsBlocking(Transform self, Vector3 damagePosition)
    {
        Vector3 directionOfAttack = (damagePosition - self.transform.position).normalized;
        var forward = self.transform.forward;
        var angle = Vector3.Angle(forward, directionOfAttack);
        return angle <= blockAngle / 2f;
    }
}