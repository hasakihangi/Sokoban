using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridMapInitializer : MonoBehaviour  
{
    public Collider boardPlane; // 既然这个就有collider, 射线检测是一定可以cast到的? 不一定, 可能不是检测的Layer, 目前仅由射线检测到walkableLayer, 才是Enable, 其余都是Disable
    // public LayerMask obstacleLayer;
    public LayerMask walkableLayer;
    public float rayCastHeight = 30f;
    public float rayCastLength = 60f;
    public float cellSize = 1;
    
    private LayerMask RayCastDetectLayer => /*obstacleLayer |*/ walkableLayer;

    public GridMap<T> GetGridMap<T>() where T: struct, ICell
    {
        GridMap<T> gridMap = CreateGridMap<T>();
        SetupGridMap(gridMap);
        return gridMap;
    }

    private GridMap<T> CreateGridMap<T>() where T: struct, ICell
    {
        Bounds bounds = boardPlane.bounds;
        Vector3 boundsOrigin = bounds.center - bounds.extents;
        Vector3 size = bounds.size;
        int width = (int)(size.x/cellSize);
        int height = (int)(size.z/cellSize);
        GridMap<T> gridMap = new GridMap<T>(boundsOrigin, cellSize, width, height);
        return gridMap;
    }

    private void SetupGridMap<T>(GridMap<T> gridMap) where T: struct, ICell
    {
        for (int j = 0; j < gridMap.Height; j++)
        {
            for (int i = 0; i < gridMap.Width; i++)
            {
                gridMap[i,j].Init();
                Vector3 worldPos = gridMap.GridPosToWorldPos(new Vector2Int(i,j));
                Ray ray = new Ray(worldPos + rayCastHeight * Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, rayCastLength, RayCastDetectLayer))
                {
                    if (LayerMaskUtils.IsMaskContainAny(walkableLayer, 1 << hit.collider.gameObject.layer))
                    {
                        gridMap[i,j].Enable = true;
                        continue;
                    }
                }
                
                gridMap[i,j].Enable = false;
            }
        }
    }
}
