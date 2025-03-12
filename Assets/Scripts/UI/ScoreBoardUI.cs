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
        var teams = GameModeSelector.gameModeManager.GetTeamsWithPlayers();
        for (int i = 0; i < teams.Count; i++)
        {
            var team = teams[i];
            if (team > 0)
            {
                scoreBars[i].gameObject.SetActive(true);
                scoreBars[i].SetMaxScore(GameModeSelector.gameModeManager.GetMaxScore());
            }
            else
            {
                scoreBars[i].gameObject.SetActive(false);
            }


        }
    }

    public void SetScore(int team,int score)
    {
        scoreBars[team].SetScore(score);
    }
}
