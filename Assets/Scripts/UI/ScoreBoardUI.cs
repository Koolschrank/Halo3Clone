using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] ScoreBarUI[] scoreBars;

    public void Start()
    {
		var gamemodeStats = GameModeSelector.gameModeManager.GameModeStats;
		if (gamemodeStats.usePVEScoring)
		{
			gameObject.SetActive(false);
			return;
		}


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
            if (team > 0 || i <2)
            {
                scoreBars[i].gameObject.SetActive(true);
                scoreBars[i].SetMaxScore(GameModeSelector.gameModeManager.GetMaxScore(i));
            }
            else
            {
                scoreBars[i].gameObject.SetActive(false);
            }


        }

        scoreBars[1].gameObject.SetActive(true);
        scoreBars[1].SetMaxScore(GameModeSelector.gameModeManager.GetMaxScore(1));
    }

    public void SetScore(int team,int score)
    {
        scoreBars[team].SetScore(score);
    }
}
