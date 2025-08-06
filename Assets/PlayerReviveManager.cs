using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReviveManager : MonoBehaviour
{

	// singeltoon
	public static PlayerReviveManager Instance { get; private set; }

	public int addIndex = 0;
	List<ReviveBodyData> reviveBodies = new List<ReviveBodyData>();


	private void Awake()
	{
		Instance = this;
	}

	

	public void AddBodyToRevive(int index,int teamIndex, Transform transform)
	{
		Debug.Log($"Adding body to revive with index: {index}");
		index += addIndex;

		if (reviveBodies.Exists(x => x.index == index))
		{
			Debug.LogWarning($"ReviveBody with index {index} already exists.");
			return;
		}

		var allPlayers = PlayerManager.instance.GetAllPlayers();
		foreach (var player in allPlayers)
		{
			if (player.TeamIndex == teamIndex)
				player.EnableObjectiveUIMarker(index);
		}

		ReviveBodyData newBody = new ReviveBodyData(index, teamIndex, transform);
		reviveBodies.Add(newBody);



		var objectivesManager = ObjectiveIndicator.instance;

		objectivesManager.GetObjective(index).SetActive(true);
		objectivesManager.GetObjective(index).SetHideDistance(0.1f);
		objectivesManager.GetObjective(index).SetTeamIndex(-1);

	}

	public void RemoveBodyToRevive(int index)
	{
		Debug.Log($"Removing body to revive with index: {index}");
		index += addIndex;
		var body = reviveBodies.Find(x => x.index == index);
		reviveBodies.Remove(body);
		var objectivesManager = ObjectiveIndicator.instance;
		objectivesManager.GetObjective(index).SetActive(false);
		objectivesManager.GetObjective(index).SetHideDistance(10000000);




	}

	private void LateUpdate()
	{
		foreach (var body in reviveBodies)
		{
			var objectivesManager = ObjectiveIndicator.instance;
			objectivesManager.GetObjective(body.index).SetPosition(body.transform.position);


		}
	}



}

public struct ReviveBodyData
{
	public int index;
	public Transform transform;
	public int teamIndex;
	public ReviveBodyData(int index, int teamIndex, Transform transform)
	{
		this.index = index;
		this.teamIndex = teamIndex;
		this.transform = transform;
	}
}
