using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System;
using Unity.Mathematics;
using System.Collections;
using Unity.VisualScripting;
public class PlayerManager : MonoBehaviour
{
    public Action<PlayerMind> OnPlayerAdded;
    public Action<PlayerMind> OnPlayerSpawned;

    // singelton
    public static PlayerManager instance;

    [SerializeField] List<PlayerMind> players = new List<PlayerMind>();
    [SerializeField] int startingLayer = 20;
    [SerializeField] int deadPlayerLayer = 31;

    [SerializeField] BodyMindConnection playerBodyPrefab;

    [SerializeField] Color[] playerColors;
    //List<int> playerLayers = new List<int>();

    [SerializeField] CinemachineCamera[] playerCameras;
    [SerializeField] CinemachineCamera[] spectatorCameras;
    int screenCount = 1;
    [SerializeField] ScreenRectArray[] screenRectValues;
    [SerializeField] ScreenRectArray[] screenRectValues_2Screens;
    [SerializeField] ScreenRectArray[] screenRectValues_3Screens;

    public bool forceSpawnTeam1OnSpawnPoint1 = false;
	List<int> fpsLayers = new List<int>();
    List<int> thirdPersonLayers = new List<int>();

    // Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        for (int i = 0; i < 8; i++)
        {
            fpsLayers.Add(startingLayer + (i *2));
            thirdPersonLayers.Add(startingLayer + (i * 2) + 1);
        }



        if (Display.displays.Length > 1)
        {
            screenCount = 2;
            Display.displays[1].Activate();
        }

        if (false && Display.displays.Length > 2)
        {
            screenCount = 3;
            Display.displays[2].Activate();
        }

    }

    public int PlayersInTeam1()
    {
        int count = 0;
        foreach (var player in players)
        {
            if (player.TeamIndex == 0)
            {
                count++;
            }
        }
        return count;
    }

    public int PlayersInTeam2()
    {
        int count = 0;
        foreach (var player in players)
        {
            if (player.TeamIndex == 1)
            {
                count++;
            }
        }
        return count;
    }

    public bool HasTeam2Players()
    {
        foreach (var player in players)
        {
            if (player.TeamIndex == 1)
            {
                return true;
            }
        }
        return false;
    }

    public List<PlayerMind> GetAllPlayers()
    {
        return players;
    }


    // get dead player layer
    public int GetDeadPlayerLayer()
    {
        return deadPlayerLayer;
    }


    public void AddPlayer(PlayerMind player)
    {
        players.Add(player);
        ReorderPlayers();

        var playerBody = Instantiate(playerBodyPrefab, Vector3.zero, Quaternion.identity);
        //playerBody.ConnectMind(player, playerCameras[players.Count-1], spectatorCameras[players.Count-1]);
        playerBody.ConnectMind(player);
        




        for (int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            p.SetLayers(fpsLayers[i], thirdPersonLayers[i]);
            for (int j = 0; j < fpsLayers.Count; j++)
            {
                if (j != i)
                {
                    p.DisableLayerInCamera(fpsLayers[j]);
                    p.EnableLayerInCamera(thirdPersonLayers[j]);
                }

            }


            p.EnableLayerInCamera(fpsLayers[i]);
            p.DisableLayerInCamera(thirdPersonLayers[i]);
            p.PlayerBody.GetComponent<BodyMindConnection>().SetCameras(playerCameras[i], spectatorCameras[i]);


        }
        player.EnableLayerInCamera(deadPlayerLayer);


        //player.SetLayers(currentLayer, currentLayer + 1);
        //player.EnableLayerInCamera(currentLayer);


        /*
        foreach (var layer in playerLayers)
        {
            player.EnableLayerInCamera(layer);
        }

        currentLayer += 1;
        foreach (var playerInList in players)
        {
            playerInList.EnableLayerInCamera(currentLayer);
        }
        players.Add(player);
        playerLayers.Add(currentLayer);
        currentLayer += 1;*/





        var playerCount = players.Count;
        for (int i = 0; i < playerCount; i++)
        {
            var screenRectsToUse = screenRectValues;
            if (screenCount == 2)
            {
                screenRectsToUse = screenRectValues_2Screens;
            }
            else if (screenCount == 3)
            {
                screenRectsToUse = screenRectValues_3Screens;
            }


            var playerCam = players[i];
            playerCam.SetScreenRect(screenRectsToUse[playerCount - 1].screenRectValues[i], i);
            var vCam = playerCameras[i];
            var spectatorCam = spectatorCameras[i];
            var fov = screenRectsToUse[playerCount - 1].screenRectValues[i].FOV;
            vCam.Lens.FieldOfView = fov;
            spectatorCam.Lens.FieldOfView = fov;
            playerCam.SetCinemaCamera(vCam);



        }

        OnPlayerAdded?.Invoke(player);
        OnPlayerSpawned?.Invoke(player);
        var gameMode = GameModeSelector.gameModeManager;


		var spawnPoint = gameMode.GetStartingSpawnPoint(player.TeamIndex);
        playerBody.transform.position = spawnPoint.position;
        playerBody.transform.rotation = spawnPoint.rotation;
        playerBody.SetPlayTeamIndex();
        if (player.TeamIndex == 0 && gameMode.GameModeStats.recolerTeam1Members)
        {
            var playerID = player.playerID;
            var colors = gameMode.GameModeStats.team1MemberColors;


			Debug.Log("player ID:" + playerID);
			var color = gameMode.GameModeStats.team1MemberColors[math.min(playerID, colors.Length - 1)];
            playerBody.SetPlayerColor(color);

		}
        else
        {
			playerBody.SetPlayerColor(playerColors[player.TeamIndex]);
		}



        StartCoroutine(ForcePlayerToSpawn(playerBody.gameObject, spawnPoint));
	}

    IEnumerator ForcePlayerToSpawn(GameObject player, Transform spawnPoint)
    {
        yield return new WaitForNextFrameUnit();
		player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        
	}


    public void UpdateTeamOfBody(PlayerMind mind)
    {
        var playerBody = mind.PlayerBody;
        var connector = playerBody.GetComponent<BodyMindConnection>();
        connector.SetPlayTeamIndex();
        connector.SetPlayerColor(playerColors[mind.TeamIndex]);
    }

    public void UpdateTeamOfEnemyAI(BodyMindConnection body, int team)
    {
        body.SetPlayTeamIndex(team);

        
    }

    public void UpdateColorOfEnemyAI(BodyMindConnection body, int team)
    {
        body.SetPlayerColor(playerColors[team]);
    }

    public void RespawnPlayer(PlayerMind player)
    {
        var spawnPoint = GameModeSelector.gameModeManager.GetFarthestSpawnPointFromEnemeies(player);
        if (GameModeSelector.gameModeManager.GameModeStats.UseStatSheet)
        {
            spawnPoint = GameModeSelector.gameModeManager.GetStartingSpawnPoint(player.TeamIndex);
        }

        if ( forceSpawnTeam1OnSpawnPoint1 &&player.TeamIndex == 0)
        {
            spawnPoint = GameModeSelector.gameModeManager.GetStartingSpawnPoint(player.TeamIndex);
		}


        var playerBody = Instantiate(playerBodyPrefab, spawnPoint.position, spawnPoint.rotation);
        playerBody.ConnectMind(player);
        playerBody.SetCameras(GetPlayerCamera(player), GetPlayerSpectatorCamera(player));

        var gameMode = GameModeSelector.gameModeManager;
		if (player.TeamIndex == 0 && gameMode.GameModeStats.recolerTeam1Members)
		{
			var playerID = player.playerID;
			var colors = gameMode.GameModeStats.team1MemberColors;

            Debug.Log("player ID:" + playerID);
			var color = gameMode.GameModeStats.team1MemberColors[math.min(playerID, colors.Length - 1)];
			playerBody.SetPlayerColor(color);

		}
		else
		{
			playerBody.SetPlayerColor(playerColors[player.TeamIndex]);
		}

        player.UpdateLayers();
        OnPlayerSpawned?.Invoke(player);

        playerBody.transform.position = spawnPoint.position;
	}

    public CinemachineCamera GetPlayerCamera(PlayerMind player)
    {
        var index = players.IndexOf(player);
        return playerCameras[index];
    }

    // get player spectator camera
    public CinemachineCamera GetPlayerSpectatorCamera(PlayerMind player)
    {
        var index = players.IndexOf(player);
        return spectatorCameras[index];
    }

    public void ReorderPlayers()
    {
        List<PlayerMind> newPlayers = new List<PlayerMind>();
        List<int> indexList = new List<int>();

        foreach (var player in players)
        {
            newPlayers.Add(player);
            indexList.Add(player.PlayerIndex);
        }



        players.Clear();
        foreach (var player in newPlayers)
        {
            bool inserted = false;
            for (int i = 0; i < players.Count; i++)
            {
                if (player.PlayerIndex < players[i].PlayerIndex)
                {
                    players.Insert(i, player);
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                players.Add(player);
            }
        }

        Debug.Log("playerIndexOrder");
        foreach (var player in players)
        {
            Debug.Log(player.PlayerIndex);
        }




    }
}

[Serializable]
public struct ScreenRectArray
{
    public ScreenRectValues[] screenRectValues;
    public ScreenRectArray(ScreenRectValues[] screenRectValues)
    {
        this.screenRectValues = screenRectValues;


    }
}


[Serializable]
public struct ScreenRectValues
{
    public float x;
    public float y;
    public float width;
    public float height;
    public float FOV;
    public int targetDisplay;

    public ScreenRectValues(float x, float y, float width, float height, float FOV, int targetDisplay)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.FOV = FOV;
        this.targetDisplay = targetDisplay;
    }

}
