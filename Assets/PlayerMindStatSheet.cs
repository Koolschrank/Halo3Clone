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
        playerStatSheetInstance = ScriptableObject.Instantiate(playerStatsSheetBlueprint);
    }




}


