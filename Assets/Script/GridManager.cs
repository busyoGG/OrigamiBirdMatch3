using System;
using System.Collections.Generic;
using EventUtils;
using GameObjectUtils;
using Newtonsoft.Json;
using PosTween;
using ReflectionUI;
using Timer;
using UnityEngine;
using Random = System.Random;

public class GridManager : Singleton<GridManager>,IGridManager
{
    private int _id;

    private GridScript[,] _grids;

    private int _size;

    private Random _random;

    private List<string> _prefabs = new();

    private GameObject _gridPanel;

    private Vector2 _offset = new(1.28f, 1.28f);

    private GridScript _cur;

    private bool _moving;

    private bool _isMatched = false;

    public void Init(int seed, int size, Transform parent)
    {
        _size = size;

        _random = new Random(seed);

        _gridPanel = new GameObject();
        _gridPanel.name = "GridPanel";
        // _gridPanel.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        _gridPanel.transform.parent = parent;
        _gridPanel.transform.localPosition = new Vector3(-2.31f, 4.47f, 0);

        //添加基础水果
        _prefabs.Add("Prefabs/clear");
        _prefabs.Add("Prefabs/ice");
        _prefabs.Add("Prefabs/banana");
        _prefabs.Add("Prefabs/blueberry");
        _prefabs.Add("Prefabs/peach");
        _prefabs.Add("Prefabs/strawberry");
        _prefabs.Add("Prefabs/kiwi");

        //添加额外效果
        // _other.Add("Prefabs/ice");
    }

    public List<List<IGrid>> GetGrids()
    {
        List<List<IGrid>> res = new();
        
        for (int i = 0; i < _size; i++)
        {
            res.Add(new ());
            for (int j = 0; j < _size; j++)
            {
                res[i].Add(_grids[i, j]);
            }
        }

        return res;
    }

    public int GetSize()
    {
        return _size;
    }

    public Random GetRandom()
    {
        return _random;
    }

    /// <summary>
    /// 创建三消面板
    /// </summary>
    public void CreatePanel()
    {
        int[,] data;

        do
        {
            data = GenerateGrid();
        } while (CheckDelete(data) || CheckNoMatch(data));

        FillGrid(data);

        //镜像给AI
        AIManager.Ins().Init(_grids, _size, _random, _prefabs.Count);
    }

    public void CreatePanel(string json)
    {
        int[,] data = JsonConvert.DeserializeObject<int[,]>(json);

        CreatePanel(data);

        //镜像给AI
        AIManager.Ins().Init(_grids, _size, _random, _prefabs.Count);
    }

    public void CreatePanel(int[,] data, bool renew = true)
    {
        int[,] temp;

        do
        {
            temp = GenerateGrid(data, renew);
        } while (CheckDelete(temp) || CheckNoMatch(temp));

        FillGrid(temp, renew);
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
                int gridType = GetRandomType();
                grid[i, j] = gridType;
            }
        }

        return grid;
    }

    private int[,] GenerateGrid(int[,] data, bool needBlock = true)
    {
        var grid = new int[_size, _size];
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (data[i, j] == -1)
                {
                    int gridType = GetRandomType(needBlock);
                    grid[i, j] = gridType;
                }
                else
                {
                    grid[i, j] = data[i, j];
                }
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
                DoMatchSpecial(grids, () =>
                {
                    _moving = true;
                    Swap(grids[0], grids[1], () =>
                    {
                        _moving = false;
                        Release();
                        DoCheckNoMatch();
                    });
                });
                
            }

            Swap(_cur, gs, Callback);
        }
    }

    private void CheckStepEnd()
    {
        if (!_moving && _isMatched)
        {
            GameManager.Ins().SetStep(GameManager.Ins().GetStep() - 1);
            AIManager.Ins().DoOperation();
            _isMatched = false;
            
            //通知更新主界面
            EventManager.TriggerEvent("MainViewUpdate", null);
            //计算技能
            SkillManager.Ins().CheckSkill("self");
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

    public void RemoveBySkill(List<IGrid> grids)
    {
        _moving = true;
        TimerUtils.Once(200, () =>
        {
            Delete(grids);
            TimerUtils.Once(200, ReGenerate);
        });
    }

    public void DoEffect(GridScript grid)
    {
        List<GridScript> clearList = new List<GridScript>();
        GridScript temp;

        switch (grid.effect)
        {
            case EffectType.BombH:
                int left = grid.x - 1;
                int right = grid.x + 1;
                while (left >= 0)
                {
                    temp = _grids[grid.y, left];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    left--;
                }

                while (right < _size)
                {
                    temp = _grids[grid.y, right];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    right++;
                }

                Delete(clearList);

                break;
            case EffectType.BombV:
                int up = grid.y - 1;
                int down = grid.y + 1;
                while (up >= 0)
                {
                    temp = _grids[up, grid.x];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    up--;
                }

                while (down < _size)
                {
                    temp = _grids[down, grid.x];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    down++;
                }

                Delete(clearList);
                break;
            case EffectType.BombCross:
                left = grid.x - 1;
                right = grid.x + 1;
                while (left >= 0)
                {
                    temp = _grids[grid.y, left];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    left--;
                }

                while (right < _size)
                {
                    temp = _grids[grid.y, right];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    right++;
                }

                up = grid.y - 1;
                down = grid.y + 1;
                while (up >= 0)
                {
                    temp = _grids[up, grid.x];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    up--;
                }

                while (down < _size)
                {
                    temp = _grids[down, grid.x];
                    if (temp != null && temp.blockType == BlockType.Fruit && temp.gridType != 0)
                    {
                        clearList.Add(temp);
                    }

                    down++;
                }

                Delete(clearList);
                break;
        }
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
                if (data[i, j] <= 1)
                {
                    continue;
                }

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
                if (data[i, j] <= 1)
                {
                    continue;
                }

                if (data[i, j] == data[i + 1, j] && data[i, j] == data[i + 2, j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckNoMatch(int[,] data)
    {
        for (int i = 0; i < _size - 1; i++)
        {
            for (int j = 0; j < _size - 1; j++)
            {
                bool res = true;

                var start = data[i, j];

                if (start == 1) continue;

                //检测左右交换

                (data[i, j], data[i, j + 1]) = (data[i, j + 1], data[i, j]);

                res = CheckMatchAfter(data, i, j);
                if (!res)
                {
                    res = CheckMatchAfter(data, i, j + 1);
                }

                (data[i, j], data[i, j + 1]) = (data[i, j + 1], data[i, j]);

                if (res)
                {
                    return false;
                }

                //检测上下交换

                (data[i, j], data[i + 1, j]) = (data[i + 1, j], data[i, j]);

                res = CheckMatchAfter(data, i, j);
                if (!res)
                {
                    res = CheckMatchAfter(data, i + 1, j);
                }

                (data[i, j], data[i + 1, j]) = (data[i + 1, j], data[i, j]);

                if (res)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 检查是否完全不能消除
    /// </summary>
    /// <param name="size"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CheckNoMatch()
    {
        for (int i = 0; i < _size - 1; i++)
        {
            for (int j = 0; j < _size - 1; j++)
            {
                bool res = true;

                var start = _grids[i, j];

                if (start.blockType != BlockType.Fruit) continue;

                //检测左右交换
                var end = _grids[i, j + 1];

                Swap(start, end);

                res = CheckMatchAfter(start) || CheckMatchSpecial(start);

                if (!res)
                {
                    res = CheckMatchAfter(end) || CheckMatchSpecial(end);
                }

                Swap(start, end);

                if (res)
                {
                    return false;
                }

                //检测上下交换
                end = _grids[i + 1, j];

                Swap(start, end);

                res = CheckMatchAfter(start) || CheckMatchSpecial(start);

                if (!res)
                {
                    res = CheckMatchAfter(end) || CheckMatchSpecial(end);
                }

                Swap(start, end);

                if (res)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 填充面板
    /// </summary>
    /// <param name="data"></param>
    private void FillGrid(int[,] data, bool renew = true)
    {
        if (renew)
        {
            _grids = new GridScript[_size, _size];
        }

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (_grids[i, j] == null)
                {
                    CreateGrid(data[i, j], i, j, new Vector3(_offset.x * j, -_offset.y * i, 0));
                }
            }
        }
    }

    /// <summary>
    /// 删除匹配项
    /// </summary>
    /// <param name="grids"></param>
    private void Delete(List<GridScript> grids, bool doAction = true)
    {
        foreach (var grid in grids)
        {
            if (_grids[grid.y, grid.x] != null)
            {
                int type = grid.gridType;
                ObjManager.Ins().Recycle(_prefabs[type], grid.gameObject);

                _grids[grid.y, grid.x] = null;

                grid.OnRemove(doAction);
                grid.UnRegister();

                GameManager.Ins().AddScore();
            }
        }
    }
    
    /// <summary>
    /// 删除匹配项
    /// </summary>
    /// <param name="grids"></param>
    private void Delete(List<IGrid> grids, bool doAction = true)
    {
        foreach (var data in grids)
        {
            GridScript grid = data as GridScript;
            
            if (_grids[grid.y, grid.x] != null)
            {
                int type = grid.gridType;
                ObjManager.Ins().Recycle(_prefabs[type], grid.gameObject);

                _grids[grid.y, grid.x] = null;

                grid.OnRemove(doAction);
                grid.UnRegister();

                GameManager.Ins().AddScore();
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
        Swap(start, end);

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
    /// 无位移交换
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void Swap(GridScript start, GridScript end)
    {
        _grids[start.y, start.x] = end;
        _grids[end.y, end.x] = start;

        int x = start.x;
        int y = start.y;

        start.x = end.x;
        start.y = end.y;

        end.x = x;
        end.y = y;
    }

    /// <summary>
    /// 检查匹配
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private List<GridScript> CheckMatch(GridScript grid)
    {
        List<GridScript> res = new List<GridScript>();

        if (grid.blockType == BlockType.Fruit)
        {
            List<GridScript> match = new List<GridScript>() { grid };

            int x = grid.x + 1;
            GridScript cur;
            while (x < _size)
            {
                cur = _grids[grid.y, x];

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
                cur = _grids[grid.y, x];

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
                cur = _grids[y, grid.x];

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
                cur = _grids[y, grid.x];

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
        }

        return res;
    }

    private bool CheckMatchAfter(GridScript grid)
    {
        if (grid.blockType == BlockType.Fruit)
        {
            List<GridScript> match = new List<GridScript>() { grid };

            int x = grid.x + 1;
            GridScript cur;

            while (x < _size)
            {
                cur = _grids[grid.y, x];

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
                cur = _grids[grid.y, x];

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
                return true;
            }

            match.RemoveRange(1, match.Count - 1);

            int y = grid.y + 1;

            while (y < _size)
            {
                cur = _grids[y, grid.x];

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
                cur = _grids[y, grid.x];

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
                return true;
            }
        }

        return false;
    }

    private bool CheckMatchAfter(int[,] data, int indY, int indX)
    {
        int grid = data[indY, indX];

        if (grid != 1)
        {
            List<int> match = new List<int> { grid };

            int x = indX + 1;
            int cur;
            while (x < _size)
            {
                cur = data[indY, x];

                if (cur != grid)
                {
                    break;
                }

                match.Add(cur);

                ++x;
            }

            x = indX - 1;
            while (x >= 0)
            {
                cur = data[indY, x];

                if (cur != grid)
                {
                    break;
                }

                match.Add(cur);

                --x;
            }

            if (match.Count >= 3)
            {
                return true;
            }

            match.RemoveRange(1, match.Count - 1);

            int y = indY + 1;

            while (y < _size)
            {
                cur = data[y, indX];

                if (cur != grid)
                {
                    break;
                }

                match.Add(cur);

                ++y;
            }

            y = indY - 1;
            while (y >= 0)
            {
                cur = data[y, indX];

                if (cur != grid)
                {
                    break;
                }

                match.Add(cur);

                --y;
            }

            if (match.Count >= 3)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckMatchSpecial(GridScript grid)
    {
        if (grid.blockType == BlockType.Fruit && grid.effect != EffectType.None)
        {
            if (grid.x + 1 < _size && _grids[grid.y, grid.x + 1].effect != EffectType.None)
            {
                return true;
            }

            if (grid.x - 1 >= 0 && _grids[grid.y, grid.x - 1].effect != EffectType.None)
            {
                return true;
            }

            if (grid.y + 1 < _size && _grids[grid.y + 1, grid.x].effect != EffectType.None)
            {
                return true;
            }

            if (grid.y - 1 >= 0 && _grids[grid.y - 1, grid.x].effect != EffectType.None)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 执行普通匹配
    /// </summary>
    /// <param name="grids"></param>
    private void DoMatchNormal(GridScript[] grids, Action callback = null)
    {
        //检测是否匹配
        int notMatchCount = 0;

        foreach (var grid in grids)
        {
            var match = CheckMatch(grid);

            if (match.Count > 0)
            {
                Delete(match);

                // 生成特殊炸弹
                GridScript matchStart = match[0];
                GridScript newOne = null;

                if (match.Count >= 5)
                {
                    Debug.Log("合成清屏炸弹");
                    newOne = CreateGrid(0, matchStart.y, matchStart.x,
                        new Vector3(_offset.x * matchStart.x, -_offset.y * matchStart.y, 0));
                    newOne.SetEffect(EffectType.Clear);
                }
                else if (match.Count >= 4)
                {
                    newOne = CreateGrid(matchStart.gridType, matchStart.y, matchStart.x,
                        new Vector3(_offset.x * matchStart.x, -_offset.y * matchStart.y, 0));
                    if (matchStart.x == match[1].x)
                    {
                        newOne.SetEffect(EffectType.BombV);
                    }
                    else
                    {
                        newOne.SetEffect(EffectType.BombH);
                    }
                }

                if (newOne != null)
                {
                    _grids[newOne.y, newOne.x] = newOne;
                }
            }
            else
            {
                notMatchCount++;
            }
        }

        if (notMatchCount == grids.Length)
        {
            //没有匹配的情况
            callback?.Invoke();
            CheckStepEnd();
        }
        else
        {
            _isMatched = true;
            TimerUtils.Once(200, ReGenerate);
            //匹配的情况 
        }
    }

    /// <summary>
    /// 执行特别匹配
    /// </summary>
    /// <param name="grids"></param>
    /// <param name="callback"></param>
    private void DoMatchSpecial(GridScript[] grids, Action callback = null)
    {
        var start = grids[0];
        var end = grids[1];
        List<GridScript> clearList = new();

        if (start.effect != EffectType.Clear && end.effect == EffectType.Clear)
        {
            (start, end) = (end, start);
        }

        switch (start.effect)
        {
            case EffectType.Clear:
                clearList.Add(start);
                if (end.effect == EffectType.Clear)
                {
                    //清屏
                    foreach (var g in _grids)
                    {
                        if (g != null)
                        {
                            if (g.blockType == BlockType.Fruit)
                            {
                                clearList.Add(g);
                            }
                        }
                    }

                    clearList.Add(end);
                }
                else if (end.effect != EffectType.None)
                {
                    //所有同类水果都变换同类型炸弹
                    foreach (var g in _grids)
                    {
                        if (g != null)
                        {
                            if (g.blockType == BlockType.Fruit && g.gridType == end.gridType)
                            {
                                g.SetEffect(end.effect);
                                clearList.Add(g);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var g in _grids)
                    {
                        if (g != null)
                        {
                            if (g.blockType == BlockType.Fruit && g.gridType == end.gridType)
                            {
                                clearList.Add(g);
                            }
                        }
                    }
                }

                Delete(clearList);
                break;
            case EffectType.BombH:
            case EffectType.BombV:
                if (end.effect == EffectType.BombH || end.effect == EffectType.BombV)
                {
                    //都变成十字炸弹
                    start.effect = EffectType.BombCross;
                    end.effect = EffectType.BombCross;
                    clearList.Add(start);
                    clearList.Add(end);

                    Delete(clearList);
                }
                else
                {
                    DoMatchNormal(grids, callback);
                    return;
                }

                break;
            default:
                DoMatchNormal(grids, callback);
                return;
        }

        _isMatched = true;
        TimerUtils.Once(200, ReGenerate);
    }

    private void DoCheckNoMatch()
    {
        //如果方块无法实现交换 重新生成
        if (CheckNoMatch())
        {
            List<GridScript> clearList = new();
            //清屏
            foreach (var g in _grids)
            {
                if (g != null)
                {
                    if (g.blockType == BlockType.Fruit && g.gridType > 1)
                    {
                        clearList.Add(g);
                    }
                }
            }

            Delete(clearList, false);
            // TimerUtils.Once(200, ReGenerate);
            int[,] data = new int[_size, _size];

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_grids[i, j] != null)
                    {
                        data[i, j] = _grids[i, j].gridType;
                    }
                    else
                    {
                        data[i, j] = -1;
                    }
                }
            }

            CreatePanel(data, false);
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

        if (string.IsNullOrEmpty(gs.id))
        {
            gs.id = _id++.ToString();
        }

        gs.x = j;
        gs.y = i;
        gs.gridType = gridType;
        gs.pos = pos;
        gs.blockType = CheckBlock(gridType);

        gs.Register();

        if (gs.y >= 0 && gs.y < _size && gs.x >= 0 && gs.x < _size)
        {
            _grids[gs.y, gs.x] = gs;
        }

        return gs;
    }

    /// <summary>
    /// 重新生成方块
    /// </summary>
    private void ReGenerate()
    {
        //先移动 后生成

        int[] offsetY = new int[_size];

        int duration = 300;

        for (int i = _size - 1; i >= 0; i--)
        {
            for (int j = _size - 1; j >= 0; j--)
            {
                GridScript gs = _grids[i, j];
                if (gs == null)
                {
                    int y = i - 1;

                    GridScript temp = null;
                    while (y >= 0)
                    {
                        temp = _grids[y, j];
                        if (temp != null)
                        {
                            break;
                        }

                        y--;
                    }


                    if (temp == null)
                    {
                        //创建可移动grid
                        int type = GetRandomType(false);

                        temp = CreateGrid(type, i, j, new Vector3(_offset.x * j, _offset.y * ++offsetY[j], 0));

                        PosTweenUtils.Move(temp, temp.pos, new Vector3(temp.pos.x, -_offset.y * temp.y, 0),
                            duration);
                    }
                    else
                    {
                        if (temp.blockType == BlockType.Fruit)
                        {
                            _grids[temp.y, temp.x] = null;

                            //直接下落
                            temp.x = j;
                            temp.y = i;

                            _grids[i, j] = temp;

                            PosTweenUtils.Move(temp, temp.pos, new Vector3(temp.pos.x, -_offset.y * temp.y, 0),
                                duration);
                        }
                        else
                        {
                            //斜角下落
                            int defX = temp.x;
                            int defY = temp.y;

                            int isLeft = new Random().Next() == 0 ? -1 : 1;
                            temp = GenerateForBlockedGrid(temp, isLeft, offsetY);

                            if (temp.y >= 0)
                            {
                                _grids[temp.y, temp.x] = null;
                            }

                            //移动三步走
                            int moveY = defY - temp.y;
                            int moveX = temp.x - defX;

                            defY = temp.y;
                            defX = temp.x;

                            temp.x = j;
                            temp.y = i;
                            _grids[i, j] = temp;

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
            GridScript[] grids = new GridScript[_grids.Length];

            int index = 0;
            foreach (var grid in _grids)
            {
                grids[index] = grid;
                index++;
            }

            DoMatchNormal(grids, () =>
            {
                _moving = false;
                Release();
                DoCheckNoMatch();
            });
        });
    }

    /// <summary>
    /// 顶部有不可移动的方块，按照对应的规则生成新的方块
    /// </summary>
    /// <param name="temp"></param>
    /// <param name="lr"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    private GridScript GenerateForBlockedGrid(GridScript temp, int lr, int[] offsetY)
    {
        //检查左右

        GridScript tempL = null;
        GridScript tempR = null;

        int left = temp.x - 1;
        int right = temp.x + 1;

        if (lr == -1)
        {
            if (left < 0)
            {
                lr = 1;
            }
        }
        else
        {
            if (right >= _size)
            {
                lr = -1;
            }
        }

        switch (lr)
        {
            case -1:
                while (left >= 0)
                {
                    tempL = _grids[temp.y, left];
                    if (tempL == null || tempL.blockType == BlockType.Fruit)
                    {
                        break;
                    }

                    left--;
                }

                break;
            case 1:
                while (right < _size)
                {
                    tempR = _grids[temp.y, right];
                    if (tempR == null || tempR.blockType == BlockType.Fruit)
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
            if (x + isLeft >= _size)
            {
                isLeft = -1;
            }
            else if (x + isLeft < 0)
            {
                isLeft = 1;
            }

            while (y >= 0)
            {
                temp = _grids[y, x];
                if (temp != null)
                {
                    break;
                }

                y--;
            }

            if (y < 0)
            {
                //创建可移动grid
                int type = GetRandomType(false);

                temp = CreateGrid(type, y, x + isLeft,
                    new Vector3(_offset.x * (x + isLeft), _offset.y * ++offsetY[x + isLeft], 0));
            }
            else
            {
                temp = GenerateForBlockedGrid(temp, isLeft, offsetY);
            }
        }

        // Debug.Log("斜角 ==> " + temp.x + " " + temp.y);
        return temp;
    }


    /// <summary>
    /// 获得随机类型
    /// </summary>
    /// <returns></returns>
    private int GetRandomType(bool needBlocked = true)
    {
        if (needBlocked)
        {
            return _random.Next(1, _prefabs.Count);
        }

        return _random.Next(2, _prefabs.Count);
    }

    /// <summary>
    /// 检测是否不可移动
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private BlockType CheckBlock(int type)
    {
        if (type == 1)
        {
            return BlockType.Ice;
        }

        return BlockType.Fruit;
    }
}