using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using static GameState;

public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();

    [System.Serializable]
    public class SceneTeleportBadConditions
    {
        public int gameDay;
        public List<GameVariablePair> extraConditions;
    }
    
    private GameState gameState;
    [SerializeField] private List<SceneTeleportBadConditions> badTeleportConditions;
    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }

    public bool isClosed()
    {
        foreach (SceneTeleportBadConditions stc in badTeleportConditions)
        {
            if (stc.gameDay != gameState.getGameDay()) continue;
            // check if the door is open
            foreach (GameVariablePair gvp in stc.extraConditions)
            {
                bool works = true;
                if (gameState.getGameVariableEnum(gvp.variable) != gvp.desiredValue)
                {
                    // ok, some condition isn't being met
                    works = false;
                }
                if (works) return true;

            }
        }
        return false;
    }

    public void KnockOnDoor()
    {

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
