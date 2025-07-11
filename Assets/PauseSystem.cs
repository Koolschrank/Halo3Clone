using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    // singelton
    public static PauseSystem instance;

	private void Awake()
	{
		instance = this;
	}

	bool paused = false;
	public void TogglePause()
    {
		paused = !paused;

		if (paused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}

    }
}
