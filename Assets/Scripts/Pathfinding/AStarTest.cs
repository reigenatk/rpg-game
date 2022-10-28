
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    //private AStar aStar;

    [SerializeField] private NPCPath npcPath = null;
    [SerializeField] private bool moveNPC = false;

    // the relative position in the ending scene's grid that you wanna go to
    [SerializeField] private Vector2Int finishPosition;

    // an idle down anim to default play
    [SerializeField] private AnimationClip idleDownAnimationClip = null;

    // an optional anim to play once done with the movement. This can be left null, in which case idle down will play
    [SerializeField] private AnimationClip eventAnimationClip = null;

    // the scene to move to
    [SerializeField] private SceneName sceneName;
    private NPCMovement npcMovement;
    

    private void Start()
    {

        npcMovement = npcPath.GetComponent<NPCMovement>();
        npcMovement.npcFacingDirectionAtDestination = Direction.Down;
        npcMovement.npcTargetAnimationClip = idleDownAnimationClip;

    }

    private void Update()
    {
        if (moveNPC)
        {
            moveNPC = false;

            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, sceneName, new GridCoordinate(finishPosition.x, finishPosition.y), eventAnimationClip);

            npcPath.BuildPath(npcScheduleEvent, gameObject.name);

        }


    }
}


/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// a test script to see if A* is working. See lecture 86
[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    private AStar aStar;
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int finishPosition;
    [SerializeField] private Tilemap tileMapToDisplayPathOn = null;
    [SerializeField] private TileBase tileToUseToDisplayPath = null;
    [SerializeField] private bool displayStartAndFinsh = false;
    [SerializeField] private bool displayPath = false;

    private Stack<NPCMovementStep> npcMovementSteps;

    private void Awake()
    {
        aStar = GetComponent<AStar>();

        npcMovementSteps = new Stack<NPCMovementStep>();
    }


    private void Update()
    {

        if (startPosition != null && finishPosition != null && tileMapToDisplayPathOn != null && tileToUseToDisplayPath != null)
        {
            // Display start and finish tiles
            if (displayStartAndFinsh)
            {
                *//*Debug.Log("Displaying start and finish...");*//*
                // Display start tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), tileToUseToDisplayPath);

                // Display finish tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), tileToUseToDisplayPath);
            }
            else
            // Clear start and finish
            {
                // clear start tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);

                // clear finish tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
            }

            // Display path
            if (displayPath)
            {
                // Get current scene name
                Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName sceneName);

                // Build path
                aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);
                *//*              Debug.Log("Start position " + startPosition);
                                Debug.Log("Finish position " + finishPosition);
                                Debug.Log("Stack size: " + npcMovementSteps.Count);
                                Debug.Log("Scene name " + SceneManager.GetActiveScene().name);
                *//*
                // Display path on tilemap
                foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                {
                    tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), tileToUseToDisplayPath);
                }
            }
            else
            {
                // Clear path
                if (npcMovementSteps.Count > 0)
                {
                    // Clear path on tilemap
                    foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                    {
                        tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), null);
                    }

                    // Clear movement steps
                    npcMovementSteps.Clear();
                }
            }
        }
    }
}
*/