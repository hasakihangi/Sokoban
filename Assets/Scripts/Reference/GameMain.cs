using System;
using System.Collections;
using System.Collections.Generic;
using Game.Record;
using Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour
{
     [SerializeField] private Camera usingCam;
     
     private bool[,] _map = new bool[0, 0];
     private int MapWidth => _map.GetLength(0);
     private int MapHeight => _map.GetLength(1);

     private bool InMap(int x, int y) => x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
     private bool InMap(Vector2Int g) => InMap(g.x, g.y);
     
     private List<MapTile> _grids = new List<MapTile>();
     private List<Box> _boxes = new List<Box>();
     private List<Vector2Int> _goals = new List<Vector2Int>();
     private List<Character> _characters = new List<Character>();
     private Character _current;
     private List<Vector2Int> _wishToMoveDir = new List<Vector2Int>();

     private TimelineManager _timeline = new TimelineManager();

     private List<MoveRecord> _undoRecord = new List<MoveRecord>();
     private List<MoveRecord> _redoRecord = new List<MoveRecord>();

     private bool _acceptInputInPerformance = false;

     private enum BattleState
     {
          PlayerControl,
          Performance
     }

     private BattleState _state = BattleState.PlayerControl;

     private void Start()
     {
          CreateMap(new Vector2Int(Random.Range(5, 10), Random.Range(5, 10)));
     }

     private void Update()
     {
          switch (_state)
          {
               case BattleState.PlayerControl:
                    _acceptInputInPerformance = false;
                    CheckForDirectionControl();
                    StartMoveCharacter();
                    break;
               case BattleState.Performance:
                    _timeline.Update(Time.deltaTime);
                    CheckForDirectionInPerformance();
                    break;
          }
     }

     private void CheckForDirectionInPerformance()
     {
          if (!_acceptInputInPerformance) return;
          Debug.Log("New input in performance " + _wishToMoveDir.Count);
          _wishToMoveDir.Clear();
          CheckForDirectionControl(true);
     }

     private void CheckForDirectionControl(bool onlyOne = false)
     {
          if (_current)
          {
               float xInput = Input.GetAxis("Horizontal");
               float yInput = Input.GetAxis("Vertical");
               Vector2Int mDir = Vector2Int.zero;
               bool affectInput = false;
               if (Mathf.Abs(xInput) >= 0.2f)
               {
                    mDir = xInput > 0 ? Vector2Int.right : Vector2Int.left;
                    affectInput = true;
               }
               else if (Mathf.Abs(yInput) >= 0.2f)
               {
                    mDir = yInput > 0 ? Vector2Int.up : Vector2Int.down;
                    affectInput = true;
               }

               if (affectInput)
               {
                    if (onlyOne && _wishToMoveDir.Count > 0) _wishToMoveDir.Clear();
                    _wishToMoveDir.Add(mDir);
               }
               
          }
     }

     private void CreateMap(Vector2Int mapSize)
     {
          _map = new bool[mapSize.x, mapSize.y];
          
          //地图块  todo 地图全都是平地
          foreach (MapTile grid in _grids) Destroy(grid.gameObject);
          _grids.Clear();
          _goals.Clear();
          for (int i = 0; i < MapWidth; i++)
          for (int j = 0; j < MapHeight; j++)
          {
               GameObject go = Instantiate(AssetManager.MapGrid);
               MapTile mt = go.GetComponent<MapTile>();
               bool isGoal = i == MapWidth - 1 && j == MapHeight - 1; //todo 写死
               mt.Set(Instantiate(AssetManager.SokobanImage("Sokoban_87")), true, isGoal);
               mt.transform.position = MapPosition.GetWorldPosByGrid(i, j);
               mt.transform.SetParent(transform);
               _grids.Add(mt);
               _map[i, j] = true;
               if (isGoal) _goals.Add(new Vector2Int(i, j));
          }
          //箱子
          Vector2Int[] boxPos = new Vector2Int[] { new Vector2Int(MapWidth / 2, MapHeight / 2) };
          foreach (Box box in _boxes) Destroy(box.gameObject);
          _boxes.Clear();
          foreach (Vector2Int boxP in boxPos)
          {
               GameObject go = Instantiate(AssetManager.BoxGrid);
               Box box = go.GetComponent<Box>();
               box.SetGoalState(IsGoalHere(boxP));
               box.transform.SetParent(transform);
               box.grid.pos = boxP;
               box.grid.SynchronizeMapPos();
               _boxes.Add(box);
          }
          //主角
          Vector2Int[] playerPos = new Vector2Int[] { new Vector2Int(0, 0) };
          foreach (Character character in _characters) Destroy(character.gameObject);
          _characters.Clear();
          _current = null;
          foreach (Vector2Int po in playerPos)
          {
               GameObject go = Instantiate(AssetManager.Character);
               Character cha = go.GetComponent<Character>();
               cha.grid.pos = po;
               cha.grid.SynchronizeMapPos();
               _characters.Add(cha);
               if (!_current) _current = cha;
          }

          usingCam.transform.position = new Vector3(MapWidth / 2.00f, MapHeight / 2.00f, -10);
     }

     private Box GetBoxInGrid(Vector2Int g) => GetBoxInGrid(g.x, g.y);
     private Box GetBoxInGrid(int x, int y)
     {
          foreach (Box box in _boxes)
          {
               if (box.grid.pos.x == x && box.grid.pos.y == y)
                    return box;
          }
          return null;
     }

     private Character GetCharacterInGrid(Vector2Int g) => GetCharacterInGrid(g.x, g.y);
     private Character GetCharacterInGrid(int x, int y)
     {
          foreach (Character character in _characters)
          {
               if (character.grid.pos.x == x && character.grid.pos.y == y) return character;
          }

          return null;
     }

     private MoveRecord TryMoveByDirection(Vector2Int ifCharacterInHere, Vector2Int direction, out bool moveSuccess)
     {
          moveSuccess = false;
          if (direction != Vector2Int.up && direction != Vector2Int.down && direction != Vector2Int.left && direction != Vector2Int.right) return MoveRecord.None;
          if (!InMap(ifCharacterInHere) || !_map[ifCharacterInHere.x, ifCharacterInHere.y] || GetBoxInGrid(ifCharacterInHere)) return MoveRecord.None;
          Vector2Int inDirection = ifCharacterInHere + direction;
          Box boxInDirection = GetBoxInGrid(inDirection);
          Vector2Int nextNextToMe = ifCharacterInHere + direction * 2;
          bool colliderNextNextToMe = !InMap(nextNextToMe) || !_map[nextNextToMe.x, nextNextToMe.y] || GetBoxInGrid(nextNextToMe);
          bool colliderInDirection = !InMap(inDirection) || !_map[inDirection.x, inDirection.y] ||
                                     (boxInDirection && colliderNextNextToMe);
          if (colliderInDirection) return MoveRecord.None;
          moveSuccess = true; 
          return boxInDirection
               ? new MoveRecord(ifCharacterInHere, inDirection, inDirection, nextNextToMe)
               : new MoveRecord(ifCharacterInHere, inDirection);
     }

     private void StartMoveCharacter()
     {
          if (_wishToMoveDir.Count <= 0) return;
          Vector2Int dir = _wishToMoveDir[0];
          _wishToMoveDir.RemoveAt(0);
          Debug.Log("Has command rest " + _wishToMoveDir.Count);
          DoMove(dir);
     }

     // 解释这里的逻辑
     // 将解释写在对应代码行的上方注释的位置上
     // 这是参考代码忽略, 忽略报错, 只分析逻辑
     private void DoMove(Vector2Int direction)
     {
          if (!_current && _characters.Count <= 0) return;
          if (!_current) _current = _characters[0];
          if (!_current) return;

          TimelineNode onMoveDone = new TimelineNode(_ =>
          {
               if (CheckForPlayerWin())
               {
                    _current.ChangeAction(Character.ActionStand);
                    SceneManager.LoadScene("Scenes/GameMain");
               }
               else
               {
                    Debug.Log("Command rest = " + _wishToMoveDir.Count);

                    if (_wishToMoveDir.Count > 0)
                    {
                         StartMoveCharacter();
                    }
                    else
                    {
                         _current.ChangeAction(Character.ActionStand);
                         _state = BattleState.PlayerControl;
                    }

               }

               return true;
          });

          MoveRecord mr = TryMoveByDirection(_current.grid.pos, direction, out bool moveSuccess);
          if (!moveSuccess)
          {
               StartPerformance(onMoveDone);
               return;
          }
          
          _undoRecord.Add(mr);
          _redoRecord.Clear();
          
          const float inSec = 0.2f; // 更新逻辑的时间? 不对
          
          Vector3 manStart = Vector3.zero;
          Vector3 manTarget = MapPosition.GetWorldPosByGrid(mr.characterMoveRecord.toPos);
          TimelineNode res = TimelineNode.Empty; // 这里的命名为什么是res
          
          TimelineNode manMove = new TimelineNode(e =>
          {
               if (e <= 0)
               {
                    _current.ChangeFaceTo(direction);
                    manStart = _current.transform.position;
                    _current.ChangeAction(Character.ActionMove);
               }

               float p = Mathf.Clamp01(e / inSec);
               _current.transform.position = Vector3.Lerp(manStart, manTarget, p);

               if (p > 0.8f)
               {
                    _acceptInputInPerformance = true;
               }

               if (e >= inSec)
               {
                    _acceptInputInPerformance = false;
                    _current.grid.pos = mr.characterMoveRecord.toPos;
                    _current.grid.SynchronizeMapPos(); // 同步地图的pos?
                    // 地图的pos的同步是在这里进行吗? 为什么不是调用的map方法?
                    return true;
               }

               return false;
          });
          manMove.Next.Add(onMoveDone);
          res.Next.Add(manMove); // res是其实节点, 表示result

          // 格子的逻辑位置应该要跟角色的逻辑位置同步更新吧?
          Box box = GetBoxInGrid(mr.boxMoveRecord.fromPos);
          if (box)
          {
               Vector3 boxStart = Vector3.zero;
               Vector3 boxTarget = MapPosition.GetWorldPosByGrid(mr.boxMoveRecord.toPos);
               TimelineNode boxMove = new TimelineNode(e =>
               {
                    if (e <= 0)
                    {
                         boxStart = box.transform.position;
                    }

                    float p = Mathf.Clamp01(e / inSec);
                    box.transform.position = Vector3.Lerp(boxStart, boxTarget, p);

                    // 这里是否表示同步逻辑的更新?
                    if (e >= inSec)
                    {
                         box.grid.pos = mr.boxMoveRecord.toPos;
                         box.grid.SynchronizeMapPos();
                         bool isGoal = IsGoalHere(mr.boxMoveRecord.toPos);
                         box.SetGoalState(isGoal);
                         return true;
                    }

                    return false;
               });
               res.Next.Add(boxMove);
          }
          
          StartPerformance(res);
     }


     private void StartPerformance(TimelineNode node)
     {
          _timeline.Add(node);
          _state = BattleState.Performance;
     }

     private bool IsGoalHere(Vector2Int pos)
     {
          if (!InMap(pos)) return false;
          foreach (Vector2Int goal in _goals)
          {
               if (goal == pos) return true;
          }
          return false;
     }

     private bool CheckForPlayerWin()
     {
          if (_boxes.Count <= _goals.Count)
          {
               foreach (Box box in _boxes)
                    if (!IsGoalHere(box.grid.pos)) return false;
               return true;
          }
          else
          {
               foreach (Vector2Int goal in _goals)
               {
                    Box b = GetBoxInGrid(goal);
                    if (!b) return false;
               }
               return true;
          }
     }

     public void Undo()
     {
          if (_undoRecord.Count <= 0) return;
          MoveRecord mr = _undoRecord[^1];
          _undoRecord.RemoveAt(_undoRecord.Count - 1);

          Character cha = GetCharacterInGrid(mr.characterMoveRecord.toPos);
          if (cha)
          {
               cha.grid.pos = mr.characterMoveRecord.fromPos;
               cha.grid.SynchronizeMapPos();
          }

          Box box = GetBoxInGrid(mr.boxMoveRecord.toPos);
          if (box)
          {
               box.grid.pos = mr.boxMoveRecord.fromPos;
               box.grid.SynchronizeMapPos();
          }
          
          _redoRecord.Add(mr);
     }

     public void Redo()
     {
          if (_redoRecord.Count <= 0) return;
          MoveRecord mr = _redoRecord[^1];
          _redoRecord.RemoveAt(_redoRecord.Count - 1);
          
          Character cha = GetCharacterInGrid(mr.characterMoveRecord.fromPos);
          if (cha)
          {
               cha.grid.pos = mr.characterMoveRecord.toPos;
               cha.grid.SynchronizeMapPos();
          }

          Box box = GetBoxInGrid(mr.boxMoveRecord.fromPos);
          if (box)
          {
               box.grid.pos = mr.boxMoveRecord.toPos;
               box.grid.SynchronizeMapPos();
          }
          
          _undoRecord.Add(mr);
     }
}
