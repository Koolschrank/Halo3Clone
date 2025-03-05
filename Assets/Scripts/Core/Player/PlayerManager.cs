using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System;
public class PlayerManager : MonoBehaviour
{
    public Action<PlayerMind> OnPlayerAdded;

    // singelton
    public static PlayerManager instance;

    [SerializeField] List<PlayerMind> players = new List<PlayerMind>();
    [SerializeField] int startingLayer = 20;
    [SerializeField] int deadPlayerLayer = 31;

    [SerializeField] BodyMindConnection playerBodyPrefab;

    [SerializeField] Material[] playerColors;
    int currentLayer;
    List<int> playerLayers = new List<int>();

    [SerializeField] CinemachineCamera[] playerCameras;
    [SerializeField] CinemachineCamera[] spectatorCameras;
    [SerializeField] ScreenRectArray[] screenRectValues;


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
        currentLayer = startingLayer;
    }



    // get dead player layer
    public int GetDeadPlayerLayer()
    {
        return deadPlayerLayer;
    }


    public void AddPlayer(PlayerMind player)
    {
        
        
        var playerBody = Instantiate(playerBodyPrefab, Vector3.zero, Quaternion.identity);
        playerBody.ConnectMind(player, playerCameras[players.Count], spectatorCameras[players.Count]);
        


        player.SetLayers(currentLayer, currentLayer +1);
        player.EnableLayerInCamera(currentLayer);


        player.EnableLayerInCamera(deadPlayerLayer);
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
        currentLayer += 1;


        var playerCount = players.Count;
        for (int i = 0; i < playerCount; i++)
        {
            var playerCam = players[i];
            playerCam.SetScreenRect(screenRectValues[playerCount - 1].screenRectValues[i], i);
            var vCam = playerCameras[i];
            var spectatorCam = spectatorCameras[i];
            var fov = screenRectValues[playerCount - 1].screenRectValues[i].FOV;
            vCam.Lens.FieldOfView = fov;
            spectatorCam.Lens.FieldOfView = fov;
            playerCam.SetCinemaCamera(vCam);

            
            
        }

        OnPlayerAdded?.Invoke(player);


        var spawnPoint = GameModeSelector.gameModeManager.GetStartingSpawnPoint(player.TeamIndex);
        playerBody.transform.position = spawnPoint.position;
        playerBody.transform.rotation = spawnPoint.rotation;
        playerBody.SetPlayTeamIndex();
        playerBody.SetMaterial(playerColors[player.TeamIndex]);
    }

    public void RespawnPlayer(PlayerMind player)
    {
        var spawnPoint = GameModeSelector.gameModeManager.GetFarthestSpawnPointFromEnemeies(player);
        var playerBody = Instantiate(playerBodyPrefab, spawnPoint.position, spawnPoint.rotation);
        playerBody.ConnectMind(player, GetPlayerCamera(player), GetPlayerSpectatorCamera(player));
        playerBody.SetMaterial(playerColors[player.TeamIndex]);

        player.UpdateLayers();
    }

    public Material GetPlayerColor(PlayerMind player)
    {
        var index = players.IndexOf(player);
        if (index > playerColors.Length) return playerColors[playerColors.Length -1];
        return playerColors[index];
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

    public ScreenRectValues(float x, float y, float width, float height, float FOV)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.FOV = FOV;
    }

}
