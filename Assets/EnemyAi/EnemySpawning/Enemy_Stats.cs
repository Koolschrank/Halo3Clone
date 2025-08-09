using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class Enemy_Stats : ScriptableObject
{
    public Equipment equipment;

    public float healthMultiplier = 1;
    public float shildMultiplier = 1;
    public float speedMultiplier = 1;
    public int teamIdOverrride = 5;
    public int scoreForKill = 10;
    public bool useSpecialEquipment = false;
}
