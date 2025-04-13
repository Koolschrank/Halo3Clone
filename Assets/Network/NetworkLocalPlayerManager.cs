using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkLocalPlayerManager : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef playerObject;
    [Networked, Capacity (16)] private NetworkLinkedList<NetworkObject> _spawnedCharacters => default;



    public void OnLocalPlayerSpawn()
    {

        Debug.Log("OnLocalPlayerSpawnServer  pre");
        if (HasInputAuthority)
        {
            Debug.Log("OnLocalPlayerSpawnServer");
            RPC_LocalPlayerSpawn(Runner.LocalPlayer);
        }
    }

    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_LocalPlayerSpawn(PlayerRef player)
    {
        Debug.Log("OnLocalPlayerSpawnServer post");
        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.RawEncoded % Runner.Config.Simulation.PlayerCount) * 1.5f, 3, 0);
        NetworkObject networkPlayerObject = Runner.Spawn(playerObject, spawnPosition, Quaternion.identity, player);
        // Keep track of the player avatars for easy access
        _spawnedCharacters.Add(networkPlayerObject);

        if (networkPlayerObject.TryGetComponent<PlayerMind>(out PlayerMind mind))
        {
            mind.SetControllerIndex(_spawnedCharacters.Count -1);
        }
    }

}
