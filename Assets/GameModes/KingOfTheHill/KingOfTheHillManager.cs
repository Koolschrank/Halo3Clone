using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class KingOfTheHillManager : GameModeManager
{
    /*
    [SerializeField] int hillStartIndex = 0;
    [SerializeField] Hill[] hills;
    [SerializeField] float checkInterval = 0.1f;
    [SerializeField] bool startOnStart = false;

    float checkTimer = 0;
    List<int> hillsAlreadyUsed = new List<int>();
    Hill currentHill;


    private void Start()
    {
        if (startOnStart)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
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
        gameStats.UpdateHillTimer();
        gameStats.UpdateHillMoveTimer();

        if (gameStats.CanMoveHill())
        {
            StartRandomHill();
            gameStats.ResetHillMoveTimer();
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

        int index = Random.Range(0, hills.Length);
        while (hillsAlreadyUsed.Contains(index))
        {
            index = Random.Range(0, hills.Length);
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
        hills[index].Activate();
        currentHill = hills[index];
        SetDominatingTeam(-1);
        currentHill.OnTeamChanged += SetDominatingTeam;

    }

    void SetDominatingTeam(int team)
    {
        (GameMode_KingOfTheHill)gameModeStats.SetDominatingTeam(team);
    }

    void CheckCurrentHill()
    {
        if (currentHill == null) return;

        currentHill.ScanHill();
    }*/
}
