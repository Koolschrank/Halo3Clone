using UnityEngine;
using UnityEngine.SceneManagement;

public class GravityOverrider : MonoBehaviour
{
	// singelton instance
	public static GravityOverrider Instance { get; private set; }

	[SerializeField] Vector3 customGravity = new Vector3(0f, 0f, 0f); // Default gravity
	public float playerGravityMultiplier = 0.5f;
	Vector3 baseGravity;

	void OnEnable()
	{
		// Ensure singleton instance
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		baseGravity = Physics.gravity;
		
		SceneManager.sceneLoaded += ResetGravityOnSceneLoad;
	}

	private void Start()
	{
		Physics.gravity = customGravity; // Set custom gravity
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= ResetGravityOnSceneLoad;
	}

	void ResetGravityOnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		Physics.gravity = baseGravity; // Default gravity
	}
}
