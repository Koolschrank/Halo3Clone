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

    public virtual Transform GetFarthestSpawnPointFromEnemeies(PlayerMind playerMind)
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
        PlayerManager.instance.OnPlayerSpawned += PlayerSpawned;
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
        if (gameModeStats.ReasignsTeamsInPlayerOrder)
        {
            ReorderPlayerTeams();
        }

    }

    public virtual void PlayerSpawned(PlayerMind player)
    {

    }

    public void ReorderPlayerTeams()
    {
        var allPlayers = new List<PlayerMind>();
        allPlayers.AddRange(PlayerManager.instance.GetAllPlayers());

        foreach (var team in teams)
        {
            team.Clear();
        }

        int playerCount = allPlayers.Count;

        float teamSplit = (playerCount-1) / 2;
        // order players so that first half of players are team 0 and second half are team 1
        for (int i = 0; i < playerCount; i++)
        {
            


            var player = allPlayers[i];
            // first half of player (rounded up) are in team 0 the rest are in team 1
            // if index is higher than teamSplit then player is in team 1
            int teamIndex = (float)i > teamSplit ? 1 : 0;

            teams[teamIndex].Add(player);

            player.AssignTeam(teamIndex);
            ChangeTeamOfBody(player, teamIndex);
        }
    }

    public virtual void PlayerSwitchTeams(PlayerMind player)
    {



        // switch player to next possible team
        int currentIndex = player.TeamIndex;
        int nextIndex = (currentIndex + 1) % gameModeStats.TeamCount;
        // remove player from current team
        teams[currentIndex].Remove(player);
        // add player to next team
        teams[nextIndex].Add(player);

        player.AssignTeam(nextIndex);

        if (teams[nextIndex].Count == 1 || teams[currentIndex].Count == 0)
        {
            OnTeamAdded?.Invoke();
        }

        ChangeTeamOfBody(player, nextIndex);



    }

    public void ChangeTeamOfBody(PlayerMind mind, int teamIndex)
    {
        var playerBody = mind.PlayerBody;
        if (playerBody == null)
        {
            return;
        }

        var spawnPoint = GetStartingSpawnPoint(teamIndex);
        var characterController = playerBody.GetComponent<CharacterController>();
        characterController.enabled = false;
        playerBody.transform.position = spawnPoint.position;
        playerBody.transform.rotation = spawnPoint.rotation;
        characterController.enabled = true;
        PlayerManager.instance.UpdateTeamOfBody(mind);
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

    public List<int> GetTeamsWithPlayers()
    {
        List<int> teamsWithPlayers = new List<int>();
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].Count > 0)
            {
                teamsWithPlayers.Add(teams[i].Count);
            }
            else
            {
                teamsWithPlayers.Add(0);
            }
        }
        return teamsWithPlayers;
    }

    public List<int> GetTeamPoints()
    {
        return teamPoints;
    }

    public int GetMaxScore()
    {
        return gameModeStats.PointsToWin;
    }

    public bool HasWeaponPickups => gameModeStats.HasWeaponPickups;

    public int GetPointsLeftForTeamToWin(int teamIndex)
    {
        int pointsToWin = gameModeStats.PointsToWin;
        int pointsOfTeam = teamPoints[teamIndex];
        return pointsToWin - pointsOfTeam;
    }

}

[Serializable]
public class SpawnSystem
{
    public Transform[] teamStartSpawnPoints;
    public Transform[] basicSpawnPoints;
    public int lastSpawnPointIndex = 0;
    


    public Transform GetStartSpawnPoint(int teamIndex)
    {
        if (teamIndex < teamStartSpawnPoints.Length)
        {
            lastSpawnPointIndex = teamIndex;
            return teamStartSpawnPoints[teamIndex];
        }
        else
        {
            var randomIndex = UnityEngine.Random.Range(0, teamStartSpawnPoints.Length);
            lastSpawnPointIndex = randomIndex;
            return basicSpawnPoints[randomIndex];
        }

    }

    public Transform GetRandomSpawnPoint()
    {
        var randomIndex = UnityEngine.Random.Range(0, basicSpawnPoints.Length);
        lastSpawnPointIndex = randomIndex;
        return basicSpawnPoints[randomIndex];
    }

    public Transform GetFarthestSpawnPointFromEnemeies(List<Transform> enemies)
    {
        Transform farthest = basicSpawnPoints[0];
        float maxDistance = 0;
        int spawnPointIndex = 0;
        foreach (var spawnPoint in basicSpawnPoints)
        {
            float distance = 0;
            foreach (var enemy in enemies)
            {
                distance += Vector3.Distance(spawnPoint.position, enemy.position);
            }
            if (distance > maxDistance && spawnPointIndex != lastSpawnPointIndex)
            {
                maxDistance = distance;
                farthest = spawnPoint;
                lastSpawnPointIndex = spawnPointIndex;

            }
            spawnPointIndex++;
        }

        return farthest;
    }

    public Transform GetFarthestSpawnPointFromEnemeies(List<Transform> enemies, Transform enemyObjective, float multiplierForObjective)
    {
        Transform farthest = basicSpawnPoints[0];
        float maxDistance = 0;
        int spawnPointIndex = 0;
        foreach (var spawnPoint in basicSpawnPoints)
        {
            float distance = 0;
            foreach (var enemy in enemies)
            {
                distance += Vector3.Distance(spawnPoint.position, enemy.position);
            }

            distance += Vector3.Distance(spawnPoint.position, enemyObjective.position) * multiplierForObjective;
            if (distance > maxDistance && spawnPointIndex != lastSpawnPointIndex)
            {
                maxDistance = distance;
                farthest = spawnPoint;
                lastSpawnPointIndex = spawnPointIndex;

            }
            spawnPointIndex++;
        }

        return farthest;
    }



}

