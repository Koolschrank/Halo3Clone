using UnityEngine;

public class TimeSpeedController : MonoBehaviour
{
	[Range(0f, 1f)]
	public float timeSpeed = 1f;

	public void ApplyTimeSpeed()
	{
		Time.timeScale = timeSpeed;
		Debug.Log($"Time scale set to {timeSpeed}");
	}
}
