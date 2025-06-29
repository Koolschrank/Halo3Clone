using UnityEngine;

public class EnemyItemDropManager : MonoBehaviour
{
    [SerializeField] GameObject healthPickUp;
    [SerializeField] GameObject ammoPickUp;

    [SerializeField] GameObject[] basicPickUps;

    [SerializeField] float pickUpDropTime = 60f;
    [SerializeField] float emergencyDropTime = 30f;
    [SerializeField] int framesToCheckForPlayerState = 300;

    [SerializeField] float cooldownLossOnKill = 2f;

    float dropCooldown = 0f;

    bool shortOnAmmo = false;
    bool shortOnHealth = false;

    GameObject lastDropedItem = null;

    bool disableDrops = false;

    private void Start()
    {
        

        dropCooldown = pickUpDropTime;

        var enemySpawner = EnemySpawner.instance;
        if (!enemySpawner.isAutoActiveOnThisMap)
        {
            disableDrops = true;
            enabled = false;
        }
            
        enemySpawner.OnEnemySpawned += (enemy) => 
        {
            enemy.GetComponent<Health>().OnPreDeath += () => EnemyKilled(enemy);
        };
    }


    private void Update()
    {


        if ( Time.frameCount % framesToCheckForPlayerState == 0)
        {
            if (!shortOnHealth && !shortOnAmmo && ArePlayersShortOnAmmo())
            {
                shortOnAmmo = true;
                dropCooldown = emergencyDropTime;
            }
            if (!shortOnHealth && ArePlayersShortOnHealth())
            {
                shortOnHealth = true;
                dropCooldown = emergencyDropTime;
            }
            
        }

        dropCooldown -= Time.deltaTime;
    }

    public void EnemyKilled(GameObject enemy)
    {
        if (disableDrops) return;

        dropCooldown -= cooldownLossOnKill;

        if (dropCooldown > 0) return;


        

        DropItem(enemy);

    }


    bool ArePlayersShortOnAmmo()
    {
        return false;
    }

    bool ArePlayersShortOnHealth()
    {
        var allPlayers = PlayerManager.instance.GetAllPlayers();
        foreach (var player in allPlayers)
        {
            if (player.IsDead) return true;
        }


        return false;
    }

    void DropItem(GameObject enemy)
    {
        var item = SelectItemToDrop();
        var itemInstance = Instantiate(item, enemy.transform.position, Quaternion.identity);
        itemInstance.transform.SetParent(null);
        itemInstance.transform.position = enemy.transform.position;
        dropCooldown = pickUpDropTime;
    }

    GameObject SelectItemToDrop()
    {
        

        if (shortOnHealth)
        {
            return healthPickUp;
        }
        else if (shortOnAmmo)
        {
            return ammoPickUp;
        }
        GameObject itemToDrop = basicPickUps[Random.Range(0, basicPickUps.Length)];

        if (itemToDrop == lastDropedItem)
            return SelectItemToDrop(); // Avoid dropping the same item twice in a row



        return itemToDrop;

    }
}
