using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> npcMovementStepStack;

    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent, string nameOfPersonToBuildPathFor)
    {
        Debug.Log("Building path for " + nameOfPersonToBuildPathFor);
        ClearPath();

        // If schedule event is for the same scene as the current NPC scene
        Debug.Log("checking: npcscheduleevent.toSceneName is " + npcScheduleEvent.toSceneName + " vs npcMovement.npcCurrentScene is " + npcMovement.npcCurrentScene);
        if (npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            Debug.Log("For NPC " + nameOfPersonToBuildPathFor + " npcmovement scene " + npcMovement.npcCurrentScene + " currentGridPos " + npcCurrentGridPosition + " targetGridPos " + npcTargetGridPosition);
            // Build path and add movement steps to movement step stack
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);

        }
        // else if the schedule event is for a location in another scene
        else if (npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;

            // Get scene route matchingSchedule
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());
            Debug.Log("tried to access entry with key " + npcMovement.npcCurrentScene.ToString() + npcScheduleEvent.toSceneName.ToString());

            // Has a valid scene route been found?
            if (sceneRoute != null)
            {
                // Loop through scene paths in reverse order

                for (int i = sceneRoute.scenePathList.Count - 1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;

                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    // Check if this is the final destination (MEANING ITS 99999 or whatever absurdly large value of 9s we set it to, in the scriptable object).
                    if (scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        Debug.Log("1");
                        // If so use final destination grid cell
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        Debug.Log("2");
                        // else use scene path to position
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    // Check if this is the starting position
                    if (scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        Debug.Log("3");
                        // if so use npc position
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        Debug.Log("4");
                        // else use scene path from position
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);

                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    Debug.Log("Scenepath found for " + gameObject.name + " fromGridPosition is " + fromGridPosition + " toGridPosition is " + toGridPosition + " going to scene " + npcScheduleEvent.toSceneName);

                    // Build path and add movement steps to movement step stack
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
            }
            else
            {
                // if no scene path specified, lets just move the person there instantly. Think the fast track part of A* if it lags behind in time
                Debug.Log("No scene path, moving " + gameObject.name + "to " + npcScheduleEvent.toGridCoordinate.x + ", " + npcScheduleEvent.toGridCoordinate.y + " in scene " + npcScheduleEvent.toSceneName);
                NPCMovement npcMovement = GetComponent<NPCMovement>();

                // gotta do this so that the animations are gonna be played properly. Cuz its still gonna use NPCMovement to trigger the stuff
                npcMovement.SetScheduleEventDetails(npcScheduleEvent);
                Vector3 npcNextWorldPosition = npcMovement.GetWorldPosition(new Vector3Int(npcScheduleEvent.toGridCoordinate.x, npcScheduleEvent.toGridCoordinate.y, 0), npcScheduleEvent.toSceneName);
                GetComponent<Rigidbody2D>().position = npcNextWorldPosition;
                npcMovement.npcIsMoving = false;
                npcMovement.npcCurrentScene = npcScheduleEvent.toSceneName;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        // Debug.Log("Number of stuffs on stack " + npcMovementStepStack.Count);
        // If stack count >1, update times and then pop off 1st item which is the starting position
        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop(); // discard starting step

            // Debug.Log("Set schedule event details");
            // Set schedule event details in NPC movement. This actually triggers the animations and the movement
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    /// <summary>
    /// Update the path movement steps with expected gametime
    /// </summary>
    public void UpdateTimesOnPath()
    {
        // Get current game time
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if (previousNPCMovementStep == null)
                previousNPCMovementStep = npcMovementStep;

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            // depending on wherther there is diagonal movement, populate a timeSpan object that dictates how long it should take
            // for NPC to get to the next location. Formula is distance / speed. This is in real time btw, since we divide by seconds per game second
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNPCMovementStep = npcMovementStep;
        }

    }

    /// <summary>
    /// returns true if the previous movement step is diagonal to movement step, else returns false
    /// </summary>
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if ((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}