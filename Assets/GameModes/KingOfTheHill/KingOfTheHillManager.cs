using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


public class KingOfTheHillManager : GameModeManager
{
    public Action OnNextHillPlaced;
    
    [SerializeField] int hillStartIndex = 0;
    [SerializeField] Hill[] hills;
    [SerializeField] float checkInterval = 0.1f;

    float checkTimer = 0;
    List<int> hillsAlreadyUsed = new List<int>();
    [NonSerialized]
    public Hill currentHill;

    public Action<int> OnDominatingTeamChanged;
    public Action<float> OnHillMoveTimerChanged;
    public int teamOnHill = -1;
    float timeOnHillUntilNextPointScore = 0;
    float hillMoveTimer = 0;


	

	public override void ResetGame()
    {
        base.ResetGame();


        hillsAlreadyUsed.Clear();

        foreach (Hill hill in hills)
        {
            hill.Deactivate();
        }

        StartHill(hillStartIndex);
        




    }

	public override void PlayerDied(PlayerMind player)
	{
		base.PlayerDied(player);


        if (gameModeStats.usePVEScoring)
        {
			// check if all players are dead
			bool allPlayersDead = true;
			var allPlayers = PlayerManager.instance.GetAllPlayers();
			foreach (var p in allPlayers)
			{
				if (!p.IsDead)
				{
					allPlayersDead = false;
					break;
				}
			}
			if (allPlayersDead)
			{
				currentHill.SetLastTeamOnHill(1);
			}
		}
		
	}

	protected override void GainPoints(int teamIndex, int points)
    {
        base.GainPoints(teamIndex, points);
        var gameModeStats = (GameMode_KingOfTheHill)this.gameModeStats;
		if (teamIndex == 0 && gameModeStats.moveHillWhenScoreReached && teamPoints[teamIndex] % gameModeStats.scoreToMoveHill == 0)
        {
            hillMoveTimer = 0;
        }
    }
	public void EndGame()
    {

    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0)
        {
            CheckCurrentHill();
            checkTimer = checkInterval;
        }
        var gameStats = (GameMode_KingOfTheHill)gameModeStats;
        UpdateHillTimer();
        UpdateHillMoveTimer();

        if (CanMoveHill())
        {
            StartRandomHill();
            ResetHillMoveTimer();
            OnNextHillPlaced?.Invoke();


            if (gameModeStats.usePVEScoring )
            {
                var allPlayers = PlayerManager.instance.GetAllPlayers();
                foreach (var player in allPlayers)
                {
                    if (!player.IsDead)
                    {
                        player.AddScore((int)(teamPoints[0] * 0.4));
                    }
				}

			}
		}

       

    }

    void StartRandomHill()
    {
        StartHill(GetRandomHillIndex());
        LogSystem.logSystem.PrintLog("Hill moved");

		if (gameModeStats.usePVEScoring)
		{
			currentHill.SetLastTeamOnHill(1);
		}
	}

    int GetRandomHillIndex()
    {
        if (hillsAlreadyUsed.Count == hills.Length)
        {
            hillsAlreadyUsed.Clear();
        }

        int index = UnityEngine.Random.Range(0, hills.Length);
        while (hillsAlreadyUsed.Contains(index))
        {
            index = UnityEngine.Random.Range(0, hills.Length);
        }
        return index;
    }

    void StartHill(int index)
    {
        if (currentHill != null)
        {
            currentHill.Deactivate();
            currentHill.OnTeamChanged -= SetDominatingTeam;
        }

        hillsAlreadyUsed.Add(index);
        currentHill = hills[index];
        currentHill.Activate();
        SetDominatingTeam(-1);
        currentHill.OnTeamChanged += SetDominatingTeam;
        var KTH_values = (GameMode_KingOfTheHill)gameModeStats;
        hillMoveTimer = KTH_values.HillMoveTime;


		


	}

    void SetDominatingTeam(int team)
    {
        teamOnHill = team;
        ResetHillPointTimer();
    }

    void CheckCurrentHill()
    {
        if (currentHill == null) return;
        
        currentHill.ScanHill();
    }

    public override void PlayerJoined(PlayerMind player)
    {
        base.PlayerJoined(player);
        ResetHillMoveTimer();

        if (gameModeStats.usePVEScoring)
        {
			player.EnableObjectiveUIMarker();
			var marker = ObjectiveIndicator.instance;
			marker.GetObjective(0).SetHideDistance(0);
		}
    }

    public void ResetHillMoveTimer()
    {
        var KTH_values = (GameMode_KingOfTheHill)gameModeStats;
        hillMoveTimer = KTH_values.HillMoveTime;
    }

    public void ResetHillPointTimer()
    {
        var KTH_values = (GameMode_KingOfTheHill)gameModeStats;
        timeOnHillUntilNextPointScore = KTH_values.TimeToScore;
    }


    public void UpdateHillTimer()
    {
        if (teamOnHill == -1)
        {
            return;
        }

        timeOnHillUntilNextPointScore -= Time.deltaTime;
        if (timeOnHillUntilNextPointScore <= 0)
        {
            GainPoints(teamOnHill, 1);
            ResetHillPointTimer();
        }
    }

    public void UpdateHillMoveTimer()
    {
        var KTH_values = (GameMode_KingOfTheHill)gameModeStats;
        if (!KTH_values.MoveHill)
        {
            return;
        }
        hillMoveTimer -= Time.deltaTime;
        if (KTH_values.team2usesOtherPointsToWin  && teamPoints.Count>1)
        {


   //         var pointsTeam2NeedsToWin = KTH_values.team2PointsToWin - teamPoints[1];
			//ObjectiveIndicator.instance.GetObjective(0).SetText(pointsTeam2NeedsToWin.ToString());
		}
        else
        {
			ObjectiveIndicator.instance.GetObjective(0).SetText(((int)hillMoveTimer).ToString());
		}


            
    }

    public bool CanMoveHill()
    {
        return hillMoveTimer <= 0 && hills.Length > 1;
    }
}
