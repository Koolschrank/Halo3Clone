using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] ScoreBarUI[] scoreBars;

    public void Start()
    {
        GameModeSelector.gameModeManager.OnPointsUpdated += SetScore;
        GameModeSelector.gameModeManager.OnTeamAdded += TeamJoined;

        var score = GameModeSelector.gameModeManager.GetTeamPoints();
        var teamsCount = GameModeSelector.gameModeManager.GetTeamsWithPlayers();



        for (int i = 0; i < scoreBars.Length; i++)
        {
            scoreBars[i].gameObject.SetActive(false);
        }

        TeamJoined();
    }

    public void TeamJoined()
    {
        for (int i = 0; i < GameModeSelector.gameModeManager.GetTeamsWithPlayers(); i++)
        {
            scoreBars[i].gameObject.SetActive(true);
            scoreBars[i].SetMaxScore(GameModeSelector.gameModeManager.GetMaxScore());
        }
    }

    public void SetScore(int team,int score)
    {
        scoreBars[team].SetScore(score);
    }
}
