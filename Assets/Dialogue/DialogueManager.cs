using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static TimeManager;

public class DialogueManager : MonoBehaviour
{


    PlayableDirector p;


/*    // converts from editor friendly enum to a string and actually calls StartDialogue from Yarn spinner
    public void StartDialogueEnum(Dialogue d)
    {
        Yarn.Unity.DialogueRunner dr = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<Yarn.Unity.DialogueRunner>();
        dr.Stop();

        // sends Enum to string. Since we made it such that Yarn nodes are exact same as the enum names, this works
        dr.StartDialogue(Enum.GetName(typeof(Dialogue), d));
    }*/

    public void StartDialogueString(string s)
    {
        Yarn.Unity.DialogueRunner dr = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<Yarn.Unity.DialogueRunner>();
        dr.Stop();
        dr.StartDialogue(s);
    }

    [System.Serializable]
    public class DialogueWithTime
    {
        public string dialogueToPlay;
        public GameTime earliestTime;
        public int dayToPlay;
    }

    // we're gonna use strings instead
/*
    // all the interactable dialogue in the game
    [System.Serializable]
    public enum Dialogue
    {
        D1_Bedroom_Bed,
        D1_Bedroom_Bed_Gotosleep,
        D1_Bedroom_Chips,
        D1_Bedroom_Peripherals,
        D1_Bedroom_Trash,
        D1_Bedroom_Window,
        D1_Commons_Boxes,
        D1_Commons_Oven,
        D1_Commons_Sink,
        D1_Commons_Fridge,
        D1_Commons_Trash,
        D1_Commons_Sofa,
        D1_Commons_Door,
        D1_Commons_DiningTable,
        D1_Commons_TV,
        D1_Commons_Microwave,
        D1_Brain_Painting,
        D1_Brain_FumeHood,
        D1_Brain_Mainframe,
        D1_Brain_Desk,
        D1_Brain_Hangup,
        D1_Brain_Bed,
        D1_Kab_Poster,
        D1_Kab_Bench,
        D1_Kab_Bed,
        D1_Kab_Kotatsu,
        D1_Bathroom_SinkAndMirror,
        D1_Bathroom_BrushTeeth,
        D1_Bathroom_Shower,
        D1_Bathroom_Toilet,
        D1_Lancelot_PolarBear,
        D1_Lancelot_Desk,
        D1_Lancelot_Discoball,
        D1_Lancelot_Subwoofers,
        D1_Lancelot_NorthKorea,
    }*/
}
