using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool isAutoActiveOnThisMap = true; // Flag to check if the spawner is active

     bool isPvPGame = false; // Flag to check if it's a PvP game
    [SerializeField] int AIPerTeam = 3; // Number of AI enemies per team


    [SerializeField] int enemyTeamId = 1; // Team ID for the enemy team
    [SerializeField] GameObject enemyPrefab;


    [SerializeField] Enemy_Wave[] enemyWaves;

    [SerializeField] Enemy_Wave enemiesForPVPGames;

    [SerializeField] int waveIndex = 0;
    EnemyWaveInstance activeWave;
    [SerializeField] float spawnRateMultiplier = 1; // Multiplier for spawn rate
    float nextWaveDelay = 5;

    public List<GameObject> activeEnemies = new List<GameObject>();

    public static EnemySpawner instance;


    int team1EnemyCount = 0;
    int team2EnemyCount = 0;

    public bool IsAutoActiveOnThisMap => isAutoActiveOnThisMap;

    public void SetPVPGame()
    {
        isPvPGame = true;

        activeWave = new EnemyWaveInstance(enemiesForPVPGames);

        activeWave.SetDuration(1000000000000f); // bassicly infinite duration
    }

    private void Awake()
    {
        

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        


    }

    public void Start()
    {
        var maploader = MapLoader.instance;
        if (maploader != null)
        {
            AIPerTeam = Math.Max(1, (int)(AIPerTeam * maploader.AIAmountMultiplier));
            var wave = maploader.Enemies;
            if (wave != null)
                enemiesForPVPGames = wave;
        }
    }

    public void StartEnemySpawner()
    {
        if (isAutoActiveOnThisMap)
        {
            return;
        }
        else
        {
            if (GameModeSelector.gameModeManager.GameModeStats.HasAiPlayers || (MapLoader.instance != null && MapLoader.instance.HasAIEnemies()))
            {
                SetPVPGame();

            }
            else
            {
                gameObject.SetActive(false);
            }
                
        }



    }


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
            if (!isPvPGame &&activeWave.IsWaveOver())
            {
                nextWaveDelay = activeWave.GetWaveEndBrakeTime();
                activeWave = null;
            }
        }
    }

    private void SpawnEnemy(Enemy_Stats stats)
    {

        Transform spawnPoint;
        var teamId = stats.teamIdOverrride;
        if (isPvPGame)
        {
            if (team1EnemyCount >= AIPerTeam && team2EnemyCount >= AIPerTeam)
            {
                activeWave.SetDuration(1);
                return;
            }


            teamId = 0;
            if (team1EnemyCount < AIPerTeam && team2EnemyCount > team1EnemyCount)
            {
                teamId = 0;
                team1EnemyCount++;
               
            }
            else if (team2EnemyCount < AIPerTeam)
            {
                teamId = 1;
                team2EnemyCount++;
               
            }



            spawnPoint = GetPVPSpawnPoint(teamId);
        }
        else
        {
            spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId, 3);
        }



            
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);


        var equipment = stats.equipment;
        enemy.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
        var health = enemy.GetComponent<CharacterHealth>();
        health.MultiplyHealth(stats.healthMultiplier);
        health.MultiplyShild(stats.shildMultiplier);

        activeEnemies.Add(enemy);
        health.OnDeath += () =>
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy, 120f);
        };

        var movement = enemy.GetComponent<PlayerMovement>();
        movement.MultiplySpeed(stats.speedMultiplier);

        // get child of name EnemyAI
        
        var score = enemy.GetComponent<GainScore>();
        score.scoreAmount = stats.scoreForKill;



        if (isPvPGame)
        {
            if (teamId == 0)
            {
                health.OnDeath += () =>
                {
                    team1EnemyCount--;
                };
            }
            else
            {
                health.OnDeath += () =>
                {
                    team2EnemyCount--;
                };
            }

        }
        


        PlayerManager.instance.UpdateTeamOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), teamId);


    }


    public Transform GetPVPSpawnPoint(int teamIndex)
    {
        Transform spawnPoint = GameModeSelector.gameModeManager.GetFarthestSpawnPointInCludingAIEnemies(teamIndex);
        return spawnPoint;
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

    public void SetDuration(float duration)
    {
        this.duration = duration;
    }






}
