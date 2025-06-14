using System;
using UnityEngine;

public class PlayerBodyStatSheet : MonoBehaviour
{
    public Action OnStatSheetUpdated;
    public Action OnStartEquipmentEquip;


    public PlayerStatsSheet playerStatsSheetBlueprint;
    public bool useStatSheet = true;

    [HideInInspector]
    public PlayerStatsSheet playerStatsSheetInstance;


    private void Start()
    {
        if (useStatSheet && playerStatsSheetBlueprint != null)
        {
            SetStatSheet(playerStatsSheetBlueprint);
        }
    }


    public void SetStatSheet(PlayerStatsSheet newStatSheet)
    {
        useStatSheet = true;
        playerStatsSheetInstance = ScriptableObject.Instantiate(newStatSheet); 
        OnStatSheetUpdated?.Invoke();
        OnStartEquipmentEquip?.Invoke();
        
    }

    public void ApplyStatUpgrade(StatUpgrader stat)
    {
        if (playerStatsSheetInstance == null)
        {
            Debug.LogError("PlayerStatsSheet instance is null. Cannot apply stat upgrader.");
            return;
        }

        playerStatsSheetInstance.ApplyModifiers(stat);
        OnStatSheetUpdated?.Invoke();


        GetComponent<BodyMindConnection>().ApplyUpgradeToMind(stat);
    }


    
}
