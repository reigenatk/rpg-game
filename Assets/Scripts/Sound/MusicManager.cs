using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MusicManager : MonoBehaviour
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
                if (c.day == day && c.sceneName == scene)
                {
                    // then this condition matches, we should play this song
                    return true;
                }
            }
            return false;
        }
    }

    [SerializeField] List<Music> musicToPlay;

    [YarnCommand("stopAllMusic")]
    public void stopAllMusic()
    {
        foreach (Music m in musicToPlay)
        {
            if (m.music.GetComponent<AudioSource>().isPlaying)
            {
                m.music.GetComponent<AudioSource>().Stop();
            }
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
                m.music.GetComponent<AudioSource>().Play();
            }
        }
    }

    // this will be called each time we load a new scene (from LevelLoader)
    public void playMusic(int currentDay, SceneName currentSceneName)
    {
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
                    return;
                }
                else
                {
                    if (curAudioSource != null)
                    {
                        // can do Pause() here instead if we want themes to pick up where they left off.
                        curAudioSource.Stop();
                    }
                    m.music.GetComponent<AudioSource>().Play();
                    curAudioObject = m.music;
                    return;
                }
            }
        }

        // if we get here it means we found no suitable songs
        // so just stop the current song, this scene should be silent
        stopAllMusic();
        return;
    }

}
