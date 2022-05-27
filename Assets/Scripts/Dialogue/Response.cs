using UnityEngine;

// need this to be able to see the response objects in the editor 
[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private DialogueObject dialogueObject;

    public string ResponseText => responseText;

    public DialogueObject Dialogue => dialogueObject;
}
