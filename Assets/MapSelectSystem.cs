using UnityEngine;

public class MapSelectSystem : MenuSystem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

	public override void SelectButton()
	{
		if (menuButtons.Count == 0) return;
		MapLoader.instance.SetSceneToLoad(menuButtons[selectedButtonIndex].mapName);

		base.SelectButton();
	}
}



