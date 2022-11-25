using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameState;


// a same scene teleport DOES NOT LOAD a different scene. All it does is teleport based on some conditions to SOME OTHER location in the same scene.
// for instance I use this in the dreams to conveniently teleport the player to different places 
// after a certain first scene has finished. Cuz I'm too lazy to make a new scene for each separate part of the dream.
public class SameSceneTeleport : MonoBehaviour
{

    // the pos to go to
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();

    // yarn variables and their desired values
    [SerializeField] List<YarnVariablePair> YarnVariables;
    GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }

    IEnumerator TeleportPlayer(Player player)
    {

        Debug.Log("[Same Scene Teleport] player from position " + player.transform.position + " to " + scenePositionGoto);
        float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

        float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

        float zPosition = 0f;
        yield return StartCoroutine(LevelLoader.Instance.Fade(1.0f));
        player.transform.position = new Vector3(xPosition, yPosition, zPosition);
        yield return StartCoroutine(LevelLoader.Instance.Fade(0.0f));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        // check if each condition is met. if not, set isPlaying to false
        foreach (YarnVariablePair yv in YarnVariables)
        {
            if (gameState.getYarnVariable(yv.YarnVariable) != yv.desiredValue)
            {
                Debug.Log("Broken on " + yv.YarnVariable);
                return;
            }
        }

        // if we get here, all conditions were met.
        StartCoroutine(TeleportPlayer(player));
    }
}
