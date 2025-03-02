using UnityEngine;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] GameMode gameMode;

    [Header("Dependencies")]
    [SerializeField] DeathMatchManager deathMatchManager;
    [SerializeField] KingOfTheHillManager kingOfTheHillManager;


    public static GameModeManager gameModeManager;

    private void Start()
    {
        if (gameMode is GameMode_Deathmatch)
        {
            deathMatchManager.StartGame(gameMode);
            gameModeManager = deathMatchManager;
        }
        else if (gameMode is GameMode_KingOfTheHill)
        {
            kingOfTheHillManager.StartGame(gameMode);
            gameModeManager = kingOfTheHillManager;
        } 
    }
}
