using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameState;
using static TimeManager;

public class PlayerLoad : MonoBehaviour
{
    // the actual object to load
    [SerializeField]        
    public GameObject player;
    private GameState gameState;
    private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class PlayerCondtional
    {
        // the day it must be, set -1 if any day works
        public int dayToPlay;
        // the scene this will play on
        public SceneName scene;
        // some extra conditions that must be either true or false
        public List<GameVariablePair> extraConditions;

        // all the valid times for this player to be in this scene
        public List<ChunkOfTime> validTimes;

        public Vector3 loadPosition;
        public bool shouldAddPlayer(SceneName scene)
        {
            GameState gameState = FindObjectOfType<GameState>();
            // Debug.Log("Consindering " + c.cutsceneToPlay.ToString());

            // obviously check if scenes match
            if (scene != this.scene)
            {
                return false;
            }

            if (gameState.getGameDay() == -1 || (gameState.getGameDay() != this.dayToPlay))
            {
                // Debug.Log("Game day didn't match, value was " + gameState.getGameDay());
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
            if (!foundValidTime) return false;
           

            // check if each condition is met. if not, set isPlaying to false
            foreach (GameVariablePair gv in this.extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    // Debug.Log("Broken on " + gv.variable.ToString());
                    return false;
                }
            }
            return true;
        }
    }
    [SerializeField] List<PlayerCondtional> playerLoadConditions;

    // this will be called from LevelLoader, right before we are loading scenes
    public bool checkAddPlayer(SceneName sceneName)
    {
        foreach (PlayerCondtional pc in playerLoadConditions)
        {
            // if any of the conditions is true, then ok add the player into that scene
            if (pc.shouldAddPlayer(sceneName))
            {
                // return because we should only ever add once, anyways
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // if (gameState.getYarnVariable("$coomerWasExposed") == true)
        // {
        //     spriteRenderer.enabled = false;
        // }
    }
}
