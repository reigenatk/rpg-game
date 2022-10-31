using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : Singleton<NPCManager>
{
    // for cross scene movement
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;


    // ok so you might wonder what this is. Due to me fkin up and not following instructions (but also partly my choice), we didnt set all the tilemap grid's transforms to 0,0,0
    // as a result we need this to work. Whenever you add a new scene, simply put the transform of the tilemap grid in this list, else the NPC moving between scenes wont work. Simple as.
    [SerializeField] List<TilemapOffset> tilemapOffsets;

    [System.Serializable]
    public class TilemapOffset
    {
        public Vector3 offset;
        public SceneName sceneName;

    }

    public Vector3 getTilemapOffset(SceneName sceneName)
    {
        foreach (TilemapOffset to in tilemapOffsets)
        {
            if (to.sceneName == sceneName)
            {
                return to.offset;
            }
        }
        Debug.Log("TILEMAP for " + sceneName + "NOT FOUND");
        return Vector3.zero;
    }

    [HideInInspector]
    public NPC[] npcArray;

    private AStar aStar;

    protected override void Awake()
    {
        base.Awake();

        // Create sceneRoute dictionary
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                // Check for duplicate routes in dictionary
                if (sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("** Duplicate Scene Route Key Found ** Check for duplicate routes in the scriptable object scene route list");
                    continue;
                }

                // Add route to dictionary
                
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
                Debug.Log("Added to sceneRouteDictionary, an entry with key " + so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString());
            }
        }

        aStar = GetComponent<AStar>();

        // Get NPC gameobjects in scene
        npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        // after scene has loaded, set all the NPCs to be active so they start moving around
        SetNPCsActiveStatus();
    }

    // this methods checks what scene it is, and checks whether or not each NPC should be active in this scene
    private void SetNPCsActiveStatus()
    {
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;

        // Get scene route from dictionary
        if (sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}