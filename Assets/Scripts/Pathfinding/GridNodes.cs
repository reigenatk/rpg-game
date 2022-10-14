using UnityEngine;

// gridnodes simply is a grid of A* Node objects
public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNode;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        // this creates an empty array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    // this is just a getter 
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height && xPosition >= 0 && yPosition >= 0)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range " + "requested x was " + xPosition + " width is " + width + " requested y was " + yPosition + " height is " + height);
            return null;
        }
    }
}
