using System;
using System.Collections;
using System.Collections.Generic;
using EventUtils;
using GameObjectUtils;
using PosTween;
using Timer;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class GridManager : Singleton<GridManager>
{
    private int _id;

    private GridScript[,] _grid;

    private int _size;

    private Random _random;

    private List<string> _prefabs = new List<string>();

    private List<string> _other = new List<string>();

    private Dictionary<string, bool> _blockDic = new Dictionary<string, bool>();

    private GameObject _gridPanel;

    private Vector2 _offset = new Vector2(1.28f, 1.28f);

    private GridScript _cur;

    private bool _moving = false;

    public void Init(int seed, int size)
    {
        _size = size;

        _random = new Random(seed);

        _gridPanel = new GameObject();
        _gridPanel.name = "GridPanel";
        _gridPanel.transform.position = new Vector3(-_size * _offset.x * 0.5f, _size * _offset.y * 0.5f, 0);

        //添加基础水果
        _prefabs.Add("Prefabs/banana");
        _prefabs.Add("Prefabs/blueberry");
        _prefabs.Add("Prefabs/peach");
        _prefabs.Add("Prefabs/strawberry");
        _prefabs.Add("Prefabs/kiwi");
        _prefabs.Add("Prefabs/ice");

        //添加额外效果
        // _other.Add("Prefabs/ice");

        //初始化属性
        _blockDic.Add("Prefabs/ice", true);
    }

    /// <summary>
    /// 创建三消面板
    /// </summary>
    public void CreatePanel()
    {
        int[,] data;

        data = GenerateGrid();

        do
        {
            data = GenerateGrid();
        } while (CheckDelete(data));

        FillGrid(data);
    }

    /// <summary>
    /// 生成三消布局
    /// </summary>
    /// <returns></returns>
    private int[,] GenerateGrid()
    {
        var grid = new int[_size, _size];
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                int gridType = _random.Next(_prefabs.Count);
                grid[j, i] = gridType;
            }
        }

        return grid;
    }

    /// <summary>
    /// 鼠标按下选中元素
    /// </summary>
    /// <param name="gs"></param>
    public void SetSelect(GridScript gs)
    {
        _cur = gs;
    }

    public void SetHover(GridScript gs)
    {
        if (_cur != null && _cur != gs && !_moving && Math.Abs(_cur.x - gs.x) <= 1 && Math.Abs(_cur.y - gs.y) <= 1)
        {
            _moving = true;

            GridScript[] grids = { _cur, gs };

            void Callback()
            {
                DoMatch(grids, () =>
                {
                    _moving = true;
                    Swap(grids[0], grids[1], () =>
                    {
                        _moving = false;
                        Release();
                    });
                });
            }

            Swap(_cur, gs, Callback);
        }
    }

    /// <summary>
    /// 释放鼠标按下时选中的元素
    /// </summary>
    public void Release()
    {
        _cur = null;
    }

    public void Remove(GridScript grid)
    {
        Delete(new List<GridScript>() { grid });
    }

    /// <summary>
    /// 检查是否有可以马上消除的
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CheckDelete(int[,] data)
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size - 2; j++)
            {
                if (data[i, j] == data[i, j + 1] && data[i, j] == data[i, j + 2])
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < _size - 2; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (data[i, j] == data[i + 1, j] && data[i, j] == data[i + 2, j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 检查是否完全不能消除
    /// </summary>
    /// <param name="size"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CheckNoPath(int[,] data)
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size - 2; j++)
            {
            }
        }

        return true;
    }

    /// <summary>
    /// 填充面板
    /// </summary>
    /// <param name="data"></param>
    private void FillGrid(int[,] data)
    {
        _grid = new GridScript[_size, _size];
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                CreateGrid(data[i, j], i, j, new Vector3(_offset.x * j, -_offset.y * i, 0));
            }
        }
    }

    /// <summary>
    /// 删除匹配项
    /// </summary>
    /// <param name="grids"></param>
    private void Delete(List<GridScript> grids)
    {
        //TODO 无法一次消除的情况

        foreach (var grid in grids)
        {
            if (_grid[grid.y, grid.x] != null)
            {
                int type = grid.gridType;
                ObjManager.Ins().Recycle(_prefabs[type], grid.gameObject);

                _grid[grid.y, grid.x] = null;

                grid.OnRemove();
                grid.UnRegister();
            }
        }
    }

    /// <summary>
    /// 交换
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void Swap(GridScript start, GridScript end, Action callback)
    {
        _grid[start.y, start.x] = end;
        _grid[end.y, end.x] = start;

        int x = start.x;
        int y = start.y;

        start.x = end.x;
        start.y = end.y;

        end.x = x;
        end.y = y;

        start.SetFront();

        Action callback1 = () =>
        {
            start.Reset();
            callback?.Invoke();
        };

        PosTweenUtils.Move(start, start.pos, end.pos, 300, 0, callback1);
        PosTweenUtils.Move(end, end.pos, start.pos, 300);
    }

    /// <summary>
    /// 检查匹配
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private List<GridScript> CheckMatch(GridScript grid)
    {
        List<GridScript> res = new List<GridScript>();

        List<GridScript> match = new List<GridScript>() { grid };

        int x = grid.x + 1;
        GridScript cur;
        while (x < _size)
        {
            cur = _grid[grid.y, x];

            if (cur == null || cur.gridType != grid.gridType)
            {
                break;
            }

            if (cur != grid)
            {
                match.Add(cur);
            }

            ++x;
        }

        x = grid.x - 1;
        while (x >= 0)
        {
            cur = _grid[grid.y, x];

            if (cur == null || cur.gridType != grid.gridType)
            {
                break;
            }

            if (cur != grid)
            {
                match.Add(cur);
            }

            --x;
        }

        if (match.Count >= 3)
        {
            res.AddRange(match);
        }

        match.RemoveRange(1, match.Count - 1);

        int y = grid.y + 1;

        while (y < _size)
        {
            cur = _grid[y, grid.x];

            if (cur == null || cur.gridType != grid.gridType)
            {
                break;
            }

            if (cur != grid)
            {
                match.Add(cur);
            }

            ++y;
        }

        y = grid.y - 1;
        while (y >= 0)
        {
            cur = _grid[y, grid.x];

            if (cur == null || cur.gridType != grid.gridType)
            {
                break;
            }

            if (cur != grid)
            {
                match.Add(cur);
            }

            --y;
        }

        if (match.Count >= 3)
        {
            if (res.Count > 0)
            {
                match.RemoveAt(0);
            }

            res.AddRange(match);
        }

        return res;
    }

    /// <summary>
    /// 开始匹配过程
    /// </summary>
    /// <param name="grids"></param>
    private void DoMatch(GridScript[] grids, Action callback = null)
    {
        //检测是否匹配
        int notMatchCount = 0;

        foreach (var grid in grids)
        {
            var match = CheckMatch(grid);

            if (match.Count > 0)
            {
                Delete(match);
            }
            else
            {
                notMatchCount++;
            }


            //TODO 生成特殊炸弹
            if (match.Count >= 5)
            {
            }
            else if (match.Count >= 4)
            {
            }
        }

        if (notMatchCount == grids.Length)
        {
            //没有匹配的情况
            callback?.Invoke();
        }
        else
        {
            TimerUtils.Once(100, ReGenerate);
            //匹配的情况 
        }
    }

    /// <summary>
    /// 获取grid
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GridScript CreateGrid(int gridType, int i, int j, Vector3 pos)
    {
        GameObject obj = ObjManager.Ins().GetRes(_prefabs[gridType]);

        obj.transform.parent = _gridPanel.transform;

        GridScript gs = obj.GetComponent<GridScript>();

        if (gs.id.Equals(string.Empty))
        {
            gs.id = _id++.ToString();
        }

        gs.x = j;
        gs.y = i;
        gs.gridType = gridType;
        gs.pos = pos;

        gs.enabled = !_blockDic.ContainsKey(_prefabs[gridType]);
        gs.Register();

        _grid[gs.y, gs.x] = gs;

        return gs;
    }

    private void ReGenerate()
    {
        //先移动 后生成

        int[] offsetY = new int[_size];

        int duration = 300;

        for (int i = _size - 1; i >= 0; i--)
        {
            for (int j = _size - 1; j >= 0; j--)
            {
                GridScript gs = _grid[i, j];
                if (gs == null)
                {
                    int y = i - 1;

                    GridScript temp = null;
                    while (y >= 0)
                    {
                        temp = _grid[y, j];
                        if (temp != null)
                        {
                            break;
                        }

                        y--;
                    }


                    if (temp == null)
                    {
                        //创建可移动grid
                        int type = _random.Next(_prefabs.Count);
                        while (_blockDic.ContainsKey(_prefabs[type]))
                        {
                            type = _random.Next(_prefabs.Count);
                        }

                        temp = CreateGrid(type, i, j, new Vector3(_offset.x * j, _offset.y * ++offsetY[j], 0));

                        PosTweenUtils.Move(temp, temp.pos, new Vector3(temp.pos.x, -_offset.y * temp.y, 0),
                            duration);
                    }
                    else
                    {
                        if (temp.enabled)
                        {
                            _grid[temp.y, temp.x] = null;

                            //直接下落
                            temp.x = j;
                            temp.y = i;

                            _grid[i, j] = temp;

                            PosTweenUtils.Move(temp, temp.pos, new Vector3(temp.pos.x, -_offset.y * temp.y, 0),
                                duration);
                        }
                        else
                        {
                            //检查左右
                            int defX = temp.x;
                            int defY = temp.y;

                            int isLeft = new Random().Next() == 0 ? -1 : 1;
                            temp = GenerateForBlockedGrid(temp, isLeft, offsetY);

                            if (temp.y >= 0)
                            {
                                _grid[temp.y, temp.x] = null;
                            }

                            //移动三步走
                            int moveY = defY - temp.y;
                            int moveX = temp.x - defX;

                            defY = temp.y;
                            defX = temp.x;

                            temp.x = j;
                            temp.y = i;
                            _grid[i, j] = temp;

                            float dis = Math.Abs(temp.x - defX) + temp.y - defY;

                            int step1 = (int)(300 * (moveY / dis));
                            int step2 = (int)(300 * (2 / dis));
                            int step3 = (int)(300 * ((dis - moveY - 2) / dis));

                            PosTweenUtils.Move(temp, temp.pos,
                                new Vector3(temp.pos.x, temp.pos.y - _offset.y * moveY, 0),
                                step1, 0, () =>
                                {
                                    PosTweenUtils.Move(temp, temp.pos,
                                        new Vector3(temp.pos.x - _offset.x * (moveX > 0 ? 1 : -1),
                                            temp.pos.y - _offset.y, 0),
                                        step2, 0, () =>
                                        {
                                            PosTweenUtils.Move(temp, temp.pos,
                                                new Vector3(_offset.x * temp.x, -_offset.y * temp.y, 0),
                                                step3);
                                        });
                                });
                        }
                    }
                }
            }
        }

        //检测匹配
        TimerUtils.Once(350, () =>
        {
            GridScript[] grids = new GridScript[_grid.Length];

            int index = 0;
            foreach (var grid in _grid)
            {
                grids[index] = grid;
                index++;
            }

            DoMatch(grids, () =>
            {
                _moving = false;
                Release();
            });
        });
    }

    private GridScript GenerateForBlockedGrid(GridScript temp, int lr, int[] offsetY)
    {
        //检查左右

        GridScript tempL = null;
        GridScript tempR = null;

        switch (lr)
        {
            case -1:
                int left = temp.x - 1;
                while (left >= 0)
                {
                    tempL = _grid[temp.y, left];
                    if (tempL == null || tempL.enabled)
                    {
                        break;
                    }

                    left--;
                }

                break;
            case 1:
                int right = temp.x + 1;
                while (right < _size)
                {
                    tempR = _grid[temp.y, right];
                    if (tempR == null || tempR.enabled)
                    {
                        break;
                    }

                    right++;
                }

                break;
        }


        if (tempL == null && tempR != null)
        {
            // dis = tempR.x - temp.x;
            temp = tempR;
        }
        else if (tempR == null && tempL != null)
        {
            // dis = temp.x - tempL.x;
            temp = tempL;
        }
        else
        {
            //左右都为null
            int isLeft = new Random().Next() == 0 ? -1 : 1;
            int y = temp.y - 1;
            int x = temp.x;
            while (y >= 0)
            {
                temp = _grid[y, x];
                if (temp != null)
                {
                    break;
                }

                y--;
            }

            if (y < 0)
            {
                //创建可移动grid
                int type = _random.Next(_prefabs.Count);
                while (_blockDic.ContainsKey(_prefabs[type]))
                {
                    type = _random.Next(_prefabs.Count);
                }

                CreateGrid(type, y, x + isLeft, new Vector3(_offset.x * temp.x, _offset.y * ++offsetY[temp.x], 0));
            }
            else
            {
                temp = GenerateForBlockedGrid(temp, isLeft, offsetY);
            }
        }

        // Debug.Log("斜角 ==> " + temp.x + " " + temp.y);
        return temp;
    }
}