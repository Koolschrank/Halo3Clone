using System;
using UnityEngine;

public static class PickUpManager
{
    public static Action<Weapon_PickUp> OnPickUpSpawned;

    public static void OnPickUpSpawnedInvoke(Weapon_PickUp pickUp)
    {
        OnPickUpSpawned?.Invoke(pickUp);
    }
}
