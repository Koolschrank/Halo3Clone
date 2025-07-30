using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class MenuSystem : MonoBehaviour
{
    public Action<MenuButton> OnButtonHover;
	public Action<MenuButton> OnButtonSelect;
	public Action OnCancel;



	public List<MenuButton> menuButtons;
    public int selectedButtonIndex = 0;

    public RectTransform firstButtonTransform;
    public Vector2 buttonOffset;


    public virtual void PlaceButtons(List<MenuButton> newButtons)
    {
		// remove existing buttons
        foreach (var button in menuButtons)
        {
            Destroy(button.gameObject);
        }
        menuButtons.Clear();

        int index = 0;
		foreach (var button in newButtons)
        {
            var newButton = Instantiate(button, firstButtonTransform.position, Quaternion.identity, transform);
            menuButtons.Add(newButton);
            newButton.GetComponent<RectTransform>().position = firstButtonTransform.position + new Vector3(buttonOffset.x, buttonOffset.y,0) * index;
			index++;
		}

        selectedButtonIndex = 0;
	}

    public void ClearButtons()
    {
        foreach (var button in menuButtons)
        {
            Destroy(button.gameObject);
        }
        menuButtons.Clear();
        selectedButtonIndex = 0;
	}


	void Start()
	{
		UpdateButtonSelection();
	}

    public void Enter()
    {
        if (menuButtons.Count == 0) return;
        if (selectedButtonIndex >= menuButtons.Count)
        {
            selectedButtonIndex = 0;
		}
		UpdateButtonSelection();
    }

	public void GoForward()
    {
        if (menuButtons.Count == 0) return;
        selectedButtonIndex = (selectedButtonIndex + 1) % menuButtons.Count;
        UpdateButtonSelection();
	}

    public void GoBackward()
    {
        if (menuButtons.Count == 0) return;
        selectedButtonIndex = (selectedButtonIndex - 1 + menuButtons.Count) % menuButtons.Count;
        UpdateButtonSelection();
    }

	public virtual void Cancel()
	{
		OnCancel?.Invoke();
		foreach (var button in menuButtons)
		{
			button.Hover(false);
		}
		selectedButtonIndex = 0;
	}

	public virtual void SelectButton()
    {
        if (menuButtons.Count == 0) return;
        menuButtons[selectedButtonIndex].Select();
        OnButtonSelect?.Invoke(menuButtons[selectedButtonIndex]);
	}
	protected void UpdateButtonSelection()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            var button = menuButtons[i];
			button.Hover(i == selectedButtonIndex);
		}

		OnButtonHover?.Invoke(menuButtons[selectedButtonIndex]);
	}
}
