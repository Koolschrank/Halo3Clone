using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Action<GameObject> OnEnemySpawned;


    [SerializeField] public bool isAutoActiveOnThisMap = true; // Flag to check if the spawner is active

     bool isPvPGame = false; // Flag to check if it's a PvP game
    [SerializeField] int AIPerTeam = 3; // Number of AI enemies per team


    [SerializeField] int enemyTeamId = 1; // Team ID for the enemy team
    [SerializeField] GameObject enemyPrefab;


    [SerializeField] Enemy_Wave[] enemyWaves;

    [SerializeField] Enemy_Wave enemiesForPVPGames;

	[SerializeField] Enemy_Wave alliesForPVPGames;

	[SerializeField] int waveIndex = 0;
    EnemyWaveInstance activeWave;
    [SerializeField] float spawnRateMultiplier = 1; // Multiplier for spawn rate

	[SerializeField] float spawnRateCooldownMultiplier = 0.5f; // Multiplier for spawn rate
	float nextWaveDelay = 5;

    public List<GameObject> activeEnemies = new List<GameObject>();

    public static EnemySpawner instance;


    int team1EnemyCount = 0;
    int team2EnemyCount = 0;

    KingOfTheHillManager kingOfTheHillManager;


   

	public void KillAllEnemies()
    {
        DamagePackage damagePackage = new DamagePackage(1000000);


        // revers for loop through all active enemies and apply damage
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy != null)
            {
                var health = enemy.GetComponent<CharacterHealth>();
                if (health != null)
                {
                    health.TakeDamage(damagePackage);
                }
            }
        }
        activeEnemies.Clear();
    }

    public bool IsAutoActiveOnThisMap => isAutoActiveOnThisMap;

    public void SetPVPGame()
    {
        isPvPGame = true;

		if (GameModeSelector.gameModeManager.GameModeStats.useEnemyWaves)
		{
			activeWave = new EnemyWaveInstance(enemyWaves[waveIndex]);
            if (GameModeSelector.gameModeManager is KingOfTheHillManager)
            {
				kingOfTheHillManager = (KingOfTheHillManager)GameModeSelector.gameModeManager;
				

				kingOfTheHillManager.OnNextHillPlaced += () =>
                {
                    waveIndex++;
                    if (waveIndex >= enemyWaves.Length)
                    {
                        waveIndex = 0; // Reset to the first wave if we reach the end
					}
					activeWave = new EnemyWaveInstance(enemyWaves[waveIndex]);
				};
			}
		}
        else
        {
			activeWave = new EnemyWaveInstance(enemiesForPVPGames);
		}
           

        


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
            spawnRateMultiplier *= maploader.AIAmountMultiplier;
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
				MapLoader.instance.SetAIEnemiesTeam2(true);
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
            float delta = Time.deltaTime * spawnRateCooldownMultiplier;
			if (kingOfTheHillManager != null && kingOfTheHillManager.teamOnHill == 1)
            {
                delta *= 0.05f;

			}


				activeWave.UpdateTimers(delta, spawnRateMultiplier);

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
		var gamemode = GameModeSelector.gameModeManager.GameModeStats;
		if (isPvPGame)
        {
            var AIPerTeam1 = AIPerTeam;
            var AIPerTeam2 = AIPerTeam;

            bool hasTeam1AIEnemies = MapLoader.instance.IsAIEnemiesTeam1();
            bool hasTeam2AIEnemies = MapLoader.instance.IsAIEnemiesTeam2();


            if (hasTeam1AIEnemies && hasTeam2AIEnemies && !PlayerManager.instance.HasTeam2Players())
            {
                
                bool hasTeam1almostWon = GameModeSelector.gameModeManager.HasTeam1AlmostWon();
                int playersInTeam1 = PlayerManager.instance.PlayersInTeam1();

                AIPerTeam1 -= (int)(playersInTeam1 * spawnRateMultiplier);

                AIPerTeam2 +=  (int)(playersInTeam1 * spawnRateMultiplier);
                //if (hasTeam1almostWon)
                //{
                //    AIPerTeam2 += (int)(playersInTeam1 * spawnRateMultiplier);
                //}

            }
            else
            {
                var team1PlayerCount = PlayerManager.instance.PlayersInTeam1();
                var team2PlayerCount = PlayerManager.instance.PlayersInTeam2();

                AIPerTeam1 +=  -team1PlayerCount + team2PlayerCount;
                AIPerTeam2 += -team2PlayerCount + team1PlayerCount;

            }

            

            if (hasTeam1AIEnemies && !hasTeam2AIEnemies)
            {
                AIPerTeam2 = 0;
            }
            else if (!hasTeam1AIEnemies && hasTeam2AIEnemies)
            {
                AIPerTeam1 = 0;
                

			}
            if(gamemode.team2LoosesScoreWhenTeam1scores)
            {
				AIPerTeam2 = (int)((float)AIPerTeam2 / activeWave.enemyWave.spawnInterval);
			}
			

			if (team1EnemyCount >= AIPerTeam1 && team2EnemyCount >= AIPerTeam2)
            {
                activeWave.SetDuration(1);
                return;
            }
           



            teamId = 0;
            if (team1EnemyCount < AIPerTeam1 && team2EnemyCount > team1EnemyCount)
            {
                teamId = 0;
                team1EnemyCount++;
               
            }
            else if (team2EnemyCount < AIPerTeam2)
            {
                teamId = 1;
                team2EnemyCount++;
               
            }

            if (PlayerManager.instance.PlayersInTeam2() == 0 && teamId ==1)
				spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId, 2);
			else
			{
                if (PlayerManager.instance.forceSpawnTeam1OnSpawnPoint1 && teamId == 0)
                {
                    spawnPoint = GameModeSelector.gameModeManager.GetStartingSpawnPoint(0);
				}
                else
                {
					spawnPoint = GetPVPSpawnPoint(teamId);
				}
                    
			}
		}
        else
        {
            spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId, 2);
        }



            
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.rotation;
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);

        

        if (teamId == 0 && gamemode.team2LoosesScoreWhenTeam1scores)
        {
            stats = alliesForPVPGames.GetRandomEnemy();
		}


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

        if (isAutoActiveOnThisMap)
        {
            var arms = enemy.GetComponent<PlayerArms>();
            arms.RightArm.GetBulletSpawner().SetOnlyEnemyIsPlayerTeam(true);
        }



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


        int colorIndex = teamId;
        if (GameModeSelector.gameModeManager.GameModeStats.EnemyTeamsWorkingTogether && teamId != 0)
        {
            colorIndex = stats.teamIdOverrride;
        }
        PlayerManager.instance.UpdateColorOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), colorIndex);

        OnEnemySpawned?.Invoke(enemy);

        enemy.GetComponent<CharacterHealth>().SetDamageMultiplier(GameModeSelector.gameModeManager.GameModeStats.ai_damageMultiplier);
	}


    public Transform GetPVPSpawnPoint(int teamIndex)
    {
        Transform spawnPoint = GameModeSelector.gameModeManager.GetFarthestSpawnPointInCludingAIEnemies(teamIndex);
        return spawnPoint;
    }
}


public class EnemyWaveInstance
{
    public Enemy_Wave enemyWave;
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
