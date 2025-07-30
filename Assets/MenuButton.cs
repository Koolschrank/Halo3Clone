using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MenuButton : MonoBehaviour
{
	public Action OnHover;

	public Image buttonImage;
	public List<MenuButton> SubButtons;

	[TextArea]
	public string discription;
	public Sprite discriptionImage = null;

	public Color baseColor = Color.white;
	public Color hoverColor = Color.yellow;
	public Color selectedColor = Color.green;



	[Header("values")]
	public GameMode gameMode;
	public string mapName = "";
	public bool setAIAmount = false;
	public float aiAmountMultiplier = 1;
	public Sprite mapImage;

	public void Select()
	{
		buttonImage.color = selectedColor;
	}


	public void Hover(bool isHoverd)
	{
		if (isHoverd)
		{
			buttonImage.color = hoverColor;
			OnHover?.Invoke();
		}
		else
		{
			buttonImage.color = baseColor;
		}
	}
}
