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

        if (!skipDreams)
        {
            // then play the starting dialogue (aka dream) for the night
            switch (day)
            {
                case 1:
                    dm.StartDialogueString("TrainStation");
                    break;
                default:
                    // player explores the dark world first
                    break; 
            }
        }
        else if (skipDreams == true)
        {
            FindObjectOfType<LevelLoader>().wakeUp();
        }
    }



}
