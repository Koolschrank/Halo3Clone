using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ReviveBody : MonoBehaviour
{
	public Action<float> OnReviveProgress;
	public Action<Vector3> OnReviveBody;



	[SerializeField] Collider reviveCollider;
	[SerializeField] float reviveTime = 3f;
	[SerializeField] float timeToReset = 0.5f;
	[SerializeField] float valueMultiplierWhenStoped = 0.1f;

	int index = -1;
	[NonSerialized]
	public int teamIndex = -1;

	bool active = false;
	float reviveTimer = 0f;
	PlayerMind playerMind;

	float lastUpdateTime = 0f;

	public float Progress
	{
		get
		{
			if (reviveTime == 0f)
			{
				return 0f;
			}
			return reviveTimer / reviveTime;
		}
	}


	public void Activate(PlayerMind ownerOfBody)
	{
		if (ownerOfBody == null)
		{
			return;
		}

		Debug.Log("ReviveBody: Activate");
		if (active)
		{
			return;
		}
		playerMind = ownerOfBody;
		active = true;
		enabled = true;
		index = ownerOfBody.playerID;
		teamIndex = ownerOfBody.TeamIndex;

		PlayerReviveManager.Instance.AddBodyToRevive(index,teamIndex, transform);
		reviveCollider.enabled = true;
		gameObject.SetActive(true);
	}


	private void Start()
	{
		if (gameObject.layer != 13)
		{
			gameObject.layer = 13;
		}
	}





	private void OnTriggerEnter(Collider other)
	{
		if (!active)
		{
			return;
		}
		// check if tag is player
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerReviver>().AddReviveBody(this);
		}

	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerReviver>().RemoveReviveBody(this);
		}
		

	}
	bool isReviving = false;
	private void Update()
	{
		if (!active)
		{
			return;
		}

		isReviving = false;
		if (reviveTimer != 0f && lastUpdateTime + timeToReset < Time.timeSinceLevelLoad)
		{
			ResetRevive();
		}

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		stoppedTime -= Time.deltaTime;

		// set layer index to 13
		if (gameObject.layer != 13)
		{
			gameObject.layer = 13; 
		}
		
	}

	public void ResetRevive()
	{
		reviveTimer = 0f;
		OnReviveProgress?.Invoke(0f);
	}

	float stoppedTime = 0f;
	public void Stop(float stopTime)
	{
		stoppedTime = stopTime;

	}

	public void AddValue(float value)
	{
		if (stoppedTime > 0)
			value *= valueMultiplierWhenStoped;

		if (reviveTimer <0)
		{
			reviveTimer = 0f;
		}

		if ( isReviving)
		{
			return;
		}
		isReviving = true;
		lastUpdateTime = Time.timeSinceLevelLoad;
		reviveTimer += value;
		if (reviveTimer >= reviveTime)
		{
			Revive();
		}

		OnReviveBody?.Invoke(transform.position);
	}

	public void Revive()
	{
		if (playerMind == null)
		{
			return;
		}
		

		OnReviveBody?.Invoke(transform.position);
		reviveTimer = 0f;
		gameObject.SetActive(false);

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		playerMind.RevivePlayer(transform.position + Vector3.up * 1f);

		reviveCollider.enabled = false;
		

	}

	private void OnDisable()
	{
		if (index != -1)
			PlayerReviveManager.Instance.RemoveBodyToRevive(index);
	}
}
