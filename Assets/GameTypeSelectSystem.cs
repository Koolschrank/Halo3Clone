using UnityEngine;
using System;

public class GameTypeSelectSystem : MenuSystem
{
	public Action OnHoverTutorialButton;
	public Action OnHoverExitTutorialButton;
	public int tutorialButtonIndex = 0;
	public int modefiersButtonIndex = 1;
	public GameObject modifierPage;

	protected override void UpdateButtonSelection()
	{
		base.UpdateButtonSelection();
		if (selectedButtonIndex == tutorialButtonIndex)
		{
			OnHoverTutorialButton?.Invoke();
		}
		else
		{
			OnHoverExitTutorialButton?.Invoke();
		}

		if (selectedButtonIndex == modefiersButtonIndex)
		{
			modifierPage.SetActive(true);
		}
		else
		{
			modifierPage.SetActive(false);
		}
	}

	public void ClickWithIndex(int index)
	{
		var button = menuButtons[index];
		if (button != null)
		{
			selectedButtonIndex = index;

			MenuHeadSystem.Instance.SetSelectTypeToNone();
			
			MenuHeadSystem.Instance.EnterGameTypeMenu();
			UpdateButtonSelection();
			SelectButton();
			
		}
		
	}

}
