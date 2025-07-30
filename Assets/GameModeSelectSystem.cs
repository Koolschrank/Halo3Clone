using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeSelectSystem : MenuSystem
{
	public TextMeshProUGUI discription;



	public override void PlaceButtons(List<MenuButton> newButtons)
	{
		base.PlaceButtons(newButtons);
		foreach (MenuButton button in menuButtons)
		{
			button.OnHover += () => SetDiscription(button);
		}
	}

	public override void Cancel()
	{
		base.Cancel();
		discription.text = "";
	}


	public void SetDiscription(MenuButton button)
	{
		discription.text = button.discription;
	}

	public override void SelectButton()
	{
		if (menuButtons.Count == 0) return;
		var button = menuButtons[selectedButtonIndex];
		MapLoader.instance.SetGameMode(button.gameMode);

		if (button.setAIAmount)
			MapLoader.instance.SetAIAmount(button.aiAmountMultiplier);
		base.SelectButton();
	}
}
