using UnityEngine;
using System;
using UnityEngine.Events;

public class MeleeAttacker : MonoBehaviour
{
    public Action<PlayerMeleeAttack> OnAttackStart;
    public Action<PlayerMeleeAttack> OnAttackHit;

    public UnityEvent OnMeleeHitEvent;

    [SerializeField] PlayerMovement playerMovement; // reference to the player movement for applying force
	[SerializeField] BodyMindConnection bodyMindConnection; // reference to the body mind connection for rumble
    [SerializeField] PlayerTeam playerTeam; // reference to the player team for team index
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
	[NonSerialized]
	public bool shildGainOnMelee = false; 

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
        if (playerMovement.inPushedState) return null;

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
		launchInstance = new LaunchInstance(attackData, target, self.transform.position);
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

        var direction = (launchInstance.target.transform.position - self.transform.position).normalized;
		var goalPosition = launchInstance.target.transform.position - direction * launchInstance.meleeAttack.launchStopDistance;

		var targetPosition = Vector3.Lerp(launchInstance.originalPosition, goalPosition, launchProgress);
        var distanceToTarget = Vector3.Distance(self.transform.position, launchInstance.target.transform.position);

		if ( distanceToTarget >= launchInstance.meleeAttack.launchStopDistance)
		{
			self.transform.position = targetPosition;
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

            if (collider.gameObject.tag == "AIEnemy")
            {
                damagePackage.damageAmount *= attackData.DamageMultiplierVSAI;
                if (shildGainOnMelee)
                {
                    this.health.SetShildRegenMelee();

                }
            }



            damagePackage.origin = hitPoint;
            // direction of self move 
            var direction = transform.forward;
            damagePackage.forceVector = direction * attackData.Force;
            damagePackage.owner = self;

            damagePackage.hitPoint = hitPoint;
            damagePackage.impactType = ImpactType.wholeBody;
            damagePackage.isMeleeDamage = true;
            damagePackage.isInstantNedler = attackData.nedlerMelee;


            if (collider.gameObject == self)
            {
                continue;
            }
            hits++;


            if (collider.TryGetComponent<CharacterHealth>(out CharacterHealth health))
            {
                if (health.gameObject.GetComponent<PlayerTeam>().TeamIndex == playerTeam.TeamIndex)
                {
                    damagePackage.damageAmount *= attackData.DamageMultiplierAgainstTeamMates;
                }


                health.TakeDamage(damagePackage);

                var playerImpact = collider.GetComponent<PlayerPhysicsImpulse>();
                PlayerImpactStruct playerImpactStruct = new PlayerImpactStruct();
                var forceDirection = (transform.forward + Vector3.up * attackData.ForceOffset).normalized; //(collider.transform.position - (transform.position+ Vector3.up * attackData.ForceOffset )).normalized;
                playerImpactStruct.impactForce = forceDirection * attackData.ForceOnPlayers;



                playerImpactStruct.resetGravity = attackData.launchResetsGravity;
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
            OnMeleeHitEvent?.Invoke();


            if (bodyMindConnection.Mind != null)
            {
                int playerIndex = bodyMindConnection.Mind.playerID;
                RumbleManager.Instance.TriggerRumble(meleeRumble_hit, playerIndex);
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

        if (attackData.spawnHitObject && attackData.hitObject != null)
        {
            var offset = transform.rotation * attackData.hitObjectOffset;
			var hitImpact =Instantiate(attackData.hitObject, hitPoint + offset, transform.rotation) as GameObject;
            var explosion = hitImpact.GetComponent<Explosion>();
            if (explosion != null)
            {
                explosion.Activate(self);
			}

		}
    }

    public void CancelLaunch()
    {
        InLaunch = false;
        launchTimer = 0f;
        launchInstance = new LaunchInstance();
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
    

    public LaunchInstance(PlayerMeleeAttack meleeAttack, GameObject target, Vector3 originalPosition)
    {
        this.meleeAttack = meleeAttack;
        this.target = target;
        this.originalPosition = originalPosition;
	}
}
