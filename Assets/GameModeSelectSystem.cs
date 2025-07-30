using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeSelectSystem : MenuSystem
{
	public TextMeshProUGUI discription;
	public TutorialSelectSystem tutorialSelectSystem;



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
		tutorialSelectSystem.SetTutorial(-1);
		base.Cancel();
		discription.text = "";
		
	}


	public void SetDiscription(MenuButton button)
	{
		discription.text = button.discription;
		tutorialSelectSystem.SetTutorial(button.tutorialIndex);
	}

	public override void SelectButton()
	{


		if (menuButtons.Count == 0) return;

		var button = menuButtons[selectedButtonIndex];
		if ( !button.canBeSelected)
		{
			return;
		}
		MapLoader.instance.SetGameMode(button.gameMode);

		if (button.setAIAmount)
			MapLoader.instance.SetAIAmount(button.aiAmountMultiplier);
		base.SelectButton();
	}

	public override void OnClick(MenuButton button)
	{
		selectedButtonIndex = menuButtons.IndexOf(button);
		MenuHeadSystem.Instance.SetSelectTypeToNone();
		UpdateButtonSelection();
		MenuHeadSystem.Instance.EnterGameModeMenu();
		SelectButton();

	}   
	
}
