using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public static AStar Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<NavGridPathNode> GetPath(NavGrid navigationGrid, Vector3 startPos, Vector3 targetPos)
    {
        List<NavGridPathNode> pathNodes = new List<NavGridPathNode>();

        NavGridPathNode startNode = navigationGrid.NodeFromWorldPoint(startPos);
        NavGridPathNode targetNode = navigationGrid.NodeFromWorldPoint(targetPos);

        List<NavGridPathNode> openSet = new List<NavGridPathNode>();
        HashSet<NavGridPathNode> closedSet = new HashSet<NavGridPathNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            NavGridPathNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathNodes = RetracePath(startNode, targetNode);
                break;
            }

            foreach (NavGridPathNode neighbor in navigationGrid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return pathNodes;
    }

    List<NavGridPathNode> RetracePath(NavGridPathNode startNode, NavGridPathNode endNode)
    {
        List<NavGridPathNode> path = new List<NavGridPathNode>();
        NavGridPathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(NavGridPathNode nodeA, NavGridPathNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX + dstY;
    }
}
