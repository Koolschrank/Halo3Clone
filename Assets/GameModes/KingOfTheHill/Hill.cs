using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public Action<int> OnTeamChanged;
    public Action OnActivated;
    public Action OnDeactivated;


    [SerializeField] float radius = 10;
    [SerializeField] LayerMask LayerMask;
    
    float checkTimer = 0;

    int teamOnHill = -1;

    bool active = false;

    public void Activate()
    {
        active = true;
        SetTeamOnHill(-1);
        OnActivated?.Invoke();
    }


    public void Deactivate()
    {
        active = false;
        SetTeamOnHill(-1);
        OnDeactivated?.Invoke();
    }

    public void ScanHill()
    {
        Debug.Log("Scanning hill");
        if (!active)
        {
            return;
        }

        List<int> playersOnHill = new List<int>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask);
        foreach (Collider collider in colliders)
        {
            PlayerTeam player = collider.GetComponent<PlayerTeam>();
            if (player != null)
            {
                playersOnHill.Add(player.TeamIndex);
            }
        }

        var dominatingTeam = GetDominatingTeamOnHill(playersOnHill);
        if (dominatingTeam != teamOnHill)
        {
            SetTeamOnHill(dominatingTeam);
        }

        




    }


    public int GetDominatingTeamOnHill(List<int> playersOnHill)
    {
        int[] teamCounts = new int[8];
        foreach (int team in playersOnHill)
        {
            teamCounts[team] += 1;
        }
        // return the team with the most players on the hill
        int max = 0;
        int maxIndex = -1;
        for (int i = 0; i < teamCounts.Length; i++)
        {
            if (teamCounts[i] > max)
            {
                max = teamCounts[i];
                maxIndex = i;
            }
            else if (teamCounts[i] == max)
            {
                maxIndex = -1;
            }
        }
        return maxIndex;

    }

    public void SetTeamOnHill(int teamIndex)
    {
        teamOnHill = teamIndex;

        OnTeamChanged?.Invoke(teamIndex);
    }

    // gizmo to show the hill radius
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public float Radius => radius;


}
