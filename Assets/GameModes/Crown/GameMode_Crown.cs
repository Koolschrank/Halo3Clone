using UnityEngine;
using MultiplayerGameModes;

[CreateAssetMenu(menuName = "GameModes/Crown")]
public class GameMode_Crown : GameMode
{
    [SerializeField] float timeToScore = 1f;
    [SerializeField] bool setEquipmentOnCrownPickup = true;
    [SerializeField] Equipment equipmentOnCrownPickup;

    public float TimeToScore { get { return timeToScore; } }

    public bool SetEquipmentOnCrownPickup { get { return setEquipmentOnCrownPickup; } }

    public Equipment EquipmentOnCrownPickup { get { return equipmentOnCrownPickup; } }
}
