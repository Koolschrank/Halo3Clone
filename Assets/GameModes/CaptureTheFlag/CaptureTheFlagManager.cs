using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CaptureTheFlagManager : GameModeManager
{
    [SerializeField] GameObject team1_FlagPrefab;
    [SerializeField] GameObject team2_FlagPrefab;
    GameObject team1_Flag;
    Weapon_Data team1_FlagData;
    GameObject team2_Flag;
    Weapon_Data team2_FlagData;



    [SerializeField] Transform team1_Base;
    [SerializeField] Transform team1_FlagSpawnPoint;
    [SerializeField] Transform team2_Base;
    [SerializeField] Transform team2_FlagSpawnPoint;

    [SerializeField] float flagRecoveryTimer = 15f;
    [SerializeField] float scoreRange = 1f;


    bool flag1_droped = false;
    float flag1_dropedTimer = 0;

    bool flag2_droped = false;
    float flag2_dropedTimer = 0;

    private void Update()
    {
        UpdateFlag1Position();
        UpdateFlag2Position();

    }

    public override Transform GetFarthestSpawnPointFromEnemeies(PlayerMind playerMind)
    {
        int teamIndex = playerMind.TeamIndex;

        var enemyBase = teamIndex == 0 ? team2_Base : team1_Base;

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

        return spawnSystem.GetFarthestSpawnPointFromEnemeies(enemies, enemyBase, 1.0f);
    }

    public void UpdateFlag1Position()
    {
        if (team1_Flag != null)
        {
            ObjectiveIndicator.instance.GetObjective(0).SetPosition(team1_Flag.transform.position);
        }

        if (flag1_droped)
        {
            flag1_dropedTimer -= Time.deltaTime;
            ObjectiveIndicator.instance.GetObjective(0).SetText(((int)flag1_dropedTimer).ToString());
            if (flag1_dropedTimer <= 0)
            {
                SpawnFlagPrefab_Team1();
            }
        }
        else
        {
            flag1_dropedTimer += Time.deltaTime;
            
        }

        // if flag is close to opposite base
        if (team1_Flag != null && team2_Base != null)
        {
            if (Vector3.Distance(team1_Flag.transform.position, team2_Base.position) < scoreRange)
            {
                GainPoints(1, 1);
                if (team1_Flag.TryGetComponent<PlayerArms>(out PlayerArms arms))
                {
                    arms.DeleteWeapon(team1_FlagData);
                }
                else
                {
                    Destroy(team1_Flag);
                }
                team1_Flag = null;

                SpawnFlagPrefab_Team1();
            }
        }


    }

    public void UpdateFlag2Position()
    {
        if (team2_Flag != null)
        {
            ObjectiveIndicator.instance.GetObjective(1).SetPosition(team2_Flag.transform.position);
        }

        if (flag2_droped)
        {
            flag2_dropedTimer -= Time.deltaTime;
            ObjectiveIndicator.instance.GetObjective(1).SetText(((int)flag2_dropedTimer).ToString());
            if (flag2_dropedTimer <= 0)
            {
                SpawnFlagPrefab_Team2();
            }
        }
        else
        {
            flag2_dropedTimer += Time.deltaTime;
        }

        // if flag is close to opposite base
        if (team2_Flag != null && team1_Base != null)
        {
            if (Vector3.Distance(team2_Flag.transform.position, team1_Base.position) < scoreRange)
            {
                GainPoints(0, 1);
                if (team2_Flag.TryGetComponent<PlayerArms>(out PlayerArms arms))
                {
                    arms.DeleteWeapon(team2_FlagData);
                }
                else
                {
                    Destroy(team2_Flag);
                }
                team2_Flag = null;
                SpawnFlagPrefab_Team2();
            }
        }
    }


    public override void ResetGame()
    {
        base.ResetGame();
        SpawnFlagPrefab_Team1();
        SpawnFlagPrefab_Team2();
    }

    public override void PlayerJoined(PlayerMind player)
    {
        base.PlayerJoined(player);
        //player.EnableObjectiveUIMarker();
    }

    public override void PlayerSpawned(PlayerMind player)
    {
        base.PlayerSpawned(player);

        var arms = player.Body.PlayerArms;
        arms.LeftArm.OnWeaponPickedUp += (weapon) =>
        {
            if (weapon.weaponTypeIndex == team1_FlagData.WeaponTypeIndex)
            {
                FlagPickedUp_Team1(player.Body.gameObject);
            }
            else if (weapon.weaponTypeIndex == team2_FlagData.WeaponTypeIndex)
            {
                FlagPickedUp_Team2(player.Body.gameObject);
            }
        };

        arms.RightArm.OnWeaponPickedUp += (weapon) =>
        {
            if (weapon.weaponTypeIndex == team1_FlagData.WeaponTypeIndex)
            {
                FlagPickedUp_Team1(player.Body.gameObject);
            }
            else if (weapon.weaponTypeIndex == team2_FlagData.WeaponTypeIndex)
            {
                FlagPickedUp_Team2(player.Body.gameObject);
            }
        };

        arms.LeftArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            if (weapon.Data == team1_FlagData)
            {
                FlagDroped_Team1(pickUp);
            }
            else if (weapon.Data == team2_FlagData)
            {
                FlagDroped_Team2(pickUp);
            }
        };

        arms.RightArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            if (weapon.Data == team1_FlagData)
            {
                FlagDroped_Team1(pickUp);
            }
            else if (weapon.Data == team2_FlagData)
            {
                FlagDroped_Team2(pickUp);
            }
        };
    }




    public void SpawnFlagPrefab_Team1()
    {
        if (team1_Flag != null)
        {
            Destroy(team1_Flag);
        }
        flag1_droped = false;


        team1_Flag = Instantiate(team1_FlagPrefab, team1_FlagSpawnPoint.position, Quaternion.identity);
        var pickUp = team1_Flag.GetComponent<Weapon_PickUp>();
        pickUp.BlockPickUpForTeam(0);
        team1_FlagData = pickUp.WeaponData;

        // flag
        ObjectiveIndicator.instance.GetObjective(0).SetTeamIndex(0);
        ObjectiveIndicator.instance.GetObjective(0).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(0).SetText("");
        ObjectiveIndicator.instance.GetObjective(0).SetHideDistance(1);

        // base
        ObjectiveIndicator.instance.GetObjective(2).SetPosition(team1_Base.position);
        ObjectiveIndicator.instance.GetObjective(2).SetTeamIndex(0);
        ObjectiveIndicator.instance.GetObjective(2).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(2).SetText("Base");
        ObjectiveIndicator.instance.GetObjective(2).SetHideDistance(1);
    }

    public void SpawnFlagPrefab_Team2()
    {
        if (team2_Flag != null)
        {
            Destroy(team2_Flag);
        }
        flag2_droped = false;



        team2_Flag = Instantiate(team2_FlagPrefab, team2_FlagSpawnPoint.position, Quaternion.identity);
        var pickUp = team2_Flag.GetComponent<Weapon_PickUp>();
        pickUp.BlockPickUpForTeam(1);
        team2_FlagData = pickUp.WeaponData;

        // flag
        ObjectiveIndicator.instance.GetObjective(1).SetTeamIndex(1);
        ObjectiveIndicator.instance.GetObjective(1).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(1).SetText("");
        ObjectiveIndicator.instance.GetObjective(1).SetHideDistance(1);

        // base
        ObjectiveIndicator.instance.GetObjective(3).SetPosition(team2_Base.position);
        ObjectiveIndicator.instance.GetObjective(3).SetTeamIndex(1);
        ObjectiveIndicator.instance.GetObjective(3).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(3).SetText("Base");
        ObjectiveIndicator.instance.GetObjective(3).SetHideDistance(1);

    }

    public void FlagPickedUp_Team1(GameObject player)
    {
        team1_Flag = player;
        ObjectiveIndicator.instance.GetObjective(0).SetText("Stolen");
        flag1_droped = false;
    }

    public void FlagPickedUp_Team2(GameObject player)
    {
        team2_Flag = player;
        ObjectiveIndicator.instance.GetObjective(1).SetText("Stolen");
        flag2_droped = false;
    }

    public void FlagDroped_Team1(Weapon_PickUp pickUp)
    {
        if (pickUp == null)
        {
            return;
        }
        team1_Flag = pickUp.gameObject;
        flag1_droped = true;
        flag1_dropedTimer = Mathf.Min(flag1_dropedTimer, flagRecoveryTimer);
        var pickup = team1_Flag.GetComponent<Weapon_PickUp>();
        pickup.BlockPickUpForTeam(0);

    }

    public void FlagDroped_Team2(Weapon_PickUp pickUp) {
        if (pickUp == null)
        {
            return;
        }
        team2_Flag = pickUp.gameObject;
        flag2_droped = true;
        flag2_dropedTimer = Mathf.Min(flag2_dropedTimer, flagRecoveryTimer);
        var pickup = team2_Flag.GetComponent<Weapon_PickUp>();
        pickup.BlockPickUpForTeam(1);
    }


}


