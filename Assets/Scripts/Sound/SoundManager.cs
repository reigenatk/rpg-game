using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }

    public enum Sound
    {
        NoSound,
        TypewriterSound,
        WalkingSound,
        ChipsCrunch,
        TrainWhistle,
        TrainMoving,
        TrainStation,
        Driving,
        OpenFrontDoor,
        WalkingOnWood,
        Boxes,
        Sigh,
        OpenWindow,
        WindGust,
        NotBirthdaySong,
        CandlesBlow,
        OpenShopDoor,
    }
    [SerializeField] public SoundAudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [YarnCommand("playSoundString")]
    public void playSoundString(string s, bool shouldLoop = false)
    {
        playSoundOneShot((Sound)System.Enum.Parse(typeof(Sound), s));
    }
/*    public void playSoundDetailed(Sound s, float volume, float pitch)
    {
        float oldPitch = audioSource.pitch;
        float oldVolume = audioSource.volume;
        if (volume != -1.0f)
        {
            Debug.Log("New Volume " + volume);
            audioSource.volume = volume;
        }
        if (pitch != -1.0f)
        {
            Debug.Log("New Pitch " + pitch);
            audioSource.pitch = pitch;
        }
        audioSource.PlayOneShot(GetAudioClip(s));
        audioSource.pitch = oldPitch;
        audioSource.volume = oldVolume;
    }*/

    // Update is called once per frame
    public void playSoundOneShot(Sound s)
    {

        switch (s)
        {
            case Sound.NoSound:
                break;
            default:
                audioSource.PlayOneShot(GetAudioClip(s));
                break;
        }
    }

    [YarnCommand("stopSound")]
    public void stopSound()
    {
        audioSource.Stop();
    }


    public void playTypewriterSound()
    {
        playSoundOneShot(Sound.TypewriterSound);
    }

    public AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip s in sounds)
        {
            if (s.sound == sound)
            {
                if (s.audioClips.Length == 1)
                {
                    return s.audioClips[0];
                }
                else
                {
                    float randomSound = Random.Range(0, s.audioClips.Length);
                    int idx = Mathf.FloorToInt(randomSound);
                    return s.audioClips[idx];
                }
            }
        }
        return null;
    }
}
