/*using System.Collections;
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


    [SerializeField] public SoundAudioClip[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }




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
*/