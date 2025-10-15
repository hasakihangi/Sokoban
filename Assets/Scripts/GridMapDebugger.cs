using System;
using UnityEngine;

public class GridMapDebugger: MonoBehaviour
{
    public bool debug = true;
    public GridMap<GridCell> gridMap = null;
    
    private bool Enable => gridMap != null && debug && Application.isEditor && Application.isPlaying;

    public void Set(GridMap<GridCell> gridMap)
    {
        this.gridMap = gridMap;
    }
    
    private void OnDrawGizmos()
    {
        if (Enable)
        {
            for (int j = 0; j < gridMap.Height; j++)
            {
                for (int i = 0; i < gridMap.Width; i++)
                {
                    Vector3 worldPos = gridMap.GridPosToWorldPos(i,j);
                    Gizmos.color = gridMap[i, j].Enable ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(worldPos, 0.1f);
                }
            }
        }
    }
}