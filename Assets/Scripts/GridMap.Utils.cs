using System;
using UnityEngine;

public partial class GridMap<T> where T: struct, ICell
{
    public bool IsEnable(Vector2Int gridPos)
    {
        if (this.IsGridWithin(gridPos))
        {
            T cell = _board[gridPos.x, gridPos.y];
            return cell.Enable;
        }
        else
        {
            return false;
        }
    }
    
    public bool IsGridWithin(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= _board.GetLength(0) ||
            gridPos.y < 0 || gridPos.y >= _board.GetLength(1))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Vector3 GridPosToWorldPos(int x, int y)
    {
        return GridPosToWorldPos(new Vector2Int(x,y));
    }
    
    public Vector3 GridPosToWorldPos(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(
            _originPos.x + gridPos.x * _cellSize + 0.5f * _cellSize,
            CellHeight(gridPos), 
            _originPos.z + gridPos.y * _cellSize + 0.5f * _cellSize
        );
        return worldPos;
    }

    public Vector2Int GridPosFromWorldPos(Vector3 worldPos)
    {
        Vector2 originPos = _originPos.ToVector2();
        Vector2 localPos = new Vector2(worldPos.x, worldPos.z) - originPos;
        Vector2Int gridPos = LocalPosToGridPos(localPos);
        return gridPos;
    }
    
    private float CellHeight(int x, int y) => _originPos.y;
    private float CellHeight(Vector2Int gridPos) => CellHeight(gridPos.x, gridPos.y);
    
    private Vector2Int LocalPosToGridPos(Vector2 localPos)
    {
        Vector2Int gridPos = new Vector2Int((int)(localPos.x / _cellSize), (int)(localPos.y / _cellSize));
        return gridPos;
    }
}