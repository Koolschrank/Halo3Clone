using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField] int teamIndex;

    public int TeamIndex => teamIndex;

    public void SetTeamIndex(int teamIndex)
    {
        this.teamIndex = teamIndex;

    }


}
