using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;


    // movement penalties are used by us, the creator of the maps, to encourage A* to use predetermined routes called "Paths"
    // otherwise A* will run as normal and just take shortest distance, which isn't always what we want.
    [Range(0, 100)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 100)]
    [SerializeField] private int defaultMovementPenalty = 0;

    // gridNodes is just an array of A* Node objects
    // an A* node has a position and a parent node
    private GridNodes gridNodes;


    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathFound = false;

    /// <summary>
    /// Builds a path for the given sceneName, from the startGridPosition to the endGridPosition, and adds movement steps to the passed in npcMovementStack.  Also returns true if path found or false if no path found.
    /// </summary>
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;

        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
            else
            {
                Debug.Log("No shortest Path found to scene " + sceneName);
            }
        }
        return false;
    }

    // this method assumes A* finished running. It looks at the parent of each node until it gets to the start node
    // this populates the stack that is passed in
    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStepStack.Push(npcMovementStep);
            Debug.Log("Pushing onto stack the coordinates: " + npcMovementStep.gridCoordinate + " For scene " + sceneName + " stack has size " + npcMovementStepStack.Count);
            nextNode = nextNode.parentNode;
        }
    }

    /// <summary>
    ///  Returns true if a path has been found
    /// </summary>
    private bool FindShortestPath()
    {
        // Add start node to open list
        Debug.Log("Start node " + startNode.gridPosition);
        openNodeList.Add(startNode);

        // Loop through open node list until empty
        while (openNodeList.Count > 0)
        {

            // Sort List
            openNodeList.Sort();

            //  current node = the node in the open list with the lowest fCost
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            // Debug.Log("[FindShortestPath] Current Node " + currentNode.gridPosition);

            // add current node to the closed list
            closedNodeList.Add(currentNode);

            // if the current node = target node
            //      then finish

            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;
        // Debug.Log("Current Node :" + currentNode.gridPosition);

        // Loop through all directions (8 of them)
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        // so you can see here, the "cost" that we artifically made is literally just added to the raw distance
                        // which would in turn make it harder for this path to be selected, since the overall H-cost would be higher
                        // Debug.Log("Movement penalty of node " + validNeighbourNode.gridPosition + " is " + validNeighbourNode.movementPenalty);
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    // A* processes/re-updates a node if the cost to get there is less than its current stated cost, or if it has not yet been processed.
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        // update the value to the better gCost
                        validNeighbourNode.gCost = newCostToNeighbour;

                        // update the hCost to be the straight distance between the current node and the target. This is called the heuristic function
                        // and can be a bunch of different things, we just choose the straight distance.
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                        // technically here we should update the fCost, but since we made fCost as a function then we have already updated it as long as we changed g and h

                        // also mark the parent of this updated node to point to current one
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    // 1 unit is just one literal X or Y unit in the Unity 2D plane
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // check if in bounds, isnt an obstacle, and isnt in the closed list (cannot re-process a node in A* once its processed)
    private Node GetValidNodeNeighbour(int neighboutNodeXPosition, int neighbourNodeYPosition)
    {
        // If neighbour node position is beyond grid then return null
        if (neighboutNodeXPosition >= gridWidth || neighboutNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // if neighbour is an obstacle or neighbour is in the closed list then skip
        Node neighbourNode = gridNodes.GetGridNode(neighboutNodeXPosition, neighbourNodeYPosition);

        if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
        {
            // Debug.Log(neighboutNodeXPosition + ", " + neighbourNodeYPosition + " Is obsctacle or closed already");
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        // Get grid properties dictionary for the scene
        SceneSave sceneSave;
        GridPropertiesManager gridPropertiesManager = FindObjectOfType<GridPropertiesManager>();
        foreach (KeyValuePair<string, SceneSave> entry in gridPropertiesManager.GameObjectSave.sceneData)
        {
            // Debug.Log(entry.Key);
        }
        if (gridPropertiesManager.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            // Get Dict grid property details
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                // Get grid height and width
                if (gridPropertiesManager.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    // Create nodes grid based on grid properties dictionary
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    // Create openNodeList
                    openNodeList = new List<Node>();

                    // Create closed Node List
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                int xStartPosRelative = startGridPosition.x - gridOrigin.x;
                int yStartPosRelative = startGridPosition.y - gridOrigin.y;
                int xEndPosRelative = endGridPosition.x - gridOrigin.x;
                int yEndPosRelative = endGridPosition.y - gridOrigin.y;
                Debug.Log("xPosRel is " + xStartPosRelative + " gridOrigin.x is " + gridOrigin.x + " yPosRel " + yStartPosRelative + " gridOrigin.y is " + gridOrigin.y);
                Debug.Log("StartGridPos " + startGridPosition + " endGridPos " + endGridPosition);
                // Populate start node
                startNode = gridNodes.GetGridNode(xStartPosRelative, yStartPosRelative);
                
                // Populate target node
                targetNode = gridNodes.GetGridNode(xEndPosRelative, yEndPosRelative);

                // populate obstacle and path info for grid (aka set the .isObstacle, .movementPenalty fields)
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = gridPropertiesManager.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);

                        if (gridPropertyDetails != null)
                        {
                            // If NPC obstacle
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                // Debug.Log("coords " + x + ", " + y + " is path");
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                        }
                        else
                        {
                            // Debug.Log("coords " + x + ", " + y + " is default");
                            Node node = gridNodes.GetGridNode(x, y);
                            node.movementPenalty = defaultMovementPenalty;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        Debug.Log("Set gridnodes property fields");
        return true;
    }
}