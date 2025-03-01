using System;
using UnityEngine;

public class GameMode : ScriptableObject
{
    public Action<Team, int> OnPointsGained;
    public Action<Team> OnTeamWon;


    [SerializeField] int pointsToWin = 0;
    int pointsBlueTeam = 0;
    int pointsRedTeam = 0;

    public virtual void ResetGame()
    {
        pointsBlueTeam = 0;
        pointsRedTeam = 0;
    }

    protected void GainPoints(Team team, int points)
    {
        switch (team)
        {

            case Team.Blue:
                pointsBlueTeam += points;
                Debug.Log("Blue team has " + pointsBlueTeam + " points");
                break;

            case Team.Red:
                pointsRedTeam += points;
                Debug.Log("Red team has " + pointsRedTeam + " points");
                break;
            default:
                return;
        }

        

        OnPointsGained?.Invoke(team, points);
        if (HasTeamWon(team))
        {
            OnTeamWon?.Invoke(team);
            Debug.Log("Team " + team + " has won!");
        }
    }

    bool HasTeamWon(Team team)
    {
        switch (team)
        {
            case Team.Blue:
                return pointsBlueTeam >= pointsToWin;
            case Team.Red:
                return pointsRedTeam >= pointsToWin;
            default:
                return false;
        }
    }

}

