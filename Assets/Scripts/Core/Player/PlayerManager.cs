using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System;
public class PlayerManager : MonoBehaviour
{
    // singelton
    public static PlayerManager instance;

    [SerializeField] List<PlayerMind> players = new List<PlayerMind>();
    [SerializeField] int startingLayer = 20;
    [SerializeField] int deadPlayerLayer = 31;

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] BodyMindConnection playerBodyPrefab;

    [SerializeField] Material[] playerColors;
    int currentLayer;
    List<int> playerLayers = new List<int>();

    [SerializeField] CinemachineCamera[] playerCameras;
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
        // spawn player body
        var playerBody = Instantiate(playerBodyPrefab, GetRandomSpawnPoint().position, GetRandomSpawnPoint().rotation);
        playerBody.ConnectMind(player, playerCameras[players.Count]);
        


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
        playerBody.SetMaterial(GetPlayerColor(player));


        var playerCount = players.Count;
        for (int i = 0; i < playerCount; i++)
        {
            var playerCam = players[i];
            playerCam.SetScreenRect(screenRectValues[playerCount - 1].screenRectValues[i], i);
            var vCam = playerCameras[i];
            vCam.Lens.FieldOfView = screenRectValues[playerCount - 1].screenRectValues[i].FOV;
            playerCam.SetCinemaCamera(vCam);
        }

    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    public void RespawnPlayer(PlayerMind player)
    {
        var playerBody = Instantiate(playerBodyPrefab, GetRandomSpawnPoint().position, GetRandomSpawnPoint().rotation);
        playerBody.ConnectMind(player, GetPlayerCamera(player));
        playerBody.SetMaterial(GetPlayerColor(player));

        player.UpdateLayers();
    }

    public Material GetPlayerColor(PlayerMind player)
    {
        var index = players.IndexOf(player);
        if (index > 3) return playerColors[3];
        return playerColors[index];
    }

    public CinemachineCamera GetPlayerCamera(PlayerMind player)
    {
        var index = players.IndexOf(player);
        return playerCameras[index];
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
