using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class ScreenManager : MonoBehaviour
{
    // singelton
    public static ScreenManager Instance { get; private set; }


    [SerializeField] CinemachineCamera[] playerCameras;
    [SerializeField] CinemachineCamera[] spectatorCameras;

    int screenCount = 1;
    [SerializeField] ScreenRectArray[] screenRectValues;
    [SerializeField] ScreenRectArray[] screenRectValues_2Screens;

    [SerializeField] int startingLayerLocalPlayer = 20;
    [SerializeField] int networkPlayerLayer = 31;

    List<int> fpsLayers = new List<int>();
    List<int> thirdPersonLayers = new List<int>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < 8; i++)
        {
            fpsLayers.Add(startingLayerLocalPlayer + (i * 2));
            thirdPersonLayers.Add(startingLayerLocalPlayer + (i * 2) + 1);
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


    List<PlayerInterface> playerInterfaces = new List<PlayerInterface>();

    public void AddInterface(PlayerInterface playerInterface)
    {
        playerInterfaces.Add(playerInterface);
        var interfaceCount = playerInterfaces.Count;

        for (int i = 0; i < interfaceCount; i++)
        {
            var pif = playerInterfaces[i];
            pif.SetInterfaceLayer(fpsLayers[i]);
            pif.SetHiddenLayer(thirdPersonLayers[i]);

            if (pif.PlayerBody != null)
                pif.PlayerBody.SetVisualLayer(thirdPersonLayers[i]);
            for (int j = 0; j < fpsLayers.Count; j++)
            {
                if (j != i)
                {
                    pif.DisableLayerInCamera(fpsLayers[j]);
                    pif.EnableLayerInCamera(thirdPersonLayers[j]);
                }

            }


            pif.EnableLayerInCamera(fpsLayers[i]);
            pif.DisableLayerInCamera(thirdPersonLayers[i]);
            if (pif.PlayerBody != null)
                pif.PlayerBody.SetCameras(playerCameras[i], spectatorCameras[i]);
            // p.PlayerBody.GetComponent<BodyMindConnection>().SetCameras(playerCameras[i], spectatorCameras[i]);


        }
        playerInterface.EnableLayerInCamera(networkPlayerLayer);


        for (int i = 0; i < interfaceCount; i++)
        {
            var screenRectsToUse = screenRectValues;
            if (screenCount == 2)
            {
                screenRectsToUse = screenRectValues_2Screens;
            }
            //else if (screenCount == 3)
            //{
            //    screenRectsToUse = screenRectValues_3Screens;
            //}


            var playerCam = playerInterfaces[i].InterfaceCamera;
            playerCam.SetScreenRect(screenRectsToUse[interfaceCount - 1].screenRectValues[i], i);
            var vCam = playerCameras[i];
            var spectatorCam = spectatorCameras[i];
            var fov = screenRectsToUse[interfaceCount - 1].screenRectValues[i].FOV;
            vCam.Lens.FieldOfView = fov;
            spectatorCam.Lens.FieldOfView = fov;
            //playerCam.SetCinemaCamera(vCam
        }
    }

}
