using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] PlayerUpgrades playerUpgrader;
    [SerializeField] PlayerMindStatSheet playerMindStatSheet;
    [SerializeField] bool hasKillRumble;
	[SerializeField] RumbleData killRumble;

    [Header("UI")]
    [SerializeField] Transform UIContainer;
	[SerializeField] Transform UIContainer_death;
	[SerializeField] HealthUI healthUI;
    [SerializeField] ShildUI shildUI;
	[SerializeField] ArmorUI armorUI;
	[SerializeField] WeaponUI weaponUI_RightArm;
    [SerializeField] WeaponUI weaponUI_LeftArm;
    [SerializeField] WeaponInventoryUI weaponInventoryUI;
    [SerializeField] PickUpUI pickUpUI;
    [SerializeField] DamageIndicatorUI damageIndicatorUI;
    [SerializeField] crosshairUI crosshairUI;
	[SerializeField] crosshairUI crosshairUI2;
	[SerializeField] CooldownUISystem cooldownSystem;
    [SerializeField] TeamWinUI teamWinUI;
    [SerializeField] HitMarkerUI hitMarkerUI;
    [SerializeField] MinimapUI minimapUI;
    [SerializeField] ObjectiveIndicatorUI[] objectiveIndicatorUIs;
    [SerializeField] TextMeshProUGUI crownText;
    [SerializeField] GameLogUI gameLogUI;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] PlayerNamePopUp playerNamePopUp;
    [SerializeField] UI_Score scoreUI;
    [SerializeField] UI_UpgradeMenu upgradeMenu;
    [SerializeField] UI_ReviveBar reviveBar;


	[Header("UI Settings Menu")]
    [SerializeField] SettingsQuickMenu settingsQuickMenu;
    [SerializeField] SensitivitySlider sensitivitySlider;


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
    AbilityInventory abilityInventory;
    public PlayerSettings playerSettings { get; private set; }

    public PlayerUpgrades PlayerUpgrades { get { return playerUpgrader; } }

    int firstPersonLayer;
    int thirdPersonLayer;

    public int score = 0;

    public Action<int> OnScoreChanged;
    public Action<int> OnScoreAdded;
    public Action<int> OnScoreLost;
    bool isDead = false;

    public bool IsDead { get { return isDead; } }

    public PlayerMindStatSheet PlayerMindStatSheet { get { return playerMindStatSheet; } }

    [NonSerialized]
	public int playerID = 0;


    [NonSerialized]
    public bool inRespawn = false;
	[NonSerialized]
	public float respawnTimer = 0;
	[NonSerialized]
	public float respawnTime = 0;

    public Action<float> OnTokenUseUpdate;
    public float respawnTokenUseTime = 2f;
    public float reduceTokenUseTime = 0.5f;
	float tokenButtonPressTime = 0;
    bool tokenButtonPressedDown = false;


	public void SetAlive()
    {
        isDead = false;
    }


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
		playerID = RumbleManager.Instance.OnPlayerMindJoined(playerInput);



		GameModeSelector.gameModeManager.OnTeamWon += teamWinUI.TeamWon;

        string deviceName = playerInput.devices[0].displayName + " " + playerInput.devices[0].deviceId;
        Debug.Log(deviceName + " joined");
        playerSettings = SettingsSave.instance.GetPlayerSettings(deviceName);
        playerName.text = playerSettings.playerName;

        var gamemode = GameModeSelector.gameModeManager;
        if (gamemode.GameModeStats.UseStatSheet)
        {
            playerMindStatSheet.SetStatSheet(gamemode.GameModeStats.PlayerStatSheet);
        }

            PlayerManager.instance.AddPlayer(this);

        playerInput.actions.FindActionMap("QuickMenu").Enable();


        LogSystem.logSystem.OnLogPrinted += gameLogUI.Print;

        score += GameModeSelector.gameModeManager.GameModeStats.StartScore;


        OnScoreAdded += scoreUI.SpawnScoreGain;
        OnScoreChanged += scoreUI.UpdateScore;
        if (score > 0)
        {
            scoreUI.UpdateScore(score);
        }

        if (gameObject.GetComponent<PlayerInput>().currentControlScheme == "KeyboardAndMouse")
        {
            pickUpUI.SetKeyboard();
        }
    }

    public Action<float> OnRespawnUpdate;

	private void Update()
	{
        if (inRespawn)
        {
            respawnTimer -= Time.deltaTime;
            OnRespawnUpdate?.Invoke(1 - (respawnTimer / respawnTime));

            if (respawnTimer <= 0)
            {
                Respawn();
            }


            if (tokenButtonPressedDown && GameModeSelector.gameModeManager.RespawnTokensLeft > 0)
            {
                tokenButtonPressTime += Time.deltaTime;
                
                if (tokenButtonPressTime >= respawnTokenUseTime)
                {
                    tokenButtonPressedDown = false;
                    Respawn();
                    GameModeSelector.gameModeManager.UseRespawnToken();
                    tokenButtonPressTime = 0;
				}
				OnTokenUseUpdate?.Invoke(tokenButtonPressTime / respawnTokenUseTime);
			}
            else if (tokenButtonPressTime > 0)
            {
				tokenButtonPressTime -= Time.deltaTime * reduceTokenUseTime;
                if (tokenButtonPressTime <= 0)
                {
                    tokenButtonPressTime = 0;
					OnTokenUseUpdate?.Invoke(0);
				}
                else
                {
					OnTokenUseUpdate?.Invoke(tokenButtonPressTime / respawnTokenUseTime);
				}

			}
        }
	}

	public int PlayerIndex { get { return playerSettings.playerIndex; } }

    public void SetPlayerBody(GameObject body)
    {
        playerBody = body;
        playerUpgrader.AssignBody(body);

        

	}

    public void ApplyUpgrades()
    {
        playerUpgrader.ApplyAllUpgradesOnBody(playerBody);
    }

    public void CrownCollected()
    {
        crownText.gameObject.SetActive(true);
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

    public void SetPlayerReviver(PlayerReviver playerReviver)
    {
		if (playerReviver != null)
		{
			reviveBar.SetUp(playerReviver);
		}
	}

    public void SetSpectatorTarget(CinemachineCamera camera)
    {
        spectatorCamera = camera;
    }

    public void SetPlayerInventory(PlayerInventory inventory)
    {
        if (playerInventory != null)
        {
            playerInventory.OnMiniMapDisabled -= minimapUI.DisableMiniMap;
            playerInventory.OnMiniMapEnabled -= minimapUI.EnableMiniMap;
        }

        playerInventory = inventory;
        inventory.OnMiniMapDisabled += minimapUI.DisableMiniMap;
        inventory.OnMiniMapEnabled += minimapUI.EnableMiniMap;


        weaponInventoryUI.SetUp(playerInventory);
    }

    public void SetInteractable(PlayerInteractableTrigger interactableTrigger)
    {
        pickUpUI.SetUp(interactableTrigger);
    }

    public void SetAbilityInventory(AbilityInventory inventory)
    {
        abilityInventory = inventory;
        cooldownSystem.Setup(inventory, this);
    }

	
    public void SetCinemaCamera(CinemachineCamera cCam)
    {
        playerCamera.SetCinemachineCamera(cCam);
		

	}

    



    // set aim
    public void SetPlayerAim(PlayerAim aim)
    {
        playerAim = aim;

        aim.OnSensitivityMultiplierChanged += sensitivitySlider.UpdateValues;

        playerNamePopUp.SetUp(aim);

        weaponSway1.SetUp(aim);
        weaponSway2.SetUp(aim);
    }

    // set arms
    public void SetPlayerArms(PlayerArms arms)
    {
        if (playerArms != null)
        {
            playerArms.RightArm.OnZoomIn -= playerCamera.ZoomIn;
            playerArms.RightArm.OnZoomOut -= playerCamera.ZoomOut;
			arms.LeftArm.OnZoomIn -= playerCamera.ZoomIn;
			arms.LeftArm.OnZoomOut -= playerCamera.ZoomOut;
			playerArms.OnDualWieldingEntered -= EnterDualWeaponMode;
            playerArms.OnDualWieldingExited -= EnterOneWeaponMode;
            playerArms.OnDualWieldingExited -= weaponUI_LeftArm.Disable;

            playerArms.RightArm.OnWeaponEquipStarted -= (weapon, time) => crosshairUI.ChangeSprite(weapon);
			playerArms.LeftArm.OnWeaponEquipStarted -= (weapon, time) => crosshairUI2.ChangeSprite(weapon);

            playerArms.RightArm.OnWeaponDroped -= (weapon, pickUp) =>
            {
                crosshairUI.DisableCrosshair();
            };
            playerArms.LeftArm.OnWeaponDroped -= (weapon, pickUp) =>
            {
                crosshairUI2.DisableCrosshair();
            };

		}


        playerArms = arms;
        rightArmView.SetUp(arms.RightArm);
        leftArmView.SetUp(arms.LeftArm);
        weaponUI_RightArm.SetUp(arms.RightArm);
        weaponUI_LeftArm.SetUp(arms.LeftArm);
        arms.OnDualWieldingExited += weaponUI_LeftArm.Disable;
        arms.RightArm.OnZoomIn += playerCamera.ZoomIn;
        arms.RightArm.OnZoomOut += playerCamera.ZoomOut;
		arms.LeftArm.OnZoomIn += playerCamera.ZoomIn;
		arms.LeftArm.OnZoomOut += playerCamera.ZoomOut;


		arms.OnDualWieldingEntered += EnterDualWeaponMode;
        arms.OnDualWieldingExited += EnterOneWeaponMode;

        arms.RightArm.OnWeaponEquipStarted += (weapon, time) => crosshairUI.ChangeSprite(weapon);
		arms.LeftArm.OnWeaponEquipStarted += (weapon, time) => crosshairUI2.ChangeSprite(weapon);
        arms.RightArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            crosshairUI.DisableCrosshair();
        };
        arms.LeftArm.OnWeaponDroped += (weapon, pickUp) =>
        {
            crosshairUI2.DisableCrosshair();
        };

		if (arms.LeftArm.CurrentWeapon != null)
        {
            EnterDualWeaponMode();
        }
        else
        {
            EnterOneWeaponMode();
        }

       
    }

    public void SetPlayerBuffs(PlayerBuffs playerBuffs)
    {
        armorUI.ConnectBuffs(playerBuffs);
	}


    // set bullet spawner
    public void SetBulletSpawner(BulletSpawner spawner)
    {
        if (bulletSpawner != null)
        {
            bulletSpawner.OnTargetAcquired -= crosshairUI.OnTargetAcquired;
            bulletSpawner.OnTargetLost -= crosshairUI.OnTargetLost;
            bulletSpawner.OnTargetAcquired -= crosshairUI2.OnTargetAcquired;
            bulletSpawner.OnTargetLost -= crosshairUI2.OnTargetLost;
			crosshairUI.OnTargetLost(null);
        }


        bulletSpawner = spawner;

        bulletSpawner.OnTargetAcquired += crosshairUI.OnTargetAcquired;
        bulletSpawner.OnTargetLost += crosshairUI.OnTargetLost;
        bulletSpawner.OnTargetAcquired += crosshairUI2.OnTargetAcquired;
        bulletSpawner.OnTargetLost += crosshairUI2.OnTargetLost;
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
        armorUI.SetUp(playerHealth as CharacterHealth);
		// connect health on death unity event with this function
		playerHealth.OnDeath += PlayerDeath;

        playerHealth.OnDamageTaken += damageIndicatorUI.AddDamageIndicator;
        playerHealth.OnDeath += damageIndicatorUI.Clear;
    }

    public void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();
        OnPlayerDied?.Invoke(this);

        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("PlayerGunPlay_SingleWeapon").Disable();
        playerInput.actions.FindActionMap("PlayerGunPlay_DualWeapons").Disable();

        crownText.gameObject.SetActive(false);

        isDead = true;
        
        GameModeSelector.gameModeManager.PlayerDied(this);
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


            var mind = obj.GetComponent<BodyMindConnection>().Mind;
            if (mind != null)
            {
                LogSystem.logSystem.PlayerKilled(playerSettings.playerName,mind.playerSettings.playerName);

                if (hasKillRumble)
                {
                    int playerIndex = mind.playerID;
                    RumbleManager.Instance.TriggerRumble(killRumble, playerIndex);
				}
            }
            else
            {
				//LogSystem.logSystem.PlayerKilled(playerSettings.playerName, "Enemy");
			}
        }
        var score = obj.GetComponent<GainScore>();
        if (score != null && PlayerProgression.instance.canGainEXP)
        {
            AddScore(score.scoreAmount);
        }



    }

    public int Score => score;

    public void AddScore(int amount)
    {
        this.score += amount;
        OnScoreAdded?.Invoke(amount);
        OnScoreChanged?.Invoke(this.score);
        PlayerProgression.instance.GainEXP(amount);
    }

    public void LooseScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
        OnScoreLost?.Invoke(amount);
        OnScoreChanged?.Invoke(score);
    }



    // set pick up scan
    public void SetPickUpScan(PlayerPickUpScan pickUpScan)
    {
        playerPickUpScan = pickUpScan;
        pickUpUI.SetUp(pickUpScan);
    }

	public void RespawnTokenUse(InputAction.CallbackContext context)
	{
       tokenButtonPressedDown = context.performed;
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


        if (playerSettings != null)
        {
            playerAim.SetSensetivityWithNoActionSent(this.playerSettings.sensitivity);
            // connect to PlayerSettings value change it only takes in one float instead of two
            playerAim.OnSensitivityMultiplierChanged += (value, percent) => playerSettings.SetSensitivity(value);
        }
        else
        {
            // debug error
            Debug.LogError("PlayerSettings is null");

        }
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

    public void Roll(InputAction.CallbackContext context)
    {
        if (playerMovement == null) return;
        if (context.performed)
        {
            playerMovement.TryRoll();
        }
    }


    bool reloadButtonReleased = true;
    float reloadButtonStartPressTime = 0;
    public void WeaponReload(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {

            if (playerArms.RightArm.PressReloadButtonIfNothingToPickUp())
            {
                playerArms.LeftArm.PressReloadButtonIfNothingToPickUp();
            }
               
            

            reloadButtonReleased = false;
            StartCoroutine(PickUpWeaponTimer());
            reloadButtonStartPressTime = Time.time;
        }

        if (context.canceled )
        {
            if (reloadButtonStartPressTime + holdButtonToPickUpTime > Time.time)
            {
                playerArms.RightArm.PressReloadButton();
                playerArms.LeftArm.PressReloadButton();
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

	float pastScroll = 0;
	public void Scroll(InputAction.CallbackContext context)
	{
		float scrollValue = context.ReadValue<float>();
		if (scrollValue != pastScroll)
		{
			playerArms.RightArm.PressSwitchButton();
		}
        pastScroll = scrollValue;
	}


	public void WeaponSwitch(InputAction.CallbackContext context)
    {
        if (playerArms == null) return;
        if (context.performed)
        {
            if (playerArms.RightArm.CurrentWeapon == null)
            {
                playerArms.LeftArm.DropWeapon();
                playerArms.RightArm.PressSwitchButton();
                return;
            }


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
            if (playerArms.RightArm.CurrentWeapon != null)
				playerArms.RightArm.PressZoomButton();
            else 
                playerArms.LeftArm.PressZoomButton();

		}
        else if (context.canceled)
        {
			if (playerArms.RightArm.CurrentWeapon != null)
				playerArms.RightArm.ReleaseZoomButton();
			else
				playerArms.LeftArm.ReleaseZoomButton();
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

    public void AddToSensetivity(InputAction.CallbackContext context)
    {
        if (playerAim == null) return;
        if (context.performed)
        {
            playerAim.AddSensetivity();
        }

    }

	public void TogglePause(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			PauseSystem.instance.TogglePause();
		}
	}

	public void ReduceFromSensetivity(InputAction.CallbackContext context)
    {
        if (playerAim == null) return;
        if (context.performed)
        {
            playerAim.ReduceSensetivity();
        }
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
                playerArms.LeftArm.DropWeapon();
            }
            reloadButtonReleased_2 = true;
        }
    }

    public void ChangeName(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerSettings.SetRandomName();
            playerName.text = playerSettings.playerName;
            settingsQuickMenu.EnableMenu();
		}
    }

    public void ChangeNameAdvanced(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerSettings.SetRandomNameAdvanced();
            playerName.text = playerSettings.playerName;
			settingsQuickMenu.EnableMenu();
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
        for (int i = 0; i < objectiveIndicatorUIs.Length; i++)
        {
            objectiveIndicatorUIs[i].gameObject.SetActive(true);
        }

    }

    public void EnableObjectiveUIMarker(int index)
    {
        objectiveIndicatorUIs[index].gameObject.SetActive(true);
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

    public void DisableLayerInCamera(int layer)
    {
        playerCamera.DisableLayerInCamera(layer);
    }

    public void SetMesh(GameObject mesh)
    {
        this.mesh = mesh;
	}

    GameObject mesh;
    public void RespawnWithDelay()
    {
        inRespawn = true;
        respawnTimer = GameModeSelector.gameModeManager.RespawnTime;
        respawnTime = respawnTimer;
		SwitchToSpectatorCamera();

		playerInput.actions.FindActionMap("PlayerRespawn").Enable();

		//StartCoroutine(RespawnDelay(GameModeSelector.gameModeManager.RespawnTime));
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
		UIContainer_death.gameObject.SetActive(true);

		spectatorCamera.Priority = 100;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        leftArmView.gameObject.SetActive(true);
        UIContainer.gameObject.SetActive(true);

		UIContainer_death.gameObject.SetActive(false);
		spectatorCamera.Priority = 0;


    }
	//IEnumerator RespawnDelay(float delay)
	//{
	//    SwitchToSpectatorCamera();
	//    yield return new WaitForSeconds(delay);
	//    if (IsDead)
	//        Respawn();
	//}

	public void RevivePlayer(Vector3 spawnPoint)
    {
        Respawn();
        playerMovement.transform.position =spawnPoint;
	}

	public void Respawn()
    {
        if (!IsDead) return;
		inRespawn = false;
		if (mesh != null && GameModeSelector.gameModeManager.GameModeStats.removePlayerBodyWhenRespawned)
        {
            Debug.Log("Removing player body on respawn");
			mesh.SetActive(false);
		}

	    PlayerManager.instance.RespawnPlayer(this);
        SwitchToPlayerCamera();

		playerInput.actions.FindActionMap("PlayerRespawn").Disable();


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



    public Action OnUpgradeSelectionFinished;
    int[] upgradeIndexs;
    public void SetUpUpgradeMenu(int amountOfUpgrades)
    {

        upgradeMenu.gameObject.SetActive(true);

        upgradeIndexs = playerUpgrader.GetIndexOfRandomAbiliyNotEarnedYet(amountOfUpgrades);

        List<Upgrade> upgrades = new List<Upgrade>();
        for (int i = 0; i < upgradeIndexs.Length; i++)
        {
            upgrades.Add(playerUpgrader.GetUpgrade(upgradeIndexs[i]));
        }

        upgradeMenu.AddUpgradeBoxes(upgrades.ToArray());
        upgradeMenu.OnUpgradeSelected += UpgradeSelected;


        StartCoroutine(UpgradePickDelay());
        upgradeSelected = false;

    }


    IEnumerator UpgradePickDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        playerInput.actions.FindActionMap("UpgradeSelection").Enable();
    }

    bool upgradeSelected = false;

    public void UpgradeSelected(int index)
    {

        upgradeMenu.OnUpgradeSelected -= UpgradeSelected;
        upgradeSelected = true;
        int upgradeIndex = upgradeIndexs[index];
        playerUpgrader.Upgrade(upgradeIndex);
        OnUpgradeSelectionFinished?.Invoke();

        playerInput.actions.FindActionMap("UpgradeSelection").Disable();
        upgradeMenu.gameObject.SetActive(false);
    }

    public void Upgrade_1(InputAction.CallbackContext context)
    {
        if (context.performed&& !upgradeSelected)
        {
            upgradeMenu.Select1();
        }
    }

    

	public void Upgrade_2(InputAction.CallbackContext context)
    {
        if (context.performed && !upgradeSelected)
        {
            upgradeMenu.Select2();
        }
    }

    public void Upgrade_3(InputAction.CallbackContext context)
    {
        if (context.performed && !upgradeSelected)
        {
            upgradeMenu.Select3();
        }
    }






}


