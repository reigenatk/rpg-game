using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;
    public string[] Dialogue => dialogue;

    // getters
    public Response[] Responses => responses;

    public bool HasResponses => (responses != null && responses.Length > 0);
}
