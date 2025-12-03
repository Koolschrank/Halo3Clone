using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DeathMatchManager : GameModeManager
{
    public override void PlayerJoined(PlayerMind player)
    {
        base.PlayerJoined(player);
        player.OnPlayerElimination += PlayerEliminated;
        player.OnTeamKill += TeamKill;
    }

    void PlayerEliminated(GameObject killedPlayer,PlayerMind playerWhoElimnated)
    {
        int teamIndex = playerWhoElimnated.TeamIndex;
        GainPoints(teamIndex, 1);
    }

    void TeamKill(GameObject killedPlayer, PlayerMind playerWhoElimnated)
    {
        //int teamIndex = playerWhoElimnated.TeamIndex;
        //GainPoints(teamIndex, -1);
    }

	protected override void GainPoints(int teamIndex, int points)
	{
		base.GainPoints(teamIndex, points);

        if (gameModeStats is GameMode_GunGame)
        {
            var allPlayers = PlayerManager.instance.GetAllPlayers();
            var teamScores = teamPoints[teamIndex];
			var newEquipment = gameModeStats.GetEquipmentBasedOnPoints(teamScores);
			foreach (var player in allPlayers)
            {
                if (player.TeamIndex == teamIndex && player.PlayerBody != null && !player.PlayerBody.GetComponent<CharacterHealth>().IsDead)
                {
                    StartCoroutine(DelayGainNewWeapon(player.PlayerBody, newEquipment, 0.35f));
				}
			}


            
		}
	}

    IEnumerator DelayGainNewWeapon(GameObject player, Equipment equipment, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (player != null)
        {
            var health = player.GetComponent<CharacterHealth>();
			if ( !health.IsDead)
			{
				var startEquipment = player.GetComponent<PlayerStartEquipment>();
				startEquipment.ClearEquipment();
				startEquipment.GetEquipment(equipment);
			}

			
		}
	}




}
