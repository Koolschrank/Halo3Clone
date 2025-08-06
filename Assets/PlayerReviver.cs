using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReviver : MonoBehaviour
{
	public Action OnHasReviveBody;
	public Action OnNoReviveBody;
	public Action<float> OnReviveProgress;

	List<ReviveBody> reviveBodiesCloseToPlayer = new List<ReviveBody>();
	[SerializeField] CharacterHealth characterHealth;

	[SerializeField] RightArm rightArm;
	[SerializeField] LeftArm leftArm;
	[SerializeField] MeleeAttacker meleeAttacker;
	[SerializeField] GranadeThrower granadeThrower;
	[SerializeField] PlayerTeam playerTeam;

	private void Awake()
	{
		rightArm.OnWeaponShoot += (weapon) => ResetReviveBody();
		leftArm.OnWeaponShoot += (weapon) => ResetReviveBody();
		meleeAttacker.OnAttackStart += (attack) => ResetReviveBody();
		granadeThrower.OnGranadeThrow += (granade) => ResetReviveBody();
	}

	public void AddReviveBody(ReviveBody reviveBody)
	{
		if (!reviveBodiesCloseToPlayer.Contains(reviveBody))
		{
			reviveBodiesCloseToPlayer.Add(reviveBody);
		}

		if (reviveBodiesCloseToPlayer.Count == 1)
		{
			OnHasReviveBody?.Invoke();
		}
	}

	public void RemoveReviveBody(ReviveBody reviveBody)
	{
		if (reviveBodiesCloseToPlayer.Contains(reviveBody))
		{
			reviveBodiesCloseToPlayer.Remove(reviveBody);
		}

		if (reviveBodiesCloseToPlayer.Count == 0)
		{
			OnNoReviveBody?.Invoke();
		}
	}

	private void Update()
	{
		if (reviveBodiesCloseToPlayer.Count == 0)
		{
			return; 
		}

		if (characterHealth.IsDead)
		{
			reviveBodiesCloseToPlayer.Clear();
			OnNoReviveBody?.Invoke();
			return;
		}

		// get close revive bodies
		ReviveBody closeReviveBodies = GetClosestReviveBody();
		if (closeReviveBodies != null)
		{
			closeReviveBodies.AddValue(Time.deltaTime);
			OnReviveProgress?.Invoke(closeReviveBodies.Progress);
		}
		else
		{
			reviveBodiesCloseToPlayer.Clear();
			OnNoReviveBody?.Invoke();
		}
			
	}

	public void ResetReviveBody()
	{
		var closeReviveBodies = GetClosestReviveBody();
		if (closeReviveBodies != null)
		{
			closeReviveBodies.ResetRevive();
			OnReviveProgress?.Invoke(0f);
		}

	}

	public ReviveBody GetClosestReviveBody()
	{
		if (reviveBodiesCloseToPlayer.Count == 0)
		{
			return null;
		}
		// get close revive bodies
		ReviveBody closeReviveBodies = null;
		float distance = float.MaxValue;
		foreach (var reviveBody in reviveBodiesCloseToPlayer)
		{
			if (reviveBody == null || !reviveBody.gameObject.activeInHierarchy || reviveBody.teamIndex != playerTeam.TeamIndex)
			{
				continue;
			}
			float currentDistance = Vector3.Distance(transform.position, reviveBody.transform.position);
			if (currentDistance < distance)
			{
				distance = currentDistance;
				closeReviveBodies = reviveBody;
			}
		}
		return closeReviveBodies;
	}

	public float GetProgressOfCloses()
	{
		if (reviveBodiesCloseToPlayer.Count == 0)
		{
			return 0f;
		}

		// get close revive bodies
		ReviveBody closeReviveBodies = GetClosestReviveBody();
		if (closeReviveBodies != null)
		{
			return closeReviveBodies.Progress;
		}
		else
		{
			return 0f;
		}
	}
}
