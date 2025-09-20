using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "WeaponBulletHitscan", menuName = "Weapon/WeaponBulletHitscan")]
public class Weapon_Bullet_Hitscan : Weapon_Bullet
{
    [SerializeField] float damage = 10f;
    public float damageMultiplierVSAI = 1f;
	[SerializeField] float force = 1f;
    [SerializeField] float shildDamageMultiplier = 1f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] bool canHeadShotShild = false;
    [SerializeField] float range;
    [SerializeField] LayerMask hitLayer;

    [SerializeField] float rangeUntilDamageFalloff = 100f;
    [SerializeField] AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [SerializeField] GameObject trail;
    [SerializeField] GameObject impact_body;
    [SerializeField] GameObject impact_ground;

    [SerializeField] bool doesApplyForceOnLivingPlayers = false;
    [SerializeField] float forceOnLivingPlayers = 1f;
    public bool penetration = false;
    public bool ricochet = false;
    public int maxRicochetCount = 0;
    public AutoAim ricochetAutoAim;


	[Header("Sound")]
    [SerializeField] EventReference bodyHitSound;
    [SerializeField] EventReference groundHitSound;


    public bool DoesApplyForceOnLivingPlayers => doesApplyForceOnLivingPlayers;
    public float ForceOnLivingPlayers => forceOnLivingPlayers;

	public float Damage => damage;

    public float Force => force;

    public float ShildDamageMultiplier => shildDamageMultiplier;

    public float HeadShotMultiplier => headShotMultiplier;
    public float Range => range;
    public LayerMask HitLayer => hitLayer;

    public GameObject Trail => trail;

    public GameObject ImpactBody => impact_body;

    public GameObject ImpactGround => impact_ground;

    // hit sound
    public EventReference BodyHitSound => bodyHitSound;
    public EventReference GroundHitSound => groundHitSound;

    public bool CanHeadShotShild => canHeadShotShild;

    public float GetDamageFalloff(float distance)
    {
        if (distance > rangeUntilDamageFalloff)
        {
            return damageFalloff.Evaluate((distance - rangeUntilDamageFalloff) / (range - rangeUntilDamageFalloff));
        }
        return 1f;
    }
}
