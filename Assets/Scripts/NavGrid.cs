using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    [SerializeField]
    private bool _visualizeGrid;
    [SerializeField]
    private LayerMask _unwalkableMask;
    [SerializeField]
    private Vector2 _gridWorldSize;
    [SerializeField]
    private float _nodeRadius;
    
    private NavGridPathNode[,] _grid;
    float _nodeDiameter;
    int _gridSizeX, _gridSizeY;

    void Start()
    {
        _nodeDiameter = _nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new NavGridPathNode[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, _nodeRadius, _unwalkableMask));
                _grid[x, y] = new NavGridPathNode(walkable, worldPoint, x, y);
            }
        }
    }

    public NavGridPathNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + _gridWorldSize.x / 2) / _gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + _gridWorldSize.y / 2) / _gridWorldSize.y);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return _grid[x, y];
    }

    public List<NavGridPathNode> GetNeighbors(NavGridPathNode node)
    {
        List<NavGridPathNode> neighbors = new List<NavGridPathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbors.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        // Shows grid in editor view on play
        if (_visualizeGrid)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));

            if (_grid != null)
            {
                foreach (NavGridPathNode node in _grid)
                {
                    Gizmos.color = (node.walkable) ? Color.green : Color.red;
                    Gizmos.DrawCube(node.Position, Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        }
    }
}
