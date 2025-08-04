using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ReviveBody : MonoBehaviour
{
	public Action<float> OnReviveProgress;
	public Action<Vector3> OnReviveBody;



	[SerializeField] float reviveTime = 3f;
	[SerializeField] float timeToReset = 0.5f;



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
		Debug.Log("ReviveBody: Activate");
		if (active)
		{
			return;
		}
		playerMind = ownerOfBody;
		active = true;
		enabled = true;
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
		isReviving = false;
		if (reviveTimer != 0f && lastUpdateTime + timeToReset < Time.timeSinceLevelLoad)
		{
			reviveTimer -= Time.deltaTime;
			if (reviveTime <= 0f)
			{
				reviveTimer = 0f;
				OnReviveProgress?.Invoke(0f);
			}
			else
			{
				OnReviveProgress?.Invoke(reviveTimer);
			}


		}

		transform.localPosition = Vector3.zero;

		// set layer index to 13
		if (gameObject.layer != 13)
		{
			gameObject.layer = 13; 
		}
		
	}

	public void AddValue(float value)
	{
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

		playerMind.RevivePlayer(transform.position + Vector3.up * 1f);

	}




}
