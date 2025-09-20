using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] BodyMindConnection bodyMindConnection; // reference to the body mind connection for rumble
	[SerializeField] PlayerArms playerArms;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] TargetHitCollector targetHitCollector;
    TimedSoundListInstance reloadList;
	TimedSoundListInstance reloadListLeft;
	TimedSoundListInstance switchInList;

    [SerializeField] EventReference hitSound;
    [SerializeField] EventReference killSound;


    public void Start()
    {
        playerArms.RightArm.OnWeaponReloadStarted += Reload;
        playerArms.RightArm.OnWeaponEquipStarted += SwitchIn;
        playerArms.RightArm.OnWeaponShoot += Shoot;

        playerArms.LeftArm.OnWeaponReloadStarted += ReloadLeft;
        playerArms.LeftArm.OnWeaponEquipStarted += SwitchIn;
        playerArms.LeftArm.OnWeaponShoot += Shoot;
        playerArms.RightArm.OnReloadCanceld += CancelReload;
        playerArms.LeftArm.OnReloadCanceld += CancelReloadLeft;

		meleeAttacker.OnAttackStart += MeleeSwing;
        meleeAttacker.OnAttackHit += MeleeHit;

        targetHitCollector.OnCharacterHit += HitTarget;
        targetHitCollector.OnCharacterKill += KillTarget;
    }

    public void CancelReload()
    {
        reloadList = null;
	}

    public void CancelReloadLeft()
    {
        reloadListLeft = null;
    }


	public void HitTarget(GameObject target)
    {
        if (target.tag == "AIEnemy")
        {

            return;
        }
            

        AudioManager.instance.PlayOneShot(hitSound, transform.position);
    }

    public void KillTarget(GameObject target)
    {
        if (target.tag == "AIEnemy")
        {
            AudioManager.instance.PlayOneShot(hitSound, transform.position);
            return;
        }

        AudioManager.instance.PlayOneShot(killSound, transform.position);
    }

    public void MeleeSwing(PlayerMeleeAttack melee)
    {
        AudioManager.instance.PlayOneShot(melee.SwingSound, transform.position);
    }

    public void MeleeHit(PlayerMeleeAttack melee)
    {
        AudioManager.instance.PlayOneShot(melee.HitSound, transform.position);
    }

    public void Shoot(Weapon_Arms weapon)
    {
        if (weapon == null) return;
		AudioManager.instance.PlayOneShot(weapon.ShootSound, transform.position);

    }

    public void SwitchIn(Weapon_Arms weapon, float timer)
    {

		if (weapon == null) return;
		switchInList = new TimedSoundListInstance(weapon.SwitchInSound, timer);
    }

    public void Reload(Weapon_Arms weapon, float time)
    {

		if (weapon == null) return;
		reloadList = new TimedSoundListInstance( weapon.ReloadSounds, time);


    }

    public void ReloadLeft(Weapon_Arms weapon, float time)
    {
        reloadListLeft = new TimedSoundListInstance(weapon.ReloadSounds, time);
	}

	public void Update()
    {
        UpdateSoundList(reloadList);

		UpdateSoundList(reloadListLeft);
		UpdateSoundList(switchInList);

	}

    public void UpdateSoundList(TimedSoundListInstance soundList)
    {
        if (soundList == null) return;
        if (!soundList.IsFinished())
        {
            soundList.Update(Time.deltaTime);
            if (soundList.IsTimeToPlay())
            {

				if (bodyMindConnection.Mind != null && soundList.HasNextSoundRumble())
                {
                    var rumbleData = soundList.GetNextRumble();
					var playerIndex = bodyMindConnection.Mind.playerID;
                    RumbleManager.Instance.TriggerRumble(rumbleData, playerIndex);
				}

                var nextSound = soundList.GetNextSound();

                if (nextSound.IsNull) return; // skip if sound is null
				AudioManager.instance.PlayOneShot(nextSound, transform.position);
            }
        }
    }
}



[Serializable]
public class TimedSoundList
{
    [SerializeField] TimedSound[] timedSounds;
    public TimedSound[] TimedSounds => timedSounds;
}

public class TimedSoundListInstance
{
	List<TimedSound> timedSounds;
    float timeToFinish;
    float timer;

    public TimedSoundListInstance(float timer)
    {
        timedSounds = new List<TimedSound>();
        timeToFinish = 0f;
        this.timer = timer;
    }

    public TimedSoundListInstance(TimedSoundList soundList, float timeToFinish)
    {
        this.timedSounds = new List<TimedSound>(soundList.TimedSounds);
        this.timeToFinish = timeToFinish;
        timer = 0f;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
        

    }

    public bool IsTimeToPlay()
    {
        // check if first sound is ready to play
        if (timedSounds.Count == 0) return false;
        if (timedSounds[0].IsTimeToPlay(timer / timeToFinish))
        {
            return true;
        }
        return false;
    }

    public bool HasNextSoundRumble()
    {
        if (timedSounds.Count == 0) return false;
        return timedSounds[0].hasRumble;
	}

    public RumbleData GetNextRumble()
    {
        var sound = timedSounds[0];
        return sound.rumbleData;
	}

	public EventReference GetNextSound()
    {
        if (timedSounds.Count == 0) return new EventReference();
        var sound = timedSounds[0];
        timedSounds.RemoveAt(0);
        return sound.SoundReference;
    }

    public bool IsFinished()
    {
        return timer >= timeToFinish;
    }


}

[Serializable]
public class TimedSound
{
    [SerializeField] EventReference soundReference;
    [SerializeField] float timeOfPlay = 0f;

    public bool hasRumble = false;
    public RumbleData rumbleData;

	public bool IsTimeToPlay(float time)
    {
        return time >= timeOfPlay;
    }

    public EventReference SoundReference => soundReference;




}
