using UnityEngine;
using System;

[CreateAssetMenu(menuName = "GameModes/KingOfTheHill")]
public class GameMode_KingOfTheHill : GameMode
{
    public Action<Team> OnDominatingTeamChanged;
    public Action<float> OnHillMoveTimerChanged;

    [SerializeField] float timeToScore = 0;

    [SerializeField] bool moveHill = false;
    [SerializeField] float hillMoveTime = 0;

    Team teamOnHill = Team.None;
    float timeOnHillUntilNextPointScore = 0;
    float hillMoveTimer = 0;

    public override void ResetGame()
    {
        base.ResetGame();
        teamOnHill = Team.None;
        ResetHillMoveTimer();
        ResetHillPointTimer();

    }

    public void ResetHillMoveTimer()
    {
        hillMoveTimer = hillMoveTime;
    }

    public void ResetHillPointTimer()
    {
        timeOnHillUntilNextPointScore = timeToScore;
    }

    public void SetDominatingTeam(Team team)
    {
        teamOnHill = team;
        ResetHillPointTimer();
    }

    public void UpdateHillTimer()
    {
        if (teamOnHill == Team.None)
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





}
