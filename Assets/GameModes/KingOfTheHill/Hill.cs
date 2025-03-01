using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public Action<Team> OnTeamChanged;
    public Action OnActivated;
    public Action OnDeactivated;


    [SerializeField] float radius = 10;
    
    float checkTimer = 0;

    Team teamOnHill = Team.None;

    bool active = false;

    public void Activate()
    {
        active = true;
        SetTeamOnHill(Team.None);
        OnActivated?.Invoke();
    }


    public void Deactivate()
    {
        active = false;
        SetTeamOnHill(Team.None);
        OnDeactivated?.Invoke();
    }

    public void ScanHill()
    {
        if (!active)
        {
            return;
        }

        List<Team> playersOnHill = new List<Team>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            PlayerTeam player = collider.GetComponent<PlayerTeam>();
            if (player != null)
            {
                playersOnHill.Add(player.Team);
            }
        }

        var dominatingTeam = GetDominatingTeamOnHill(playersOnHill);
        if (dominatingTeam != teamOnHill)
        {
            SetTeamOnHill(dominatingTeam);
        }




    }


    public Team GetDominatingTeamOnHill(List<Team> playersOnHill)
    {
        int blueCount = 0;
        int redCount = 0;
        foreach (Team team in playersOnHill)
        {
            switch (team)
            {
                case Team.Blue:
                    blueCount++;
                    break;
                case Team.Red:
                    redCount++;
                    break;
            }
        }
        if (blueCount > redCount)
        {
            return Team.Blue;
        }
        else if (redCount > blueCount)
        {
            return Team.Red;
        }
        else
        {
            return Team.None;
        }

    }

    public void SetTeamOnHill(Team team)
    {
        teamOnHill = team;

        OnTeamChanged?.Invoke(team);
    }

    // gizmo to show the hill radius
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public float Radius => radius;


}
