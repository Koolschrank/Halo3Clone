using UnityEngine;
using System;

[CreateAssetMenu(menuName = "GameModes/KingOfTheHill")]
public class GameMode_KingOfTheHill : GameMode
{
    /*
    public Action<int> OnDominatingTeamChanged;
    public Action<float> OnHillMoveTimerChanged;

    [SerializeField] float timeToScore = 0;

    [SerializeField] bool moveHill = false;
    [SerializeField] float hillMoveTime = 0;

    int teamOnHill = -1;
    float timeOnHillUntilNextPointScore = 0;
    float hillMoveTimer = 0;


    public void ResetHillMoveTimer()
    {
        hillMoveTimer = hillMoveTime;
    }

    public void ResetHillPointTimer()
    {
        timeOnHillUntilNextPointScore = timeToScore;
    }

    public void SetDominatingTeam(int team)
    {
        teamOnHill = team;
        ResetHillPointTimer();
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
        if (!moveHill)
        {
            return;
        }
        hillMoveTimer -= Time.deltaTime;
    }

    public bool CanMoveHill()
    {
        return hillMoveTimer <= 0;
    }



    */

}
