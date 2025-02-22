using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Projectile", menuName = "Weapon/Bullet Projectile")]
public class Weapon_Bullet_Projectile : Weapon_Bullet
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject bulletVisual;

    public GameObject BulletPrefab => bulletPrefab;
    public GameObject BulletVisual => bulletVisual;



}
