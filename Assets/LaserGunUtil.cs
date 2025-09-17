using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

public class LaserGunUtil : MonoBehaviour
{
    [SerializeField] Weapon_Visual weaponVisual;

    public RumbleData rumble_Charge;


	[SerializeField] EventReference chargeSound;
    EventInstance chargeSoundInstance;

	[SerializeField] EventReference shotSound;
	EventInstance shotSoundInstance;


    bool isCharging = false;
    bool isShooting = false;


	// start charge sound
    public void StartChargeSound()
    {
        if (isCharging) return;
        chargeSoundInstance = RuntimeManager.CreateInstance(chargeSound);
        chargeSoundInstance.start();
        isCharging = true;
	}

    // stop charge sound
    public void StopChargeSound()
    {
        if (!isCharging) return;
        chargeSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        chargeSoundInstance.release();
        isCharging = false;
	}

	// start shot sound
    public void StartShotSound()
    {
        if (isShooting) return;
        shotSoundInstance = RuntimeManager.CreateInstance(shotSound);
        shotSoundInstance.start();
        isShooting = true;
	}

    // stop shot sound
    public void StopShotSound()
    {
        if (!isShooting) return;
        shotSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        shotSoundInstance.release();
        isShooting = false;
	}




	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

        weaponVisual.OnChargeStartAction += StartChargeSound;
        weaponVisual.OnChargeEndAction += StopChargeSound;
        weaponVisual.OnShootAction += StartShotSound;
        weaponVisual.OnShootStopAction += StopShotSound;


	}

    // Update is called once per frame
    void Update()
    {

		// update positions of sounds to match the game object
        if (isCharging)
        {
            chargeSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
		}
        if (isShooting)
        {
            shotSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
		}

	}
}
