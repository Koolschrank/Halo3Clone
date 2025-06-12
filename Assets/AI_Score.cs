using UnityEngine;

public class AI_Score : MonoBehaviour
{
    [SerializeField] PlayerTeam team;
    [SerializeField] TargetHitCollector targetHitCollector;


    private void Start()
    {
        targetHitCollector.OnCharacterKill += OnCharacterKill;
    }

    private void OnCharacterKill(GameObject character)
    {
        var gameMode = GameModeSelector.gameModeManager;
        var deathmatchMode = gameMode.GameModeStats as GameMode_Deathmatch;
        if (deathmatchMode == null)
        {
            
            return;
        }
        return;

        if (character.GetComponent<PlayerTeam>().TeamIndex == team.TeamIndex)
        {
            GameModeSelector.gameModeManager.AIGainsPoints(team.TeamIndex, -1);
        }
        else
        {
            GameModeSelector.gameModeManager.AIGainsPoints(team.TeamIndex, 1);
        }
    }
}
