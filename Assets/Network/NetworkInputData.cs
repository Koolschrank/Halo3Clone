using Fusion;
using UnityEngine;

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