using Fusion;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.UIElements;

public class WeaponSpawner : NetworkBehaviour
{
    [SerializeField] bool spawnOnStart;
    [SerializeField] NetworkObject weaponPrefab;
    [SerializeField] float spawnTime;
    [SerializeField] int magazines = 4;
    

    Weapon_PickUp weapon;


    [Networked] private TickTimer spawnTimer { get; set; }

    // spawned
    public override async void Spawned()
    {
        


        await Task.Delay(100); // Wait for 0.1 seconds
        if (!GameModeSelector.gameModeManager.HasWeaponPickups)
        {
            Runner.Despawn(Object);
            return;
        }

        if (spawnOnStart)
        {
            SpawnWeapon();
        }
        else
        {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnTime);
        }

            
    }


    public void Start()
    {
        StartCoroutine(StartDelay());
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



        
    }


    public override void FixedUpdateNetwork()
    {
        if (spawnTimer.Expired(Runner) && weapon == null)
        {
            SpawnWeapon();
        }
    }



    public void SpawnWeapon()
    {
        weapon = Runner.Spawn(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon_PickUp>();
        weapon.SetAmmoWithMagazines(magazines);
        weapon.OnPickUp += WeaponPickedUp;
    }

    public void WeaponPickedUp(Weapon_PickUp weapon)
    {
        weapon.OnPickUp -= WeaponPickedUp;
        this.weapon = null;
        spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnTime);
    }

    // gizmor sphere spawn point
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }




}
