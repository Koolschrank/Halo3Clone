using UnityEngine;

public class AutoPickUp_Nuke : AutoPickUp_GainScore
{
    public override void PickUp(GameObject player)
    {

        EnemySpawner.instance.KillAllEnemies();
        base.PickUp(player);


    }

}
