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

    [SerializeField] protected bool looseWhenAllPlayersDead = false; // if true, when all players are dead, the game is over and the team with the most points wins
    [SerializeField] protected bool hasAiPlayers;
	[SerializeField] protected bool hasAiTeamMembers;
	[SerializeField] protected bool useStatSheets = false;
    [SerializeField] protected bool dontDropWeaponsOnDeath = false; // if true, players do not drop their weapons on death, instead they respawn with their starting equipment
    
    [SerializeField] int startScore = 0; // starting score for each player, used in the scoreboard
    [SerializeField] PlayerStatsSheet playerStatsSheetBlueprint;
	public bool spawnWithArmor;
	public bool EnemyTeamsWorkingTogether = false; // if true, enemy teams work together, otherwise they fight each other

    public bool team2usesOtherPointsToWin = false; // if true, team 2 uses a different points to win value, otherwise both teams use the same value
    public int team2PointsToWin = 0; // points to win for team 2, if team2usesOtherPointsToWin is true
    public bool team2LoosesScoreWhenTeam1scores = false; // if true, team 2 looses score when team 1 scores, otherwise they do not loose score
    public int amountOfPointsTeam1NeedsToScoreToMakeTeam2LoosePoints = 0; // amount of points team 1 needs to score to make team 2 loose points, if team2LoosesScoreWhenTeam1scores is true
    public bool useEnemyWaves;
    public bool noDualWieldDamageReduction = false; // if true, dual wielding does not reduce damage, otherwise it does
    public bool startingEquipmentBasedOnPlayerValues = false;
    public bool cannotDropSwitchOrPickupWeapons = false; // if true, players cannot drop, switch or pickup weapons, they can only use their starting equipment


	public Equipment StartingEquipment { get { return startingEquipment; } }
    public float TimeLimitInMinutes { get { return timeLimitInMinutes; } }

    public float ai_damageMultiplier = 1f; // multiplier for AI damage, used to balance AI difficulty
    public float weaponRespawnTimeMultiplier = 1f; // multiplier for weapon respawn time, used to balance weapon respawn time

    public bool recolerTeam1Members = false; // if true, team 1 members can recolor each other, otherwise they cannot
    public Color[] team1MemberColors; // colors for team 1 members, used to recolor team 1 members
	public Color[] team1MemberColorsUI; // colors for team 1 members, used to recolor team 1 members


    public int usedItemList = 0; // used item list, used to determine which item list to use for weapon pickups

    public bool removePlayerBodyWhenRespawned = false; // if true, player body is removed when respawned, otherwise it is not
    public bool hasRespawnTokens = false; // if true, players have respawn tokens, otherwise they do not
    public int respawnTokens = 3; // number of respawn tokens each player has, used to determine how many times a player can respawn before they are out of respawn tokens
    public bool hasReviveBodies = false; // if true, players can revive each other using revive bodies, otherwise they cannot

    public int scoreMultiplier = 1; // score multiplier for team 1, used to determine how many points team 1 gets for scoring
	public virtual Equipment GetEquipmentBasedOnPoints(int points)
    {
        return StartingEquipment;

	}

    public int GetPointsToWin(int playerCount, bool isSmallMap, int teamIndex)
    {
        int points = pointsToWin;

		if (team2usesOtherPointsToWin)
		{
			if (teamIndex == 1)
            {
                return team2PointsToWin;
			}
            else
                {
                return pointsToWin;
			}
		}

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

    public bool LooseWhenAllPlayersDead { get { return looseWhenAllPlayersDead; } }

    public bool HasAiPlayers { get { return hasAiPlayers; } }

    public bool HasAiTeamMembers { get { return hasAiTeamMembers; } }

	public bool UseStatSheet { get { return useStatSheets; } }

    public int StartScore { get { return startScore; } }

    public bool DontDropWeaponsOnDeath { get { return dontDropWeaponsOnDeath; } }
    public PlayerStatsSheet PlayerStatSheet { get { return playerStatsSheetBlueprint; } }
}

