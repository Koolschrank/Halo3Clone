using Fusion;
using Unity.VisualScripting;
using UnityEngine;


public enum InputButton
{
    Jump,
    Weapon1,
    Weapon2,
    Interact1,
    Interact2,
    SwitchWeapon,
    Ability1,
    Ability2,
    Melee
}

public struct NetworkInputData : INetworkInput
{
    
    public LocalControllerData controllerData1;
    public LocalControllerData controllerData2;
    public LocalControllerData controllerData3;
    public LocalControllerData controllerData4;
    public LocalControllerData controllerData5;
    public LocalControllerData controllerData6;
    public LocalControllerData controllerData7;
    public LocalControllerData controllerData8;

    readonly LocalControllerData this[int index] => index switch
    {
        0 => controllerData1,
        1 => controllerData2,
        2 => controllerData3,
        3 => controllerData4,
        4 => controllerData5,
        5 => controllerData6,
        6 => controllerData7,
        7 => controllerData8,
        _ => default
    };
}

public struct LocalControllerData : INetworkInput
{
    public Vector2 moveVector;
    public Vector2 aimVector;
    public NetworkButtons buttons;

}

public static class InputSplitter
{
    public static LocalControllerData GetContollerData(this NetworkInputData inputData, int controllerIndex)
    {
        return controllerIndex switch
        {
            0 => inputData.controllerData1,
            1 => inputData.controllerData2,
            2 => inputData.controllerData3,
            3 => inputData.controllerData4,
            4 => inputData.controllerData5,
            5 => inputData.controllerData6,
            6 => inputData.controllerData7,
            7 => inputData.controllerData8,
            _ => default,
        };
    }
}