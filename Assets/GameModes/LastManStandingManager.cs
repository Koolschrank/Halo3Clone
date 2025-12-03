using System;
using UnityEngine;

public class LastManStandingManager : GameModeManager
{
	public Action OnRoundOver;
	public Action OnPreNewRoundStarted;
	public Action OnNewRoundStarted;


	bool[] spawnPointsUsed = new bool[8];
	int[] teamSpawns = new int[8];

	public float restartRoundDelay = 3f;

	public FortniteStorm fortniteStorm;
	
	
	public float stormBaseShrinkDuration = 40f;
	public float stormShrinkDurationPerPlayer = 5f;
	

	


	private void Awake()
	{
		
		

		
	}


	public override void ResetGame()
	{
		base.ResetGame();
		ResetSpawnPoints();
		var lastManStandingData = gameModeStats as GameMode_LastManStanding;
		var hasStorm = lastManStandingData.hasStorm;
		fortniteStorm.shrinkDuration = stormBaseShrinkDuration;
		fortniteStorm.gameObject.SetActive(hasStorm);
	}


	public int GetSpawnPoint(int playerTeamIndex)
	{
		int TeamCount = teams.Count;

		// check if team already has a spawn point
		for (int i = 0; i < teamSpawns.Length; i++)
		{
			if (teamSpawns[i] == playerTeamIndex)
			{
				return i;
			}
		}


		System.Random rand = new System.Random();
		int spawnPointIndex = rand.Next(0, TeamCount);
		while (spawnPointsUsed[spawnPointIndex])
		{
			spawnPointIndex = rand.Next(0, TeamCount);
		}
		spawnPointsUsed[spawnPointIndex] = true;
		teamSpawns[spawnPointIndex] = playerTeamIndex;

		return spawnPointIndex;
	}

	public void ResetSpawnPoints()
	{
		for (int i = 0; i < spawnPointsUsed.Length; i++)
		{
			spawnPointsUsed[i] = false;
		}
		for (int i = 0; i < teamSpawns.Length; i++)
		{
			teamSpawns[i] = -1;
		}
	}

	public override void PlayerJoined(PlayerMind player)
	{
		base.PlayerJoined(player);
		player.OnPlayerElimination += PlayerEliminated;
		player.OnTeamKill += TeamKill;
		player.OnPlayerDied += (player) => CheckIfTeamWonRound();

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		fortniteStorm.shrinkDuration = stormBaseShrinkDuration + stormShrinkDurationPerPlayer * allPlayers.Count;

		fortniteStorm.StartStorm();
	}

	void PlayerEliminated(GameObject killedPlayer, PlayerMind playerWhoElimnated)
	{
		int teamIndex = playerWhoElimnated.TeamIndex;
		CheckIfTeamWonRound();
	}

	public void CheckIfTeamWonRound()
	{
		if (!AreMoreThanOneTeamAlive())
		{
			int winningTeamIndex = GetWinningTeamIndex();
			if (winningTeamIndex != -1 && !roundOver)
			{
				GainPoints(winningTeamIndex, 1);
			}
			RoundOver();
		}
	}

	public int GetIndexOfLastTeamStanding()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();
		int aliveTeams = 0;
		foreach (var player in allPlayers)
		{
			if (player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
			{
				aliveTeams |= (1 << player.TeamIndex);
			}
		}
		for (int i = 0; i < 32; i++)
		{
			if ((aliveTeams & (1 << i)) != 0)
			{
				return i;
			}
		}
		return -1;
	}

	bool roundOver = false;
	public void RoundOver()
	{
		if (roundOver) return;
		OnRoundOver?.Invoke();
		Invoke(nameof(StartNewRound), restartRoundDelay);
		roundOver = true;

	}

	public void StartNewRound()
	{
		fortniteStorm.StartStorm();
		KillAllPlayers();
		ResetSpawnPoints();
		RespawnAllPlayers();
		roundOver = false;
		OnPreNewRoundStarted?.Invoke();
		OnNewRoundStarted?.Invoke();

		
	}

	public void KillAllPlayers()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			DamagePackage damagePackage = new DamagePackage();
			damagePackage.damageAmount = 9999f;
			damagePackage.forceVector = Vector3.up * 100000f;
			if (player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
			{
				player.PlayerBody.GetComponent<CharacterHealth>().TakeDamage(damagePackage);
			}
		}
	}

	public void RespawnAllPlayers()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			if (player.PlayerBody == null || player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
			{
				player.Respawn();
			}
		}
	}


	public bool AreMoreThanOneTeamAlive()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();

		int[] teams = new int[8];
		foreach (var player in allPlayers) {
			if (player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
			{
				teams[player.TeamIndex] ++;
			}
		}
		int aliveTeams = 0;
		foreach (var team in teams)
			{
			if (team > 0)
			{
				aliveTeams++;
			}
		}

		return aliveTeams > 1;
	}


	public int GetWinningTeamIndex()
	{
		var allPlayers = PlayerManager.instance.GetAllPlayers();
		int[] teams = new int[8];
		foreach (var player in allPlayers)
		{
			if (player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
			{
				teams[player.TeamIndex]++;
			}
		}

		int winningTeam = 0;
		for (int i = 0; i < teams.Length; i++)
		{
			if (teams[i] > 0)
			{
				if (winningTeam == 0)
				{
					winningTeam = i;
				}
				else
				{
					// more than one team alive
					return -1;
				}
			}
		}
		return winningTeam;
	}

	void TeamKill(GameObject killedPlayer, PlayerMind playerWhoElimnated)
	{
		int teamIndex = playerWhoElimnated.TeamIndex;
		CheckIfTeamWonRound();
	}

	protected override void GainPoints(int teamIndex, int points)
	{
		base.GainPoints(teamIndex, points);

		if (gameModeStats is GameMode_GunGame)
		{
			var allPlayers = PlayerManager.instance.GetAllPlayers();
			var teamScores = teamPoints[teamIndex];
			var newEquipment = gameModeStats.GetEquipmentBasedOnPoints(teamScores);
			foreach (var player in allPlayers)
			{
				if (player.TeamIndex == teamIndex && player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
				{
					var startEquipment = player.PlayerBody.GetComponent<PlayerStartEquipment>();
					startEquipment.ClearEquipment();
					startEquipment.GetEquipment(newEquipment);
				}
			}



		}
	}
}
