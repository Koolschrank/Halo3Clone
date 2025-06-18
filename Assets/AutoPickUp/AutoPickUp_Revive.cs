using UnityEngine;

public class AutoPickUp_Revive : AutoPickUp
{
    public override void PickUp(GameObject player)
    {
        var allPlayers = PlayerManager.instance.GetAllPlayers();
        foreach (var p in allPlayers)
        {
            if (p.IsDead)
            {
                p.Respawn();
            }
            else
            {
                var body = p.PlayerBody;
                var health = body.GetComponent<Health>();
                health.Heal(health.MaxHeath); 
            }
        }
        base.PickUp(player);
    }
}
