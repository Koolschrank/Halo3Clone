using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int enemyTeamId = 1; // Team ID for the enemy team
    [SerializeField] GameObject enemyPrefab;


    [SerializeField] Enemy_Wave[] enemyWaves;
    [SerializeField] int waveIndex = 0;
    EnemyWaveInstance activeWave;
    [SerializeField] float spawnRateMultiplier = 1; // Multiplier for spawn rate
    float nextWaveDelay = 5;


    public void StartNextWave()
    {
        activeWave = new EnemyWaveInstance(enemyWaves[waveIndex]);
        waveIndex++;
    }

    private void Update()
    {
        if (activeWave == null)
        {
            if (nextWaveDelay > 0)
            {
                nextWaveDelay -= Time.deltaTime;
                return;
            }
            StartNextWave();
        }
        else
        {
            activeWave.UpdateTimers(Time.deltaTime,spawnRateMultiplier);

            if (activeWave.CanSpawnEnemy())
            {
                SpawnEnemy(activeWave.GetRandomEnemy());
            }
            if (activeWave.IsWaveOver())
            {
                nextWaveDelay = activeWave.GetWaveEndBrakeTime();
                activeWave = null;
            }
        }
    }

    private void SpawnEnemy(Enemy_Stats stats)
    {

        Transform spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId,3);
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);


        var equipment = stats.equipment;
        enemy.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
        var health = enemy.GetComponent<CharacterHealth>();
        health.MultiplyHealth(stats.healthMultiplier);
        health.MultiplyShild(stats.shildMultiplier);
        var movement = enemy.GetComponent<PlayerMovement>();
        movement.MultiplySpeed(stats.speedMultiplier);

        // get child of name EnemyAI
        var teamId = stats.teamIdOverrride;
        var score = enemy.GetComponent<GainScore>();
        score.scoreAmount = stats.scoreForKill;

        PlayerManager.instance.UpdateTeamOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), teamId);
    }
}


public class EnemyWaveInstance
{
    Enemy_Wave enemyWave;
    float duration;
    float nextSpawnTimer;

    public EnemyWaveInstance(Enemy_Wave enemyWave)
    {
        this.enemyWave = enemyWave;
        duration = enemyWave.duration;
        nextSpawnTimer = enemyWave.spawnInterval;
    }

    public void UpdateTimers(float delta, float spawnRateMult)
    {
        duration -= delta;
        nextSpawnTimer -= delta * spawnRateMult;

    }

    public bool CanSpawnEnemy()
    {
        if (nextSpawnTimer <= 0)
        {
            nextSpawnTimer = enemyWave.spawnInterval;
            return true;
        }
        return false;
    }

    public bool IsWaveOver()
    {
        if (duration <= 0)
        {
            return true;
        }
        return false;
    }

    public Enemy_Stats GetRandomEnemy()
    {
        return enemyWave.GetRandomEnemy();
    }   

    public float GetWaveEndBrakeTime()
    {
        return enemyWave.waveEndBrakeTime;
    }






}
