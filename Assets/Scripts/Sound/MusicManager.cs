using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MusicManager : Singleton<MusicManager>
{
    AudioSource audioSource;

    // we will keep track of which audio is currently playing ourselves
    [SerializeField] private GameObject curAudioObject;

    // what scene and room must match for this to play
    [System.Serializable]
    public struct Constraints
    {
        public int day;
        public SceneName sceneName;
    }

    [System.Serializable]
    public class Music
    {
        public GameObject music;
        public List<Constraints> conditions;

        // is the music only triggerable via code
        public bool isTriggerable;
        public bool checkConditions(int day, SceneName scene)
        {
            if (isTriggerable) return false;
            foreach (Constraints c in conditions)
            {
                if (((c.day == day) || (c.day == -1)) && c.sceneName == scene)
                {
                    // then this condition matches, we should play this song
                    return true;
                }
            }
            return false;
        }
    }

    [SerializeField] List<Music> musicToPlay;

    private IEnumerator stopMusic(AudioSource audioSource, float numSecondsToFadeOver)
    {
        yield return StartCoroutine(FadeAudioSource.StartFade(audioSource, numSecondsToFadeOver, 0.0f));
        audioSource.Stop();
    }

    private IEnumerator startMusic(AudioSource audioSource, float numSecondsToFadeOver)
    {
        audioSource.Play();// if it was faded out, fade it back in
        if (audioSource.volume == 0.0f)
        {
            yield return StartCoroutine(FadeAudioSource.StartFade(audioSource, numSecondsToFadeOver, 1.0f));
        }
    }

    [YarnCommand("stopAllMusic")]
    public IEnumerator stopAllMusic(float numSecondsToFadeOver)
    {
        // Debug.Log("Stopping all music");
        foreach (Music m in musicToPlay)
        {
            if (m.music.GetComponent<AudioSource>().isPlaying)
            {
                // fade the song out smoothly
                Debug.Log("Fading out " + m.music.name);
                yield return stopMusic(m.music.GetComponent<AudioSource>(), numSecondsToFadeOver);
            }
        }
    }

    public bool isMusicPlaying()
    {
        foreach (Music m in musicToPlay)
        {
            if (m.music.GetComponent<AudioSource>().isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void stopAllMusicNoCoroutine(float numSecondsToFadeOver)
    {
        StartCoroutine(stopAllMusic(numSecondsToFadeOver));
    }

    public void startAllMusicNoCoroutine(float numSecondsToFadeOver)
    {
        // get the song that was paused via curAudioObject
        if (curAudioObject != null)
        {
            AudioSource curAudioSource = curAudioObject.GetComponent<AudioSource>();
            StartCoroutine(startMusic(curAudioSource, numSecondsToFadeOver));
        }
        else
        {
            Debug.Log("No music to start");
        }

    }

    // arg is the name of the gameobject under Music Object in persistant scene
    [YarnCommand("playMusicName")]
    public void playMusicName(string nameOfMusicToPlay)
    {
        foreach (Music m in musicToPlay)
        {
            if (m.music.name == nameOfMusicToPlay)
            {
                Debug.Log("Fading in " + nameOfMusicToPlay);
                m.music.GetComponent<AudioSource>().Play();
            }
        }
    }

    // this will be called each time we load a new scene (from LevelLoader)
    public IEnumerator playMusic(int currentDay, SceneName currentSceneName)
    {
        Debug.Log("Checking if we need to play any music...");
        AudioSource curAudioSource = null;
        if (curAudioObject != null)
        {
            curAudioSource = curAudioObject.GetComponent<AudioSource>();
        }
        // find the first song that matches the current conditions (day and scene are equal)
        foreach (Music m in musicToPlay)
        {
            if (m.checkConditions(currentDay, currentSceneName))
            {
                Debug.Log("Playing song " + m.music.name);
                bool isPlaying = m.music.GetComponent<AudioSource>().isPlaying;
                
                // ok, so we've found the clip we want to play
                // the question is- is that clip already playing? If so, we don't wanna replay it from start
                // we just want to do nothing.
                if (isPlaying)
                {
                    yield break;
                }
                else
                {
                    // if we get here, we know that audio is not playing for the song we want

                    // first stop the song that is currently playing
                    if (curAudioSource != null)
                    {
                        // can do Pause() here instead if we want themes to pick up where they left off.
                        yield return StartCoroutine(stopMusic(curAudioSource, 2.0f));
                    }

                    // then start the song that we should play.
                    AudioSource audioToPlay = m.music.GetComponent<AudioSource>();
                    yield return StartCoroutine(startMusic(audioToPlay, 2.0f));
                    curAudioObject = m.music;
                    yield break;
                }
            }
        }

        // if we get here it means we found no suitable songs
        // so just stop the current song, this scene should be silent
        yield return StartCoroutine(stopAllMusic(2.0f));
        curAudioObject = null;
    }

}
