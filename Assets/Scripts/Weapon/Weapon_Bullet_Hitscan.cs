using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBulletHitscan", menuName = "Weapon/WeaponBulletHitscan")]
public class Weapon_Bullet_Hitscan : Weapon_Bullet
{
    [SerializeField] float damage = 10f;
    [SerializeField] float force = 1f;
    [SerializeField] float shildDamageMultiplier = 1f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] float range;
    [SerializeField] LayerMask hitLayer;

    [SerializeField] float rangeUntilDamageFalloff = 100f;
    [SerializeField] AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [SerializeField] GameObject trail;
    [SerializeField] GameObject impact;

    public float Damage => damage;

    public float Force => force;

    public float ShildDamageMultiplier => shildDamageMultiplier;

    public float HeadShotMultiplier => headShotMultiplier;
    public float Range => range;
    public LayerMask HitLayer => hitLayer;

    public GameObject Trail => trail;
    public GameObject Impact => impact;

    public float GetDamageFalloff(float distance)
    {
        if (distance > rangeUntilDamageFalloff)
        {
            return damageFalloff.Evaluate((distance - rangeUntilDamageFalloff) / (range - rangeUntilDamageFalloff));
        }
        return 1f;
    }
}
