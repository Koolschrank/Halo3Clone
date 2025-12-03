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


	
    public bool hasFlag = false;

	public GameObject team1_FlagPrefab;
	public Transform team1_FlagSpawnPoint;
	GameObject team1_Flag;
	Weapon_Data team1_FlagData;


	public float flagRecoveryTimer = 15f;
	bool flag1_droped = false;
	float flag1_dropedTimer = 0;

    public Transform GetRandomHill()
    {
        int index = UnityEngine.Random.Range(0, hills.Length);
        return hills[index].transform;
	}


	public override void ResetGame()
    {
        base.ResetGame();


        hillsAlreadyUsed.Clear();

        foreach (Hill hill in hills)
        {
            hill.Deactivate();
        }

        StartHill(hillStartIndex);


        if (hasFlag)
        {
            EnterFlagMode();
        }
    }
        
	public override void PlayerSpawned(PlayerMind player)
	{
        base.PlayerSpawned(player);
		if (hasFlag)
        {
            SetPlayerFlagEvents(player);
		}
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

       
        if (hasFlag)
        {
            FlagUpdate();
		}

    }

    public void FlagUpdate()
    {
		var objective = ObjectiveIndicator.instance.GetObjective(1);
		if (team1_Flag != null)
		{
            Debug.Log("Flag position: " + team1_Flag.transform.position);

			objective.SetPosition(team1_Flag.transform.position);
			objective.SetHideDistance(1);
            objective.SetText("Flag");
            objective.SetTeamIndex(0);
            objective.SetActive(true);
		}

		if (flag1_droped)
		{
			flag1_dropedTimer -= Time.deltaTime;
			objective.SetText(((int)flag1_dropedTimer).ToString());
			if (flag1_dropedTimer <= 0)
			{
				LogSystem.logSystem.PrintLog("Blue Flag Reset");
				SpawnFlagPrefab_Team1();
			}
		}
		else
		{
			flag1_dropedTimer += Time.deltaTime;

		}

	}



	public void SpawnFlagPrefab_Team1()
	{
		if (team1_Flag != null)
		{
			Destroy(team1_Flag);
		}
		flag1_droped = false;



		team1_Flag = Instantiate(team1_FlagPrefab, team1_FlagSpawnPoint.position, Quaternion.identity);
		var pickUp = team1_Flag.GetComponent<Weapon_PickUp>();
		team1_FlagData = pickUp.WeaponData;

		// flag

		ObjectiveIndicator.instance.GetObjective(1).SetActive(true);
		ObjectiveIndicator.instance.GetObjective(1).SetTeamIndex(0);
		ObjectiveIndicator.instance.GetObjective(1).SetText("Flag");
		ObjectiveIndicator.instance.GetObjective(1).SetHideDistance(1);
		ObjectiveIndicator.instance.GetObjective(1).SetPosition(team1_FlagSpawnPoint.position + Vector3.up * 1);

	}

	public void FlagPickedUp_Team1(GameObject player)
	{
		team1_Flag = player;
		ObjectiveIndicator.instance.GetObjective(1).SetActive(true);
		flag1_droped = false;
	}

	public void FlagDroped_Team1(Weapon_PickUp pickUp)
	{
		if (pickUp == null)
		{
			return;
		}
		team1_Flag = pickUp.gameObject;
		flag1_droped = true;
		flag1_dropedTimer = Mathf.Min(flag1_dropedTimer, flagRecoveryTimer);
		var pickup = team1_Flag.GetComponent<Weapon_PickUp>();
		

	}

    public void EnterFlagMode()
    {
        hasFlag = true;
        SpawnFlagPrefab_Team1();
        var allPlayers = PlayerManager.instance.GetAllPlayers();
        foreach (var player in allPlayers)
        {
            SetPlayerFlagEvents(player);
		}
	}

    public void ExitFlagMode()
    {
        hasFlag = false;
        if (team1_Flag != null)
        {
            if (team1_Flag.CompareTag("Player"))
            {
                var arms = team1_Flag.GetComponent<PlayerArms>();
                var rightWeapon = arms.RightArm.CurrentWeapon;
                if (rightWeapon.Data == team1_FlagData)
                {
                    arms.RightArm.DropWeapon();
                }
                else
                {
                    arms.LeftArm.DropWeapon();
                }
            }
            else
            {
                Destroy(team1_Flag);

            }
            team1_Flag = null;
        }
        ObjectiveIndicator.instance.GetObjective(1).SetActive(false);
        ObjectiveIndicator.instance.GetObjective(1).SetHideDistance(10000);

        var allPlayers = PlayerManager.instance.GetAllPlayers();
        foreach (var player in allPlayers)
        {
            RemovePlayerFlagEvents(player);
        }
    }

	public void SetPlayerFlagEvents(PlayerMind player)
    {
		var arms = player.PlayerBody.GetComponent<PlayerArms>();
		arms.LeftArm.OnWeaponPickedUp += (weapon) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagPickedUp_Team1(player.PlayerBody);
			}
			
		};

		arms.RightArm.OnWeaponPickedUp += (weapon) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagPickedUp_Team1(player.PlayerBody);
			}
			
		};

		arms.LeftArm.OnWeaponDroped += (weapon, pickUp) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagDroped_Team1(pickUp);
			}
			
		};

		arms.RightArm.OnWeaponDroped += (weapon, pickUp) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagDroped_Team1(pickUp);
			}
			
		};
	}

	public void RemovePlayerFlagEvents(PlayerMind player)
	{
		var arms = player.PlayerBody.GetComponent<PlayerArms>();
		arms.LeftArm.OnWeaponPickedUp -= (weapon) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagPickedUp_Team1(player.PlayerBody);
			}

		};

		arms.RightArm.OnWeaponPickedUp -= (weapon) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagPickedUp_Team1(player.PlayerBody);
			}

		};

		arms.LeftArm.OnWeaponDroped -= (weapon, pickUp) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagDroped_Team1(pickUp);
			}

		};

		arms.RightArm.OnWeaponDroped -= (weapon, pickUp) =>
		{
			if (weapon.Data == team1_FlagData)
			{
				FlagDroped_Team1(pickUp);
			}

		};
	}



    Hill lastHill;
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
        while (hillsAlreadyUsed.Contains(index) || hills[index] == lastHill)
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

        lastHill = currentHill;



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


			//marker.GetObjective(1).SetHideDistance(0);
            //marker.GetObjective(1).SetActive(false);
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
