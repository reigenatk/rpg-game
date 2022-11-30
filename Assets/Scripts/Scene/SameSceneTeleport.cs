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
    [SerializeField] private float extraDelay; // change this if you want it to take a little longer to go to next scene 
    [SerializeField] private AudioClip teleportSound; // a sound to optionally play before teleporting
    GameState gameState;
    [SerializeField] SpriteRenderer teleportSprite;

    // if it has a sprite to go with the telporter (AKA dream stuff)
    bool isThereWorkingTeleport = true;

    void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }
    private void Update()
    {


        foreach (YarnVariablePair yv in YarnVariables)
        {
            if (gameState.getYarnVariable(yv.YarnVariable) != yv.desiredValue)
            {
                isThereWorkingTeleport = false;
            }
        }
    
        if (teleportSprite != null && isThereWorkingTeleport)
        {
            teleportSprite.enabled = true;

        }
    }
    IEnumerator TeleportPlayer(Player player)
    {

        Debug.Log("[Dream Scene Teleport] player from position " + player.transform.position + " to " + scenePositionGoto);
        float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

        float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

        float zPosition = 0f;
        yield return StartCoroutine(LevelLoader.Instance.Fade(1.0f));

        // play optional sound!
        if (teleportSound != null)
        {
            Debug.Log("Playing sound effect between same scene teleports: " + teleportSound.name);
            GameObject.Find("Audio").GetComponent<AudioSource>().PlayOneShot(teleportSound);

        }

        yield return new WaitForSeconds(extraDelay);
        player.transform.position = new Vector3(xPosition, yPosition, zPosition);
        yield return StartCoroutine(LevelLoader.Instance.Fade(0.0f));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        // check if each condition is met. if something isnt right yet, dont teleport.
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
