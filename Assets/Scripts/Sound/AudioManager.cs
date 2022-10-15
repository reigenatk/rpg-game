using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using static SoundItem;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;

    [Header("Other")]
    // Sound list and dictionary
    [SerializeField] private SO_SoundList so_soundList = null;

    private Dictionary<SoundName, SoundItem> soundDictionary;
    List<Sound> currentlyPlaying = new List<Sound>();

    protected override void Awake()
    {
        base.Awake();

        // Initialise sound dictionary
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        // Load sound dictionary with sounds
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }
    }

    [YarnCommand("playSoundString")]
    public void playSoundString(string s, bool shouldLoop = false)
    {

        PlaySound((SoundName)System.Enum.Parse(typeof(SoundName), s));
    }


    public void playTypewriterSound()
    {
        Debug.Log("play typewriter sound");
        PlaySound(SoundName.TypewriterSound);
    }

    [YarnCommand("stopSoundString")]
    public void stopSoundString(string s)
    {
        SoundName soundToRemove = (SoundName)System.Enum.Parse(typeof(SoundName), s);
        StopSound(soundToRemove);
    }
    [YarnCommand("stopSoundStringFade")]
    public void stopSoundStringFade(string s)
    {
        SoundName soundToRemove = (SoundName)System.Enum.Parse(typeof(SoundName), s);
        StopSoundFade(soundToRemove);
    }

    public void StopSound(SoundName soundName)
    {
        foreach (Sound g in currentlyPlaying)
        {
            // if we find said sound
            if (g.soundItem.soundName == soundName)
            {
                g.transform.parent.gameObject.SetActive(false);
                currentlyPlaying.Remove(g);
                break;
            }
        }
    }

    public void StopSoundFade(SoundName soundName)
    {
        foreach (Sound g in currentlyPlaying)
        {
            // if we find said sound
            if (g.soundItem.soundName == soundName)
            {
                StartCoroutine(FadeAudioAndDelete(g));

                break;
            }
        }
    }

    // this fades out an audio sample before deleting it. So it doesn't instantly stop.
    IEnumerator FadeAudioAndDelete(Sound g)
    {
        GameObject parent = g.transform.parent.gameObject;
        AudioSource a = parent.GetComponent<AudioSource>();
        yield return StartCoroutine(FadeAudioSource.StartFade(a, 3.0f, 0.0f));
        parent.SetActive(false);
        currentlyPlaying.Remove(g);
    }




    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem);
            // Debug.Log(soundGameObject);
            // Debug.Log(sound + " soundItem is " + sound.soundItem.soundName);

            currentlyPlaying.Add(sound);
            soundGameObject.SetActive(true);
            

            // this is kinda a hack since soundClips is an array but I am assuming that the clips (if there are multiple) are all around the same length, so it doesn't rly matter if we're a bit late or early
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClips[0].length));
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
        currentlyPlaying.Remove(soundGameObject.GetComponent<Sound>());
    }
}