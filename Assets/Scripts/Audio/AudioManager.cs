using UnityEngine;
// fmod
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // instance
    public static AudioManager instance;
    List<EventInstance> eventInstances = new List<EventInstance>();

    // start 
    private void Start()
    {
        // set instance
        instance = this;
    }

    // play sound
    public void PlayOneShot(EventReference soundEvent, Vector3 position)
    {
        // play sound
        RuntimeManager.PlayOneShot(soundEvent, position);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        // create event instance
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    // ondestroy
    private void OnDestroy()
    {
        // release all event instances
        foreach (var item in eventInstances)
        {
            // stop item
            item.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            item.release();
        }

    }
}
