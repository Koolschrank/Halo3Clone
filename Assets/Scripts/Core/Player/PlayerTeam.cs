using System;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    public Action<int> OnTeamIndexChanged;


    [SerializeField] int teamIndex;

    public int TeamIndex => teamIndex;

    public void SetTeamIndex(int teamIndex)
    {
        if (teamIndex == this.teamIndex) return;


        this.teamIndex = teamIndex;
        OnTeamIndexChanged.Invoke(teamIndex);


    }


}
