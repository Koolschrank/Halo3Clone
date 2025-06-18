using UnityEngine;

public class AutoPickUp_AmmoRefill : AutoPickUp
{
    public override void PickUp(GameObject player)
    {
        var allPlayers = PlayerManager.instance.GetAllPlayers();

        foreach (var p in allPlayers)
        {
            var body = p.PlayerBody;
            if (body != null)
            {
                var leftArm = body.GetComponent<LeftArm>();
                if (leftArm != null)
                {
                    leftArm.RefillAmmoOfWeapon();
                }
                var rightArm = body.GetComponent<RightArm>();
                if (rightArm != null)
                {
                    rightArm.RefillAmmoOfWeapon();
                }

                var inventory = body.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.RefillReserveOfAllWeaponsYouOwn();
                }
            }
        }


        base.PickUp(player);
    }
}
