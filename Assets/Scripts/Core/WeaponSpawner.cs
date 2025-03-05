using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] bool spawnOnStart;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] float spawnTime;
    [SerializeField] int magazines = 4;
    float spawnTimer;

    Weapon_PickUp weapon;

    public void Start()
    {
        if (spawnOnStart)
        {
            SpawnWeapon();
        }
    }

    public void Update()
    {
        if (weapon == null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTime)
            {
                SpawnWeapon();
                spawnTimer = 0;
            }
        }
    }

    public void SpawnWeapon()
    {
        weapon = Instantiate(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon_PickUp>();
        weapon.SetAmmoWithMagazines(magazines);
        weapon.OnPickUp += WeaponPickedUp;
    }

    public void WeaponPickedUp(Weapon_PickUp weapon)
    {
        weapon.OnPickUp -= WeaponPickedUp;
        this.weapon = null;
    }




}
