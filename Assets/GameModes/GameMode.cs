using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : ScriptableObject
{
    [SerializeField] string gameModeName;
    [SerializeField] Equipment startingEquipment;
    [SerializeField] protected float timeLimitInMinutes =  10f;
    [SerializeField] protected int pointsToWin = 0;
    [SerializeField] protected int teamCount = 2;
    [SerializeField] protected float respawnTime = 4;
    [SerializeField] protected bool hasWeaponPickups = false;
    [SerializeField] protected bool reasignsTeamsInPlayerOrder = false; // first half of players are team blue second half are team red 

    [SerializeField] protected float pointsToWinMultiplier_MoreThan4Players = 1f;
    [SerializeField] protected float pointsToWinMultiplier_smallMap = 1f;


    


    public Equipment StartingEquipment { get { return startingEquipment; } }
    public float TimeLimitInMinutes { get { return timeLimitInMinutes; } }

    public int GetPointsToWin(int playerCount, bool isSmallMap)
    {
        int points = pointsToWin;

        if (playerCount >= 4)
        {
            points = Mathf.RoundToInt(points * pointsToWinMultiplier_MoreThan4Players);

        }
        if (isSmallMap)
        {
            points = Mathf.RoundToInt(points * pointsToWinMultiplier_smallMap);
        }
        return points;
    }

    public int PointsToWin { get { return pointsToWin; } }

    public int TeamCount { get { return teamCount; } }

    public string GameModeName { get { return gameModeName; } }

    public bool HasWeaponPickups { get { return hasWeaponPickups; } }

    public bool ReasignsTeamsInPlayerOrder { get { return reasignsTeamsInPlayerOrder; } }

    public float RespawnTime {  get { return respawnTime; } }
}

