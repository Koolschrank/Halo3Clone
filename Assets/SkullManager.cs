using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;

public class SkullManager : MonoBehaviour
{
	public Action OnClearSkulls;
	public Action<Skull> OnActivateSkull;


	// instance of the SkullManager
	public static SkullManager instance;


	public Skull[] randomSkulls;
	public List<Skull> startSkulls = new List<Skull>();
	[NonSerialized]
	public List<Skull> activeSkulls = new List<Skull>();

	public List<WaveSkullData> waveSkulls = new List<WaveSkullData>();


	public PlayerManager playerManager;
	public EnemySpawner enemySpawner;
	



	private void Awake()
	{
		instance = this;
		StartCoroutine(AwakeDelay());

	}

	public IEnumerator AwakeDelay()
	{
		yield return new WaitForNextFrameUnit();

		var gamemode = GameModeSelector.gameModeManager.GameModeStats;
		if (!gamemode.useSkulls)
		{
			enabled = false;
			yield break;
		}
		else
		{
			playerManager.OnPlayerSpawned += PlayerSpawned;
			enemySpawner.OnWaveStart += WaveStart;
			WaveStart(0);
		}
	}

	public void WaveStart(int wave)
	{
		bool hasNewSkullsThisWave = false;
		{
			foreach (var s in waveSkulls)
			{
				if (s.waveIndex == wave)
				{
					hasNewSkullsThisWave = true;
					break;
				}
			}
			if (!hasNewSkullsThisWave)
			{
				return;
			}
		}


		WaveSkullData skullData = waveSkulls.Find(data => data.waveIndex == wave);
		

		List<Skull> skullsToActivate = new List<Skull>(skullData.skulls);
		if (skullData.randomExtraSkulls > 0)
		{
			List<int> skullsUsed = new List<int>();
			for (int i = 0; i < skullData.randomExtraSkulls; i++)
			{
				int randomIndex;
				do
				{
					randomIndex = UnityEngine.Random.Range(0, randomSkulls.Length);
				} while (skullsUsed.Contains(randomIndex));
				
				skullsUsed.Add(randomIndex);
				skullsToActivate.Add(randomSkulls[randomIndex]);
			}
		}

		SetActiveSkulls(skullsToActivate);

	}

	private void Start()
	{
		SetActiveSkulls(startSkulls);
	}

	public void SetActiveSkulls(List<Skull> skulls)
	{
		foreach (var skull in activeSkulls)
		{
			if (skull != null)
			{
				skull.Deactivate();
			}
		}
		activeSkulls.Clear();
		OnClearSkulls?.Invoke();
		foreach (var skull in skulls)
		{
			if (skull != null)
			{
				activeSkulls.Add(skull);
				skull.Activate();
				OnActivateSkull?.Invoke(skull);
			}
		}
	}

	public void ActivateSkull(Skull skull)
	{
		skull.Activate();
	}

	public void DeactivateSkull(Skull skull)
	{
		skull.Deactivate();
	}

	public void PlayerSpawned(PlayerMind player)
	{
		foreach (var skull in activeSkulls)
		{
			skull.PlayerSpawned(player);
		}
	}
}


[Serializable]
public struct WaveSkullData
{
	public int waveIndex;
	public Skull[] skulls;
	public int randomExtraSkulls;
}