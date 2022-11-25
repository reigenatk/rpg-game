using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yarn.Unity;
using static GameState;
using static TimeManager;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] GameState gameState;
    [SerializeField] Player player;
    [SerializeField] DialogueRunner dialogueRunner;

    // the reason why we only disable non-NPC animators, is cuz we still want NPCs, that is people who move around on the map, to keep moving.
    // At least I think we do. When we talk to someone, the NPCs around shouldn't stop. Though they might stop since we pause time during cutscene dialogues... huh
    // I will have to test this later.
    [SerializeField] GameObject nonNPCObject;
    [SerializeField] GameObject NPCsObject;
    [SerializeField] GameObject AlwaysShowObjects;
    PlayableDirector p;


    public void DialogueNodeStartedPlaying()
    {
        // https://answers.unity.com/questions/1496855/controlling-timeline-through-script.html
        // so problem is, unity doesnt keep playing the animation that was playing in the timeline, if we pause it mid-timeline. 
        // what this means is, if I have say kabowski look up during the timeline, then timeline pauses (for dialogue)
        // then kabowski's animator will RESOLVE TO WHATEVER DEFAULT ANIM IS ON HIS ANIMATOR! Which is bad, I want it to keep playing
        // whatever the timeline was playing. So I'm hoping using the scripting API, I can somehow find the clip that 
        // was playing on each character, and then just manually play it somehow?

        // maybe another less complex solution- disable animators on all non-NPCs before entering dialogue?
        // That way they will freeze? Then enable right before dialogue starts playing again? This is just like how we handled it for
        // the player char. yeah, that might work... OK EDIT: I'll just do the same for NPCs too, cuz NPCs won't move if time is paused
        // so if I don't pause animations then it'll look like they're walking stationary which is arguably worse than just pausing them.
        // I have no clue how to make the NPCs still move on a schedule tho if I pause time for cutscenes... hm. Problem for another day maybe?
        if (gameState.cutscenePlaying != null)
        {
            // if there's a cutscene playing, pause the cutscene when running the dialgoue
            /*TimelineAsset timelineAsset = (TimelineAsset) gameState.cutscenePlaying.playableAsset;
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                foreach (var clip in track.GetClips())
                {
                    
                }
            }*/
            gameState.cutscenePlaying.Pause();

            // we need to do this otherwise the non npcs and npcs will do their default animations which we dont want
            foreach (Animator a in nonNPCObject.GetComponentsInChildren<Animator>())
            {
                // get each animator, then disable it!
                a.enabled = false;
            }
            foreach (Animator a in NPCsObject.GetComponentsInChildren<Animator>())
            {
                // get each animator, then disable it!
                a.enabled = false;
            }
            foreach (Animator a in AlwaysShowObjects.GetComponentsInChildren<Animator>())
            {
                // get each animator, then disable it!
                a.enabled = false;
            }
        }
        
        gameState.setGameVariable("isDialoguePlaying", true);
        gameState.currentRunningDialogueNode = dialogueRunner.CurrentNodeName;
        player.DisableMovementAndAnimations();
    }

    public void DialogueNodeFinishedPlaying()
    {
        if (gameState.cutscenePlaying != null)
        {
            // if there's a cutscene playing, resume the cutscene when the dialogue is done running
            Debug.Log("resuming cutscene " + gameState.cutscenePlaying.name);
            gameState.cutscenePlaying.Resume();

            // enable all non-npc animators
            foreach (Animator a in nonNPCObject.GetComponentsInChildren<Animator>())
            {
                // get each animator, then enable it!
                a.enabled = true;
            }
            foreach (Animator a in NPCsObject.GetComponentsInChildren<Animator>())
            {

                a.enabled = true;
            }
            foreach (Animator a in AlwaysShowObjects.GetComponentsInChildren<Animator>())
            {

                a.enabled = true;
            }
        }
        gameState.setGameVariable("isDialoguePlaying", false);

        player.EnableMovementAndAnimations();
    }

    public void DialogueFinishedPlaying()
    {
        NPCMovement NPCBeingTalkedTo = FindObjectOfType<GameState>().currentNPCBeingTalkedTo;
        if (NPCBeingTalkedTo != null)
        {
            // say that we're done talking to this NPC
            NPCBeingTalkedTo.isNPCBeingTalkedTo = false;

            // also say that it is NOT moving, so that the FixedUpdate in NPCMovement.cs will run and try to find the next task for it to do
            // because if npcIsMoving is true, the NPC will not do anything, its assumed it already has a movement
            NPCBeingTalkedTo.npcIsMoving = false;

            // then also say that we're not talking to this NPC anymore.
            FindObjectOfType<GameState>().currentNPCBeingTalkedTo = null;
        }

        // also set there to be no more dialogue playing, this will start up game time again
        gameState.currentRunningDialogueNode = null;
    }
    public void StartDialogueString(string s)
    {
        Yarn.Unity.DialogueRunner dr = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<Yarn.Unity.DialogueRunner>();

        // stop whatever dialogue is still running, dont wanna run two dialogues at once!
        dr.Stop();
        dr.StartDialogue(s);
    }

    [System.Serializable]
    public class DialogueWithTime
    {
        public string dialogueToPlay;
        public GameTime earliestTime;
        public int dayToPlay;
        public List<GameVariablePair> extraConditions;
    }

    // we're gonna use strings instead
/*
    // all the interactable dialogue in the game 
    // EDIT: LOL no way too much dialogue, this  was a shit idea
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
