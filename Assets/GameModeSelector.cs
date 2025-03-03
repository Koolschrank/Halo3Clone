using System.Collections;
using UnityEngine;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] GameMode gameMode;
    [SerializeField] float winScreenTime = 5;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
