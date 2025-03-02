using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMind : MonoBehaviour
{
    public UnityEvent OnPlayerDeath;
    public Action<PlayerMind> OnPlayerDied;
    public Action<PlayerMind> OnPlayerElimination;
    public Action<PlayerMind> OnTeamKill;

    //[SerializeField] Camera playerCamera;
    [SerializeField] Arm_FPSView armsView;
    [SerializeField] WeaponSway weaponSway;
    //[SerializeField] PlayerFOV playerFOV;
    [SerializeField] PlayerInput playerInput;

    CinemachineCamera spectatorCamera;
    [SerializeField] PlayerCamera playerCamera;
    //[SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] PlayerTeam team;


    [Header("UI")]
    [SerializeField] Transform UIContainer;
    [SerializeField] HealthUI healthUI;
    [SerializeField] ShildUI shildUI;
    [SerializeField] WeaponUI weaponUI;
    [SerializeField] PickUpUI pickUpUI;
    [SerializeField] DamageIndicatorUI damageIndicatorUI;
    [SerializeField] crosshairUI crosshairUI;
    [SerializeField] CooldownUI granadeCooldown;
    

    GameObject playerModel;
    PlayerMovement playerMovement;
    PlayerAim playerAim;
    PlayerArms playerArms;
    Health playerHealth;
    BulletSpawner bulletSpawner;
    PlayerPickUpScan playerPickUpScan;
    PlayerInventory playerInventory;



    int firstPersonLayer;
    int thirdPersonLayer;

    // set Player model
    public void SetPlayerModel(GameObject model)
    {
        playerModel = model;
    }

    // set movement
    public void SetPlayerMovement(PlayerMovement movement)
    {
        playerMovement = movement;
        weaponSway.SetUp(playerMovement);
    }

    public void SetSpectatorTarget(CinemachineCamera camera)
    {
        spectatorCamera = camera;
    }

    public void SetPlayerInventory(PlayerInventory inventory)
    {
        if (playerInventory != null)
        {
            playerInventory.OnGranadeChargeChanged -= granadeCooldown.UpdateCooldown;
        }

        playerInventory = inventory;
        inventory.OnGranadeChargeChanged += granadeCooldown.UpdateCooldown;
    }

    public void SetCinemaCamera(CinemachineCamera cCam)
    {
        playerCamera.SetCinemachineCamera(cCam);
    }



    // set aim
    public void SetPlayerAim(PlayerAim aim)
    {
        playerAim = aim;
    }

    // set arms
    public void SetPlayerArms(PlayerArms arms)
    {
        playerArms = arms;
        armsView.SetUp(arms);
        weaponUI.SetUp(arms);
        arms.OnZoomIn += playerCamera.ZoomIn;
        arms.OnZoomOut += playerCamera.ZoomOut;

    }

    // set bullet spawner
    public void SetBulletSpawner(BulletSpawner spawner)
    {
        if (bulletSpawner != null)
        {
            bulletSpawner.OnTargetAcquired -= crosshairUI.OnTargetAcquired;
            bulletSpawner.OnTargetLost -= crosshairUI.OnTargetLost;
            crosshairUI.OnTargetLost(null);
        }


        bulletSpawner = spawner;

        bulletSpawner.OnTargetAcquired += crosshairUI.OnTargetAcquired;
        bulletSpawner.OnTargetLost += crosshairUI.OnTargetLost;
    }

    // set health
    public void SetHealth(Health health)
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= PlayerDeath;
        }

        playerHealth = health;
        healthUI.SetUp(playerHealth);
        shildUI.SetUp(playerHealth as CharacterHealth);
        // connect health on death unity event with this function
        playerHealth.OnDeath += PlayerDeath;

        playerHealth.OnDamageTaken += damageIndicatorUI.AddDamageIndicator;
    }

    public void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();
        OnPlayerDied?.Invoke(this);
    }

    public void PlayerElimination(GameObject obj)
    {
        var otherPlayer = obj.GetComponent<PlayerTeam>();
        if (otherPlayer != null) {
            if (otherPlayer.TeamIndex == team.TeamIndex)
            {
                OnTeamKill?.Invoke(this);
            }
            else
            {
                OnPlayerElimination?.Invoke(this);

            }
        }
        

            
    }

    // set pick up scan
    public void SetPickUpScan(PlayerPickUpScan pickUpScan)
    {
        playerPickUpScan = pickUpScan;
        pickUpUI.SetUp(pickUpScan);
    }




    private void Start()
    {
        PlayerManager.instance.AddPlayer(this);
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (playerMovement == null) return;

        Vector2 movement = context.ReadValue<Vector2>();
        playerMovement.UpdateMoveInput(movement);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (playerAim == null) return;

        Vector2 look = context.ReadValue<Vector2>();
        playerAim.UpdateAimInput(look);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (playerMovement == null) return;

        playerMovement.TryJump();
    }

    public void WeaponTrigger(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        playerArms.UpdateWeaponTrigger(context.ReadValue<float>()> 0);
    }

    public void WeaponReload(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            if (playerArms == null) return;
            playerArms.PressReloadButton();
        }
    }

    public void WeaponSwitch(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.PressSwitchButton();
        }
    }

    public void WeaponPickUp(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.TryPickUpWeapon();
        }
    }

    public void ThrowGranade(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.PressGranadeButton();
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerMovement.ToggleCrouch();
        }
    }

    public void MeleeAttack(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.PressMeleeButton();
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;

        if (context.performed)
        {
            playerArms.PressZoomButton();
        }
        else if (context.canceled)
        {
            playerArms.ReleaseZoomButton();
        }
    }

    public void SetLayers(int FPS_Layer, int ThirdPerson_Layer)
    {
        firstPersonLayer = FPS_Layer;
        thirdPersonLayer = ThirdPerson_Layer;


        UpdateLayers();
    }

    public void UpdateLayers()
    {
        if (transform != null && playerModel != null)
        {
            UtilityFunctions.SetLayerRecursively(gameObject, firstPersonLayer);
            UtilityFunctions.SetLayerRecursively(playerModel, thirdPersonLayer);
        }
    }

    public void EnableLayerInCamera(int layer)
    {
        playerCamera.EnableLayerInCamera(layer);
    }


    public void RespawnWithDelay(float delay)
    {
        StartCoroutine(RespawnDelay(delay));
    }

    public void SwitchToSpectatorCamera()
    {
        // add camera to spectator camera offset as child
        //transform.SetParent(null);
        //playerCamera.transform.SetParent(spectatorCameraOffset);
        //playerCamera.transform.localPosition = Vector3.zero;
        //playerCamera.transform.localRotation = Quaternion.identity;
        armsView.gameObject.SetActive(false);
        UIContainer.gameObject.SetActive(false);
        spectatorCamera.Priority = 100;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        armsView.gameObject.SetActive(true);
        UIContainer.gameObject.SetActive(true);
        spectatorCamera.Priority = 0;


    }
    IEnumerator RespawnDelay(float delay)
    {
        SwitchToSpectatorCamera();
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    public void Respawn()
    {
        PlayerManager.instance.RespawnPlayer(this);
        SwitchToPlayerCamera();
    }

    public void SetScreenRect(ScreenRectValues screen, int channel)
    {
        playerCamera.SetScreenRect(screen, channel);
    }

    public int TeamIndex { get { return team.TeamIndex; } }

    public void AssignTeam(int team)
    {
        this.team.SetTeamIndex(team);
    }


    TargetHitCollector hitCollector;
    public void ConnectPlayerElimination(TargetHitCollector hitCollector)
    {
        if (this.hitCollector != null)
        {
            this.hitCollector.OnCharacterKill -= PlayerElimination;
        }

        this.hitCollector = hitCollector;

        hitCollector.OnCharacterKill += PlayerElimination;
    }


}


