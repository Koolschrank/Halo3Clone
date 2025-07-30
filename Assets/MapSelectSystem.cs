using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectSystem : MenuSystem
{
	public Image imageMask;
	public Image mapImage;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public override void PlaceButtons(List<MenuButton> newButtons)
	{
		base.PlaceButtons(newButtons);
		foreach (MenuButton button in menuButtons)
		{
			button.OnHover += () => SetImage(button);
		}
	}

	public override void Cancel()
	{
		base.Cancel();
		imageMask.gameObject.SetActive(false);
	}

	public void SetImage(MenuButton button)
	{
		mapImage.sprite = button.mapImage;
		imageMask.gameObject.SetActive(true);
	}

	public override void SelectButton()
	{
		if (menuButtons.Count == 0) return;
		MapLoader.instance.SetSceneToLoad(menuButtons[selectedButtonIndex].mapName);

		base.SelectButton();
	}
}



