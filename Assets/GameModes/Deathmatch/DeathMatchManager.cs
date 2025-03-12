using UnityEngine;

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
        int teamIndex = playerWhoElimnated.TeamIndex;
        GainPoints(teamIndex, -1);
    }




}
