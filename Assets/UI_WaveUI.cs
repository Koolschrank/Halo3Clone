using TMPro;
using UnityEngine;

public class UI_WaveUI : MonoBehaviour
{

    public TextMeshProUGUI text;
    public int pointsForNextWave = 100;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {



        var gamemode = GameModeSelector.gameModeManager;
        
        if (!gamemode.GameModeStats.usePVEScoring)
        {
                       gameObject.SetActive(false);
            return;
		}


        gamemode.OnPointsUpdated += (team, points) =>
        {
            if (team == 0)
            {
                UpdatePoints(points);
            }
        };

        UpdatePoints(0);
	}

    public void UpdatePoints(int points)
    {
        int wave = 1;

        while (points >= pointsForNextWave)
        {
            points -= pointsForNextWave;
            wave++;
		}

        text.text = wave.ToString() + "/10";
	}
}
