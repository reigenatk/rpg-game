using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameState;

public class PlayerLoad : MonoBehaviour
{
    // the actual object to load
    [SerializeField]        
    public GameObject player;

    [System.Serializable]
    public class PlayerCondtional
    {
        // the day it must be
        public int dayToPlay;
        // the scene this will play on
        public SceneName scene;
        // some extra conditions that must be either true or false
        public List<GameVariablePair> extraConditions;

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
            if (gameState.getGameDay() != this.dayToPlay)
            {
                // Debug.Log("Game day didn't match, value was " + gameState.getGameDay());
                return false;
            }

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
}
