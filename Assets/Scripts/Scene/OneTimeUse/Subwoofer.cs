using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeManager;

public class Subwoofer : MonoBehaviour
{
    [SerializeField] List<ChunkOfTime> chunkOfTimesToPlayMusic;
    AudioSource audioSource;
    Animator animator;
    bool isSubwooferPlaying;
    GameState gameState;
    private void Start()
    {
        isSubwooferPlaying = false;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (ChunkOfTime cot in chunkOfTimesToPlayMusic)
        {
            if (cot.isInChunk(TimeManager.Instance.gt))
            {
                // then it is in chunk, play the music

                if (!isSubwooferPlaying && !gameState.cutscenePlaying)
                { 
                    
                    isSubwooferPlaying = true;
                    audioSource.Play();
                    animator.SetBool("activeSubwoofer", true);
                }


                MusicManager.Instance.stopAllMusicNoCoroutine(1.0f);

                return;
            }
        }

        // otherwise its in no chunks of times so it should stop.
        if (isSubwooferPlaying)
        {
            MusicManager.Instance.startAllMusicNoCoroutine(1f);
            isSubwooferPlaying = false;

            // lets pause instead of stop so we can play thru the whole song
            audioSource.Pause(); 
            animator.SetBool("activeSubwoofer", false);
        }

    }
}
