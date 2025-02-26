using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponSoundManager : MonoBehaviour
{

    [SerializeField] PlayerArms playerArms;
    TimedSoundListInstance reloadList;
    TimedSoundListInstance switchInList;


    public void Start()
    {
        playerArms.OnWeaponReloadStarted += Reload;
        playerArms.OnWeaponEquipStarted += SwitchIn;
        playerArms.OnWeaponShoot += Shoot;
    }

    public void Shoot(Weapon_Arms weapon)
    {
        AudioManager.instance.PlayOneShot(weapon.ShootSound, transform.position);

    }

    public void SwitchIn(Weapon_Arms weapon, float timer)
    {
        switchInList = new TimedSoundListInstance(weapon.SwitchInSound, timer);
    }

    public void Reload(Weapon_Arms weapon, float time)
    {
        reloadList = new TimedSoundListInstance( weapon.ReloadSounds, time);


    }

    public void Update()
    {
        UpdateSoundList(reloadList);
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
