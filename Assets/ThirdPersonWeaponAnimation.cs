using System.Collections;
using UnityEngine;

public class ThirdPersonWeaponAnimation : MonoBehaviour
{
	[SerializeField] Weapon_Model mainScript;
	[SerializeField] GameObject weaponModel;
	[SerializeField] Transform newOffset;
	[SerializeField] Transform stabelizer;
	[SerializeField] Animator animator;
	[SerializeField] GameObject attackParticle;

	public void PlayAttackParticle()
	{
		if (attackParticle != null)
		{
			attackParticle.SetActive(false);
			attackParticle.SetActive(true);
			StartCoroutine(DisableParticleAfterDelay(1f));
		}
	}

	IEnumerator DisableParticleAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		attackParticle.SetActive(false);
	}

	private void Awake()
	{
		if (mainScript != null)
			mainScript.OnStartManuelAnimations += AttachWeapon;
	}

	private void OnDestroy()
	{
		if (mainScript != null)
		{
			mainScript.OnStartManuelAnimations -= AttachWeapon;

			if (attached)
			{
				mainScript.OnZoomUpdate -= UpdateBlock;
				mainScript.OnMeleeAttack -= PlayMeleeAttack;
			}
				
		}
	}

	bool attached = false;

	public void AttachWeapon()
	{
		attached = true;
		mainScript.OnMeleeAttack += PlayMeleeAttack;
		mainScript.OnZoomUpdate += UpdateBlock;	

		if (weaponModel != null && newOffset != null)
		{
			weaponModel.transform.SetParent(newOffset);
			weaponModel.transform.localPosition = Vector3.zero;
			weaponModel.transform.localRotation = Quaternion.identity;
		}
	}

	public void UpdateBlock(bool val)
	{

		if (animator != null)
		{
			animator.SetBool("Block", val);
		}
	}

	public void PlayMeleeAttack()
	{
		if (animator != null)
		{
			Stabilize();
			animator.SetTrigger("MeleeAttack");
			// disable block
			animator.SetBool("Block", false);
		}
	}

	public void AttackEnd()
	{
		StopStabilizing();
	}

	private Quaternion savedRotationXZ;
	private bool isStabilizing = false;

	public void Stabilize()
	{
		savedRotationXZ = stabelizer.rotation; // save world rotation
		isStabilizing = true;
	}

	public void StopStabilizing()
	{
		isStabilizing = false;
		stabelizer.localRotation = Quaternion.identity;
	}

	void LateUpdate()
	{
		if (isStabilizing)
		{
			// Keep saved XZ, keep current Y
			Vector3 current = stabelizer.rotation.eulerAngles;
			Vector3 target = new Vector3(
				savedRotationXZ.eulerAngles.x,
				current.y,
				savedRotationXZ.eulerAngles.z
			);

			stabelizer.rotation = Quaternion.Euler(target);
		}
	}
}
