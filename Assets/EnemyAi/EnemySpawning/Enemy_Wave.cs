using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "ScriptableObjects/EnemyWave", order = 1)]
public class Enemy_Wave : ScriptableObject
{
    public Enemy_Stats[] enemyStats;
    public float spawnInterval = 5f;
    public float duration = 10f;
    public float waveEndBrakeTime = 5f;


    public Enemy_Stats GetRandomEnemy()
    {
        if (enemyStats.Length == 0)
        {
            Debug.LogError("No enemy stats available.");
            return null;
        }
        int randomIndex = Random.Range(0, enemyStats.Length);
        return enemyStats[randomIndex];
    }
}
