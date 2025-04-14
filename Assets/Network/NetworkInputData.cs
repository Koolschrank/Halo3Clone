using Fusion;
using UnityEngine;


public enum InputButton
{
    Jump,
    UseWeapon1,
    UseWeapon2,
    ReloadWeapon1,
    ReloadWeapon2,
    SwitchWeapon,
    UseWeaponAbility,
    UseAbility1,
    UseAbility2,
    pickUpWeapon1,
    pickUpWeapon2,
    dropWeapon1,
    dropWeapon2,
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
}

public struct LocalControllerData : INetworkInput
{
    public Vector2 moveVector;
    public Vector2 aimVector;
    public NetworkButtons buttons;

}

public static class InputSplitter
{
    public static LocalControllerData GetContollerData(NetworkInputData inputData, int controllerIndex)
    {
        switch (controllerIndex)
        {
            case 0: return inputData.controllerData1;
            case 1: return inputData.controllerData2;
            case 2: return inputData.controllerData3;
            case 3: return inputData.controllerData4;
            case 4: return inputData.controllerData5;
            case 5: return inputData.controllerData6;
            case 6: return inputData.controllerData7;
            case 7: return inputData.controllerData8;
        }
        return default(LocalControllerData);
    }
}