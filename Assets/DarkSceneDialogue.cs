using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSceneDialogue : MonoBehaviour
{
    // Checks what day it is, runs certain dialogue based on that.
    void Start()
    {
        int day = FindObjectOfType<GameState>().getGameDay();
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        switch (day)
        {
            case 1:
                dm.StartDialogueString("TrainStation");
                break;
            case 2:
                dm.StartDialogueString("D2_Sleep");
                break;
        }
    }


}
