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





