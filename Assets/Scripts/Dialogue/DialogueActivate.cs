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
            yield return new WaitForSeconds(1f);
            Outerloop:
                continue;
        }
        // if we get here its bad because we should've at least played some dialogue already
        // this means all dialogues are too early to be played? Which is an error

    }

    public bool isAnotherPlayer()
    {
        return GetComponent<PlayerLoad>() != null;
    }

    // direction is the direction that OUR player is facing, so other person should be opposite.
    private void checkHumanFacing(string direction)
    {
        if (isAnotherPlayer())
        {
            Animator animator = GetComponent<Animator>();
            // so for example we want "Base Layer.kabowski-left-idle"
            // down animations are just called "idle", so "kabowski-idle" 
            string animationStateName = "";
            switch (direction)
            {
                case "Left":
                    animationStateName = "Base Layer." + gameObject.name.ToLower() + "-right-idle";
                    break;
                case "Right":
                    animationStateName = "Base Layer." + gameObject.name.ToLower() + "-left-idle";
                    break;
                case "Down":
                    animationStateName = "Base Layer." + gameObject.name.ToLower() + "-up-idle";
                    break;
                case "Up":
                    animationStateName = "Base Layer." + gameObject.name.ToLower() + "-idle";
                    break;
            }
            Debug.Log("Playing animation " + animationStateName);
            animator.Play(animationStateName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInside = true;
        if (other.CompareTag("Player") && gameObject.layer == LayerMask.NameToLayer("Dialogue Colliders") && other.TryGetComponent(out Player player))
        {
            // player interacts with this one
            player.interactables.Add(this);
            if (soundToPlay != null)
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
