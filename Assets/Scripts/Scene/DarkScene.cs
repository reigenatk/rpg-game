using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkScene : MonoBehaviour
{
    // Checks what day it is, runs certain dialogue based on that.
    private IEnumerator Start()
    {
        int day = FindObjectOfType<GameState>().getGameDay();
        DialogueManager dm = FindObjectOfType<DialogueManager>();

        // this scene always starts with no music
        // fade out currently playing music, if any
        yield return StartCoroutine(FindObjectOfType<MusicManager>().stopAllMusic(2.0f));

        Debug.Log("Darkscene Day is " + day);

        // then play the starting dialogue (aka dream) for the night
        switch (day)
        {
            case 1:
                dm.StartDialogueString("TrainStation");
                break;
            case 2:
                dm.StartDialogueString("D2_Sleep");
                break;
            default:
                break;
        }
    }


}
