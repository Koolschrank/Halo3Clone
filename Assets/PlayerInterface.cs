using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    PlayerBody playerBody;
    int interfaceLayer = 0;
    int hidenPlayerLayer = 0;

    public PlayerCamera InterfaceCamera => playerCamera;
    

    public void Start()
    {
        ScreenManager.Instance.AddInterface(this);
    }

    public PlayerBody PlayerBody
    {
        get { return playerBody; }
        set { playerBody = value; }
    }

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
