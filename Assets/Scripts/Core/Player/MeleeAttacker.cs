using UnityEngine;
using System;

public class MeleeAttacker : MonoBehaviour
{
    public Action<PlayerMeleeAttack> OnAttackStart;
    public Action<PlayerMeleeAttack> OnAttackHit;

    [SerializeField] BodyMindConnection bodyMindConnection; // reference to the body mind connection for rumble
	[SerializeField] CharacterHealth health;
    [SerializeField] GameObject self;
    [SerializeField] float velocityYOffset = 0.5f;
    PlayerMeleeAttack meleeData;
    float attackDelay = 0f;
    [SerializeField] PlayerBodyStatSheet statSheet; // reference to the body stat sheet for damage calculation
    [SerializeField] float dualWieldingDamageMultiplier = 0.75f; // multiplier for dual wielding, can be set by other scripts if needed

	float damageMultiplier = 1f; // multiplier for damage, can be set by other scripts if needed

    [SerializeField] RumbleData meleeRumble_miss;
    [SerializeField] RumbleData meleeRumble_hit;

	private void Awake()
    {
        if (statSheet != null)
        {
            statSheet.OnStatSheetUpdated += SetStatSheet;
        }
    }

    public void SetStatSheet()
    {
        if (!statSheet.useStatSheet) 
        {
            return;
        }

        damageMultiplier = statSheet.playerStatsSheetInstance.meleeDamageMultiplier;
    }


    private void Start()
    {
        health.OnDeath += CancelAttack;



    }

    bool isDualWielding = false;
	public void AttackStart(PlayerMeleeAttack attackData, float timeMultiplier, bool isDualWielding)
    {
        meleeData = attackData;
        attackDelay = meleeData.Delay * timeMultiplier;
        this.isDualWielding = isDualWielding;
		OnAttackStart?.Invoke(attackData);

    }

    // update
    public void Update()
    {
        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0)
            {
                Attack(meleeData);
            }
        }
    }

    // attack
    public void Attack(PlayerMeleeAttack attackData)
    {
        var hitPoint = transform.position + transform.forward * attackData.MeleeDistance;
        var radius = attackData.MeleeRadius;
        var colliders = Physics.OverlapSphere(hitPoint, radius, attackData.EnemyLayer);

        if (colliders.Length == 0)
        {
            return;
        }
        int hits = 0;

        foreach (var collider in colliders)
        {
            DamagePackage damagePackage = new DamagePackage(attackData.Damage * damageMultiplier);

            if (isDualWielding)
            {
                damagePackage.damageAmount *= dualWieldingDamageMultiplier;
			}

			damagePackage.origin = hitPoint;
            // direction of self move 
            var direction = transform.forward;
            damagePackage.forceVector = direction * attackData.Force;
            damagePackage.owner = self;
           
            damagePackage.hitPoint = hitPoint;
            damagePackage.impactType = ImpactType.wholeBody;
            damagePackage.isMeleeDamage = true;


			if (collider.gameObject == self)
            {
                continue;
            }
            hits++;


            if (collider.TryGetComponent<Health>(out Health health))
            {
				
				health.TakeDamage(damagePackage);
                
            }

            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(transform.forward * attackData.Force, ForceMode.Impulse);
            }
        }

        if (hits > 0)
        {
            OnAttackHit?.Invoke(attackData);


            if (bodyMindConnection.Mind != null)
            {
                int playerIndex = bodyMindConnection.Mind.playerID;
                RumbleManager.Instance.TriggerRumble (meleeRumble_hit, playerIndex);
			}
        }
        else
        {
            if (bodyMindConnection.Mind != null)
            {
                int playerIndex = bodyMindConnection.Mind.playerID;
                RumbleManager.Instance.TriggerRumble(meleeRumble_miss, playerIndex);
			}
		}

    }

    public void CancelAttack()
    {
        attackDelay = 0;
    }


}
