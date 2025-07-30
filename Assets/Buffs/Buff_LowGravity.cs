using UnityEngine;


[CreateAssetMenu(fileName = "Buff_DualWield", menuName = "Buffs/LowGravity")]
public class Buff_LowGravity : Buff
{
	[SerializeField] float gravityMultiplier = 1;
	[SerializeField] float movementSpeedMultiplier = 1;

	[SerializeField] bool ignoreGravity = false; // if true, the player will not be affected by gravity at all, useful for zero gravity effects

	public override void ApplyBuff(GameObject player)
	{
		if (player.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
		{
			if (!ignoreGravity)
				playerMovement.gravityMultiplier = gravityMultiplier;
			playerMovement.SetMovementSpeedMultiplier(movementSpeedMultiplier);
		}
	}

	public override void RemoveBuff(GameObject player)
	{
		if (player.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
		{
			if (!ignoreGravity)
				playerMovement.gravityMultiplier = 1;
			playerMovement.SetMovementSpeedMultiplier(1);
		}
	}
}
