using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class KingOfTheHillManager : GameModeManager
{
    
    [SerializeField] int hillStartIndex = 0;
    [SerializeField] Hill[] hills;
    [SerializeField] float checkInterval = 0.1f;

    float checkTimer = 0;
    List<int> hillsAlreadyUsed = new List<int>();
    Hill currentHill;

    public Action<int> OnDominatingTeamChanged;
    public Action<float> OnHillMoveTimerChanged;
    int teamOnHill = -1;
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
        }



    }

    void StartRandomHill()
    {
        StartHill(GetRandomHillIndex());
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
        ObjectiveIndicator.instance.GetObjective(0).SetNumber((int)hillMoveTimer);
    }

    public bool CanMoveHill()
    {
        return hillMoveTimer <= 0 && hills.Length > 1;
    }
}
