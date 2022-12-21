using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using static TimeManager;

public class Subwoofer : MonoBehaviour
{
    [SerializeField] List<ChunkOfTime> chunkOfTimesToPlayMusic;
    [SerializeField] List<AudioClip> songsToPlay;
    AudioSource audioSource;
    Animator animator;
    public bool isSubwooferPlaying;
    GameState gameState;
    AudioLowPassFilter lowpass;
    Discoball dball;
    public bool turnedItDown = false; // has Steven talked to the person about shutting the hell up? This is one time event.
    [SerializeField] float turnedDownVolume;
    [SerializeField] float turnedUpVolume;
    public void turnItDown()
    {
        turnedItDown = true;

    }

    private void Start()
    {
        isSubwooferPlaying = false;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        gameState = FindObjectOfType<GameState>();
        lowpass = GetComponent<AudioLowPassFilter>();
        dball = FindObjectOfType<Discoball>();
    }

    bool isInRoomWeCareAbout()
    {
        if (gameState.getCurrentSceneEnum() == SceneName.LancelotRoom || gameState.getCurrentSceneEnum() == SceneName.KabowskiRoom
                   || gameState.getCurrentSceneEnum() == SceneName.Bedroom || gameState.getCurrentSceneEnum() == SceneName.BathroomWTF
                   || gameState.getCurrentSceneEnum() == SceneName.Commons || gameState.getCurrentSceneEnum() == SceneName.BrainsRoom)
        {
            return true;
        }
        return false;
    }



    // Update is called once per frame
    void Update()
    {
        if (gameState.getYarnVariable("$hasTurnedDownMusic") == true)
        {
            turnedItDown = true;
        }
        foreach (ChunkOfTime cot in chunkOfTimesToPlayMusic)
        {
            if (cot.isInChunk(TimeManager.Instance.gt))
            {
                if (!dball.isRunning)
                {
                    dball.startDiscoball();
                }
                // then it is in chunk, play the music

                // dont play if we are not in a cutscene, not already playing it, and also if its day 1 (we dont wanna overwhelm new player so dont play for day 1)
                if (!isSubwooferPlaying && !gameState.cutscenePlaying && gameState.getGameDay() != 1)
                {
                    if (isInRoomWeCareAbout())
                    {
                        Debug.Log("Starting up subwoofer");
                        isSubwooferPlaying = true;

                        // if we have no song, add one in. We will have no song either cuz 1. its the first time or 2. We finished a song previously and then clip was set to null
                        if (audioSource.clip == null)
                        {
                            audioSource.clip = songsToPlay[Random.Range(0, songsToPlay.Count)];
                        }
                        
                        audioSource.Play();
                        animator.SetBool("activeSubwoofer", true);
                    }
                }

                // if its playing, check that we in the house. Otherwise we dont care- we're too far away
                if (isSubwooferPlaying && isInRoomWeCareAbout())
                {
                    // if a cutscene is playing, mute it
                    if (gameState.cutscenePlaying != null)
                    {
                        stopSubwoofer();
                    }
                    if (gameState.getCurrentSceneEnum() == SceneName.LancelotRoom)
                    {
                        // no lowpass
                        lowpass.cutoffFrequency = 22000;
                    }
                    else
                    {
                        // we're in another room, lets lerp from the value we want and max using the distance from the subwoofer
                        /*float distance = Vector3.Distance(GetComponent<Player>().transform.position, gameObject.transform.position);
                        lowpass.cutoffFrequency = Mathf.Lerp(979, 22000, )*/

                        // nvm too fancy, lets just set it to 979 for now (value we had in ableton)
                        lowpass.cutoffFrequency = 979;
                    }

                    // what volume to use?
                    if (turnedItDown == false)
                    {
                        audioSource.volume = turnedUpVolume;
                        // repeatedly pause all music- we dont want any other music to play when subwoofer is active
                        MusicManager.Instance.stopAllMusicNoCoroutine(1.0f);
                    }
                    else
                    {
                        audioSource.volume = turnedDownVolume;
                        if (!MusicManager.Instance.isMusicPlaying())
                        {
                            MusicManager.Instance.startAllMusicNoCoroutine(1f);
                        }
                    }

                }
                else
                {
                    // not in relevant scene- STOP subwoofer music
                    if (isSubwooferPlaying)
                    {
                        stopSubwoofer();
                        if (!MusicManager.Instance.isMusicPlaying())
                        {
                            MusicManager.Instance.startAllMusicNoCoroutine(1f);
                        }
                    }
                }


                return;
            }
        }

        // otherwise its in no chunks of times so it should stop.
        if (isSubwooferPlaying)
        {
            stopSubwoofer();
        }

    }

    public void stopSubwoofer()
    {
        if (dball.isRunning)
        {
            dball.stopDiscoball();
        }
        Debug.Log("Stopping subwoofer");
        if (!MusicManager.Instance.isMusicPlaying())
        {
            MusicManager.Instance.startAllMusicNoCoroutine(1f);
        }
        isSubwooferPlaying = false;

        // if finished with the song, remove the song so we can queue a different one next time
        if (!audioSource.isPlaying)
        {
            audioSource.clip = null;
        }
        else
        {
            // lets pause instead of stop so we can continue where we left off next time
            audioSource.Pause();
        }


        animator.SetBool("activeSubwoofer", false);
    }
}
