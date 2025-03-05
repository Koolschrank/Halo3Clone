using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBulletGranade", menuName = "Weapon/WeaponBulletGranade")]
public class Weapon_Bullet_Granade : Weapon_Bullet
{
    [SerializeField] GranadeStats granadeStats;

    public GranadeStats GranadeStats => granadeStats;



}
