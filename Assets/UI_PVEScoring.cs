using TMPro;
using UnityEngine;

public class UI_PVEScoring : MonoBehaviour
{


	public TextMeshProUGUI nameText;
	public TextMeshProUGUI percentageText;

	public Color baseColor = Color.white;
	public Color dangerColor = Color.red;

	public GameObject lossPointsUI;

	public GameObject criticalObject;




	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        var gamemodeStats = GameModeSelector.gameModeManager.GameModeStats;
        if (!gamemodeStats.usePVEScoring)
        {
            gameObject.SetActive(false);
            return;
		}

		var pveScoring = PVEPointsManagment.instance;
		pveScoring.OnLifePointsChanged += UpdatePercentage;

		pveScoring.OnEnterCriticalMode += EnterDangerState;
		pveScoring.OnExitCriticalMode += ExitDangerState;

		pveScoring.OnLifePointsLossStart += ShowLossPointsUI;
		pveScoring.OnLifePointsLossEnd += HideLossPointsUI;

		HideLossPointsUI();
		ExitDangerState();
	}

	private void ShowLossPointsUI()
	{
		lossPointsUI.SetActive(false);
		percentageText.color = dangerColor;
		nameText.color = dangerColor;
	}

	private void HideLossPointsUI()
	{
		lossPointsUI.SetActive(false);
		percentageText.color = baseColor;
		nameText.color = baseColor;
	}

	public void UpdatePercentage(float value)
	{
		// Clamp to ensure value stays between 0 and 100
		value = Mathf.Clamp(value, 0f, 100f);

		// Format with one decimal place

		if (value == 100f)
		{
			percentageText.text = "100%";
		}
		else
		{
			if (inDangerState)
				percentageText.text = $"{value:F1}%";
			else
				percentageText.text = $"{value:F0}%";
		}
			
	}

	bool inDangerState = false;
	public void EnterDangerState()
		{
		//percentageText.color = dangerColor;
		criticalObject.SetActive(true);
		inDangerState = true;
	}

	public void ExitDangerState()
		{
		//percentageText.color = baseColor;
		criticalObject.SetActive(false);
		inDangerState = false;
	}



}
