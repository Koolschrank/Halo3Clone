using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_MoveSpeed", menuName = "Upgrades/Move Speed Upgrade")]
public class Upgrade_MoveSpeed : Upgrade
{
    [SerializeField] float moveSpeedMultiplier = 1.20f;

    public override void Apply(GameObject body)
    {
        var playerMovement = body.GetComponent<PlayerMovement>();
        playerMovement.MultiplyMaxMoveSpeed(moveSpeedMultiplier);
    }
}
