using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using static DialogueManager;
using static GameState;
using static SoundItem;
using static TimeManager;

public class DialogueActivate : MonoBehaviour, Interactable
{

    // our strategy will be to have different dialogues to run based on certain times
    // so for example, dialogue A will have two pieces of info, the dialogue itself and the 
    // earliest time at which it can play. If the current time is later than that earliest time
    // then we will play that dialogue. Otherwise, we move down the list to the next dialogue in line.

    // this is used for example when we want to sleep, well, we want user to see the "Go to sleep?" 
    // dialogue when its later than a certain time. But otherwise, we want it to do the usual dialogue.
    // so we should MAKE SURE TO PUT THE DIALOGUES IN A CERTAIN ORDER in the editor
    // for now we can do the latest ones (most restrictive) last, and so we iterate from the back of the list
    [SerializeField] private List<DialogueWithTime> dialoguesToRun;
    [SerializeField] private SpriteRenderer[] dialogueImages;

    private Player player;
    private bool isInside;

    // ok didn't name this the best- this is the sound that plays when you hit the collider!
    // so like, a piece of trash when u step on it for instance would use this field
    [SerializeField] SoundName soundToPlay; 



    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void Interact(Player player)
    {
        StartCoroutine(triggerDialogue(player));
    }

    IEnumerator triggerDialogue(Player player)
    {
        GameState gameState = FindObjectOfType<GameState>();
        Vector3 position = player.transform.position;
        string direction = player.getPlayerDirection();

        // check that player is facing right way
        if (direction == "Down" && player.transform.position.y < transform.position.y)
        {
            yield return new WaitForSeconds(0f);
        }
        else if (direction == "Right" && player.transform.position.x > transform.position.x)
        {
            yield return new WaitForSeconds(0f);
        }
        if (direction == "Up" && player.transform.position.y > transform.position.y)
        {
            yield return new WaitForSeconds(0f);
        }
        if (direction == "Left" && player.transform.position.x < transform.position.x)
        {
            yield return new WaitForSeconds(0f);
        }

        // check that player is colliding
        if (!isInside) yield return new WaitForSeconds(0f);



        // ok now we are confirmed talking
        // check whether the object we are talking to is human or not
        // for now, we will do this by checking whether or not the object has a PlayerLoad script on it
        // since only other players will have this script.
        // if so, we gotta change the animation on the person so that they are "facing" our player
        checkHumanFacing(direction);


        // decide which dialogue to use
        GameTime gt = FindObjectOfType<TimeManager>().gt;
        bool foundDialogue = false;

        for (int i = dialoguesToRun.Count - 1; i >= 0; i--)
        {
            if (foundDialogue) break;

            DialogueWithTime dwt = dialoguesToRun[i];
            Debug.Log("Considering Dialogue " + dwt.ToString());

            // we will let day = -1 in the dialogue specification to mean, play any day.
            if (dwt.dayToPlay != -1 && (dwt.dayToPlay != gameState.getGameDay()))
            {
                Debug.Log("Failed to match days, expected day is " + dwt.dayToPlay);
                continue;
            }
            // check all game conditions 
            foreach (GameVariablePair gv in dwt.extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    Debug.Log("fail on gamevariable " + gv.variable.ToString() + " value was " + gameState.getGameVariableEnum(gv.variable));

                    // continue the outer loop
                    goto Outerloop;
                }
            }

            // if current time is earlier than this earliest time limit, then dont play it
            if (gt.compareTimes(dwt.earliestTime) == true)
            {
                continue;
            }

            // ok it passed all checks, run it
            foundDialogue = true;
            
            // if we are later than this limit, then play this dialogue
            DialogueManager dm = GameObject.FindGameObjectWithTag("Manager").GetComponent<DialogueManager>();
            
            dm.StartDialogueString(dwt.dialogueToPlay);

            yield break; // all done since we played our dialogue


            Outerloop:
                continue;
        }
        // if we get here its bad because we should've at least played some dialogue already
        // this means all dialogues are too early to be played? Which is an error

    }

    // remember this script is on the NPC so we can just check for the presence of an NPC script to know if its an NPC or an inanimate object
    public bool isAnotherPlayer()
    {
        Debug.Log("Transform " + transform);
        Debug.Log("TransformParent " + transform.parent);
        if (transform.parent.gameObject == null)
        {
            return false;
        }
        return transform.parent.gameObject.GetComponent<NPCMovement>();
    }

    // direction is the direction that OUR player is facing, so other person should be opposite.
    private void checkHumanFacing(string direction)
    {
        if (isAnotherPlayer())
        {

            // since this sits in the dialogue collider object we gotta go one level up to get the NPCMovement
            NPCMovement npcMovement = transform.parent.gameObject.GetComponent<NPCMovement>();
            npcMovement.isNPCBeingTalkedTo = true;
            FindObjectOfType<GameState>().currentNPCBeingTalkedTo = npcMovement;
            Debug.Log("Setting current NPC being talked to value of " + npcMovement);
            Animator animator = transform.parent.gameObject.GetComponent<Animator>();

            // so for example we want "Base Layer.kabowski-left-idle"
            // down animations are just called "idle", so "kabowski-idle" 
            string animationTriggerName = "";
            switch (direction)
            {
                case "Left":
                    animationTriggerName = "idleRight";
                    break;
                case "Right":
                    animationTriggerName = "idleLeft";
                    break;
                case "Down":
                    animationTriggerName = "idleUp";
                    break;
                case "Up":
                    animationTriggerName = "idleDown";
                    break;
            }
            // reset all previous animations
            npcMovement.ResetAllAnimation();

            // stop any moving coroutines (otherwise character will still move a bit before fully stopping)
            if (npcMovement.moveToGridPositionRoutine != null)
            {
                StopCoroutine(npcMovement.moveToGridPositionRoutine);
            }
            Debug.Log("Playing animation on NPC " + animationTriggerName);
            animator.SetBool(animationTriggerName, true);


        }
    }

    [YarnCommand("SetNPCAnimation")]
    public void SetNPCAnimation(string boolName)
    {
        NPCMovement npcMovement = transform.parent.gameObject.GetComponent<NPCMovement>();
        npcMovement.ResetAllAnimation();
        Debug.Log("Playing animation on NPC " + boolName);
        Animator animator = transform.parent.gameObject.GetComponent<Animator>();
        animator.SetBool(boolName, true);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        isInside = true;
        if (other.CompareTag("Player") && gameObject.layer == LayerMask.NameToLayer("Dialogue Colliders") && other.TryGetComponent(out Player player))
        {
            // player interacts with this one
            player.interactables.Add(this);
            // only play the sound if the object is in the scene tho
            if (soundToPlay != SoundName.NoSound && transform.parent.GetComponent<SpriteRenderer>().enabled == true)
            {
                AudioManager.Instance.PlaySound(soundToPlay);
            }
        }
/*        foreach (SpriteRenderer s in dialogueImages) {
            s.enabled = true;
        }*/
 

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInside = false;
/*        foreach (SpriteRenderer s in dialogueImages)
        {
            s.enabled = false;
        }*/
        if (other.CompareTag("Player") && gameObject.layer == LayerMask.NameToLayer("Dialogue Colliders") && other.TryGetComponent(out Player player))
        {
            // ok this is a bit confusing but basically its asking, is the 
            // current interactable equal to this object, if so then delete. Cuz we can 
            // have multiple interactables in a certain moment so we have to check which one it is
            if (player.interactables.Contains(this))
            {
                player.interactables.Remove(this);
            }
        }
    }

}
