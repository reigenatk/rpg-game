using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivate : MonoBehaviour, Interactable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private SpriteRenderer[] dialogueImages;
    [SerializeField] private LevelLoader levelLoader;
    
    public void Interact(Player player)
    {
        StartCoroutine(fadeScreenInAndOut(player));
    }

    IEnumerator fadeScreenInAndOut(Player player)
    {
        levelLoader.FadeScreenOut();
        player.disableMovement = true;
        yield return new WaitForSeconds(1f);
        player.setPosition(gameObject.transform.position + new Vector3(0, -2, 0));
        player.faceUpwards();
        player.DialogueUI.showDialogue(dialogueObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            // player interacts with this one
            player.Interactable = this;
        }
        foreach (SpriteRenderer s in dialogueImages) {
            s.enabled = true;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {

        foreach (SpriteRenderer s in dialogueImages)
        {
            s.enabled = false;
        }
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
