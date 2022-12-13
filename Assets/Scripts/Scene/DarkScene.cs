using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkScene : MonoBehaviour
{
    [SerializeField] private bool skipDreams;

    // Checks what day it is, runs certain dialogue based on that.
    private IEnumerator Start()
    {
        // the value in here is the day that we are MOVING to. 
        int day = FindObjectOfType<GameState>().getGameDay();
        DialogueManager dm = FindObjectOfType<DialogueManager>();

        // this scene always starts with no music
        // fade out currently playing music, if any
        yield return StartCoroutine(FindObjectOfType<MusicManager>().stopAllMusic(2.0f));

        Debug.Log("Darkscene Day is " + day);

        /*
         * day 2 = dream 1
         * day 3 = dream 2
         * day 4 = dream 3
         * day 5 = dream 4
         * day 6 = dream 5
         * day 7 = explores the dark scene, talks to the reaper once again, and they say their goodbyes
         */

        if (!skipDreams)
        {
            // then play the starting dialogue (aka dream) for the night
            if (day >= 8)
            {
                // done with all the dreams already then just wakeup
                StartCoroutine(wakeUpAfterTime());
            }
            else
            {
                // just explore the dream world
            }

        }
        else if (skipDreams == true)
        {
            StartCoroutine(wakeUpAfterTime());
        }
    }

    public IEnumerator wakeUpAfterTime()
    {
        yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        FindObjectOfType<LevelLoader>().wakeUp();
    }

}


