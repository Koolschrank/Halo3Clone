using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMind : MonoBehaviour
{
    public UnityEvent OnPlayerDeath;
    public Action<PlayerMind> OnPlayerDied;
    public Action<GameObject, PlayerMind> OnPlayerElimination;
    public Action<GameObject, PlayerMind> OnTeamKill;

    //[SerializeField] Camera playerCamera;
    [SerializeField] Arm_FPSView rightArmView;
    [SerializeField] Arm_FPSView leftArmView;
    
    [SerializeField] WeaponSway weaponSway1;

    [SerializeField] WeaponSway weaponSway2;
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
    [SerializeField] WeaponUI weaponUI_RightArm;
    [SerializeField] WeaponUI weaponUI_LeftArm;
    [SerializeField] PickUpUI pickUpUI;
    [SerializeField] DamageIndicatorUI damageIndicatorUI;
    [SerializeField] crosshairUI crosshairUI;
    [SerializeField] CooldownUI granadeCooldown;
    [SerializeField] TeamWinUI teamWinUI;
    [SerializeField] HitMarkerUI hitMarkerUI;
    [SerializeField] MinimapUI minimapUI;
    [SerializeField] ObjectiveIndicatorUI objectiveIndicatorUI;

    [Header("Input Settings")]
    [SerializeField] float holdButtonToPickUpTime = 0.2f;


    GameObject playerBody;
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


    public void EnterOneWeaponMode()
    {
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("PlayerGunPlay_SingleWeapon").Enable();
        playerInput.actions.FindActionMap("PlayerGunPlay_DualWeapons").Disable();
    }

    public void EnterDualWeaponMode()
    {
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("PlayerGunPlay_SingleWeapon").Disable();
        playerInput.actions.FindActionMap("PlayerGunPlay_DualWeapons").Enable();
    }

    public void Start()
    {
        GameModeSelector.gameModeManager.OnTeamWon += teamWinUI.TeamWon;

        PlayerManager.instance.AddPlayer(this);

        
    }

    public void SetPlayerBody(GameObject body)
    {
        playerBody = body;
    }

    public GameObject PlayerBody { get { return playerBody; } }

    // set Player model
    public void SetPlayerModel(GameObject model)
    {
        playerModel = model;
    }

    // set movement
    public void SetPlayerMovement(PlayerMovement movement)
    {
        playerMovement = movement;
        weaponSway1.SetUp(playerMovement);
        weaponSway2.SetUp(playerMovement);
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
            playerInventory.OnMiniMapDisabled -= minimapUI.DisableMiniMap;
            playerInventory.OnMiniMapEnabled -= minimapUI.EnableMiniMap;
        }

        playerInventory = inventory;
        inventory.OnGranadeChargeChanged += granadeCooldown.UpdateCooldown;
        inventory.OnMiniMapDisabled += minimapUI.DisableMiniMap;
        inventory.OnMiniMapEnabled += minimapUI.EnableMiniMap;
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
        if (playerArms != null)
        {
            playerArms.RightArm.OnZoomIn -= playerCamera.ZoomIn;
            playerArms.RightArm.OnZoomOut -= playerCamera.ZoomOut;
            playerArms.OnDualWieldingEntered -= EnterDualWeaponMode;
            playerArms.OnDualWieldingExited -= EnterOneWeaponMode;
            playerArms.OnDualWieldingExited -= weaponUI_LeftArm.Disable;

        }


        playerArms = arms;
        rightArmView.SetUp(arms.RightArm);
        leftArmView.SetUp(arms.LeftArm);
        weaponUI_RightArm.SetUp(arms.RightArm);
        weaponUI_LeftArm.SetUp(arms.LeftArm);
        arms.OnDualWieldingExited += weaponUI_LeftArm.Disable;
        arms.RightArm.OnZoomIn += playerCamera.ZoomIn;
        arms.RightArm.OnZoomOut += playerCamera.ZoomOut;


        arms.OnDualWieldingEntered += EnterDualWeaponMode;
        arms.OnDualWieldingExited += EnterOneWeaponMode;

        if (arms.LeftArm.CurrentWeapon != null)
        {
            EnterDualWeaponMode();
        }
        else
        {
            EnterOneWeaponMode();
        }
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
        playerHealth.OnDeath += damageIndicatorUI.Clear;
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
                OnTeamKill?.Invoke(obj,this);
            }
            else
            {
                OnPlayerElimination?.Invoke(obj, this);

            }
        }
        

            
    }

    // set pick up scan
    public void SetPickUpScan(PlayerPickUpScan pickUpScan)
    {
        playerPickUpScan = pickUpScan;
        pickUpUI.SetUp(pickUpScan);
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
        playerArms.RightArm.UpdateWeaponTrigger(context.ReadValue<float>()> 0);
    }


    bool reloadButtonReleased = true;
    float reloadButtonStartPressTime = 0;
    public void WeaponReload(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {

            if (playerArms.RightArm.PressReloadButtonIfNothingToPickUp())
                return;

            reloadButtonReleased = false;
            StartCoroutine(PickUpWeaponTimer());
            reloadButtonStartPressTime = Time.time;
        }

        if (context.canceled )
        {
            if (reloadButtonStartPressTime + holdButtonToPickUpTime > Time.time)
            {
                playerArms.RightArm.PressReloadButton();
            }

            reloadButtonReleased = true;

        }
    }

    IEnumerator PickUpWeaponTimer()
    {
        yield return new WaitForSeconds(holdButtonToPickUpTime);
        if (!reloadButtonReleased)
        {
            playerArms.RightArm.TryPickUpWeapon();
        }
    }

    bool switchButtonReleased = true;
    float switchButtonStartPressTime = 0;

    public void WeaponSwitch(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            if (playerArms.RightArm.CurrentWeapon.WeaponType != WeaponType.oneHanded && !playerArms.CanDualWield2HandedWeapons)
            {
                playerArms.RightArm.PressSwitchButton();
                return;
            }



            if (!playerInventory.HasWeapon)
            {
                if (playerArms.RightArm.PressSwitchButtonIfNothingToPickUp())
                    return;
            }


            

            switchButtonReleased = false;
            StartCoroutine(SwitchWeaponTimer());
            switchButtonStartPressTime = Time.time;
        }

        if (context.canceled)
        {
            if (switchButtonStartPressTime + holdButtonToPickUpTime > Time.time)
            {
                playerArms.RightArm.PressSwitchButton();
            }
            switchButtonReleased = true;
        }

    }

    IEnumerator SwitchWeaponTimer()
    {
        yield return new WaitForSeconds(holdButtonToPickUpTime);
        if (!switchButtonReleased)
        {
            if (playerArms.LeftArm.CanPickUpWeapon())
            {
                playerArms.LeftArm.TryPickUpWeapon();
            }
            else
            {
                playerArms.LeftArm.PressSwitchButton();
            }
        }
    }

    public void WeaponPickUp(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.RightArm.TryPickUpWeapon();
        }
    }

    public void ThrowGranade(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            playerArms.RightArm.PressGranadeButton();
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
            playerArms.RightArm.PressMeleeButton();
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;

        if (context.performed)
        {
            playerArms.RightArm.PressZoomButton();
        }
        else if (context.canceled)
        {
            playerArms.RightArm.ReleaseZoomButton();
        }
    }

    public void WeaponTrigger_1(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        playerArms.RightArm.UpdateWeaponTrigger(context.ReadValue<float>() > 0);
    }

    public void WeaponTrigger_2(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        playerArms.LeftArm.UpdateWeaponTrigger(context.ReadValue<float>() > 0);
    }

    public void WeaponReload_1(InputAction.CallbackContext context)
    {
        WeaponReload(context);

    }

    bool reloadButtonReleased_2 = true;
    float reloadButtonStartPressTime_2 = 0;
    public void WeaponReload_2(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {

            reloadButtonReleased_2 = false;
            StartCoroutine(PickUpWeaponTimer_2());
            reloadButtonStartPressTime_2 = Time.time;
        }

        if (context.canceled)
        {
            if (reloadButtonStartPressTime_2 + holdButtonToPickUpTime > Time.time)
            {
                playerArms.LeftArm.PressReloadButton();
            }
            reloadButtonReleased_2 = true;
        }
    }

    IEnumerator PickUpWeaponTimer_2()
    {
        yield return new WaitForSeconds(holdButtonToPickUpTime);
        if (!reloadButtonReleased_2)
        {

            if (playerArms.LeftArm.CanPickUpWeapon())
            {
                playerArms.LeftArm.TryPickUpWeapon();
            }
            else
            {
                

                playerArms.LeftArm.PressSwitchButton();
            }
        }
    }



    public void SwitchTeam(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameModeSelector.gameModeManager.PlayerSwitchTeams(this);
        }
    }

    public void SetLayers(int FPS_Layer, int ThirdPerson_Layer)
    {
        firstPersonLayer = FPS_Layer;
        thirdPersonLayer = ThirdPerson_Layer;


        UpdateLayers();
    }

    public void EnableObjectiveUIMarker()
    {
        objectiveIndicatorUI.gameObject.SetActive(true);
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
        leftArmView.gameObject.SetActive(false);
        UIContainer.gameObject.SetActive(false);
        spectatorCamera.Priority = 100;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        leftArmView.gameObject.SetActive(true);
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
        shildUI.SetTeamColor(team);
    }


    TargetHitCollector hitCollector;
    public void ConnectPlayerElimination(TargetHitCollector hitCollector)
    {
        if (this.hitCollector != null)
        {
            this.hitCollector.OnCharacterKill -= PlayerElimination;

            this.hitCollector.OnCharacterHit -= hitMarkerUI.ShowHitMarker;
            this.hitCollector.OnCharacterKill -= hitMarkerUI.ShowKillMarker;

        }

        this.hitCollector = hitCollector;

        hitCollector.OnCharacterKill += PlayerElimination;
        hitCollector.OnCharacterHit += hitMarkerUI.ShowHitMarker;
        hitCollector.OnCharacterKill += hitMarkerUI.ShowKillMarker;
    }


}


