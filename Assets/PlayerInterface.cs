using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInterface : MonoBehaviour
{
    public Action<PlayerBody> OnPlayerBodySet;


    [SerializeField] PlayerCamera playerCamera;
    PlayerBody playerBody;
    int interfaceLayer = 0;
    int hidenPlayerLayer = 0;

    public int InterfaceLayer => interfaceLayer;
    public int HidenPlayerLayer => hidenPlayerLayer;


    public PlayerCamera InterfaceCamera => playerCamera;
   

    public PlayerBody PlayerBody
    {
        get { return playerBody; }
        set { 
            playerBody = value;
            OnPlayerBodySet?.Invoke(playerBody);
        }
    }

    public PlayerCamera PlayerCamera => playerCamera;

    public void EnableLayerInCamera(int layer)
    {
        playerCamera.EnableLayerInCamera(layer);
    }

    public void DisableLayerInCamera(int layer)
    {
        playerCamera.DisableLayerInCamera(layer);
    }

    public void SetInterfaceLayer(int layer)
    {
        interfaceLayer = layer;
        gameObject.layer = layer;
    }

    public void SetHiddenLayer(int layer)
    {
        hidenPlayerLayer = layer;
        
    }


}
