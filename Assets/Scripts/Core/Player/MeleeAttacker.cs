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


    [NonSerialized]
    public bool InLaunch;
	[NonSerialized]
	public LaunchInstance launchInstance;
	[NonSerialized]
	public float launchTimer = 0f; // timer for launch, can be set by other scripts if needed

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

    public GameObject GetClosesLaunchTarget(PlayerMeleeAttack attackData)
    {


		var colliders = Physics.OverlapSphere(transform.position, attackData.launchDistance, attackData.launchTargetLayer);
        Transform closesTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.gameObject == self)
            {
                continue;
            }
            

			// Check if the collider is in angle
			Vector3 hitDirection = (collider.transform.position - transform.position).normalized;
			float angle = Vector3.Angle(transform.forward, hitDirection);
            if (angle > attackData.launchAngle)
            {
                continue;
			}


			float distance = Vector3.Distance(transform.position, collider.transform.position);
			if (distance < closestDistance)
            {
                closestDistance = distance;
                closesTarget = collider.transform;
            }
		}

        return closesTarget != null ? closesTarget.gameObject : null;
	}


    public void SetUpLaunch(GameObject target, PlayerMeleeAttack attackData)
    {
        if (target == null)
        {
            return;
        }
        InLaunch = true;

        var direction = (target.transform.position - self.transform.position).normalized;
		var targetPosition = target.transform.position - direction * attackData.launchStopDistance;

		launchInstance = new LaunchInstance(attackData, target, self.transform.position, targetPosition);
        launchTimer = attackData.launchTime;
	}
    


	bool isDualWielding = false;
	public void AttackStart(PlayerMeleeAttack attackData, float timeMultiplier, bool isDualWielding)
    {
        meleeData = attackData;
        attackDelay = meleeData.Delay * timeMultiplier;
        this.isDualWielding = isDualWielding;
		OnAttackStart?.Invoke(attackData);

        if (attackData.hasLaunch)
        {
			var launchTarget = GetClosesLaunchTarget(attackData);
			if (launchTarget != null)
			{
				SetUpLaunch(launchTarget, attackData);
			}
		}
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
        if (InLaunch)
        {
            UpdateLaunch();
		}

    }

    public void UpdateLaunch()
    {
        launchTimer -= Time.deltaTime;
        if (launchTimer <= 0)
        {
            InLaunch = false;
            launchTimer = 0f;
		}

        var launchProgress = 1f - (launchTimer / launchInstance.meleeAttack.launchTime);
        launchProgress = launchInstance.meleeAttack.launchCurve.Evaluate(launchProgress);

        var targetPosition = Vector3.Lerp(launchInstance.originalPosition, launchInstance.targetPosition, launchProgress);
        self.transform.position = targetPosition;


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

                var playerImpact = collider.GetComponent<PlayerPhysicsImpulse>();
				PlayerImpactStruct playerImpactStruct = new PlayerImpactStruct();
                var directionToPlayer = (collider.transform.position - transform.position).normalized;
                playerImpactStruct.impactForce = directionToPlayer * attackData.ForceOnPlayers;
                playerImpactStruct.resetGravity = false;
                playerImpact.AddImpulse(playerImpactStruct);

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


public struct LaunchInstance
{
	public PlayerMeleeAttack meleeAttack;
	public GameObject target;
    public Vector3 originalPosition;
    public Vector3 targetPosition;
    

    public LaunchInstance(PlayerMeleeAttack meleeAttack, GameObject target, Vector3 originalPosition, Vector3 targetPosition)
    {
        this.meleeAttack = meleeAttack;
        this.target = target;
        this.originalPosition = originalPosition;
        this.targetPosition = targetPosition;
	}
}
