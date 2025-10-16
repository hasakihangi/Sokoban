using System;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraResult
{
    public Dictionary<Vector2Int, Vector2Int> _cameFrom = new Dictionary<Vector2Int, Vector2Int>();
    public Dictionary<Vector2Int, float> _distanceTable = new Dictionary<Vector2Int, float>();
    
    private DijkstraResult() { }
    public static DijkstraResult Get() => new DijkstraResult();

    public void Clear()
    {
        _cameFrom.Clear();
        _distanceTable.Clear();
    }
    
    public bool ConstructPath(Vector2Int to, List<Vector2Int> path)
    {
        if (!_cameFrom.ContainsKey(to))
        {
            return false;
        }

        Vector2Int current = to;
        path.Add(to);

        while (_cameFrom.TryGetValue(current, out Vector2Int previous))
        {
            current = previous;
            path.Add(current);
        }

        path.Reverse();
        return true;
    }
}