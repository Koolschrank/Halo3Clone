using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MenuHeadSystem : MonoBehaviour
{
	// singelton
	public static MenuHeadSystem Instance { get; private set; }

	public GameTypeSelectSystem gameTypeSelectSystem;
	public GameModeSelectSystem gameModeSelectSystem;
	public MapSelectSystem mapSelectSystem;
	public MenuSelectedType menuSelectedType;

	Controller controller;


	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		gameTypeSelectSystem.OnButtonSelect += (button) => EnterGameModeMenu();
		gameModeSelectSystem.OnButtonSelect += (button) => EnterMapMenu();
		mapSelectSystem.OnButtonSelect += (button) => StartGame();

		gameModeSelectSystem.OnCancel += EnterGameTypeMenu;
		mapSelectSystem.OnCancel += EnterGameModeMenu;

		gameTypeSelectSystem.OnButtonHover += (button) => SetUpGameModes(button.SubButtons);
		gameModeSelectSystem.OnButtonHover += (button) => SetUpMaps(button.SubButtons);

		


		controller = new Controller();
		controller.Enable();
		controller.QuickMenu.Down.performed += PressDown;
		controller.QuickMenu.Up.performed += PressUp;
		controller.QuickMenu.Left.performed += PressLeft;
		controller.QuickMenu.Right.performed += PressRight;
		controller.QuickMenu.Select.performed += PressSelect;
		controller.QuickMenu.Cancel.performed += PressCancel;
	}

	private void Start()
	{
		menuSelectedType = MenuSelectedType.Map;
		EnterGameTypeMenu();
	}

	private void OnDisable()
	{
		controller.Disable();
		controller.QuickMenu.Down.performed -= PressDown;
		controller.QuickMenu.Up.performed -= PressUp;
		controller.QuickMenu.Left.performed -= PressLeft;
		controller.QuickMenu.Right.performed -= PressRight;
		controller.QuickMenu.Select.performed -= PressSelect;
		controller.QuickMenu.Cancel.performed -= PressCancel;
	}

	private void PressDown(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		GoDown();
	}

	private void PressUp(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		GoUp();
	}
	private void PressLeft(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		GoLeft();
	}

	private void PressRight(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		GoRight();
	}

	private void PressSelect(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		Select();
	}

	private void PressCancel(CallbackContext context)
	{
		if (!context.ReadValueAsButton()) return;
		Cancel();
	}





	public void SetUpGameModes(List<MenuButton> buttons)
	{
		gameModeSelectSystem.PlaceButtons(buttons);
	}

	public void SetUpMaps(List<MenuButton> buttons)
	{
		mapSelectSystem.PlaceButtons(buttons);
	}

	public void EnterGameTypeMenu()
	{
		if (menuSelectedType == MenuSelectedType.GameType)
			return;
		SetMenuSelected(MenuSelectedType.GameType);
		gameTypeSelectSystem.Enter();
		gameModeSelectSystem.Cancel();
		mapSelectSystem.ClearButtons();

		StartCoroutine(CancelModeMenu());
	}

	IEnumerator CancelModeMenu()
	{
		yield return new WaitForEndOfFrame();
		gameModeSelectSystem.Cancel();
		mapSelectSystem.ClearButtons();
	}

	public void EnterGameModeMenu()
	{
		if (menuSelectedType == MenuSelectedType.GameMode)
			return;
		SetMenuSelected(MenuSelectedType.GameMode);
		gameModeSelectSystem.Enter();
		mapSelectSystem.Cancel();
	}

	public void EnterMapMenu()
	{
		if (menuSelectedType == MenuSelectedType.Map)
			return;
		SetMenuSelected(MenuSelectedType.Map);
		mapSelectSystem.Enter();
	}

	public void StartGame()
	{
		MapLoader.instance.LoadMap();
	}

	public void SetMenuSelected(MenuSelectedType newMenu)
	{
		if (menuSelectedType == newMenu)
			return;
		menuSelectedType = newMenu;
	}

	public void Cancel()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				break;
			case MenuSelectedType.GameMode:
				gameModeSelectSystem.Cancel();
				break;
			case MenuSelectedType.Map:
				mapSelectSystem.Cancel();
				break;
		}
	}

	public void Select()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				gameTypeSelectSystem.SelectButton();
				break;
			case MenuSelectedType.GameMode:
				gameModeSelectSystem.SelectButton();
				break;
			case MenuSelectedType.Map:
				mapSelectSystem.SelectButton();
				break;
		}
	}


	public void GoLeft()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				gameTypeSelectSystem.GoBackward();
				break;
			case MenuSelectedType.GameMode:
				break;
			case MenuSelectedType.Map:
				mapSelectSystem.Cancel();
				break;
		}
	}

	public void GoRight()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				gameTypeSelectSystem.GoForward();
				break;
			case MenuSelectedType.GameMode:
				gameModeSelectSystem.SelectButton();
				break;
			case MenuSelectedType.Map:
				break;
		}
	}

	public void GoUp()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				break;
			case MenuSelectedType.GameMode:
				gameModeSelectSystem.GoBackward();
				break;
			case MenuSelectedType.Map:
				mapSelectSystem.GoBackward();
				break;
		}
	}

	public void GoDown()
	{
		switch (menuSelectedType)
		{
			case MenuSelectedType.GameType:
				gameTypeSelectSystem.SelectButton();
				break;
			case MenuSelectedType.GameMode:
				gameModeSelectSystem.GoForward();
				break;
			case MenuSelectedType.Map:
				mapSelectSystem.GoForward();
				break;
		}
	}

	public void SetSelectTypeToNone()
	{
		menuSelectedType = MenuSelectedType.none;
	}

}

public enum MenuSelectedType
{
	GameType,
	GameMode,
	Map,
	none = 10
}
