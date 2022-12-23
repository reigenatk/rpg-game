using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderDarkScene : MonoBehaviour
{
    GameState gameState;
    DialogueManager dm;
    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
        dm = FindObjectOfType<DialogueManager>();
        FindObjectOfType<GameUI>().disableUI();
        dm.StartDialogueString("TrainStation");
    }

    
}
