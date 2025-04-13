using Fusion;
using UnityEngine;


public class NetworkPlayer: NetworkBehaviour
{
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkControllerData data))
        {
            data.moveVector.Normalize();
            _cc.Move(5 * data.moveVector * Runner.DeltaTime);
        }
    }
}