using Fusion;
using UnityEngine;




//public struct NetworkInputData : INetworkInput
//{
//    public LocalControllerData[] controllerDatas;
//}

public struct NetworkControllerData : INetworkInput
{
    public Vector2 moveVector;
    public Vector2 aimVector;
    public NetworkButtons buttons;


}





