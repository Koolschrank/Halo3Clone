using System;
using UnityEngine;

public class PlayerBodyStatSheet : MonoBehaviour
{
    public Action OnStatSheetUpdated;


    public PlayerStatsSheet playerStatsSheetBlueprint;
    public bool useStatSheet = true;

    [HideInInspector]
    public PlayerStatsSheet playerStatsSheetInstance;


    private void Start()
    {
        if (playerStatsSheetBlueprint != null)
        {
            playerStatsSheetInstance = new PlayerStatsSheet(playerStatsSheetBlueprint);
            OnStatSheetUpdated?.Invoke();
        }
    }


    public void SetStatSheet(PlayerStatsSheet newStatSheet)
    {
        playerStatsSheetInstance = newStatSheet;
        OnStatSheetUpdated?.Invoke();
    }

    public void ApplyStatUpgrade(StatUpgrader stat)
    {
        if (playerStatsSheetInstance == null)
        {
            Debug.LogError("PlayerStatsSheet instance is null. Cannot apply stat upgrader.");
            return;
        }
        stat.ApplyModifiers(playerStatsSheetInstance);
        OnStatSheetUpdated?.Invoke();
    }


    
}
