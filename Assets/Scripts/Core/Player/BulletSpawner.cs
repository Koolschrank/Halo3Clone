using Swan;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public Action<Transform> OnTargetAcquired;
    public Action<Transform> OnTargetLost;


    Transform target;


    [SerializeField] Transform mainTransform;
    [SerializeField] LayerMask autoAimLayerMask;
    [SerializeField] RightArm rightArm;
    [SerializeField] LeftArm leftArm;
    //[SerializeField] PlayerArms playerArms;
    

    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] GranadeThrower granadeThrower;
    [SerializeField] LayerMask wallCheck;

    float accuracyMultiplier = 1f;
    bool onlyEnemyIsPlayerTeam = false;

    public bool CannotSpawnBullets = false;


    public float AccuracyMultiplier
    {
        get { return accuracyMultiplier; }
        set { accuracyMultiplier = value; }
    }


    public void SetOnlyEnemyIsPlayerTeam(bool onlyEnemyIsPlayerTeam)
    {
        this.onlyEnemyIsPlayerTeam = onlyEnemyIsPlayerTeam;
    }

    bool ignoreDualWieldDamageMultiplier = false;
	private void Start()
	{
        var gamemodeStats = GameModeSelector.gameModeManager.GameModeStats;
		ignoreDualWieldDamageMultiplier = gamemodeStats.noDualWieldDamageReduction;

	}

	//AutoAim autoAimOfCurrentWeapon;

	/*
    public void Start()
    {
        playerArms.RightArm.OnWeaponEquipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = weapon.AutoAim;
        };

        if (playerArms.RightArm.CurrentWeapon != null)
        {
            autoAimOfCurrentWeapon = playerArms.RightArm.CurrentWeapon.AutoAim;
        }

        playerArms.RightArm.OnWeaponUnequipStarted += (weapon, time) =>
        {
            autoAimOfCurrentWeapon = null;
        };

        playerArms.RightArm.OnWeaponDroped += (weapon) =>
        {
            autoAimOfCurrentWeapon = null;
        };

    }*/

	public void Update()
    {
        Transform newTarget = null;
        var rightArmWeapon = rightArm.CurrentWeapon;
        if (rightArmWeapon != null)
        {
            var rightArmAutoAim = rightArm.CurrentWeapon.AutoAim;
            newTarget = GetAutoAimTarget(transform.position, transform.forward,rightArmAutoAim.Radius, rightArmAutoAim.RaycastLenght);
        }

        if (newTarget == null)
        {
            var leftArmWeapon = leftArm.CurrentWeapon;
            if (leftArmWeapon != null)
            {
                var leftArmAutoAim = leftArm.CurrentWeapon.AutoAim;
                newTarget = GetAutoAimTarget(transform.position, transform.forward, leftArmAutoAim.Radius, leftArmAutoAim.RaycastLenght);

            }

        }

           


        if (newTarget != target)
        {
            if (newTarget)
            {
                OnTargetLost?.Invoke(target);
                OnTargetAcquired?.Invoke(newTarget);
            }
            else
            {
                OnTargetLost?.Invoke(target);
            }
            target = newTarget;
        }
    }



    public Transform GetAutoAimTarget(Vector3 startPosition, Vector3 shootDirection,float radius, float lenght)
    {

        //sphere cast
        if (Physics.SphereCast(startPosition, radius, shootDirection, out RaycastHit hit, lenght, autoAimLayerMask))
        {
            // make a ray cast to check if there is a wall between player and target
            var direction = (hit.point - startPosition).normalized;
			if (Physics.Raycast(startPosition, direction, out RaycastHit wallHit, Vector3.Distance(startPosition, hit.point), wallCheck))
            {
                return null;
            }



            if (hit.collider.TryGetComponent<PlayerTeam>(out PlayerTeam pt))
            {
                if ( pt.TeamIndex == playerTeam.TeamIndex || (onlyEnemyIsPlayerTeam && playerTeam.TeamIndex == 0))
                {
                    
                    return null;
                }

            }

            if (hit.collider.TryGetComponent<CharacterHealth>(out CharacterHealth ch))
            {
                if (ch.IsHeadAreaCloserThanMainBody(hit.point))
                {
                    return ch.GetHead();
                }
            }

            return hit.transform;
        }

        return null;
    }


    public GameObject[] ShootGranade(Weapon_Arms weapon)
    {
       if (CannotSpawnBullets)
        {
            return new GameObject[0];
		}

		var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;

        Weapon_Bullet_Granade granade_data = weapon.Bullet as Weapon_Bullet_Granade;
        var forward = transform.forward;
        var target = GetAutoAimTarget(transform.position, transform.forward, autoAimRadius, autoAimRaycastLenght);
        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - transform.position), autoAimLerp).normalized;
        }

        int bulletCount = weapon.BulletsPerShot;
        GameObject[] granades = new GameObject[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            //var forwardForThisBullet = forward + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy;
            var inaccuracy = UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy * AccuracyMultiplier;
            // spawn projectile at transform position and rotate it to forward
            granades[i] = granadeThrower.ThrowGranadeWithWeapon(granade_data.GranadeStats, inaccuracy);

            
        }

        return granades;



    }

    public Vector3[] ShootHitScanAdvanced(Weapon_Arms weapon)
    {
		if (CannotSpawnBullets)
		{
			return new Vector3[0];
		}
		Weapon_Bullet_Hitscan bullet = weapon.Bullet as Weapon_Bullet_Hitscan;
		
		var autoAim = weapon.AutoAim;
		var autoAimRaycastLenght = autoAim.RaycastLenght;
		var autoAimRadius = autoAim.Radius;
		var autoAimLerp = autoAim.AimLerp;



		float range = bullet.Range;
		LayerMask hitLayer = bullet.HitLayer;

		float damageMultiplier = 1f;
		if (weapon.IsBeingDualWielded && !ignoreDualWieldDamageMultiplier)
		{
			damageMultiplier = weapon.DamageMultiplierWhenDualWielded;
		}

		if (GameModeSelector.gameModeManager.GameModeStats.team2LoosesScoreWhenTeam1scores)
		{
			damageMultiplier *= bullet.damageMultiplierVSAI;
		}

		DamagePackage damagePackage = new DamagePackage(bullet.Damage * damageMultiplier);
		damagePackage.origin = mainTransform.position;
		damagePackage.owner = mainTransform.gameObject;
		damagePackage.headShotMultiplier = bullet.HeadShotMultiplier;
		damagePackage.shildDamageMultiplier = bullet.ShildDamageMultiplier;
		damagePackage.canHeadShotShild = bullet.CanHeadShotShild;

		Transform cameraTransform = transform;
		var forward = cameraTransform.forward;
		var target = GetAutoAimTarget(transform.position, transform.forward, autoAimRadius, autoAimRaycastLenght);
		if (target)
		{
			forward = Vector3.Lerp(forward, (target.position - cameraTransform.position), autoAimLerp).normalized;
		}

		int bulletCount = weapon.BulletsPerShot;
		Vector3 shotDirection = forward;
		List<Vector3> hitPoints = new List<Vector3>();
        

		for (int i = 0; i < bulletCount; i++)
		{
			Vector3 lastWallHit = cameraTransform.position;
			Vector3 shotDirectionForThisBullet = shotDirection + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy * AccuracyMultiplier;

            var hits = DoRicochetRaycast(cameraTransform.position, shotDirectionForThisBullet, bullet.maxRicochetCount, bullet.Range, wallCheck, bullet.HitLayer, bullet.penetration, bullet.ricochetAutoAim);
            int hitIndex = 0;
			foreach (RaycastHit hit in hits)
			{
                bool noHit = hit.collider == null;
                if (noHit)
                {
                    hitPoints.Add(hit.point);
					hitIndex++; 
					break;
				}
				if (hit.collider.gameObject == playerTeam.gameObject && hitIndex == 0) // hit self
				{
                    continue;
				}

                var directionToHit = (hit.point - lastWallHit).normalized;
				damagePackage.forceVector = directionToHit * bullet.Force;
				damagePackage.hitPoint = hit.point;
				damagePackage.origin = lastWallHit;
				damagePackage.damageAmount = bullet.Damage * damageMultiplier * bullet.GetDamageFalloff(hit.distance);


				bool bodyHit = false;
				// if hit health
				if (hit.collider.TryGetComponent<Health>(out Health health))
				{
					health.TakeDamage(damagePackage);

					bodyHit = true;


					if (!health.IsDead && bullet.DoesApplyForceOnLivingPlayers)
					{
						var forceVector = shotDirectionForThisBullet.normalized * bullet.ForceOnLivingPlayers;
						if (hit.collider.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse playerImpulse))
						{
							playerImpulse.AddImpulse(new PlayerImpactStruct()
							{
								impactForce = forceVector,
								resetGravity = false
							});
						}
					}
				}
				else
				{
					// if layer is dead player layer
					if (hit.collider.gameObject.layer == PlayerManager.instance.GetDeadPlayerLayer())
					{
						bodyHit = true;
					}


				}
				if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
				{
					rb.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
				}

				if (bodyHit)
				{
					AudioManager.instance.PlayOneShot(bullet.BodyHitSound, hit.point);
					GameObject impact = Instantiate(bullet.ImpactBody, hit.point, Quaternion.identity);
					// get normal of hit point
					impact.transform.forward = hit.normal;

				}
				else
				{
                    lastWallHit = hit.point;

					AudioManager.instance.PlayOneShot(bullet.GroundHitSound, hit.point);
					GameObject impact = Instantiate(bullet.ImpactGround, hit.point, Quaternion.identity);

					impact.transform.forward = hit.normal;
					hitPoints.Add(hit.point);
				}
				
				hitIndex++;

			}
			if(hitPoints.Count == 0)
			{
				hitPoints.Add(cameraTransform.position + shotDirectionForThisBullet * range);
			}
		}

		return hitPoints.ToArray();
	}


	public Vector3[] ShootHitScan(Weapon_Arms weapon)
    {
        if (CannotSpawnBullets)
        {
            return new Vector3[0];
		}
		Weapon_Bullet_Hitscan bullet = weapon.Bullet as Weapon_Bullet_Hitscan;
		if (bullet.ricochet || bullet.penetration)
		{
            return ShootHitScanAdvanced(weapon);
		}
		var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;


        
        float range = bullet.Range;
        LayerMask hitLayer = bullet.HitLayer;

        float damageMultiplier = 1f;
        if (weapon.IsBeingDualWielded && !ignoreDualWieldDamageMultiplier)
        {
            damageMultiplier = weapon.DamageMultiplierWhenDualWielded;
        }

		if (GameModeSelector.gameModeManager.GameModeStats.team2LoosesScoreWhenTeam1scores)
		{
			damageMultiplier *= bullet.damageMultiplierVSAI;
		}

		DamagePackage damagePackage = new DamagePackage(bullet.Damage * damageMultiplier);
        damagePackage.origin = mainTransform.position;
        damagePackage.owner = mainTransform.gameObject;
        damagePackage.headShotMultiplier = bullet.HeadShotMultiplier;
        damagePackage.shildDamageMultiplier = bullet.ShildDamageMultiplier;
        damagePackage.canHeadShotShild = bullet.CanHeadShotShild;

        Transform cameraTransform = transform;
        RaycastHit hit;
        var forward = cameraTransform.forward;
        var target = GetAutoAimTarget(transform.position, transform.forward, autoAimRadius, autoAimRaycastLenght);




        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - cameraTransform.position), autoAimLerp).normalized;
        }

        int bulletCount = weapon.BulletsPerShot;
        Vector3 shotDirection = forward;
        Vector3[] hitPoints = new Vector3[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 shotDirectionForThisBullet = shotDirection + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy * AccuracyMultiplier;


            
           

            if (Physics.Raycast(cameraTransform.position, shotDirectionForThisBullet, out hit, range, hitLayer))
            {
                if (hit.collider.gameObject == playerTeam.gameObject) // hit self
                {
					// do it again but with a small offset to avoid hitting self
					Physics.Raycast(cameraTransform.position + shotDirectionForThisBullet *0.5f, shotDirectionForThisBullet, out hit, range, hitLayer);
				}


				damagePackage.forceVector = shotDirectionForThisBullet.normalized * bullet.Force;
                damagePackage.hitPoint = hit.point;

                damagePackage.damageAmount = bullet.Damage * damageMultiplier * bullet.GetDamageFalloff(hit.distance);

                bool bodyHit = false;
                // if hit health
                if (hit.collider.TryGetComponent<Health>(out Health health))
                {
                    health.TakeDamage(damagePackage);
                    
                    bodyHit = true;


                    if (!health.IsDead && bullet.DoesApplyForceOnLivingPlayers)
                    {
                        var forceVector = shotDirectionForThisBullet.normalized * bullet.ForceOnLivingPlayers;
                        if (hit.collider.TryGetComponent<PlayerPhysicsImpulse>(out PlayerPhysicsImpulse playerImpulse))
                        {
                            playerImpulse.AddImpulse(new PlayerImpactStruct()
                            {
                                impactForce = forceVector,
                                resetGravity = false
                            });
						}
					}
                }
                else
                {
                    // if layer is dead player layer
                    if (hit.collider.gameObject.layer == PlayerManager.instance.GetDeadPlayerLayer())
                    {
                        bodyHit = true;
                    }

                    
                }
                if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.AddForceAtPosition(damagePackage.forceVector, hit.point, ForceMode.Impulse);
                }

                if (bodyHit)
                {
                    AudioManager.instance.PlayOneShot(bullet.BodyHitSound, hit.point);
                    GameObject impact = Instantiate(bullet.ImpactBody, hit.point, Quaternion.identity);
                    // get normal of hit point
                    impact.transform.forward = hit.normal;
                    
                }
                else
                {
                    AudioManager.instance.PlayOneShot(bullet.GroundHitSound, hit.point);
                    GameObject impact = Instantiate(bullet.ImpactGround, hit.point, Quaternion.identity);
                   
                    impact.transform.forward = hit.normal;
                }
                hitPoints[i] = hit.point;

               
            }
            else
            {
                hitPoints[i] = cameraTransform.position + shotDirectionForThisBullet * range;
            }
        }

        return hitPoints;


    }

    public GameObject[] ShootProjectile(Weapon_Arms weapon)
    {

        if (CannotSpawnBullets)
        {
            return new GameObject[0];
        }

			var autoAim = weapon.AutoAim;
        var autoAimRaycastLenght = autoAim.RaycastLenght;
        var autoAimRadius = autoAim.Radius;
        var autoAimLerp = autoAim.AimLerp;

        Weapon_Bullet_Projectile bullet_data = weapon.Bullet as Weapon_Bullet_Projectile;
        var forward = transform.forward;
        var target = GetAutoAimTarget(transform.position, transform.forward, autoAimRadius, autoAimRaycastLenght);
        if (target)
        {
            forward = Vector3.Lerp(forward, (target.position - transform.position), autoAimLerp).normalized;
        }

        

		int bulletCount = weapon.BulletsPerShot;
        GameObject[] bullets = new GameObject[bulletCount];
        for (int i = 0; i < bulletCount; i++)
        {
            var forwardForThisBullet = forward + UnityEngine.Random.insideUnitSphere * weapon.Inaccuracy * AccuracyMultiplier;
            // spawn projectile at transform position and rotate it to forward
            GameObject projectile = Instantiate(bullet_data.BulletPrefab, transform.position, Quaternion.LookRotation(forwardForThisBullet));

            if (projectile.TryGetComponent<Bullet>(out Bullet bullet))
            {
                bullet.SetUp(mainTransform.gameObject);
                 
                if (weapon.IsBeingDualWielded && !ignoreDualWieldDamageMultiplier)
                {
                    bullet.ApplyDamageMultiplier(weapon.DamageMultiplierWhenDualWielded);
                }
                

            }
            bullets[i] = projectile;
        }

        return bullets;

    }


	public List<RaycastHit> DoRicochetRaycast(Vector3 startPosition, Vector3 direction, int ricochetCount, float maxDistance, LayerMask wallMask, LayerMask playerMask, bool penetration, AutoAim autoAim)
	{
		List<RaycastHit> playersHit = new List<RaycastHit>();

		Vector3 currentPosition = startPosition;
		Vector3 currentDirection = direction.normalized;
        bool hasAutoAim = autoAim != null;

		for (int i = 0; i < ricochetCount; i++)
		{
            if (i != 0 && hasAutoAim)
            {
				var target = GetAutoAimTarget(currentPosition, currentDirection, autoAim.Radius, autoAim.RaycastLenght);
				if (target)
				{
					currentDirection = Vector3.Lerp(currentDirection, (target.position - currentPosition), autoAim.AimLerp).normalized;
				}
			}

			// 1. Check wall hit first
			if (Physics.Raycast(currentPosition, currentDirection, out RaycastHit wallHit, maxDistance, wallMask))
			{
				Debug.DrawLine(currentPosition, wallHit.point, Color.red, 2f);

				float segmentLength = Vector3.Distance(currentPosition, wallHit.point);

				// 2. Check for players along this same segment
				RaycastHit[] playerHits = Physics.RaycastAll(currentPosition, currentDirection, segmentLength + 0.5f, playerMask);
				System.Array.Sort(playerHits, (a, b) => a.distance.CompareTo(b.distance));
				foreach (var hit in playerHits)
				{
					if (!playersHit.Contains(hit))
					{
						playersHit.Add(hit);
						Debug.DrawLine(currentPosition, hit.point, Color.green, 2f);

                        if (!penetration && hit.collider.gameObject.TryGetComponent<CharacterHealth>(out CharacterHealth health) && ( i != 0 ||hit.collider.gameObject != playerTeam.gameObject))
                        {
                            // stop ricocheting if we hit a player and penetration is false
                            return playersHit;
						}
					}
				}

				// 3. Reflect direction for next bounce
				currentDirection = Vector3.Reflect(currentDirection, wallHit.normal);
				currentPosition = wallHit.point + currentDirection * 0.01f; // small offset to avoid self-collision
			}
			else
			{
				// No wall hit: ray just goes max distance
				Debug.DrawRay(currentPosition, currentDirection * maxDistance, Color.blue, 2f);

				// Check for players along this last ray
				RaycastHit[] playerHits = Physics.RaycastAll(currentPosition, currentDirection, maxDistance, playerMask);
				foreach (var hit in playerHits)
				{
					if (!playersHit.Contains(hit))
					{
						playersHit.Add(hit);
						Debug.DrawLine(currentPosition, hit.point, Color.green, 2f);
					}
				}

				// create a fake hit at max distance to show where the bullet would end
				playersHit.Add(new RaycastHit()
				{
					point = currentPosition + currentDirection * maxDistance,
					normal = -currentDirection
				});

				break; // stop ricocheting
			}
		}

		return playersHit;
	}
}
