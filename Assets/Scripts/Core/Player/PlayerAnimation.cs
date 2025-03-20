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


    [Header("Shild")]
    [SerializeField] SkinnedMeshRenderer[] playerMeshes;
    [SerializeField] Transform[] shildBrakeParticals;
    [SerializeField] GameObject shildDepletedVisual;
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

    [Header("Granade")]
    [SerializeField] Transform granadeSocket;
    GameObject granadeVisual;



    // start
    public void Start()
    {
        playerMovement.OnJump += Jump;
        playerMovement.OnCrouch += () => UpdateCrouch(true);
        playerMovement.OnStandUp += () => UpdateCrouch(false);
        

        // connect reload
        playerArms.RightArm.OnWeaponReloadStarted += Reload;
        // connect switch weapon
        playerArms.RightArm.OnWeaponUnequipStarted += SwitchOutWeapon;
        playerArms.RightArm.OnWeaponEquipStarted += SwitchInWeapon;
        // throw granade
        playerArms.RightArm.OnGranadeThrowStarted += ThrowGranadeStart;
        playerArms.RightArm.OnGranadeThrow += ThrowGranade;
        playerArms.RightArm.OnMeleeWithWeaponStarted += Melee;
        playerArms.RightArm.OnWeaponDroped += DropWeapon;
        playerInventory.OnWeaponAddedToInventory += PutWeaponInBackpack;
        playerInventory.OnWeaponDrop += DropInvetoryWeapon;

        playerArms.LeftArm.OnWeaponEquipStarted += SwitchInLeftWeapon;

        playerArms.LeftArm.OnWeaponDroped += DropWeaponLeftWeapon;
        playerArms.LeftArm.OnWeaponUnequipFinished += DropWeaponLeftWeapon;

        characterHealth.OnShildDamageTaken += ShildDamageTaken;
        characterHealth.OnShildDepleted += ShildDepleted;
        characterHealth.OnShildRechargeStarted += ShildRechargeStarted;
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

    public void Update()
    {
        UpdateInAir();
        UpdateMove();
        UpdateGrip();
        UpdateAim();
        UpdateShild();
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
        animator.SetBool("InAir", !cc.isGrounded);

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

            animator.SetFloat("WeaponType", (float)weaponModel.WeaponAnimationIndex);
        }
        UtilityFunctions.SetLayerRecursively(weaponVisual, gameObject.layer);
    }

    public void SwitchInLeftWeapon(Weapon_Arms weapon, float animationDuration)
    {

        if (weapon == null)
        {
            return;
        }

        Debug.Log("SwitchInLeftWeapon");



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
        }
        UtilityFunctions.SetLayerRecursively(weaponVisualLeftHand, gameObject.layer);
    }

    public void Melee(Weapon_Arms weapon, float animationDuration)
    {
        var meleeClip = GetAnimationClipByName("Melee");
        var animationLenght = GetAnimationLenght(meleeClip);
        SetAnimationSpeed(meleeClip, animationLenght, animationDuration);
        animator.SetTrigger("Melee");
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

    public void ThrowGranadeStart(GranadeStats granade)
    {
        float animationDuration =granade.ThrowDelay;

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
    }

    public void ShildDepleted()
    {
        SetShildVisualPower(0);
        shildVisualRecoveryTimer = 0;
        foreach (var partical in shildBrakeParticals)
        {
            partical.gameObject.SetActive(true);
        }
        shildDepletedVisual.SetActive(true);
    }

    public void ShildRechargeStarted()
    {
        shildDepletedVisual.SetActive(false);

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

    public void UpdateShildStrength(float percentage)
    {
        float percentageCurved = shildStrengthCurve.Evaluate(percentage);
        float shildStrength = Mathf.Lerp(maxShildStrength, minShildStrength, percentageCurved);
        foreach (var smr in playerMeshes)
        {
            Material materialInstance = smr.material;

            if (materialInstance.GetFloat("_Strength") == shildStrength)
            {
                return;
            }
            materialInstance.SetFloat("_Strength", shildStrength);
        }
    }

    public void SetShildVisualPower(float power)
    {
        float truePower = shildVisualRecoveryCurve.Evaluate(power);

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
        leftHandGripActive = true;
    }

    public void UpdateCrouch(bool value)
    {
        animator.SetBool("Crouch", value);
    }
}
