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
    [SerializeField] LineView lineView;

    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SoundName, SoundItem> soundDictionary2;
    List<Sound> currentlyPlaying = new List<Sound>();

    [SerializeField] private float fastTalkingSpeed = 55.0f;
    [SerializeField] private float slowTalkingSpeed = 25.0f;
    [SerializeField] private float normalTalkingSpeed = 40.0f;


    protected override void Awake()
    {
        base.Awake();

        // Initialise sound dictionary
        soundDictionary = new Dictionary<SoundName, SoundItem>();
        soundDictionary2 = new Dictionary<SoundName, SoundItem>();
        // Load sound dictionary with sounds
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }
        foreach (SoundItem soundItem in so_soundList.soundDetails_2)
        {
            soundDictionary2.Add(soundItem.soundName, soundItem);
        }
    }

    [YarnCommand("playSoundString")]
    public void playSoundString(string s, bool shouldLoop = false)
    {
        Debug.Log("[playSoundString] called for sound " + s);
        PlaySound((SoundName)System.Enum.Parse(typeof(SoundName), s));
    }


    public void playTypewriterSound()
    {
        // kind of unintuitive but I'll try- adjust the typewriter SPEED too depending on whose talking
        // make Nikolai talk slower, the Brain faster, and everyone else at normal speed. 

        switch (lineView.personTalking)
        {
            case "Brain":
                lineView.typewriterEffectSpeed = fastTalkingSpeed;
                break;
            case "Nikolai":
                lineView.typewriterEffectSpeed = slowTalkingSpeed;
                break;
            default:
                lineView.typewriterEffectSpeed = normalTalkingSpeed;
                break;
        }

        // typerwriter sounds!
        switch (lineView.personTalking)
        {
            case "Stacy":
                PlaySound(SoundName.StacyTypewriterSound);
                break;
            case "Kabowski":
                PlaySound(SoundName.KabowskiTypewriterSound);
                break;
            case "Nikolai":
                PlaySound(SoundName.NikolaiTypewriterSound);
                break;
            case "Brain":
                PlaySound(SoundName.BrainTypewriterSound);
                break;
            case "Chase":
                PlaySound(SoundName.BloomerTypewriterSound);
                break;
            case "Michael":
                PlaySound(SoundName.BoomerTypewriterSound);
                break;
            case "Laura":
                PlaySound(SoundName.DoomerGirlTypewriterSound);
                break;
            case "Keith":
                PlaySound(SoundName.CoomerTypewriterSound);
                break;
            case "Pepe":
                PlaySound(SoundName.PepeTypewriterSound);
                break;
            case "Dylan":
                PlaySound(SoundName.DoomerTypewriterSound);
                break;
            case "Sawyer":
                PlaySound(SoundName.SoyRedditTypewriterSound);
                break;
            case "Benny":
                PlaySound(SoundName.SoyDiscordTypewriterSound);
                break;
            case "Voice":
                PlaySound(SoundName.VoiceTypewriterSound);
                break;
            case "Chris":
                PlaySound(SoundName.ZoomerTypewriterSound);
                break;
            case "Mom":
                PlaySound(SoundName.MomTypewriterSound);
                break;
            case "Dad":
                PlaySound(SoundName.DadTypewriterSound);
                break;
            case "Reaper":
                PlaySound(SoundName.ReaperTypewriter);
                break;
            case "Steven (Dying)":
                PlaySound(SoundName.DyingRobotTypewriter);
                break;
            case "Doctor":
                PlaySound(SoundName.DoctorTypewriter);
                break;
            case "Steven (Child)":
                PlaySound(SoundName.PlayerTypewriterYoung);
                break;
            case "Jeffery":
                PlaySound(SoundName.NerdFriendTypewriter);
                break;
            case "Allen":
                PlaySound(SoundName.BrotherTypewriter);
                break;
            default:
                PlaySound(SoundName.TypewriterSound);
                break;
        }
        
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
                g.gameObject.SetActive(false);
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
        GameObject obj = g.gameObject;
        AudioSource a = obj.GetComponent<AudioSource>();
        yield return StartCoroutine(FadeAudioSource.StartFade(a, 3.0f, 0.0f));
        obj.SetActive(false);
        currentlyPlaying.Remove(g);
    }




    public void PlaySound(SoundName soundName)
    {
        Debug.Log("soundDictionary is of size " + soundDictionary.Count);
        if (soundDictionary.ContainsKey(SoundName.ArriveHome))
        {
            print("ArriveHome");
        }
        if (soundPrefab == null)
        {
            Debug.Log("SoundPrefab null, can't play sound");
            return;
        }
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem))
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
        else if (soundDictionary2.TryGetValue(soundName, out SoundItem soundItem2))
        {
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem2);
            // Debug.Log(soundGameObject);
            // Debug.Log(sound + " soundItem is " + sound.soundItem.soundName);

            currentlyPlaying.Add(sound);
            soundGameObject.SetActive(true);


            // this is kinda a hack since soundClips is an array but I am assuming that the clips (if there are multiple) are all around the same length, so it doesn't rly matter if we're a bit late or early
            StartCoroutine(DisableSound(soundGameObject, soundItem2.soundClips[0].length));
        }
        else
        {
            Debug.Log("Coudn't find sound with enum name " + soundName + " soundItem is " + soundItem.soundName + " soundPrefab is null? " + (soundPrefab == null));
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
        currentlyPlaying.Remove(soundGameObject.GetComponent<Sound>());
    }
}