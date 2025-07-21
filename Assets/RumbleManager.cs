using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RumbleManager : MonoBehaviour
{
	// singleton instance
    public static RumbleManager Instance { get; private set; }

	[SerializeField] PlayerInputManager playerInput;
	[SerializeField] RumbleData rumbleSettings_test;
    [SerializeField] bool testRumble = false;
    [SerializeField] float testRumbleLoopDuration = 2f;

    private float testRumbleTimer = 0f;

    List<PlayerInput> players = new List<PlayerInput>();
    List<PlayerRumbleStack> playerRumbleStacks = new List<PlayerRumbleStack>();


    [Header("Settings")]
    [SerializeField] RumbleTypeSettings rumbleTypeSettings_shoot;
    [SerializeField] RumbleTypeSettings rumbleTypeSettings_melee;
    [SerializeField] RumbleTypeSettings rumbleTypeSettings_damage;
    [SerializeField] RumbleTypeSettings rumbleTypeSettings_explosion;
    [SerializeField] RumbleTypeSettings rumbleTypeSettings_reload;
	[SerializeField] RumbleTypeSettings rumbleTypeSettings_other;

	private void Awake()
	{
        Instance = this;
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		//playerInput.onPlayerJoined += OnPlayerJoined;

	}

    public int OnPlayerJoined(PlayerInput player)
    {
        players.Add(player);
        var playerRumbleStack = new PlayerRumbleStack();
        playerRumbleStack.gamepad = player.devices[0] as Gamepad; // Get the first device of the player as a Gamepad
        playerRumbleStacks.Add(playerRumbleStack);

		Debug.Log($"Player joined: {player.playerIndex}");

        return players.Count - 1; // Return the index of the newly added player
	}

    

	// Update is called once per frame
	void Update()
    {
        if (testRumble)
        {
            testRumbleTimer += Time.deltaTime;
            if (testRumbleTimer >= testRumbleLoopDuration)
            {
                testRumbleTimer = 0f;
                TriggerRumble(rumbleSettings_test,0);
            }
        }

        foreach (var playerRumbleStack in playerRumbleStacks)
        {
            playerRumbleStack.UpdateRumble();
		}

	}

    public void TriggerRumble(RumbleData rumbleData, int playerIndex)
    {
        //Gamepad gamepad = Gamepad.current; // Get the current gamepad
        if (players.Count <= playerIndex) return;
        var player = players[playerIndex]; // Assuming you want to rumble the first player
        var rumbleStack = playerRumbleStacks[playerIndex];

        var settings = new RumbleTypeSettings();
        switch (rumbleData.rumbleType)
        {
            case RumbleType.shoot:
                settings = rumbleTypeSettings_shoot;
                break;
            case RumbleType.melee:
                settings = rumbleTypeSettings_melee;
                break;
            case RumbleType.damage:
                settings = rumbleTypeSettings_damage;
                break;
            case RumbleType.explosion:
                settings = rumbleTypeSettings_explosion;
                break;
            case RumbleType.reload:
                settings = rumbleTypeSettings_reload;
                break;
			case RumbleType.other:
                settings = rumbleTypeSettings_other;
                break;
		}

        rumbleStack.AddRumbleSettings(rumbleData, settings);


		//if (gamepad != null)
		//      {
		//          StartCoroutine(RumbleCoroutine(gamepad, rumbleSettings));
		//      }
		//      else
		//      {
		//          Debug.LogWarning("No gamepad connected to trigger rumble.");
		//      }
	}

    //IEnumerator RumbleCoroutine(Gamepad gamepad, RumbleSettings rumbleSettings)
    //{
    //    if (gamepad != null)
    //    {
    //        gamepad.SetMotorSpeeds(rumbleSettings.lowMotorIntensity, rumbleSettings.highMotorIntensity);
    //        yield return new WaitForSeconds(rumbleSettings.duration);
    //        gamepad.SetMotorSpeeds(0f, 0f); // Stop rumble after duration
    //    }
    //}
}


	[Serializable]
public struct RumbleData
{
    public float duration;
    
	public float intensity;
	public AnimationCurve intensityCurve; // Optional: Use an AnimationCurve for more complex rumble patterns
	public RumbleType rumbleType;

    public RumbleData(float duration, float intensity, AnimationCurve intensityCurve, RumbleType rumbleType)
    {
        this.duration = duration;
        this.intensity = intensity;
        this.intensityCurve = intensityCurve;
        this.rumbleType = rumbleType;
	}
}


[Serializable]
public class RumbleTypeSettings 
{
    [UnityEngine.Range(0,2)]
    public float lowMotorIntensity = 0.5f; // Low motor intensity
    [UnityEngine.Range(0,2)]
    public float highMotorIntensity = 0.5f; // High motor intensity
}


public enum RumbleType
{
    shoot,
    melee,
    damage,
    explosion,
    reload,
    other = 20,
}

public struct RumbleInstance
{
    public int id;
    public float duration;
    public float lowMotorIntensity;
    public float highMotorIntensity;
    public float startTime;

    public AnimationCurve powerCurve;

	public RumbleInstance(int id, float duration, float lowMotorIntensity, float highMotorIntensity, AnimationCurve powerCurve, float startTime )
	{
        this.id = id;
        this.duration = duration;
        this.lowMotorIntensity = lowMotorIntensity;
        this.highMotorIntensity = highMotorIntensity;
        this.startTime = startTime;
        this.powerCurve = powerCurve;
	}

}


public class PlayerRumbleStack
{
    public Gamepad gamepad;
    public List<RumbleInstance> rumbleSettingsStack = new List<RumbleInstance>();
    int index = 0;


	public RumbleInstance AddRumbleSettings(RumbleData rumbleData, RumbleTypeSettings settings)
    {
        var rumbleInstance = new RumbleInstance(
            index,
            rumbleData.duration, 
            Math.Clamp( rumbleData.intensity * settings.lowMotorIntensity,0,1),
			Math.Clamp(rumbleData.intensity * settings.highMotorIntensity, 0, 1),
            rumbleData.intensityCurve,
			Time.timeSinceLevelLoad

		);
        index++;
		rumbleSettingsStack.Add(rumbleInstance);
		UpdateRumble();

        return rumbleInstance;

	}

    public void RemoveRumbleSettings(int id)
    {
        rumbleSettingsStack.RemoveAll(r => r.id == id);
		UpdateRumble();
	}

    public void UpdateRumble()
    {
        if (gamepad == null || rumbleSettingsStack.Count == 0)
        {
            return;
		}

		var highestLowMotorIntensity = 0f;
        var highestHighMotorIntensity = 0f;

        List<RumbleInstance> completedRumbles = new List<RumbleInstance>();

		foreach (var rumble in rumbleSettingsStack)
        {
            var elapsedTime = Time.timeSinceLevelLoad - rumble.startTime;
            var percentComplete = elapsedTime / rumble.duration;
            var power = rumble.powerCurve.Evaluate(percentComplete);
			var completed = percentComplete >= 1f;

            if (completed)
            {
				completedRumbles.Add(rumble);
			}
            else
            {
				if (rumble.lowMotorIntensity > highestLowMotorIntensity * power)
				{
					highestLowMotorIntensity = rumble.lowMotorIntensity * power;
				}
				if (rumble.highMotorIntensity > highestHighMotorIntensity * power)
				{
					highestHighMotorIntensity = rumble.highMotorIntensity * power;
				}
			}
		}

        foreach (var rumble in completedRumbles)
        {
            RemoveRumbleSettings(rumble.id);
        }

		// clamp the intensities to a maximum of 1
        highestLowMotorIntensity = Mathf.Clamp(highestLowMotorIntensity, 0f, 1f);
        highestHighMotorIntensity = Mathf.Clamp(highestHighMotorIntensity, 0f, 1f);

		gamepad.SetMotorSpeeds(highestLowMotorIntensity, highestHighMotorIntensity);
	}


	//public IEnumerator RumbleTimer( RumbleInstance rumbleSettings)
	//{
	//	yield return new WaitForSeconds(rumbleSettings.duration);
	//	RemoveRumbleSettings(rumbleSettings.id);
		
	//}

    
}