using TMPro;
using UnityEngine;

public class UI_ExtraLifes : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI extraLifesText;
	void Start()
    {
        var gameMode = GameModeSelector.gameModeManager;
        
        if (!gameMode.GameModeStats.hasRespawnTokens)
        {
            gameObject.SetActive(false);
            return;
		}

        gameMode.OnRespawnTokensChanged += UpdateExtraLifesText;
        UpdateExtraLifesText(gameMode.RespawnTokensLeft);
            
	}

    void UpdateExtraLifesText(int tokensLeft)
    {
        extraLifesText.text = tokensLeft.ToString();

        if (tokensLeft <= 0)
        {
            gameObject.SetActive(false);
        }
        
	}





	// Update is called once per frame
	void Update()
    {
        
    }
}
