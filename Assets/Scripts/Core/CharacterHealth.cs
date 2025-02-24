using System;
using UnityEngine;

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

    public Action<float> OnShildChanged;
    public Action OnShildDepleted;

    protected override void Start()
    {
        base.Start();
        if (setMaxHeathOnStart)
            currentShild = maxShild;
    }

    // update
    public override void Update()
    {
        base.Update();
        if(shildRegenTimer > 0)
        {
            shildRegenTimer -= Time.deltaTime;
        }
        else if (currentShild < maxShild)
        {
            

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
        if ( currentShild <= 0 && damagePackage.headShotMultiplier > 1 && headShotArea.IsHeadShot(damagePackage.hitPoint) )
        {
            damage *= damagePackage.headShotMultiplier;
        }

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
            Die(damagePackage);
        }
        else
        {
            if (hasHealthRegen)
            {
                healthRegenTimer = healthRegenDelay;
            }
            shildRegenTimer = shildRegenDelay;
        }
       
        OnDamageTaken?.Invoke(damagePackage);

    }

    protected void Die(DamagePackage damagePackage)
    {
        ragdollTrigger.Activate(damagePackage);
        base.Die();

    }

    public float ShildPercentage
    {
        get
        {
            return currentShild / maxShild;
        }
    }
}
