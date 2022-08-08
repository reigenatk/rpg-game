using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    [SerializeField] public SoundAudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void playSound(Sound s)
    {
        switch (s)
        {
            case Sound.NoSound:
                break;
            case Sound.TypewriterSound:
                audioSource.PlayOneShot(GetAudioClip(s));
                break;
            case Sound.WalkingSound:
                float oldPitch = audioSource.pitch;
                audioSource.pitch = 1.0f;
                audioSource.PlayOneShot(GetAudioClip(s));
                audioSource.pitch = oldPitch;
                break;
            case Sound.ChipsCrunch:
                audioSource.PlayOneShot(GetAudioClip(s));
                break;
        }
    }

    public void playTypewriterSound()
    {
        playSound(Sound.TypewriterSound);
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
