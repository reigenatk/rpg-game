using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkScene : MonoBehaviour
{
    // Checks what day it is, runs certain dialogue based on that.
    void Start()
    {
        int day = FindObjectOfType<GameState>().getGameDay();
        DialogueManager dm = FindObjectOfType<DialogueManager>();

        // this scene always starts with no music
        FindObjectOfType<MusicManager>().stopAllMusic();
        Debug.Log("Day is " + day);
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
