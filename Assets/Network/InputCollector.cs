using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCollector : MonoBehaviour, INetworkInput
{
    // singleton instance
    public static InputCollector Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public List<PlayerController> localControllers = new List<PlayerController>();


    public void AddPlayerController(PlayerController controller)
    {
        localControllers.Add(controller);
    }

    public void OnInput(NetworkInput input)
    {
        var data = new NetworkInputData();
        int playerCount = localControllers.Count;
        if (playerCount > 0)
        {
            data.controllerData1 = GetControllerData(0);
        }
        if (playerCount > 1)
        {
            data.controllerData2 = GetControllerData(1);
        }
        if (playerCount > 2)
        {
            data.controllerData3 = GetControllerData(2);
        }
        if (playerCount > 3)
        {
            data.controllerData4 = GetControllerData(3);
        }
        if (playerCount > 4)
        {
            data.controllerData5 = GetControllerData(4);
        }
        if (playerCount > 5)
        {
            data.controllerData6 = GetControllerData(5);
        }
        if (playerCount > 6)
        {
            data.controllerData7 = GetControllerData(6);
        }
        if (playerCount > 7)
        {
            data.controllerData8 = GetControllerData(7);
        }
        input.Set(data);
    }


    public LocalControllerData GetControllerData(int index)
    {
        if (index < 0 || index >= localControllers.Count)
        {
            throw new System.IndexOutOfRangeException("Invalid controller index");
        }
        var controller = localControllers[index];

        int buttonCount = controller.ButtonData.Count;
        NetworkButtons buttonsCollected = new NetworkButtons();
        
        foreach (InputButton button in Enum.GetValues(typeof(InputButton)))
        {
            buttonsCollected.Set(button, controller.ButtonData.TryGetValue(button, out var value) && value);
        }
        return new LocalControllerData
        {
            moveVector = controller.MoveVector,
            aimVector = controller.AimVector,
            buttons = buttonsCollected

        };
    }

}



