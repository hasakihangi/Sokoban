using System;
using System.Collections.Generic;
using DebugTool;
using UnityEngine;


public class DebugLinesController: DebugController<(Vector3, Vector3)>, IDebugGizmosController
{
    public Color color;

    public DebugLinesController(Color color)
    {
        this.color = color;
    }

    public void AddLine(Vector3 point1, Vector3 point2)
    {
        _items.Add((point1, point2));
    }

    public void AddLines(IEnumerable<(Vector3, Vector3)> lines)
    {
        _items.AddRange(lines);
    }

    public void DrawGizmos()
    {
        Gizmos.color = color;
        foreach (var line in _items)
        {
            Gizmos.DrawLine(line.Item1, line.Item2);
        }
    }
}

// public struct DebugLine
// {
//     public Vector3 point1;
//     public Vector3 point2;
//
//     public DebugLine(Vector3 point1, Vector3 point2, Color color)
//     {
//         this.point1 = point1;
//         this.point2 = point2;
//     }
//
//     public void DrawGizmos()
//     {
//         Gizmos.DrawLine(point1, point2);
//     }
// }
