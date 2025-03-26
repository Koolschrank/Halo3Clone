using UnityEngine;

public class CaptureTheFlagManager : MonoBehaviour
{
    [SerializeField] GameObject flagPrefab;

    [SerializeField] Transform team1_Base;
    [SerializeField] Transform team1_FalgSpawnPoint;
    [SerializeField] Transform team2_Base;
    [SerializeField] Transform team2_FalgSpawnPoint;

    [SerializeField] float movementSpeedDebuffWithFlag = 0.4f;


}


