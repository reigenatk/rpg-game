using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;
using static GameState;
using static TimeManager;

public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();

    [SerializeField] private float extraDelay; // change this if you want it to take a little longer to go to next scene 
    [SerializeField] private AudioClip teleportSound; // a sound to optionally play before teleporting
 

    // check if there's any dialogue to play on this teleport
    bool isThereDialogueToPlay = false;
    // check if we should teleport the player after this dialogue or not
    bool shouldTeleportAfterDialogue = false;
    bool shouldShowTeleportSprite = false;

    // optional sprite renderer
    // basically this sprite renderer is turned ON when the conditions to teleport are MET
    // and otherwise, its turned off.
    [SerializeField] SpriteRenderer teleportSprite;


    // if it has a sprite to go with the telporter (AKA dream stuff). Check this every frame (i know not efficient but whatever)
    bool isThereWorkingTeleport = true;

    // a special condition is just anything other than open
    [System.Serializable]
    public class SceneTeleportSpecialConditions
    {
        public int gameDay;
        public List<GameVariablePair> extraConditions;
        // yarn variables and their desired values
        [SerializeField] List<YarnVariablePair> YarnVariables;
        public List<ChunkOfTime> validTimes;

        // if this field is null we will take it to mean that door is just locked w/ no response
        // otherwise, its the name of the node to play for the response that the other person will give
        public string knockDialogue = null;
        public bool shouldTeleportAfter;

        public bool isSpecialConditionMet()
        {
            GameState gameState = FindObjectOfType<GameState>();

            if (gameDay != -1 && (gameState.getGameDay() != gameDay))
            {
                Debug.Log("Game day didn't match, value was " + gameState.getGameDay());
                return false;
            }

            bool foundValidTime = false;
            // check if current time falls inside of chunk of valid times
            foreach (ChunkOfTime cot in validTimes)
            {
                if (cot.isInChunk(TimeManager.Instance.gt))
                {
                    // its in the chunk, player is valid
                    foundValidTime = true;
                    break;
                }
            }
            if (!foundValidTime && validTimes.Count != 0)
            {
                Debug.Log("Not within any valid time chunk ");
                return false;
            }


            // check if each condition is met. if not, set isPlaying to false
            foreach (GameVariablePair gv in this.extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    Debug.Log("Broken on " + gv.variable.ToString());
                    return false;
                }
            }
            foreach (YarnVariablePair yv in YarnVariables)
            {
                if (gameState.getYarnVariable(yv.YarnVariable) != yv.desiredValue)
                {
                    Debug.Log("Broken on yarn variable " + yv.YarnVariable.ToString() + " value was " + gameState.getYarnVariable(yv.YarnVariable) + " expected value is " + yv.desiredValue);
                    return false;
                }
            }

            return true;
        }
    }

    private GameState gameState;
    [SerializeField] private List<SceneTeleportSpecialConditions> specialTeleportConditions;
    [SerializeField] private List<SceneTeleportSpecialConditions> spriteShowConditions;

    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }
    private void Update()
    {
        foreach (SceneTeleportSpecialConditions ssc in spriteShowConditions)
        {
            if (ssc.isSpecialConditionMet())
            {
                shouldShowTeleportSprite = true;
            }
            else
            {
                shouldShowTeleportSprite = false;
            }
        }

        if (teleportSprite != null && shouldShowTeleportSprite)
        {
            teleportSprite.sortingLayerName = "Character";
        }
        else
        {
            if (teleportSprite != null)
            {
                teleportSprite.sortingLayerName = "Default";

            }
        }
    }
    /*    public void KnockOnDoor()
        {
            // play knocking on door cutscene
            // first check what direction the door is compared to player, so we know which knocking anim to play
            Player player = FindObjectOfType<Player>();

            if (knockCutsceneToPlay == "KnockLeft")
            {
                player.setPosition(player.transform.position + new Vector3(0.1f, 0, 0));
            }
            else if (knockCutsceneToPlay == "KnockRight")
            {
                player.setPosition(player.transform.position + new Vector3(-0.1f, 0, 0));
            }
            else if (knockCutsceneToPlay == "KnockUp")
            {
                player.setPosition(player.transform.position + new Vector3(0, -0.1f, 0));
            }

            LevelLoader.Instance.playCutscene(knockCutsceneToPlay);


        }*/

    public void playKnockDialogue()
    {
        Debug.Log("checking for dialogue to play before teleport");
        // first check what we should do
        foreach (SceneTeleportSpecialConditions stc in specialTeleportConditions)
        {
            if (stc.isSpecialConditionMet())
            {
                Debug.Log("Special condition met");
                if (stc.knockDialogue != null)
                {
                    // play that special dialogue
                    Debug.Log("Playing dialogue before teleport: " + stc.knockDialogue);
                    FindObjectOfType<DialogueManager>().StartDialogueString(stc.knockDialogue);
                    isThereDialogueToPlay = true;
                    if (stc.shouldTeleportAfter)
                    {
                        Debug.Log("Should Teleport");
                        shouldTeleportAfterDialogue = true;
                    }

                    return; // found a working dialogue so lets return
                }
                else
                {
                    
                }
            }
        }
        Debug.Log("No dialogue assigned with this special condition, going to just teleport then");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with teleporter " + gameObject.name);
        Player player = collision.GetComponent<Player>();

        // let gameState know that we are currently in this teleporter.
        if (gameState.curSceneTeleport == this)
        {
            Debug.Log("We were already in this teleporter, don't run for now");
            // we were just in this, so dont run it again for now
            return;
        }
        else
        {
            gameState.curSceneTeleport = this;
        }
        if (player != null)
        {
            // check if there's dialogue to play
            if (FindObjectOfType<LevelLoader>().dialoguesEnabled)
            {
                // only play if we're not debugging
                playKnockDialogue();
            }


            //  Calculate players new position
            //  if new positions are specified for x or y, then use the current value
            if (isThereDialogueToPlay)
            {
                StartCoroutine(WaitToteleportPlayer(player));
            }
            else
            {
                // just teleport the player instantly
                TeleportPlayer(player);
            }
            
        }

    }

    public void TeleportPlayer(Player player)
    {
        GameState gameState = FindObjectOfType<GameState>();
        Debug.Log("teleporting player from " + gameState.getCurrentSceneEnum() + " to " + sceneNameGoto);
        float xPosition = scenePositionGoto.x;

        float yPosition = scenePositionGoto.y;

        float zPosition = 0f;
        LevelLoader.Instance.FadeAndLoadScene(sceneNameGoto, new Vector3(xPosition, yPosition, zPosition), delay: extraDelay, clipToPlay: teleportSound);
    }

    public IEnumerator WaitToteleportPlayer(Player player)
    {
        while (gameState.currentRunningDialogueNode != "" && gameState.currentRunningDialogueNode != null)
        {
            Debug.Log("In dialogue still");
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Is dialogue to play set to null again");
        isThereDialogueToPlay = false; // dialogue is done


        // Teleport to new scene (pass in an optional sound to play as well)
        // never mind- do all the teleporting via yarn instead. Cuz sometimes we can just play a dialgoue and then NOT teleport the player
        // or sometimes we wanna play a cutscene after playing the dialogue.


        // if we should teleport after dialogue as specified by the field, then do it
        if (shouldTeleportAfterDialogue)
        {
            shouldTeleportAfterDialogue = false; // reset this for next use
            TeleportPlayer(player);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null && player.GetComponent<BoxCollider2D>().enabled == true)
        {
            if (gameState.curSceneTeleport == this)
            {
                gameState.curSceneTeleport = null;
            }
        }
    }
}   
