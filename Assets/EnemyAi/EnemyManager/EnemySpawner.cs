using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int enemyTeamId = 1; // Team ID for the enemy team
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval = 5f;

    [SerializeField] Equipment[] enemyEquipments;

    float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemy()
    {

        Transform spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId,3);
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);

        enemy.GetComponent<PlayerStartEquipment>().GetEquipment(enemyEquipments[Random.Range(0, enemyEquipments.Length)]);

        // get child of name EnemyAI
        PlayerManager.instance.UpdateTeamOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), enemyTeamId);
    }
}
