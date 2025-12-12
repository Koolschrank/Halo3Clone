using System;
using UnityEngine;

public class Arm_FPSView : MonoBehaviour
{
    [SerializeField] PlayerMind playerMind;
	[SerializeField] Arm playerArm;
    [SerializeField] Transform granadeSpawnPoint;
    [SerializeField] WeaponSway weaponSway;

	[SerializeField] Transform baseArm;
	[SerializeField] Transform aimArm;
    [SerializeField] float aimSpeed;
	[SerializeField] float aimRotationSpeed;
    [SerializeField] float goOutOfAimMultiplier = 2f;
	Weapon_Visual weaponVisual;
    [SerializeField] bool ignoreAim;
    [SerializeField] float swayStrenghtWhenAim = 0.3f;
    [SerializeField] bool autoAim;
    [SerializeField] bool reverseX;

    [SerializeField] FPS_Arms fps_Arms;

	bool hasAimPosition;
    bool inAim;

	private void Update()
	{
		UpdateAim();
	}

	public void SetUp(Arm newArm)
    {
        if (weaponVisual != null)
            RemoveWeapon(null);


        if (playerArm != null)
        {
            playerArm.OnWeaponEquipStarted -= EquipWeapon;
            playerArm.OnWeaponUnequipFinished -= RemoveWeapon;
            playerArm.OnWeaponDroped -= (weapon,pickUp)  => RemoveWeapon(weapon);
            playerArm.OnGranadeThrowStarted -= ThrowGranadeStart;
            playerArm.OnGranadeThrow -= ThrowGranade;
			playerArm.OnZoomIn -= (weapon) => { inAim = true; };
			playerArm.OnZoomOut -= (weapon) => { inAim = false; };
		}



        playerArm = newArm;

        playerArm.OnWeaponEquipStarted += EquipWeapon;
        playerArm.OnWeaponUnequipFinished += RemoveWeapon;
        playerArm.OnWeaponDroped += (weapon, pickUp) => RemoveWeapon(weapon);
        playerArm.OnGranadeThrowStarted += ThrowGranadeStart;
        playerArm.OnGranadeThrow += ThrowGranade;
        playerArm.OnZoomIn += (weapon) => { inAim = true; };
        playerArm.OnZoomOut += (weapon) => { inAim = false; };



		if (playerArm.CurrentWeapon != null)
        {
            EquipWeapon(playerArm.CurrentWeapon);
        }
    }


    public void EquipWeapon(Weapon_Arms weapon, float time)
    {
        EquipWeapon(weapon);
    }

    public void EquipWeapon(Weapon_Arms weapon)
    {
        if (weaponVisual != null)
        {
            Destroy(weaponVisual.gameObject);
        }
		baseArm.localPosition = Vector3.zero;
		weaponVisual = Instantiate(weapon.WeaponFPSModel, baseArm.transform);
        weaponVisual.SetUp(weapon);
        weaponVisual.PlayerID = playerMind.playerID;

		UtilityFunctions.SetLayerRecursively(weaponVisual.gameObject, gameObject.layer);

        hasAimPosition = weaponVisual.HasAimPosition;
        if (hasAimPosition)
        {
			aimArm.transform.localPosition = weaponVisual.AimPosition.localPosition;
            aimArm.transform.localRotation = weaponVisual.AimPosition.localRotation;

            if (reverseX)
            {
                var p = aimArm.transform.localPosition;
                var r = aimArm.transform.localRotation;

                aimArm.transform.position = new Vector3(-p.x, p.y, p.z);
				aimArm.transform.localRotation = new Quaternion(r.x, -r.y, -r.z, r.w);

			}
		}


        if (weaponVisual.rightArmAnker != null)
            fps_Arms.SetRightAnker(weaponVisual.rightArmAnker);
        if (weaponVisual.leftArmAnker != null)
            fps_Arms.SetLeftAnker(weaponVisual.leftArmAnker);


	}

    public void RemoveWeapon(Weapon_Arms weapon_Arms)
    {
        if (weaponVisual == null) return;
        Destroy(weaponVisual.gameObject);
    }


    public void ThrowGranadeStart(GranadeStats stats, float time)
    {
        if (weaponVisual == null) return;
        weaponVisual.ThrowGranadeStart(stats, time);
    }

    public void ThrowGranade(GameObject granade, GranadeStats stats)
    {
        var clone = Instantiate(stats.GranadeClonePrefab, granadeSpawnPoint.position, granade.transform.rotation);
        var granadeScript = granade.GetComponent<Granade>();
        granadeScript.AddGranadeCopy(clone.transform);

        UtilityFunctions.SetLayerRecursively(clone, gameObject.layer);


    }

    public void UpdateAim()
    {
        if (ignoreAim || !hasAimPosition) return;
        if (!weaponVisual) return;


        if (!ignoreAim && hasAimPosition && (inAim || autoAim))
        {
			baseArm.localPosition = Vector3.MoveTowards(baseArm.localPosition, aimArm.localPosition, Time.deltaTime * aimSpeed);
			baseArm.localRotation = Quaternion.RotateTowards(baseArm.localRotation, aimArm.localRotation, Time.deltaTime * aimRotationSpeed);
			weaponSway.strenght = swayStrenghtWhenAim;

		}
        else
        {
			baseArm.localPosition = Vector3.MoveTowards(baseArm.localPosition, Vector3.zero, Time.deltaTime * aimSpeed * goOutOfAimMultiplier);

			baseArm.localRotation = Quaternion.RotateTowards(baseArm.localRotation, Quaternion.identity, Time.deltaTime * aimRotationSpeed * goOutOfAimMultiplier);
			weaponSway.strenght = 1;
		}
    }

    
}
