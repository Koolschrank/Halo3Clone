using System.Collections;
using UnityEngine;
using MultiplayerGameModes;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] GameMode gameMode;
    [SerializeField] float winScreenTime = 5;

    [Header("Dependencies")]
    [SerializeField] DeathMatchManager deathMatchManager;
    [SerializeField] KingOfTheHillManager kingOfTheHillManager;
    [SerializeField] CrownManager crownManager;
    [SerializeField] CaptureTheFlagManager captureTheFlagManager;


    public static GameModeManager gameModeManager;

    private void Start()
    {
        var mapLoader = MapLoader.instance;
        if (mapLoader != null)
        {
            gameMode = mapLoader.GetGameMode();
        }


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
        else if (gameMode is GameMode_Crown)
        {
            crownManager.StartGame(gameMode);
            gameModeManager = crownManager;
        }
        else if (gameMode is GameMode_CaptureTheFlag)
        {
            captureTheFlagManager.StartGame(gameMode);
            gameModeManager = captureTheFlagManager;
        }

        gameModeManager.OnTeamWon += StartReloadGameWithTimer;
    }

    public void StartReloadGameWithTimer(int teamWon)
    {
        gameModeManager.OnTeamWon -= StartReloadGameWithTimer;
        StartCoroutine(ReloadGameWithTime());
    }

    IEnumerator ReloadGameWithTime()
    {
        yield return new WaitForSeconds(winScreenTime);
        ReloadScene();
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
