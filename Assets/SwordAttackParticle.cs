using UnityEngine;
using System;
using System.Collections;

public class SwordAttackParticle : MonoBehaviour
{

    public Weapon_Model weaponModel;

    public float delay = 0.5f;
    public GameObject swordParticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponModel.OnMeleeAttack += PlayMeleeAttackAnimation;
    }

	private void OnDestroy()
	{
		   
        weaponModel.OnMeleeAttack -= PlayMeleeAttackAnimation;
	}

	private void PlayMeleeAttackAnimation()
    {
        StartCoroutine(PlayParticleAfterDelay());

	}

    IEnumerator
        PlayParticleAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        swordParticle.SetActive(true);
        yield return new WaitForSeconds(1f);
        swordParticle.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
