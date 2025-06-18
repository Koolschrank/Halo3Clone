using UnityEngine;

public class AutoPickUp_GainScore : AutoPickUp
{
    [SerializeField] private int scoreAmount = 500;

    public override void PickUp(GameObject player)
    {
        foreach (var p in PlayerManager.instance.GetAllPlayers())
        {
            p.AddScore(scoreAmount);
        }


        base.PickUp(player);
    }
}
