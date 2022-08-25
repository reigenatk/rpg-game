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

    
    // a special condition is just anything other than open
    [System.Serializable]
    public class SceneTeleportSpecialConditions
    {
        public int gameDay;
        public List<GameVariablePair> extraConditions;
        public List<ChunkOfTime> validTimes;

        // if this field is null we will take it to mean that door is just locked w/ no response
        // otherwise, its the name of the node to play for the response that the other person will give
        public string knockDialogue = null;
        public bool isSpecialConditionMet()
        {
            GameState gameState = FindObjectOfType<GameState>();

            if (gameState.getGameDay() == -1 || (gameState.getGameDay() != gameDay))
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
            if (!foundValidTime)
            {
                Debug.Log("Not within any valid time chunk "); return false;
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
            return true;
        }
    }

    private GameState gameState;
    [SerializeField] private List<SceneTeleportSpecialConditions> specialTeleportConditions;

    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }

    public bool isClosed()
    {
        foreach (SceneTeleportSpecialConditions stc in specialTeleportConditions)
        {
            if (stc.isSpecialConditionMet())
            {
                return true;
            }
        }
        return false;
    }

    public void KnockOnDoor()
    {
        LevelLoader levelLoader = LevelLoader.Instance;
        // play knocking on door cutscene
        // first check what direction the door is compared to player, so we know which knocking anim to play
        Player player = FindObjectOfType<Player>();
        PlayableDirector knockingCutscene;
        if (transform.position.x <= player.transform.position.x)
        {
            StartCoroutine(playKnockAnim("left"));
        }
        else
        {
            StartCoroutine(playKnockAnim("right"));
        }

    }

    private IEnumerator playKnockAnim(string direction)
    {
        Player player = FindObjectOfType<Player>();
        SoundManager sm = FindObjectOfType<SoundManager>();
        if (direction == "left")
        {
            Debug.Log("playing left knock anim");
            player.GetComponent<Animator>().SetTrigger("knockLeft");
            sm.playSoundString("KnockOnDoor");
        }
        else if (direction == "right")
        {
            Debug.Log("playing right knock anim");
            player.GetComponent<Animator>().SetTrigger("knockRight");
            sm.playSoundString("KnockOnDoor");
        }
        yield return new WaitForSeconds(5.0f);
        playKnockDialogue();
    }

    public void playKnockDialogue()
    {
        // first check what we should do
        foreach (SceneTeleportSpecialConditions stc in specialTeleportConditions)
        {
            if (stc.isSpecialConditionMet())
            {
                // ok so check which condition it is, either no response or more dialogue
                if (stc.knockDialogue == null)
                {
                    // then no response
                    FindObjectOfType<DialogueManager>().StartDialogueString("Knock_No_Response");
                    return;
                }
                else
                {
                    // otherwise play that special dialogue
                    FindObjectOfType<DialogueManager>().StartDialogueString(stc.knockDialogue);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        // let gameState know that we are currently in this teleporter.
        gameState.curSceneTeleport = this;
        if (player != null)
        {
            // is the door openable?
            if (isClosed() == true)
            {
                FindObjectOfType<DialogueManager>().StartDialogueString("DoorIsntOpen");
                return;
            }
            
            //  Calculate players new position
            //  if new positions are specified for x or y, then use the current value

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;

            // Teleport to new scene
            LevelLoader.Instance.FadeAndLoadScene(sceneNameGoto, new Vector3(xPosition, yPosition, zPosition));

        }

    }
}
