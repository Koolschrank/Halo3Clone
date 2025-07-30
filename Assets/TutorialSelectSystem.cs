using System.Collections.Generic;
using UnityEngine;

public class TutorialSelectSystem : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialGameobjects;

	[SerializeField] GameObject background;


	public void SetTutorial(int index)
	{
		foreach (var go in tutorialGameobjects)
		{
			go.SetActive(false);
		}
		if (index < 0 || index >= tutorialGameobjects.Length)
		{
			background.SetActive(false);
			return;
		}
		
		tutorialGameobjects[index].SetActive(true);
		background.SetActive(true);
	}


}
