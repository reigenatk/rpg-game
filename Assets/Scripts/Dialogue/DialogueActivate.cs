using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivate : MonoBehaviour, Interactable
{
    [SerializeField] private string DialogueToRun;
    [SerializeField] private SpriteRenderer[] dialogueImages;
/*    [SerializeField] private LevelLoader levelLoader;*/
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

        Yarn.Unity.DialogueRunner dr = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<Yarn.Unity.DialogueRunner>();
        dr.Stop();
        dr.StartDialogue(DialogueToRun);
        yield return new WaitForSeconds(1f);
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
