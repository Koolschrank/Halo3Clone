using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] bool spawnOnStart;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] float spawnTime;
    [SerializeField] int magazines = 4;
    [SerializeField] float weaponDistanceToAttemtRespawn = 5f;
	float spawnTimer;

    Weapon_PickUp weapon;


    [SerializeField] bool isRandom;
    [SerializeField] GameObject[] randomWeapons;

    bool onlySpawnAtStart;

	public void Start()
    {
        StartCoroutine(StartDelay());

        if (GameModeSelector.gameModeManager != null)
            spawnTime *= GameModeSelector.gameModeManager.GameModeStats.weaponRespawnTimeMultiplier;
	}

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        DelayStart();
    }

    public void DelayStart()
    {
        if (!GameModeSelector.gameModeManager.HasWeaponPickups)
        {
            Destroy(gameObject);
            return;
        }

		if (GameModeSelector.gameModeManager as LastManStandingManager)
		{
            var lastManStandingManager = GameModeSelector.gameModeManager as LastManStandingManager;
            onlySpawnAtStart = true;
            lastManStandingManager.OnNewRoundStarted += () =>
            {
                
                SpawnWeapon();
            };
		}



		if (spawnOnStart)
        {
            SpawnWeapon();
        }
    }


    public void Update()
    {
        if (onlySpawnAtStart)
            return;

		if (weapon == null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTime)
            {
                SpawnWeapon();
                spawnTimer = 0;
            }
        }
        else
        {
			var distanceToWeapon = Vector3.Distance(transform.position, weapon.transform.position);
			if (weapon != null && distanceToWeapon > weaponDistanceToAttemtRespawn)
			{
                WeaponPickedUp(weapon);
			}
		}
            

	}

    public void SpawnWeapon()
    {
        if (isRandom)
        {
            int randomIndex = Random.Range(0, randomWeapons.Length);
            weaponPrefab = randomWeapons[randomIndex];
		}
		weapon = Instantiate(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon_PickUp>();

		weapon.SetAmmoWithMagazines(magazines);
        weapon.OnPickUp += WeaponPickedUp;
    }

    public void WeaponPickedUp(Weapon_PickUp weapon)
    {
        weapon.OnPickUp -= WeaponPickedUp;
        this.weapon = null;
    }

    // gizmor sphere spawn point
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }




}
