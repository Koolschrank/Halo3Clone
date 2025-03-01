using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField] Team team;

    public Team Team => team;

    public void SetTeam(Team team)
    {
        this.team = team;
    }
}



public enum Team
{
    None,
    Blue,
    Red,
    
}
