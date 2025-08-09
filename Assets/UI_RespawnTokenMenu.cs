using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RespawnTokenMenu : MonoBehaviour
{
    [SerializeField] PlayerMind playerMind;
	[SerializeField] TextMeshProUGUI respawnText;
	[SerializeField] Slider tockenUseTimer;

	[SerializeField] Image tokenButtonIcon;
	[SerializeField] Sprite keyboardIcon;

	public void SetKeyboardIcon()
	{
		Debug.Log("Setting keyboard icon for respawn token button");
		tokenButtonIcon.sprite = keyboardIcon;
	}
	

	private void Awake()
	{
		if (GameModeSelector.gameModeManager.GameModeStats.hasRespawnTokens)
		{
			UpdateTokens(GameModeSelector.gameModeManager.RespawnTokensLeft);
			GameModeSelector.gameModeManager.OnRespawnTokensChanged += UpdateTokens;
			playerMind.OnTokenUseUpdate += UpdateTokenUseTimer;
		}
		else
		{
			gameObject.SetActive(false);
			tockenUseTimer.gameObject.SetActive(false);
		}
	}

	private void UpdateTokens(int val)
	{
		if (respawnText != null)
		{
			respawnText.text =  val.ToString() + " Tokens";
		}

		if (val == 0)
		{
			gameObject.SetActive(false);
			tockenUseTimer.gameObject.SetActive(false);
		}
	}

	private void UpdateTokenUseTimer(float val)
	{
		if (tockenUseTimer != null)
		{
			tockenUseTimer.value = val;
		}
		else
		{
			
		}
	}




}
