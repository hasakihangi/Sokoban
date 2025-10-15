using System;
using UnityEngine;

public partial class GridMap<T> where T: struct, ICell
{
    public Vector3 _originPos;
    public float _cellSize;
    public T[,] _board;
    
    public int Width => _board.GetLength(0);
    public int Height => _board.GetLength(1);
    
    public GridMap(Vector3 originPos, float cellSize, int width, int height)
    {
        this._originPos = originPos;
        this._cellSize = cellSize;
        this._board = new T[width,height];
    }

    public bool Get(Vector2Int gridPos, out T value)
    {
        value = default;
        if (IsGridWithin(gridPos))
        {
            value = _board[gridPos.x, gridPos.y];
            return true;
        }

        return false;
    }

    public void Set(Vector2Int gridPos, T value)
    {
        if (IsGridWithin(gridPos))
        {
            _board[gridPos.x, gridPos.y] = value;
        }
    }
    
    public ref T this[int x, int y]
    {
        get
        {
            return ref _board[x,y];
        }
    }

    public ref T this[Vector2Int gridPos]
    {
        get
        {
            return ref _board[gridPos.x, gridPos.y];
        }
    }
}