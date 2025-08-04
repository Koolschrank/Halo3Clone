using UnityEngine;
using UnityEngine.UI;

public class UI_RespawnTimer : MonoBehaviour
{
    public Slider respawnSlider;
    public PlayerMind playerMind;

	private void Start()
	{
		playerMind.OnRespawnUpdate += UpdateRespawnSlider;
	}

	private void UpdateRespawnSlider(float timeLeft)
	{
		if (respawnSlider != null)
		{
			respawnSlider.value = timeLeft;
		}
	}



}
