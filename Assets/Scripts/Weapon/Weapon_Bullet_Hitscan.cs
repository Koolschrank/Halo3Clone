using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBulletHitscan", menuName = "Weapon/WeaponBulletHitscan")]
public class Weapon_Bullet_Hitscan : Weapon_Bullet
{
    [SerializeField] float damage = 10f;
    [SerializeField] float force = 1f;
    [SerializeField] float headShotMultiplier = 1f;
    [SerializeField] float range;
    [SerializeField] LayerMask hitLayer;

    [SerializeField] GameObject trail;
    [SerializeField] GameObject impact;

    public float Damage => damage;

    public float Force => force;

    public float HeadShotMultiplier => headShotMultiplier;
    public float Range => range;
    public LayerMask HitLayer => hitLayer;

    public GameObject Trail => trail;
    public GameObject Impact => impact;
}
