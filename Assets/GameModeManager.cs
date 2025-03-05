using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    protected GameMode gameModeStats;

    public Action OnTeamAdded;
    public Action<int, int> OnPointsUpdated;
    public Action<int> OnTeamWon;

    protected List<List<PlayerMind>> teams = new List<List<PlayerMind>>();
    protected List<int> teamPoints = new List<int>();

    [SerializeField] protected SpawnSystem spawnSystem;


    public Transform GetStartingSpawnPoint(int teamIndex)
    {
        return spawnSystem.GetStartSpawnPoint(teamIndex);
    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnSystem.GetRandomSpawnPoint();
    }

    public Transform GetFarthestSpawnPointFromEnemeies(PlayerMind playerMind)
    {
        int teamIndex = playerMind.TeamIndex;
        List<Transform> enemies = new List<Transform>();
        foreach (var team in teams)
        {
            if (team.Count > 0 && team[0].TeamIndex != teamIndex)
            {
                foreach (var player in team)
                {
                    enemies.Add(player.transform);
                }
            }
        }
        if (enemies.Count == 0)
        {
            return spawnSystem.GetStartSpawnPoint(teamIndex);
        }

        return spawnSystem.GetFarthestSpawnPointFromEnemeies(enemies);
    }

    public virtual void ResetGame()
    {
        teams.Clear();
        teamPoints.Clear();
        for (int i = 0; i < gameModeStats.TeamCount; i++)
        {
            teams.Add(new List<PlayerMind>());
            teamPoints.Add(0);
        }
    }

    public void StartGame(GameMode gameModeStats)
    {


        this.gameModeStats = gameModeStats;
        gameObject.SetActive(true);
        ResetGame();
        PlayerManager.instance.OnPlayerAdded += PlayerJoined;
    }

    public virtual void PlayerJoined(PlayerMind player)
    {
        int index = GetIndexOfNextTeamToJoin();
        teams[index].Add(player);
        player.AssignTeam(index);

        if (teams[index].Count == 1)
        {
            OnTeamAdded?.Invoke();
        }

    }





    int GetIndexOfNextTeamToJoin()
    {
        // return index of team with least players
        int index = 0;
        int min = int.MaxValue;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].Count < min)
            {
                min = teams[i].Count;
                index = i;
            }
        }
        return index;

    }

    public virtual void PlayerDied(PlayerMind player)
    {

    }

    protected void GainPoints(int teamIndex, int points)
    {
        teamPoints[teamIndex] += points;
        Debug.Log("Team " + teamIndex + " gained " + points + " points. Total: " + teamPoints[teamIndex]);



        OnPointsUpdated?.Invoke(teamIndex, teamPoints[teamIndex]);
        if (HasTeamWon(teamIndex))
        {
            OnTeamWon?.Invoke(teamIndex);
            Debug.Log("Team " + teamIndex + " has won!");
        }


    }

    protected void LosePoints(int teamIndex, int points)
    {
        teamPoints[teamIndex] -= points;
        Debug.Log("Team " + teamIndex + " lost " + points + " points. Total: " + teamPoints[teamIndex]);
    }

    bool HasTeamWon(int teamIndex)
    {
        return teamPoints[teamIndex] >= gameModeStats.PointsToWin;
    }

    public Equipment GetEquipment()
    {
        return gameModeStats.StartingEquipment;
    }

    public int GetTeamsWithPlayers()
    {
        int count = 0;
        foreach (var team in teams)
        {
            if (team.Count > 0)
            {
                count++;
            }
        }
        return count;
    }

    public List<int> GetTeamPoints()
    {
        return teamPoints;
    }

    public int GetMaxScore()
    {
        return gameModeStats.PointsToWin;
    }

}

[Serializable]
public class SpawnSystem
{
    public Transform[] teamStartSpawnPoints;
    public Transform[] basicSpawnPoints;


    public Transform GetStartSpawnPoint(int teamIndex)
    {
        if (teamIndex < teamStartSpawnPoints.Length)
        {
            return teamStartSpawnPoints[teamIndex];
        }
        else
        {
            return basicSpawnPoints[UnityEngine.Random.Range(0, basicSpawnPoints.Length)];
        }

    }

    public Transform GetRandomSpawnPoint()
    {
        return basicSpawnPoints[UnityEngine.Random.Range(0, basicSpawnPoints.Length)];
    }

    public Transform GetFarthestSpawnPointFromEnemeies(List<Transform> enemies)
    {
        Transform farthest = basicSpawnPoints[0];
        float maxDistance = 0;
        foreach (var spawnPoint in basicSpawnPoints)
        {
            float distance = 0;
            foreach (var enemy in enemies)
            {
                distance += Vector3.Distance(spawnPoint.position, enemy.position);
            }
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = spawnPoint;
            }
        }
        return farthest;
    }
}

