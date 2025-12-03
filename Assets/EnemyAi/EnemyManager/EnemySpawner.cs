using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawner : MonoBehaviour
{
    public Action<GameObject> OnEnemySpawned;
    public Action<int> OnWaveStart;


    [SerializeField] public bool isAutoActiveOnThisMap = true; // Flag to check if the spawner is active

     bool isPvPGame = false; // Flag to check if it's a PvP game
    [SerializeField] int AIPerTeam = 3; // Number of AI enemies per team


    [SerializeField] int enemyTeamId = 1; // Team ID for the enemy team
    [SerializeField] GameObject enemyPrefab;


    [SerializeField] Enemy_Wave[] enemyWaves;

    [SerializeField] Enemy_Wave enemiesForPVPGames;


	[SerializeField] Equipment[] specialEquipments;

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

    public bool tutorialMode = false; // Flag to check if it's in tutorial mode
    public Enemy_Stats[] tutorialEnemyStats; // Enemy stats for the tutorial mode
    public Transform[] tutorialSpawnPoints; // Spawn point for the tutorial enemies

    public int extraTeammates = 0;
    public int extraEnemies = 0;
    public float enemyDamageReduction = 1f; // Damage reduction for enemies
    public int enemiesThatSpawnAtStartOfWave = 3;

    public bool enemiesDoNotDropLoot = false;
    public LayerMask groundLayer;

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
                    if (GameModeSelector.gameModeManager.GetTeamPoints()[0] % 100 == 0)
                    {
						waveIndex++;
						if (waveIndex >= enemyWaves.Length)
						{
							waveIndex = 0; // Reset to the first wave if we reach the end
						}
						activeWave = new EnemyWaveInstance(enemyWaves[waveIndex]);
						OnWaveStart?.Invoke(waveIndex);
					}
                    

                    SpawnEnemiesAtObjective(enemiesThatSpawnAtStartOfWave);

				};


				OnWaveStart?.Invoke(waveIndex);
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
        if (tutorialMode)
        {
            EnterTutorialMode();
			return;
        }



        if (isAutoActiveOnThisMap)
        {
            return;
        }
        else
        {
            if (GameModeSelector.gameModeManager.GameModeStats.HasAiPlayers || (MapLoader.instance != null && MapLoader.instance.HasAIEnemies()))
            {
				MapLoader.instance.SetAIEnemiesTeam2(true);
                if (GameModeSelector.gameModeManager.GameModeStats.HasAiTeamMembers)
                {
					MapLoader.instance.SetAIEnemiesTeam1(true);
				}

				SetPVPGame();

            }
            else
            {
                gameObject.SetActive(false);
            }
                
        }



    }

    public void EnterTutorialMode()
    {
        int index = 0;
		foreach (var spawnPoint in tutorialSpawnPoints)
        {
            SpawnTutorialEnemy(index);
			index++;
		}
	}

    public void SpawnTutorialEnemy(int index)
    {
        var spawnPoint = tutorialSpawnPoints[index];
		var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
		var stats = tutorialEnemyStats[index];
		var equipment = stats.equipment;
		enemy.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
		var health = enemy.GetComponent<CharacterHealth>();
		health.MultiplyHealth(stats.healthMultiplier);
		health.MultiplyShild(stats.shildMultiplier);

		activeEnemies.Add(enemy);
		health.OnDeath += () =>
		{
			activeEnemies.Remove(enemy);
            SpawnTutorialEnemy(index);
			Destroy(enemy, 40f);

		};

		var movement = enemy.GetComponent<PlayerMovement>();
		movement.MultiplySpeed(0);

        var arms = enemy.GetComponent<PlayerArms>();
        arms.RightArm.GetBulletSpawner().CannotSpawnBullets = true;

		PlayerManager.instance.UpdateTeamOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), 1);


		int colorIndex = stats.teamIdOverrride;
		
		PlayerManager.instance.UpdateColorOfEnemyAI(enemy.GetComponent<BodyMindConnection>(), colorIndex);

		OnEnemySpawned?.Invoke(enemy);

		enemy.GetComponent<CharacterHealth>().SetDamageMultiplier(GameModeSelector.gameModeManager.GameModeStats.ai_damageMultiplier);

        enemy.gameObject.GetComponentInChildren<AI_Shoot>().cannotShoot = true;

		activeEnemies.Add(enemy);
	}


    public void StartNextWave()
    {
        activeWave = new EnemyWaveInstance(enemyWaves[waveIndex]);
        waveIndex++;
    }

    public float noSpawnSlowDownTimerAfterHillMove = 15f;
    float noSpawnSlowDownTimer = 0;

    private void Update()
    {
        if (tutorialMode) return;

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
            noSpawnSlowDownTimer -= Time.deltaTime;
			if (kingOfTheHillManager != null && noSpawnSlowDownTimer<=0 && kingOfTheHillManager.teamOnHill == 1)
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

	public void SpawnEnemiesAtObjective(int amount)
	{
		noSpawnSlowDownTimer = noSpawnSlowDownTimerAfterHillMove;

		var gamemode = GameModeSelector.gameModeManager;
        var gameModeKOTH = gamemode as KingOfTheHillManager;
        if (gameModeKOTH != null)
        {
			var spawnpoint = gameModeKOTH.currentHill.transform;
			int teamId = 1;
			for (int i = 0; i < amount; i++)
            {
				
				var stats = activeWave.GetRandomEnemy();

				while (activeWave.enemyWave.MaxSpecialEnemies <= specialEnemyCount && stats.useSpecialEquipment) 
				{
					stats = alliesForPVPGames.GetRandomEnemy();
				} 


				var equipment = stats.equipment;

				bool isSpecialEnemy = false;
				if (stats.useSpecialEquipment)
				{
					var newEquipment = specialEquipments[UnityEngine.Random.Range(0, specialEquipments.Length)];
					newEquipment.ChangeSize(equipment.PlayerSize, equipment.PlayerSizeOffset, equipment.PlayerCenterOffset);
					equipment = newEquipment;
					isSpecialEnemy = true;
                    specialEnemyCount++;
				}

				Vector2 randomInCircle = UnityEngine.Random.insideUnitCircle * 4f;
				Vector3 randomInCircle3D = new Vector3(randomInCircle.x, 0, randomInCircle.y);

				Vector3 spawnPosition = spawnpoint.position + Vector3.up * 2 + randomInCircle3D;

				// make raycast from spawnpoint to spawnPosition to check if it hits the ground
                RaycastHit hit;
                if (Physics.Raycast(spawnpoint.position, spawnPosition - spawnpoint.position, out hit, Vector3.Distance(spawnpoint.position, spawnPosition), groundLayer))
                {
                    spawnPosition = spawnpoint.position + Vector3.up * 2;

				}


					Quaternion spawnRotation = spawnpoint.rotation;
				GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);

				enemy.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
				var health = enemy.GetComponent<CharacterHealth>();
				health.MultiplyHealth(stats.healthMultiplier);
				health.MultiplyShild(stats.shildMultiplier);

				activeEnemies.Add(enemy);
				health.OnDeath += () =>
				{
					activeEnemies.Remove(enemy);
					Destroy(enemy, 40f);
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
					team2EnemyCount++;

					health.OnDeath += () =>
					{
						team2EnemyCount--;
					};

                    if (isSpecialEnemy)
                    {
						health.OnDeath += () =>
						{
							specialEnemyCount--;
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

				enemy.GetComponent<CharacterHealth>().SetDamageMultiplier(GameModeSelector.gameModeManager.GameModeStats.ai_damageMultiplier * enemyDamageReduction);

			}





		}
        
	}

    public int possibleSpawnPointsWhenSpawningEnemy = 2;
    int specialEnemyCount = 0;
	private void SpawnEnemy(Enemy_Stats stats)
    {

		if (tutorialMode) return;
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

                if (AIPerTeam1 <2) AIPerTeam1 = 2; // Ensure at least 2 AI enemies in team 1
                

												   

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


            AIPerTeam1 += extraTeammates;
            AIPerTeam2 += extraEnemies;
			

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
				spawnPoint = GameModeSelector.gameModeManager.GetRandomFarthestSpawnPoint(enemyTeamId, possibleSpawnPointsWhenSpawningEnemy);
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

		while (activeWave.enemyWave.MaxSpecialEnemies <= specialEnemyCount && stats.useSpecialEquipment) 
            {
            stats = activeWave.enemyWave.GetRandomEnemy();
		}


		bool isSpecialEnemy = false;
		var equipment = stats.equipment;
        if (stats.useSpecialEquipment)
        {
            var newEquipment = specialEquipments[UnityEngine.Random.Range(0, specialEquipments.Length)];
            newEquipment.ChangeSize(equipment.PlayerSize, equipment.PlayerSizeOffset, equipment.PlayerCenterOffset);
			equipment = newEquipment;
            specialEnemyCount++;
			isSpecialEnemy = true;
		}

        enemy.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
        var health = enemy.GetComponent<CharacterHealth>();
        health.MultiplyHealth(stats.healthMultiplier);
        health.MultiplyShild(stats.shildMultiplier);

        activeEnemies.Add(enemy);
        health.OnDeath += () =>
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy, 40f);
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

            if (isSpecialEnemy)
            {
				health.OnDeath += () =>
				{
					specialEnemyCount--;

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

        enemy.GetComponent<CharacterHealth>().SetDamageMultiplier(GameModeSelector.gameModeManager.GameModeStats.ai_damageMultiplier * enemyDamageReduction);

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
