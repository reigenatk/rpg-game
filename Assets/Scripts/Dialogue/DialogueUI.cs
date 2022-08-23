using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueUI  : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject dialogueBox;

    private Typewriter typewriterEffect;
    private ResponseHandler responseHandler;
    [SerializeField] private Player player;



    private void Start()
    {
        typewriterEffect = GetComponent<Typewriter>();
        responseHandler = GetComponent<ResponseHandler>();
        closeDialogueBox();
        // showDialogue(testDialogue);
    }
    public void showDialogue(DialogueObject dialogueObject)
    {
/*        player.disableMovement = true;*/
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            // if there are responses
            if (i == dialogueObject.Dialogue.Length-1 && dialogueObject.HasResponses) 
            {
                break;
            }

            

            yield return null;
            // wait for user to press space to show next dialogue
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));


        }

        // show the responses
        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            // once done close the box
            closeDialogueBox();
        }
        
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);
        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                typewriterEffect.Stop();
            }
        }
    }

    private void closeDialogueBox()
    {
/*        player.disableMovement = false;*/
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
