using TMPro;
using UnityEngine;

public class TeamWinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        text.enabled = false;
    }

    public void TeamWon(int teamIndex)
    {

		text.enabled = true;
		if (GameModeSelector.gameModeManager.GameModeStats.usePVEScoring)
        {
			if (teamIndex == 0)
			{
				text.text = "You Won";
			}
            else
            {
				text.text = "You Lost, generator empty";
			}
            return;
		}


        
        text.text = $"Team {teamIndex + 1} won!";
    }
}
