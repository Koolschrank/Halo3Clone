using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Reverences")]
    [SerializeField] Transform lookTransform;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] CharacterController cc;
    [SerializeField] Animator animator;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] Transform aimTarget;
    [SerializeField] Transform weaponSocket;
    [SerializeField] Transform weaponSocketLeftHand;

	[SerializeField] Transform weaponSocketShild;

	[SerializeField] Transform weaponSocketBack;
	[SerializeField] Rig rig;


    [Header("Shild")]
    [SerializeField] SkinnedMeshRenderer[] playerMeshes;
    [SerializeField] Transform[] shildBrakeParticals;
    [SerializeField] GameObject shildDepletedVisual;

	[SerializeField] GameObject[] shildRechageVisual;
	[SerializeField] float minShildStrength = 3;
    [SerializeField] float maxShildStrength = 12;
    [SerializeField] AnimationCurve shildStrengthCurve;

    GameObject weaponVisual;
    GameObject weaponVisualLeftHand;
    [SerializeField] Transform backpackWeaponSocket;
    GameObject backpackWeaponVisual;

    [SerializeField] MultiAimConstraint rightHandWeaponGrip;
    
    [SerializeField] TwoBoneIKConstraint leftHandWeaponGrip;

    [Header("Settings")]
    [SerializeField] float landRaycastDistance = 2.5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float shildVisualRecoveryTime = 1;
    [SerializeField] AnimationCurve shildVisualRecoveryCurve;

    float shildVisualRecoveryTimer = 0;

    bool rightHandGripActive = true;
    bool leftHandGripActive = true;

    [Header("Settings")]
    [SerializeField] float gripChangeTime = 0.1f;
    [SerializeField] float localIdealDirectionChangeTime = 0.1f; // time it takes to change the local ideal direction

    [Header("Granade")]
    [SerializeField] Transform granadeSocket;
    GameObject granadeVisual;

    Vector3 localIdealDirection = Vector3.forward;

    public float powerMultiplier = 1;

    [Header("Flinch")]
    [SerializeField] float flinchWithShild = 0.4f;
    [SerializeField] float flinchWithoutShild = 0.9f;

	// start
	public void Start()
    {
        playerMovement.OnJump += Jump;
        playerMovement.OnCrouch += () => UpdateCrouch(true);
        playerMovement.OnStandUp += () => UpdateCrouch(false);
        playerMovement.OnRollStarted += Roll;
        playerMovement.OnRollEnded += RollEnd;
		characterHealth.OnFlinch += Flinch;


		// connect reload
		playerArms.RightArm.OnWeaponReloadStarted += Reload;
        // connect switch weapon
        playerArms.RightArm.OnWeaponUnequipStarted += SwitchOutWeapon;
        playerArms.RightArm.OnWeaponEquipStarted += SwitchInWeapon;
        // throw granade
        playerArms.RightArm.OnGranadeThrowStarted += ThrowGranadeStart;
        playerArms.RightArm.OnGranadeThrow += ThrowGranade;
        playerArms.RightArm.OnMeleeWithWeaponStarted += Melee;
        playerArms.RightArm.OnWeaponDroped += (weapon,pickup) => DropWeapon(weapon);
        playerInventory.OnWeaponAddedToInventory += (weapon,ammo)  => PutWeaponInBackpack(weapon);
        playerInventory.OnWeaponDrop += DropInvetoryWeapon;

        playerArms.LeftArm.OnWeaponEquipStarted += SwitchInLeftWeapon;

        playerArms.LeftArm.OnWeaponDroped += (weapon, pickup) => DropWeaponLeftWeapon(weapon);
        playerArms.LeftArm.OnWeaponUnequipFinished += DropWeaponLeftWeapon;

		playerArms.LeftArm.OnZoomIn += (weapon) => SetShildOnHand();

		playerArms.LeftArm.OnZoomOut += (weapon) => SetShildOnBack();


		characterHealth.OnShildDamageTaken += ShildDamageTaken;
        characterHealth.OnShildDepleted += ShildDepleted;
        characterHealth.OnShildRechargeStarted += ShildRechargeStarted;
        characterHealth.OnShildHealStarted += ShildRechargeParticle;
		characterHealth.OnDeath += DisableShildpPartical;
        characterHealth.OnShildChanged += UpdateShildStrength;

        if (weaponVisual == null)
        {
            var weapon = playerArms.RightArm.GetWeaponInHand();
            var switchInTime = playerArms.RightArm.GetWeaponInHandSwitchInTime();
            SwitchInWeapon(weapon, switchInTime);
        }
        if (weaponVisualLeftHand == null)
        {
            var weapon = playerArms.LeftArm.GetWeaponInHand();
            var switchInTime = playerArms.LeftArm.GetWeaponInHandSwitchInTime();
            SwitchInLeftWeapon(weapon, switchInTime);
        }
        if (backpackWeaponVisual == null)
        {
            var weapon = playerInventory.GetWeapon();
            if (weapon != null)
            {
                PutWeaponInBackpack(weapon);
            }
        }
    }

	private void Flinch()
	{
        animator.SetFloat("FlinchIndex", UnityEngine.Random.Range(0f, 1f));
        animator.Play("Flinch", 4,0);
	}

	public void ChangeLocalIdealDirection(Vector3 direction)
    {
        localIdealDirection = direction.normalized;
    }

    public void ResetLocalIdealDirection()
    {
        localIdealDirection = Vector3.forward; // reset to the parent forward direction
    }

    public void Update()
    {
       // UpdateLocalDirection();
        UpdateInAir();
        UpdateMove();
        UpdateGrip();
        UpdateAim();
        UpdateShild();
    }

    public void Roll(Vector3 direction, float rollTime)
    {
        ChangeLocalIdealDirection(direction);

        var rollClip = GetAnimationClipByName("Roll");
        var animationLenght = GetAnimationLenght(rollClip);
        SetAnimationSpeed(rollClip, animationLenght, rollTime);
        animator.SetTrigger("Roll");
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(3, 1);

        DisableLeftHandGrip();
        DisableRightHandGrip();
        rig.weight = 0; // disable rigging during roll
    }

    public void RollEnd()
    {
        StartCoroutine(RollEndDelay());
    }

    IEnumerator RollEndDelay()
    {
        yield return new WaitForSeconds(0.0f); // wait for a short time before ending the roll
        ResetLocalIdealDirection();
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(3, 0); // reset roll layer weight
        EnableLeftHandGrip();
        EnableRightHandGrip();
        rig.weight = 1; // enable rigging after roll
    }

    public void UpdateLocalDirection()
    {
        if (localIdealDirection == Vector3.zero)
        {
            return; // no change
        }
        
        // lerp the local ideal direction to the new direction
        var currentDirection = transform.forward;
        if (currentDirection == localIdealDirection)
        {
            return; // no change needed
        }
        var newDirection = Vector3.MoveTowards(currentDirection, localIdealDirection, Time.deltaTime * localIdealDirectionChangeTime);
        // set lokal forward
        transform.forward = transform.parent.forward + newDirection;

    }

    public void UpdateAim()
    {
        var forward = lookTransform.transform.forward;
        var position = lookTransform.transform.position;
        var targetPosition = position + forward * 10;
        aimTarget.position = targetPosition;
    }


    public void UpdateInAir()
    {
        animator.SetBool("InAir", !cc.isGrounded || playerMovement.inPushedState);

        if (!cc.isGrounded && cc.velocity.y < 0)
        {
            // shoot a raycast down to check if player is grounded
            if (Physics.Raycast(cc.transform.position, Vector3.down, landRaycastDistance, groundLayer))
            {
                animator.SetTrigger("Land");
            }
        }
        else
        {
            animator.ResetTrigger("Land");
        }
    }

    public void UpdateMove()
    {
        var velocity = cc.velocity;
        var maxSpeed = playerMovement.MaxMoveSpeed;

        float forwardVelocity = Vector3.Dot(velocity, transform.forward);
        float rightVelocity = Vector3.Dot(velocity, transform.right);

        animator.SetFloat("MoveX", forwardVelocity / maxSpeed);
        animator.SetFloat("MoveZ", rightVelocity / maxSpeed);
    }

    public void UpdateGrip()
    {
        var rightHandGripChangeThisFrame = Time.deltaTime / gripChangeTime;
        var leftHandGripChangeThisFrame = Time.deltaTime / gripChangeTime;

        if (!rightHandGripActive)
        {
            rightHandGripChangeThisFrame = -rightHandGripChangeThisFrame;
        }
        rightHandWeaponGrip.weight = Mathf.Clamp(rightHandWeaponGrip.weight + rightHandGripChangeThisFrame, 0, 1);

        if (!leftHandGripActive)
        {
            leftHandGripChangeThisFrame = -leftHandGripChangeThisFrame;
        }

        leftHandWeaponGrip.weight = Mathf.Clamp(leftHandWeaponGrip.weight + leftHandGripChangeThisFrame, 0, 1);
    }

    public void UpdateShild()
    {
        if (shildVisualRecoveryTimer > 0)
        {
            shildVisualRecoveryTimer -= Time.deltaTime;
            SetShildVisualPower(shildVisualRecoveryTimer / shildVisualRecoveryTime);
        }
        else
        {
            SetShildVisualPower(0);
        }
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Reload(Weapon_Arms weapon,float animationDuration)
    {
        if (weaponVisualLeftHand != null && weaponVisualLeftHand.GetComponent<Weapon_Model>().IsShild) return;


        var reloadClip = GetAnimationClipByName("Reload");
        var animationLenght = GetAnimationLenght(reloadClip);
        SetAnimationSpeed(reloadClip, animationLenght, animationDuration);
        animator.SetTrigger("Reload");
    }

    public void SwitchOutWeapon(Weapon_Arms weapon, float animationDuration)
    {
        var switchOutClip = GetAnimationClipByName("SwitchOut");
        var animationLenght = GetAnimationLenght(switchOutClip);
        SetAnimationSpeed(switchOutClip, animationLenght, animationDuration);

        animator.SetTrigger("SwitchOut");
    }

    public void DropWeapon(Weapon_Arms weapon)
    {
        if (weaponVisual != null)
        {
            Destroy(weaponVisual.gameObject);
        }
    }

    public void DropWeaponLeftWeapon(Weapon_Arms weapon)
    {
        if (weaponVisualLeftHand != null)
        {
            Destroy(weaponVisualLeftHand.gameObject);

            EnableLeftHandGrip();
        }
    }

    public void DropInvetoryWeapon(Weapon_Arms weapon)
    {
        if (backpackWeaponVisual != null)
        {
            Destroy(backpackWeaponVisual.gameObject);
        }
    }

    public void SwitchInWeapon(Weapon_Arms weapon, float animationDuration)
    {
        if (weapon == null)
        {
            return;
        }

        //var child = transform.GetChild(0);

        var switchInClip = GetAnimationClipByName("SwitchIn");
        var animationLenght = GetAnimationLenght(switchInClip);
        SetAnimationSpeed(switchInClip, animationLenght, animationDuration);

        animator.SetTrigger("SwitchIn");



        if (weaponVisual != null)
        {
            Destroy(weaponVisual.gameObject);
        }

        weaponVisual = Instantiate(weapon.WeaponModel, weaponSocket);
        weaponVisual.transform.localPosition = Vector3.zero;
        weaponVisual.transform.localRotation = Quaternion.identity;
        if (weaponVisual.TryGetComponent<Weapon_Model>(out Weapon_Model weaponModel))
        {
			weaponModel.SetUp(weapon);
			//if (weaponVisualLeftHand == null || !weaponVisualLeftHand.GetComponent<Weapon_Model>().IsShild)
            if (!shilding)
            {
				
				var animationValue = 0f;
				if (weaponModel.WeaponAnimationIndex == 1)
					animationValue = 0.5f;
				else if (weaponModel.WeaponAnimationIndex == 2)
					animationValue = 1f;
				animator.SetFloat("WeaponType", animationValue);

                EnableLeftHandGrip();
			}
        }
        UtilityFunctions.SetLayerRecursively(weaponVisual, gameObject.layer);
    }

    public void SwitchInLeftWeapon(Weapon_Arms weapon, float animationDuration)
    {

        if (weapon == null)
        {
            return;
        }




        if (weaponVisualLeftHand != null)
        {
            Destroy(weaponVisualLeftHand.gameObject);
        }

        weaponVisualLeftHand = Instantiate(weapon.WeaponModel, weaponSocketLeftHand);
        weaponVisualLeftHand.transform.localPosition = Vector3.zero;
        weaponVisualLeftHand.transform.localRotation = Quaternion.identity;
        if (weaponVisualLeftHand.TryGetComponent<Weapon_Model>(out Weapon_Model weaponModel))
        {
            weaponModel.SetUp(weapon);

            if (weaponModel.IsShild)
            {
                SetShildOnBack();
			}
        }

		


		UtilityFunctions.SetLayerRecursively(weaponVisualLeftHand, gameObject.layer);
    }

    public void Melee(Weapon_Arms weapon, float animationDuration)
    {
        string meleeName = "Melee";
        if (weapon.ShootType == ShootType.Melee)
        {
            meleeName ="Melee2";
		}

		var meleeClip = GetAnimationClipByName(meleeName);
        var animationLenght = GetAnimationLenght(meleeClip);
        SetAnimationSpeed(meleeClip, animationLenght, animationDuration);
        animator.SetTrigger(meleeName);
    }

    public void PutWeaponInBackpack(Weapon_Arms weapon)
    {
        if (backpackWeaponVisual != null)
        {
            Destroy(backpackWeaponVisual.gameObject);
        }
        backpackWeaponVisual = Instantiate(weapon.WeaponModel, backpackWeaponSocket);
        backpackWeaponVisual.transform.localPosition = Vector3.zero;
        backpackWeaponVisual.transform.localRotation = Quaternion.identity;
        UtilityFunctions.SetLayerRecursively(backpackWeaponVisual, gameObject.layer);
    }

    public void ThrowGranadeStart(GranadeStats granade, float time)
    {
        float delayPercent = granade.ThrowDelay / granade.ThrowTime;
        float animationDuration = delayPercent * time;

        var throughInClip = GetAnimationClipByName("Throw");
        var animationLenght = GetAnimationLenght(throughInClip);
        SetAnimationSpeed(throughInClip, animationLenght, animationDuration);


        animator.SetTrigger("ThrowGranade");

        // set LeftArm Layer weight to 1
        animator.SetLayerWeight(2, 1);
        DisableLeftHandGrip();

        granadeVisual = Instantiate(granade.GranadeClonePrefab, granadeSocket);
        granadeVisual.transform.localPosition = Vector3.zero;
        granadeVisual.transform.localRotation = Quaternion.identity;
        UtilityFunctions.SetLayerRecursively(granadeVisual, gameObject.layer);
    }

    public void ThrowGranade(GameObject granade, GranadeStats granadeStats)
    {
        if (granadeVisual == null)
        {
            return;
        }

        // unparent granade
        granadeVisual.transform.parent = null;
        var granadeScript = granade.GetComponent<Granade>();
        granadeScript.AddGranadeCopy(granadeVisual.transform);
        granadeVisual = null;
        
    }

    public void DisableLeftHandLayer()
    {
        animator.SetLayerWeight(2, 0);
    }

    public void Die()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * landRaycastDistance);
    }



    AnimationClip GetAnimationClipByName(string animationName)
    {
        animationName = GetTextAfterLastUnderscore(animationName);


        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            var clipName = GetTextAfterLastUnderscore(clip.name);
            if (clipName == animationName)
            {
                return clip; // Return the matching AnimationClip
            }
        }
        return null; // Return null if not found
    }

    public float GetAnimationLenght(AnimationClip animationClip)
    {
        return animationClip.length;
    }

    public void SetAnimationSpeed(AnimationClip clip, float animationLenght, float animationTime)
    {
        //AnimationSpeed
        var speed = animationLenght / animationTime;

        animator.SetFloat("AnimationSpeed", speed); // Adjust speed of animation using a float parameter

    }

    string GetTextAfterLastUnderscore(string input)
    {
        int index = input.LastIndexOf('_');

        return index >= 0 ? input.Substring(index + 1) : input; // Return original if no "_"
    }


    public void ShildDamageTaken()
    {
        shildVisualRecoveryTimer = shildVisualRecoveryTime;
        SetShildVisualPower(1);
        foreach (var partical in shildRechageVisual)
        {
            partical.gameObject.SetActive(false);
		}
	}

    public void ShildDepleted()
    {
        SetShildVisualPower(0);
        shildVisualRecoveryTimer = 0;
        //foreach (var partical in shildBrakeParticals)
        //{
        //    partical.gameObject.SetActive(true);
        //}
        shildDepletedVisual.SetActive(true);
        DisableOutline();



    }

    public void ShildRechargeStarted()
    {
        shildDepletedVisual.SetActive(false);
		

	}

    public void ShildRechargeParticle()
    {
		foreach (var partical in shildRechageVisual)
		{
			partical.gameObject.SetActive(true);
		}
	}

	public void DisableShildpPartical()
    {
        shildDepletedVisual.SetActive(false);
    }

    public void SetPlayerColor(Color color)
    {
        foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;
            materialInstance.SetColor("_ArmorColor", color);
        }
		
	}
    float outlineStrength = 1f;
	public void SetPlayerOutlineStrength(float strength)
        {
        outlineStrength = strength;
        var shildStrenght = characterHealth.ShildPercentage;
        if (!characterHealth.HasShild())
            shildStrenght = 0f;
		foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;
            materialInstance.SetFloat("_OutlinePower", strength * shildStrenght);
		}
	}

    public void DisableOutline()
        {
        foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;
            materialInstance.SetFloat("_OutlinePower", 0f);
        }
	}
    float lastShildPercentage = -1f;
	public void UpdateShildStrength(float percentage)
    {
        if (percentage >= lastShildPercentage )
        {
			shildVisualRecoveryTimer = shildVisualRecoveryTime;
			SetShildVisualPower(1f);

		}
        lastShildPercentage = percentage;

		float percentageCurved = shildStrengthCurve.Evaluate(percentage);
        float shildStrength = Mathf.Lerp(maxShildStrength, minShildStrength, percentageCurved);
        var tempOutlineStrength = outlineStrength * percentage;
		foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;

            if (materialInstance.GetFloat("_Strength") == shildStrength)
            {
                return;
            }
            materialInstance.SetFloat("_Strength", shildStrength);
            materialInstance.SetFloat("_OutlinePower", tempOutlineStrength);
		}

        if (percentage <= 0.01)
        {
            var flinchLayer = 4;
			animator.SetLayerWeight(flinchLayer, flinchWithoutShild);
		}
        else
        {
            var flinchLayer = 4;
            animator.SetLayerWeight(flinchLayer, flinchWithShild);
		}
	}

    public void SetShildVisualPower(float power)
    {
        float truePower = shildVisualRecoveryCurve.Evaluate(power) * powerMultiplier;

        foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;

            if (materialInstance.GetFloat("_Power") == truePower)
            {
                return;
            }
            materialInstance.SetFloat("_Power", truePower);
        }

    }

    public void Stun()
    {
        animator.SetTrigger("Hit");
    }

    public void SetLeftHandGrip(bool value)
    {
        leftHandGripActive = value;
    }

    public void DisableRightHandGrip()
    {
        rightHandGripActive = false;
    }

    public void DisableLeftHandGrip()
    {
        leftHandGripActive = false;
    }

    public void EnableRightHandGrip()
    {
        rightHandGripActive = true;
    }

    public void EnableLeftHandGrip()
    {
        if (weaponVisualLeftHand != null && weaponVisualLeftHand.GetComponent<Weapon_Model>().IsShild) return;

        leftHandGripActive = true;
    }

    public void UpdateCrouch(bool value)
    {
        animator.SetBool("Crouch", value);
    }

    bool shilding = false;
    public void SetShildOnHand()
    {

        if (weaponVisualLeftHand == null || !weaponVisualLeftHand.GetComponent<Weapon_Model>().IsShild) return;

		shilding = true;

		weaponVisualLeftHand.transform.SetParent(weaponSocketShild);
		weaponVisualLeftHand.transform.position = Vector3.zero;
        weaponVisualLeftHand.transform.rotation = Quaternion.identity;

		animator.SetFloat("WeaponType", 1f);
		DisableLeftHandGrip();
		weaponVisualLeftHand.transform.SetParent(weaponSocketShild);
		weaponVisualLeftHand.transform.localPosition = Vector3.zero;
		weaponVisualLeftHand.transform.localRotation = Quaternion.identity;
	}

    public void SetShildOnBack()
    {
		if (weaponVisualLeftHand == null || !weaponVisualLeftHand.GetComponent<Weapon_Model>().IsShild) return;
        shilding = false;

		weaponVisualLeftHand.transform.SetParent(weaponSocketBack);
		weaponVisualLeftHand.transform.localPosition = Vector3.zero;
		weaponVisualLeftHand.transform.localRotation = Quaternion.identity;

		var animationValue = 0f;

        if (weaponVisual != null)
        {
			var rightWeapon = weaponVisual.GetComponent<Weapon_Model>();
			if (rightWeapon.WeaponAnimationIndex == 1)
				animationValue = 0.5f;
			else if (rightWeapon.WeaponAnimationIndex == 2)
				animationValue = 1f;
			animator.SetFloat("WeaponType", animationValue);

			
		}
        else
        {
			animator.SetFloat("WeaponType", 0f);
		}
		EnableLeftHandGrip();


	}
}
