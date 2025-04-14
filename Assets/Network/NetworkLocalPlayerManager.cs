using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkLocalPlayerManager : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef playerMindPrefab;
    [SerializeField] NetworkPrefabRef playerBodyPrefab;
    [SerializeField] GameObject playerInterfacePrefab;
    [Networked, Capacity (16)] private NetworkLinkedList<NetworkObject> _spawnedCharacters => default;


    private List<PlayerInterface> playerInterfaces = new List<PlayerInterface>();


    public void OnLocalPlayerSpawn()
    {
        if (HasInputAuthority)
        {
            RPC_LocalPlayerSpawn(Runner.LocalPlayer);
        }
    }

    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_LocalPlayerSpawn(PlayerRef player)
    {

        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.RawEncoded % Runner.Config.Simulation.PlayerCount) * 1.5f, 3, 0);
        NetworkObject networkPlayerObject = Runner.Spawn(playerMindPrefab, spawnPosition, Quaternion.identity, player);
        // Keep track of the player avatars for easy access
        _spawnedCharacters.Add(networkPlayerObject);
        networkPlayerObject.transform.SetParent(transform);

        if (networkPlayerObject.TryGetComponent<PlayerMind>(out PlayerMind mind))
        {
            mind.SetControllerIndex(_spawnedCharacters.Count -1);
            mind.SetPlayerManager(this);
            SpawnPlayerBody(mind, player);

            mind.TryGetInterface();
        }

        
    }



    public void SpawnPlayerBody(PlayerMind mind, PlayerRef player)
    {
        if (HasStateAuthority)
        {
            var playerBody = Runner.Spawn(playerBodyPrefab, Vector3.zero, Quaternion.identity, player);
            PlayerBody body = playerBody.GetComponent<PlayerBody>();
            mind.ConnectToBody(body);


        }
    }

    public void CreatePlayerInterface(PlayerMind mind)
    {

        Debug.Log("CreatePlayerInterface");
        if (!HasInputAuthority) return;
        Debug.Log("CreatePlayerInterface 2");

        var playerInterfaceObject = Instantiate(playerInterfacePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        PlayerInterface playerInterface = playerInterfaceObject.GetComponent<PlayerInterface>();
        mind.SetPlayerInterface(playerInterface);
        playerInterface.transform.SetParent(mind.Body.HeadTransform);
        playerInterface.transform.localPosition = Vector3.zero;
        playerInterface.transform.localRotation = Quaternion.identity;
        playerInterfaces.Add(playerInterface);
        



    }

}
