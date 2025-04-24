using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponSoundManager : MonoBehaviour
{

    [SerializeField] ArmsExtended playerArms;
    [SerializeField] MeleeAttacker meleeAttacker;
    [SerializeField] TargetHitCollector targetHitCollector;
    TimedSoundListInstance reloadListLeftWeapon;
    TimedSoundListInstance reloadListRightWeapon;
    TimedSoundListInstance switchInListLeftWeapon;
    TimedSoundListInstance switchInListRightWeapon;

    [SerializeField] EventReference hitSound;
    [SerializeField] EventReference killSound;


    public void Start()
    {
        playerArms.OnRightWeaponReloadStarted += ReloadRightWeapon;
        playerArms.OnRightWeaponEquiped += SwitchInRightWeapon;
        playerArms.OnRightWeaponShot += Shoot;

        playerArms.OnLeftWeaponReloadStarted += ReloadLeftWeapon;
        playerArms.OnLeftWeaponEquiped += SwitchInLeftWeapon;
        playerArms.OnLeftWeaponShot += Shoot;

        meleeAttacker.OnMeleeStart += MeleeSwing;
        meleeAttacker.OnMeleeHit += MeleeHit;

        targetHitCollector.OnCharacterHit += HitTarget;
        targetHitCollector.OnCharacterKill += KillTarget;
    }

    public void HitTarget(GameObject target)
    {
        
        AudioManager.instance.PlayOneShot(hitSound, transform.position);
    }

    public void KillTarget(GameObject target)
    {
       
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
        AudioManager.instance.PlayOneShot(weapon.ShootSound, transform.position);

    }


    public void SwitchInLeftWeapon(Weapon_Arms weapon)
    {
        float time = playerArms.RemainingGetReadyTime_LeftWeapon;
        switchInListLeftWeapon = new TimedSoundListInstance(weapon.SwitchInSound, time);
    }

    public void SwitchInRightWeapon(Weapon_Arms weapon)
    {
        float time = playerArms.RemainingGetReadyTime_RightWeapon;
        switchInListRightWeapon = new TimedSoundListInstance(weapon.SwitchInSound, time);
    }





    public void ReloadLeftWeapon(int weaponTypeIndex)
    {
        float time = playerArms.RemainingReloadTime_LeftWeapon;
        reloadListLeftWeapon = new TimedSoundListInstance(ItemIndexList.Instance.GetWeaponViaIndex(weaponTypeIndex).ReloadSounds, time);
    }

    public void ReloadRightWeapon(int weaponTypeIndex)
    {
        float time = playerArms.RemainingReloadTime_RightWeapon;
        reloadListRightWeapon = new TimedSoundListInstance(ItemIndexList.Instance.GetWeaponViaIndex(weaponTypeIndex).ReloadSounds, time);
    }

    public void Update()
    {
        UpdateSoundList(reloadListLeftWeapon);
        UpdateSoundList(reloadListRightWeapon);
        UpdateSoundList(switchInListLeftWeapon);
        UpdateSoundList(switchInListRightWeapon);

    }

    public void UpdateSoundList(TimedSoundListInstance soundList)
    {
        if (soundList == null) return;
        if (!soundList.IsFinished())
        {
            soundList.Update(Time.deltaTime);
            if (soundList.IsTimeToPlay())
            {
                AudioManager.instance.PlayOneShot(soundList.GetNextSound(), transform.position);
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

    public bool IsTimeToPlay(float time)
    {
        return time >= timeOfPlay;
    }

    public EventReference SoundReference => soundReference;




}
