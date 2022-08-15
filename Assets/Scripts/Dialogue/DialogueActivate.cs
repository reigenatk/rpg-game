﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueManager;
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
    // maybe we can do the latest ones (most restrictive) last, and so we iterate from the back of the list
    [SerializeField] private List<DialogueWithTime> dialoguesToRun;
    [SerializeField] private SpriteRenderer[] dialogueImages;

    private Player player;
    private bool isInside;
    private SoundManager sm;
    [SerializeField] private SoundManager.Sound soundToPlay;


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
        /*      levelLoader.FadeScreenOut();
                player.disableMovement = true;
                yield return new WaitForSeconds(1f);
                player.setPosition(gameObject.transform.position + new Vector3(0, -2, 0));
                player.faceUpwards();
                player.DialogueUI.showDialogue(dialogueObject);*/
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
        // decide which dialogue to use
        GameTime gt = FindObjectOfType<TimeManager>().gt;
        bool foundDialogue = false;
        for (int i = dialoguesToRun.Count - 1; i >= 0; i--)
        {
            if (foundDialogue) break;

            DialogueWithTime dwt = dialoguesToRun[i];
            if (gt.compareTimes(dwt.earliestTime) == false)
            {
                foundDialogue = true;
                // if we are later than this limit, then play this dialogue
                DialogueManager dm = GameObject.FindGameObjectWithTag("Manager").GetComponent<DialogueManager>();
                dm.StartDialogueString(dwt.dialogueToPlay);
                yield return new WaitForSeconds(1f);
            }
        }
        // if we get here its bad because we should've at least played some dialogue already
        // this means all dialogues are too early to be played? Which is an error

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInside = true;
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            // player interacts with this one
            player.Interactable = this;
            if (soundToPlay != null)
            {
                GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundManager>().playSound(soundToPlay);
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
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            // ok this is a bit confusing but basically its asking, is the 
            // current interactable equal to this object, if so then delete. Cuz we can 
            // have multiple interactables in a certain moment
            if (player.Interactable is DialogueActivate d && d == this)
            {
                player.Interactable = null;
            }
        }
    }
}
