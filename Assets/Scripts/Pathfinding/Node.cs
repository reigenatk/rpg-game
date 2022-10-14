using System;
using UnityEngine;

// a node in A* is simply represented by a single tile on our tilemap in unity
// has properties like whether or not it is an obstacle, the g and h costs for A*, and also the parent node (which is also an A* concept)
public class Node : IComparable<Node>
{

    public Vector2Int gridPosition;
    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from finishing node
    public bool isObstacle = false;
    public int movementPenalty;
    public Node parentNode;


    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    // this satisfies the IComparable interface and is just a custom comparator
    // we need this because we will call .Sort() which is a .NET function on a list of Node objects
    // and since in A* we process the nodes with the lowest fcost first (sum of g and hcost), then we must sort this way.
    public int CompareTo(Node nodeToCompare)
    {
        // compare will be <0 if this instance Fcost is less than nodeToCompare.FCost
        // compare will be >0 if this instance Fcost is greater than nodeToCompare.FCost
        // compare will be ==0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
