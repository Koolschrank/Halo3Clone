using System;
using System.Collections;
using UnityEngine;

public class PVEPointsManagment : MonoBehaviour
{
	public Action<float> OnLifePointsChanged;
	public Action OnLifePointsLossStart;
	public Action OnLifePointsLossEnd;
	public Action<float> OnLifePointsGain;
	public Action OnEnterCriticalMode;
	public Action OnExitCriticalMode;
	public Action OnNoLifePoints;
	public Action OnEnterSpeedUpMode;
	public Action OnExitSpeedUpMode;
	public Action OnLifePointsGainStart;
	public Action OnLifePointsGainEnd;

	public static PVEPointsManagment instance;


	float lifePoints = 100f;
	float lifePointsLossPerSecond = 1f;
	float lifePointsGainPerSecond = 1f;
	[SerializeField] float maxLifePoints = 100f;
	[SerializeField] float criticalModeThreshold = 20f;
	[SerializeField] float speedUpLossMultiplier = 5f;
	[SerializeField] AnimationCurve lifePointsLossCurve;


	bool isLosingLifePoints = false;
	bool isGainingLifePoints = false;
	bool isInCriticalMode = false;
	bool isInSpeedUpMode = false;

	private void Awake()
	{
		instance = this;


		StartCoroutine(SetUpDelay());
	}


	IEnumerator SetUpDelay()
	{
		yield return new WaitForSeconds(0.1f);
		var gameMode = GameModeSelector.gameModeManager;
		gameMode.OnEnterSpeedUp += EnterSpeedUpMode;
		gameMode.OnExitSpeedUp += ExitSpeedUpMode;
	}

	public void SetLifePointLoss(float newValue)
	{
		lifePointsLossPerSecond = newValue;
	}

	public void SetLifePointGain(float newValue)
	{
		lifePointsGainPerSecond = newValue;
	}

	public void StartLifeGainLoss()
	{
		if (!isLosingLifePoints)
		{
			isLosingLifePoints = true;
			OnLifePointsLossStart?.Invoke();
			StopLifePointsGain(); // Stop gaining life points when losing life points
		}
		
	}

	public void StopLifeGainLoss()
	{
		if (isLosingLifePoints)
		{
			isLosingLifePoints = false;
			OnLifePointsLossEnd?.Invoke();
			
		}
	}

	public void StartLifePointsGain()
	{
		if (!isGainingLifePoints)
		{
			isGainingLifePoints = true;
			OnLifePointsGainStart?.Invoke();
			StopLifeGainLoss(); // Stop losing life points when gaining life points
		}
		
	}

	public void StopLifePointsGain()
	{
		if (isGainingLifePoints)
		{
			isGainingLifePoints = false;
			OnLifePointsGainEnd?.Invoke();
		}
	}

	public void GainLifePoints(float amount)
	{
		lifePoints += amount;
		if (lifePoints > maxLifePoints)
		{
			lifePoints = maxLifePoints;
		}
		OnLifePointsChanged?.Invoke(lifePoints);
		OnLifePointsGain?.Invoke(amount);
		if (lifePoints >= criticalModeThreshold && isInCriticalMode)
		{
			isInCriticalMode = false;
			OnExitCriticalMode?.Invoke();
		}
	}

	public void EnterSpeedUpMode()
	{
		if (!isInSpeedUpMode)
		{
			isInSpeedUpMode = true;
			OnEnterSpeedUpMode?.Invoke();
		}
	}

	public void ExitSpeedUpMode()
	{
		if (isInSpeedUpMode)
		{
			isInSpeedUpMode = false;
			OnExitSpeedUpMode?.Invoke();
		}
	}

	private void Update()
	{
		if (isLosingLifePoints)
		{
			float deltaTime = Time.deltaTime;
			float lossAmount = lifePointsLossCurve.Evaluate(lifePoints / maxLifePoints) * lifePointsLossPerSecond * deltaTime;
			if (isInSpeedUpMode)
			{
				lossAmount *= speedUpLossMultiplier;
			}
			lifePoints -= lossAmount;
			if (lifePoints <= 0f)
			{
				lifePoints = 0f;
				OnNoLifePoints?.Invoke();
			}
			OnLifePointsChanged?.Invoke(lifePoints);
			if (lifePoints < criticalModeThreshold && !isInCriticalMode)
			{
				isInCriticalMode = true;
				OnEnterCriticalMode?.Invoke();
			}
		}
		if (isGainingLifePoints)
		{
			float deltaTime = Time.deltaTime;
			float gainAmount = lifePointsGainPerSecond * deltaTime;
			lifePoints += gainAmount;
			if (lifePoints > maxLifePoints)
			{
				lifePoints = maxLifePoints;
			}
			OnLifePointsChanged?.Invoke(lifePoints);
			OnLifePointsGain?.Invoke(gainAmount);

			if (lifePoints >= criticalModeThreshold && isInCriticalMode)
			{
				isInCriticalMode = false;
				OnExitCriticalMode?.Invoke();
			}
		}
	}

}
