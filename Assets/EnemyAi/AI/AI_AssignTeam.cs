using UnityEngine;

public class AI_AssignTeam : MonoBehaviour
{ 

    [SerializeField] int teamIndex = 4;

    [SerializeField] BodyMindConnection bodyMindConnection;

    private void Start()
    {
        PlayerManager.instance.UpdateTeamOfEnemyAI(bodyMindConnection, teamIndex);
    }
}
