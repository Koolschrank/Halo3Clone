using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSave : MonoBehaviour
{
    // make instance of this class
    public static SettingsSave instance;

    List<PlayerSettings> playerSettings = new List<PlayerSettings>();

    public bool CheckIfNameTaken(string name)
    {
        for (int i = 0; i < playerSettings.Count; i++)
        {
            if (playerSettings[i].playerName == name)
            {
                return true;
            }
        }
        return false;
	}

	public void Awake()
    {
        // destroy if there is already an instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        // dont destroy on load
        DontDestroyOnLoad(this.gameObject);
    }

    public PlayerSettings GetPlayerSettings(string deviceName)
    {
        for (int i = 0; i < playerSettings.Count; i++)
        {
            if (playerSettings[i].deviceName == deviceName)
            {
                return playerSettings[i];
            }
        }

        // create PlayerSettings
        var newSettings = new PlayerSettings(deviceName, playerSettings.Count);
        playerSettings.Add(newSettings);
        return newSettings;
    }
}


public class PlayerSettings
{
    public string deviceName;


    public string playerName = "Player";
    public int playerIndex = 0;
    public float sensitivity = 1;
    

    public PlayerSettings()
    {
        sensitivity = 1;
        SetRandomNameAdvanced();
    }

    public PlayerSettings(string deviceName, int playerIndex)
    {
        this.deviceName = deviceName;
        sensitivity = 1;
        this.playerIndex = playerIndex;

        SetRandomNameAdvanced();
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }

    public void SetRandomName()
    {


        var settingsInstance = SettingsSave.instance;
        var currentName = playerName;
		while (settingsInstance.CheckIfNameTaken(currentName))
        {
			currentName = NameList.instance.GetRandomNameBasic();
		}
        playerName = currentName;


	}

    public void SetRandomNameAdvanced()
    {
        var settingsInstance = SettingsSave.instance;
        var currentName = playerName;
        while (settingsInstance.CheckIfNameTaken(currentName))
        {
			currentName = NameList.instance.GetRandomNameAdvanced();
        }
        playerName = currentName;
	}

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}
