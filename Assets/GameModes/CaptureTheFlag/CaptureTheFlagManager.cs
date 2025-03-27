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
    [SerializeField] Transform team1_FalgSpawnPoint;
    [SerializeField] Transform team2_Base;
    [SerializeField] Transform team2_FalgSpawnPoint;

    [SerializeField] float movementSpeedDebuffWithFlag = 0.4f;

    private void Update()
    {
        UpdateFlag1Position();
        UpdateFlag2Position();

    }

    public void UpdateFlag1Position()
    {
        if (team1_Flag != null)
        {
            ObjectiveIndicator.instance.GetObjective(0).SetPosition(team1_Flag.transform.position);
        }
        
    }

    public void UpdateFlag2Position()
    {
        if (team2_Flag != null)
        {
            ObjectiveIndicator.instance.GetObjective(1).SetPosition(team2_Flag.transform.position);
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
        player.EnableObjectiveUIMarker();
    }

    public override void PlayerSpawned(PlayerMind player)
    {
        base.PlayerSpawned(player);
        var arms = player.PlayerBody.GetComponent<PlayerArms>();
        arms.LeftArm.OnWeaponPickedUp += (weapon) =>
        {
            if (weapon.Data == team1_FlagData)
            {
                FlagPickedUp_Team1(player.PlayerBody);
            }
            else if (weapon.Data == team2_FlagData)
            {
                FlagPickedUp_Team2(player.PlayerBody);
            }
        };

        arms.RightArm.OnWeaponPickedUp += (weapon) =>
        {
            if (weapon.Data == team1_FlagData)
            {
                FlagPickedUp_Team1(player.PlayerBody);
            }
            else if (weapon.Data == team2_FlagData)
            {
                FlagPickedUp_Team2(player.PlayerBody);
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
        team1_Flag = Instantiate(team1_FlagPrefab, team1_FalgSpawnPoint.position, Quaternion.identity);
        var pickUp = team1_Flag.GetComponent<Weapon_PickUp>();
        team1_FlagData = pickUp.WeaponData;
        //pickUp.OnPickUp += (pickup) =>
        //{
        //    FlagPickedUp_Team1();
        //};

        
        ObjectiveIndicator.instance.GetObjective(0).SetTeamIndex(0);
        ObjectiveIndicator.instance.GetObjective(0).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(0).SetNumber(-1);
        ObjectiveIndicator.instance.GetObjective(0).SetHideDistance(1);
    }

    public void SpawnFlagPrefab_Team2()
    {
        team2_Flag = Instantiate(team2_FlagPrefab, team2_FalgSpawnPoint.position, Quaternion.identity);
        var pickUp = team2_Flag.GetComponent<Weapon_PickUp>();
        team2_FlagData = pickUp.WeaponData;
        //pickUp.OnPickUp += (pickup) =>
        //{
        //    FlagPickedUp_Team2();
        //};
        ObjectiveIndicator.instance.GetObjective(1).SetTeamIndex(1);
        ObjectiveIndicator.instance.GetObjective(1).SetActive(true);
        ObjectiveIndicator.instance.GetObjective(1).SetNumber(-1);
        ObjectiveIndicator.instance.GetObjective(1).SetHideDistance(1);

    }

    public void FlagPickedUp_Team1(GameObject player)
    {
        team1_Flag = player;
    }

    public void FlagPickedUp_Team2(GameObject player)
    {
        team2_Flag = player;
    }

    public void FlagDroped_Team1(Weapon_PickUp pickUp)
    {
        team1_Flag = pickUp.gameObject;
    }

    public void FlagDroped_Team2(Weapon_PickUp pickUp) {
        team2_Flag = pickUp.gameObject;
    }


}


