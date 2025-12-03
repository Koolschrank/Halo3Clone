using System;
using UnityEngine;
using System.Collections.Generic;

public class FortniteStorm : MonoBehaviour
{
	public KingOfTheHillManager kingOfTheHillManager;

	public Action<GameObject> OnPlayerEnterStorm;
	public Action<GameObject> OnPlayerExitStorm;
	public float damagePerSecond = 20f;
	public float damagePerSecondMultiplierOnShield = 0.5f;

	public List<CharacterHealth> charactersInStorm = new List<CharacterHealth>();

	[NonSerialized]
	public float shrinkDuration;
	[NonSerialized]
	public float shrinkProgress = 0f;
	public AnimationCurve stormShrinkCurve;


	float stormInitialSkale = 50f;
	public float stormMinSize = 10f;

	Transform target;
	Vector3 startPosition = Vector3.zero;

	public void StartStorm()
		{

		if (!gameObject.activeSelf) return;

		target = kingOfTheHillManager.GetRandomHill().transform;
		shrinkProgress = 0f;
		transform.position = startPosition;
		transform.localScale = Vector3.one * stormInitialSkale;
	}


	private void Awake()
	{
		startPosition = transform.position;
		stormInitialSkale = transform.localScale.x;
	}


	private void OnTriggerEnter(Collider other)
	{
		// check if has characterhealth component
		if (other.TryGetComponent<CharacterHealth>(out CharacterHealth character))
		{
			OnPlayerExitStorm?.Invoke(other.gameObject);
			charactersInStorm.Remove(character);
		}

	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<CharacterHealth>(out CharacterHealth character))
		{
			OnPlayerEnterStorm?.Invoke(other.gameObject);
			// check if already in list
			if (!charactersInStorm.Contains(character))
			{
				charactersInStorm.Add(character);
			}
		}

	}

	private void Update()
	{
		foreach (CharacterHealth character in charactersInStorm)
		{
			ApplyStormDamage(character);
		}

		if (shrinkProgress < 1f)
		{
			if (target == null)
			{
				target = kingOfTheHillManager.GetRandomHill().transform;
			}
			shrinkProgress += Time.deltaTime / shrinkDuration;
			float curveValue = stormShrinkCurve.Evaluate(shrinkProgress);
			float newScale = Mathf.Lerp(stormInitialSkale, stormMinSize,1- curveValue);
			transform.localScale = Vector3.one * newScale;
			transform.position = Vector3.Lerp(startPosition, target.position,1- curveValue);
		}
	}

	private void ApplyStormDamage(CharacterHealth character)
	{
		var damagePackage = new DamagePackage
		{
			damageAmount = damagePerSecond * Time.deltaTime,
			
			owner = character.ownerOfLastDamage,
			canHeadShotShild = false,
			headShotMultiplier = 1f,
			shildDamageMultiplier =	damagePerSecondMultiplierOnShield,

		};
		damagePackage.damageReductionAgainstBlock = 0;
		damagePackage.noScreenShake = true;

		character.TakeDamage(damagePackage);
	}
}
