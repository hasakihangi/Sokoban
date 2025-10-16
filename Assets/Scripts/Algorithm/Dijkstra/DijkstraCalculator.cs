using System;
using System.Collections.Generic;
using UnityEngine;

// 如果只是计算, 将参数从外边传入是不是会好一些?
// 还需要一个isPosEnable函数, 需要零分配, 写定就是0分配了
// 这个跟GridMap实际运用时都写定会比较好
public class DijkstraCalculator<T> 
{
    private Heap<DijkstraNode> _openSet = new Heap<DijkstraNode>(true);
    private HashSet<Vector2Int> _closedSet = new HashSet<Vector2Int>();

    private Func<Vector2Int, Vector2Int, float> _getCost;
    private Func<Vector2Int, bool> _isPosEnable;
    
    private DijkstraCalculator(Func<Vector2Int, Vector2Int, float> getCost, Func<Vector2Int, bool> isPosEnable)
    {
        this._getCost = getCost;
        this._isPosEnable = isPosEnable;
    }
    
    public static DijkstraCalculator<T> Get(Func<Vector2Int, Vector2Int, float> getCost, Func<Vector2Int, bool> isPosEnable) => new DijkstraCalculator<T>(getCost, isPosEnable);
    
    public bool Calculate(T[,] board, Vector2Int origin, float costValue, DijkstraResult result)
    {
        if (!IsWithinBoard(board, origin))
        {
            return false;
        }
        
        var distanceTable = result._distanceTable;
        var cameFrom = result._cameFrom;
        
        distanceTable[origin] = 0f;
        _openSet.Insert(new DijkstraNode(origin, 0f));
        
        while (_openSet.Count > 0)
        {
            DijkstraNode currentNode = _openSet.PullRoot();
            Vector2Int currentPos = currentNode.gridPos;
            
            if (!_closedSet.Add(currentPos))
            {
                continue;
            }
            
            var directions = s_Directions;
            foreach (var direction in directions)
            {
                var neighborPos = currentPos + direction;

                if (!IsWithinBoard(board, neighborPos))
                    continue;

                if (_closedSet.Contains(neighborPos))
                    continue;

                if (!_isPosEnable(neighborPos))
                    continue;
                
                float cost = distanceTable[currentPos] + _getCost(currentPos, neighborPos);

                if (cost > costValue)
                    continue;

                if (!distanceTable.ContainsKey(neighborPos) || cost < distanceTable[neighborPos])
                {
                    cameFrom[neighborPos] = currentPos;
                    distanceTable[neighborPos] = cost;
                    _openSet.Insert(new DijkstraNode(neighborPos, cost));
                }
            }
        }
        
        return true;
    }

    private static readonly Vector2Int[] s_Directions = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1) };
    
    private bool IsWithinBoard(T[,] board, Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= board.GetLength(0) ||
            gridPos.y < 0 || gridPos.y >= board.GetLength(1))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}