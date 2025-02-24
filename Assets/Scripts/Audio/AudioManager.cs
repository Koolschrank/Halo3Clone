using UnityEngine;
// fmod
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    // instance
    public static AudioManager instance;

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
}
