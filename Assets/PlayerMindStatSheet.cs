using System;
using UnityEngine;

public class PlayerMindStatSheet : MonoBehaviour
{
    [SerializeField] PlayerStatsSheet playerStatsSheetBlueprint;
    public bool usePlayerStatsSheet = true;

    [NonSerialized]
    public PlayerStatsSheet playerStatSheetInstance;

    private void Awake()
    {
        if (playerStatsSheetBlueprint != null)
            playerStatSheetInstance = ScriptableObject.Instantiate(playerStatsSheetBlueprint);
    }

    public void SetStatSheet(PlayerStatsSheet sheet)
    {
        usePlayerStatsSheet = true;
        playerStatSheetInstance = ScriptableObject.Instantiate(sheet);
    }




}


