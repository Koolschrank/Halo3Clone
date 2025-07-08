using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


public class PlayerAim : MonoBehaviour
{

    
    public Action<Vector2> OnAimUpdated;
    public Action<float, float> OnSensitivityMultiplierChanged;

    [Header("References")]
    [SerializeField] GameObject playerHead;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] CharacterHealth playerHealth;
    [SerializeField] PlayerMovement playerMovement;


    [Header("Settings")]
    [SerializeField] float aimSpeed_x = 10f;
    [SerializeField] float aimSpeed_y = 10f;
    [SerializeField] float minAngle = -70f;
    [SerializeField] float maxAngle = 70f;
    [SerializeField] float zoomAimSpeedMultiplier = 0.5f;

    [Header("Aim support Settings")]
    [SerializeField] float aimSupportDistance = 10f;
    [SerializeField] LayerMask aimSupportLayerMask;
    [SerializeField] float aimSupportSlowDown = 0.5f;

	[Header("Aim Correction Settings")]
    [SerializeField] bool hasAimCorrection = true;
	[SerializeField] float aimCorrectionDistance = 10f;
    [SerializeField] float aimCorrectionWidth = 10f;
    [SerializeField] float aimCorrectionMaxAngle = 45f;
	[SerializeField] float aimCorrectionMaxAngleForAutoCorrection = 10f;

	[SerializeField] float aimCorrectionDeadzoneAngle = 1f;
	[SerializeField] LayerMask aimCorrectionPlayerLayer;
	[SerializeField] LayerMask aimCorrectionWallLayer;
	[SerializeField] float aimCorrectionSpeed_Idle = 7.5f;
	[SerializeField] float aimCorrectionSpeed_Aim = 9f;



	Vector2 aimInput = Vector2.zero;

    
    float sensitivityMultiplier = 1f;
    [Header("Sensitivity Settings")]
    [SerializeField] float minSensitivity = 0.30f;
    [SerializeField] float maxSensitivity = 2f;
    
    [SerializeField] float sensitivityChangeSteps = 0.10f;

    [SerializeField] bool inputSlowDownStopsInput;

    List<GunKnockbackInstance> gunKnockbackInstances = new List<GunKnockbackInstance>();

    bool inputSlowDown = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

         
        // bug fix so that player stps rotating when dead
        //playerHealth.OnDeath += () =>
        //{
        //    sensitivityMultiplier = 0;
        //};

        playerMovement.OnRollStarted += (direction, duration) =>
        {
            BlockInput();
        };
        playerMovement.OnRollEnded += UnblockInput;
    }

    public void BlockInput()
    {
        aimInput = Vector2.zero;
        inputSlowDown = true;
        OnAimUpdated?.Invoke(aimInput);
    }

    public void UnblockInput()
    {
        inputSlowDown = false;
    }

    void Update()
    {
        UpdateAim();
        UpdateGunKnockbacks();
    }

    public void ForceRotation(Vector3 lookRotation)
    {
        // Force rotation of the player and head
        transform.eulerAngles = new Vector3(0, lookRotation.y, 0);
        playerHead.transform.eulerAngles = new Vector3(lookRotation.x, lookRotation.y, 0);
    }

    private void UpdateAim()
    {
        // x rotates player y rotates camera
        Vector2 input = aimInput; //controller.Player.Aim.ReadValue<Vector2>();
        float rotationX = input.x * aimSpeed_x * sensitivityMultiplier * Time.deltaTime;
        float rotationY = input.y * aimSpeed_y * sensitivityMultiplier * Time.deltaTime;

        float playerXRotation = transform.eulerAngles.y;
        float playerYRotation = playerHead.transform.eulerAngles.x;

        if (CheckIfHoverOverEnemy())
        {
            rotationX *= aimSupportSlowDown;
            rotationY *= aimSupportSlowDown;
        }
        if (playerArms.RightArm.IsInZoom)
        {
            rotationX *= zoomAimSpeedMultiplier;
            rotationY *= zoomAimSpeedMultiplier;
        }

        if (hasAimCorrection && playerArms.RightArm.CurrentWeapon != null)
        {
            var autoAimType = playerArms.RightArm.CurrentWeapon.Data.AutoAimType;
            if (autoAimType != AutoAimType.none)
            {
				UpdateAimCorrection();
				var closestPlayer = GetPlayerInAimCorrectionWithClosesAngle(autoAimType);
				var aimCorrectionInput = AimCorrectTowardsTarget(closestPlayer);

				if (aimCorrectionInput != Vector2.zero)
				{
					float aimCorrectionMultiplier = 1f;
                    float aimCorrectionSpeed = 0f;
					Vector3 hitDirection = (closestPlayer.transform.position - playerHead.transform.position).normalized;
					float angle = Vector3.Angle(playerHead.transform.forward, hitDirection);

					if (angle < aimCorrectionDeadzoneAngle)
					{
						aimCorrectionMultiplier *= angle / aimCorrectionDeadzoneAngle;
					}

					if ( angle < aimCorrectionMaxAngleForAutoCorrection)
					{
                        aimCorrectionSpeed += aimCorrectionSpeed_Idle;
					}

                    if (playerArms.RightArm.IsInZoom)
                    {
						aimCorrectionMultiplier *= zoomAimSpeedMultiplier;
                    }


					var inputMagnitude = input.magnitude;
                    var directionOverlap = Vector2.Dot(input.normalized, aimCorrectionInput.normalized);
                    //Debug.Log(inputMagnitude + " / " + directionOverlap);
                    if (directionOverlap>0)
                        aimCorrectionSpeed += aimCorrectionSpeed_Aim * inputMagnitude;// * directionOverlap;
					rotationX += aimCorrectionInput.x * aimCorrectionSpeed * Time.deltaTime * aimCorrectionMultiplier;
					rotationY += aimCorrectionInput.y * aimCorrectionSpeed * Time.deltaTime * aimCorrectionMultiplier;
				}
			}
		}

        playerXRotation += rotationX;
        playerYRotation -= rotationY;
        //playerYRotation = Mathf.Clamp(playerYRotation, minAngle, maxAngle);
        


        transform.eulerAngles = new Vector3(0, playerXRotation, 0);
        playerHead.transform.eulerAngles = new Vector3(playerYRotation, playerXRotation, 0);

    }

    List<GameObject> playersInAimCorrection = new List<GameObject>();
    public void UpdateAimCorrection()
    {
        playersInAimCorrection.Clear();

        // box cast in front of the player
        Vector3 position = playerHead.transform.position; //+ playerHead.transform.forward * aimCorrectionDistance;
        Vector3 direction = playerHead.transform.forward;
        Vector3 size = new Vector3(aimCorrectionWidth, aimCorrectionWidth, 0.1f);
        var forwardRotation = Quaternion.LookRotation(direction);
		RaycastHit[] hits = Physics.BoxCastAll(position, size / 2, direction, forwardRotation, aimCorrectionDistance, aimCorrectionPlayerLayer);
        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.GetComponent<PlayerTeam>().TeamIndex == playerTeam.TeamIndex) continue;


			// get angle from player forward towards hit point
            Vector3 hitDirection = (hit.point - playerHead.transform.position).normalized;
            float angle = Vector3.Angle(playerHead.transform.forward, hitDirection);
            // if the angle is greater than the max angle, skip this hit
            if (angle > aimCorrectionMaxAngle) continue;
;

			// check if wall is between player and hit point
			RaycastHit wallHit;
            float hitDistance = Vector3.Distance(playerHead.transform.position, hit.point);

			if (Physics.Raycast(playerHead.transform.position, hitDirection, out wallHit, hitDistance, aimCorrectionWallLayer))
            {
                continue;
			}

			Debug.Log("aim 3");

			if (hit.collider.TryGetComponent<BodyMindConnection>(out BodyMindConnection body))
            {
                playersInAimCorrection.Add(body.gameObject);
            }
        }
        
	}

    public Transform GetPlayerInAimCorrectionWithClosesAngle(AutoAimType autoAimType)
    {
        if (playersInAimCorrection.Count == 0) return null;
        Transform closestPlayer = null;
        float closestAngle = float.MaxValue;
        foreach (var player in playersInAimCorrection)
        {
            Vector3 hitDirection = (player.transform.position - playerHead.transform.position).normalized;
            float angle = Vector3.Angle(playerHead.transform.forward, hitDirection);
            if (angle < closestAngle)
            {
                closestAngle = angle;
                closestPlayer = player.transform;
            }
        }

        if (autoAimType == AutoAimType.followHead)
        {
            // If the auto aim type is follow head, we need to return the head of the player
            if (closestPlayer != null)
            {
                var bodyMind = closestPlayer.GetComponent<BodyMindConnection>();
                if (bodyMind != null)
                {
                    closestPlayer = bodyMind.GetPlayerHead().transform;
				}
			}
		}

        return closestPlayer;
	}

    public Vector2 AimCorrectTowardsTarget(Transform target)
    {
        if (target == null) return Vector2.zero;

		// You have these:
		Vector3 playerPosition = playerHead.transform.position;
		Vector3 playerForward = playerHead.transform.forward;
		Vector3 playerRight = playerHead.transform.right;
		Vector3 playerUp = playerHead.transform.up;

		Vector3 enemyPosition = target.position;

		// Direction from player to enemy (in world space)
		Vector3 toEnemy = (enemyPosition - playerPosition).normalized;

		// Project that onto the player's view plane
		float x = Vector3.Dot(toEnemy, playerRight);   // Left/right
		float y = Vector3.Dot(toEnemy, playerUp);      // Up/down

		Vector2 aimDirection = new Vector2(x, y).normalized;
		return aimDirection;
	}


	public void AddGunKnockback(GunKnockback gunKnockback)
    {
        GunKnockbackInstance instance = new GunKnockbackInstance(gunKnockback);
        gunKnockbackInstances.Add(instance);
    }

    void UpdateGunKnockbacks()
    {
        for (int i = gunKnockbackInstances.Count - 1; i >= 0; i--)
        {
            GunKnockbackInstance instance = gunKnockbackInstances[i];
            float knockback = instance.Update(Time.deltaTime);
            float playerYRotation = playerHead.transform.eulerAngles.x;
            float playerXRotation = transform.eulerAngles.y;
            playerYRotation -= knockback;

            playerHead.transform.eulerAngles = new Vector3(playerYRotation, playerXRotation, 0);




            if (instance.IsFinished())
            {
                gunKnockbackInstances.RemoveAt(i);
            }
        }

    }

    public Action<String> OnHoverOnEnemy;
    public Action<String> OnHoverOnAlly;
    public Action OnHoverOnNothing;
    public bool OnTarget;

    public bool CheckIfHoverOverEnemy()
    {
        OnTarget = false;
        Ray ray = new Ray(playerHead.transform.position, playerHead.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, aimSupportDistance, aimSupportLayerMask))
        {
            // check if hit has health component
            bool hasHitPlayer = hit.collider.TryGetComponent<PlayerTeam>(out PlayerTeam t);

            if (hasHitPlayer && t.TeamIndex != playerTeam.TeamIndex)
            {
                var mind = hit.collider.GetComponent<BodyMindConnection>().Mind;
                if (mind != null)
                {
                    
                    OnHoverOnEnemy?.Invoke(mind.playerSettings.playerName);
                }
                if (playerArms.RightArm.GetWeaponInHand() != null)
                    OnTarget = (hit.distance <= playerArms.RightArm.GetWeaponInHand().Data.AutoAim.RaycastLenght);
                return true;
            }
            else if (hasHitPlayer && t.TeamIndex == playerTeam.TeamIndex)
            {

                var mind = hit.collider.GetComponent<BodyMindConnection>().Mind;
                if (mind != null)
                {
                    OnHoverOnAlly?.Invoke(mind.playerSettings.playerName);
                }
                
                return false;
            }

            OnHoverOnNothing?.Invoke();


            return false;
        }
        OnHoverOnNothing?.Invoke();

        return false;
    }

   

    public void UpdateAimInput(Vector2 input)
    {
        if (inputSlowDown)
        {
            input = input/2; // slow down input when rolling
            if (inputSlowDownStopsInput)
            {
                input = Vector2.zero; // stop input when rolling
            }
        }
        aimInput = input;
        OnAimUpdated?.Invoke(input);
    }

    public void AddSensetivity()
    {
        var sensitivityChange = sensitivityChangeSteps;
        if (sensitivityMultiplier < 0.49999)
        {
            sensitivityChange /= 2;
        }

        AddToSensetivityMultiplier(sensitivityChange);
    }

    public void ReduceSensetivity()
    {
        var sensitivityChange = sensitivityChangeSteps;
        if (sensitivityMultiplier < 0.49999)
        {
            sensitivityChange /= 2;
        }
        AddToSensetivityMultiplier(-sensitivityChange);
    }

    public void SetSensetivityWithNoActionSent(float sensetivity)
    {
        sensitivityMultiplier = sensetivity;
        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, minSensitivity, maxSensitivity);
    }


    public void AddToSensetivityMultiplier(float addedValue)
    {
        sensitivityMultiplier += addedValue;
        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, minSensitivity, maxSensitivity);
        float percentage = (sensitivityMultiplier - minSensitivity) / (maxSensitivity - minSensitivity);
        OnSensitivityMultiplierChanged?.Invoke(sensitivityMultiplier, percentage);
    }
}


public class GunKnockbackInstance
{
    GunKnockback gunKnockback;
    float timer = 0;

    public GunKnockbackInstance(GunKnockback gunKnockback)
    {
        this.gunKnockback = gunKnockback;
        timer = gunKnockback.Duration;
    }

    public float Update(float deltaTime)
    {
        float lastTimer = timer;
        timer -= deltaTime;
        if (timer < 0)
        {
            timer = 0;
        }

        float lastFrame = Mathf.Clamp01(1- (lastTimer / gunKnockback.Duration));
        float thisFrame = Mathf.Clamp01(1 - (timer / gunKnockback.Duration));


        return gunKnockback.GetKnockback(lastFrame, thisFrame);

    }

    public bool IsFinished()
    {
        return timer <= 0;
    }
}



[Serializable]
public class GunKnockback
{
    [SerializeField] float power;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;

    public float Duration => duration;

    public float GetKnockback(float lastFrame, float thisFrame)
    {
        float lastFrameValue = curve.Evaluate(lastFrame);
        float thisFrameValue = curve.Evaluate(thisFrame);
        float delta = thisFrameValue - lastFrameValue;
        return delta * power;
    }

}
