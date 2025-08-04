using UnityEngine;
using UnityEngine.UI;

public class UI_ReviveBar : MonoBehaviour
{
    [SerializeField] GameObject visual;
    [SerializeField] Slider slider;

    PlayerReviver playerReviver;

	public void SetUp(PlayerReviver playerRevive)
    {
        Debug.Log("UI_ReviveBar: SetUp");
		if (this.playerReviver != null)
        {
			playerReviver.OnHasReviveBody -= Show;
			playerReviver.OnNoReviveBody -= Hide;
			playerReviver.OnReviveProgress -= UpdateBar;
		}
        playerReviver = playerRevive;


		slider.value = 0;
        visual.SetActive(false);
        playerRevive.OnHasReviveBody += Show;
        playerRevive.OnNoReviveBody += Hide;
        playerRevive.OnReviveProgress += UpdateBar;
	}

    private void Show()
    {
        visual.SetActive(true);
    }
    private void Hide()
    {
        visual.SetActive(false);
    }
    private void UpdateBar(float progress)
    {
        slider.value = progress;
	}
}
