using UnityEngine;

public class NavGridPathNode
{
    /// <summary>
    /// World position of the node
    /// </summary>
    public bool walkable;
    public Vector3 Position;
    public int gridX, gridY;
    public int gCost, hCost;
    public NavGridPathNode parent; // Change the type here

    public int fCost { get { return gCost + hCost; } }

    public NavGridPathNode(bool walkable, Vector3 position, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.Position = position;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}