using System;
using UnityEngine;

public struct DijkstraNode : IComparable<DijkstraNode>
{
    public Vector2Int gridPos;
    public float cost;

    public DijkstraNode(Vector2Int gridPos, float cost)
    {
        this.gridPos = gridPos;
        this.cost = cost;
    }

    public int CompareTo(DijkstraNode other)
    {
        return cost.CompareTo(other.cost);
    }
}