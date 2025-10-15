using System;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public GridMapInitializer gridMapInitializer;
    public GridMapDebugger gridMapDebugger;

    public GridMap<GridCell> gridMap;

    private void Awake()
    {
        gridMapInitializer = FindObjectOfType<GridMapInitializer>();
        gridMapDebugger = FindObjectOfType<GridMapDebugger>();
    }

    private void Start()
    {
        gridMap = gridMapInitializer.GetGridMap<GridCell>();
        gridMapDebugger.Set(gridMap);
    }
    
    
}