using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
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
        playerBody.ConnectMind(player);
        


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

    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public void RespawnPlayer(PlayerMind player)
    {
        var playerBody = Instantiate(playerBodyPrefab, GetRandomSpawnPoint().position, GetRandomSpawnPoint().rotation);
        playerBody.ConnectMind(player);
        playerBody.SetMaterial(GetPlayerColor(player));

        player.UpdateLayers();
    }

    public Material GetPlayerColor(PlayerMind player)
    {
        var index = players.IndexOf(player);
        if (index > 3) return playerColors[3];
        return playerColors[index];
    }
}
