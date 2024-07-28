using System;
using System.Collections.Generic;
using EventUtils;
using GameObjectUtils;
using ReflectionUI;
using Timer;

public class AIManager : Singleton<AIManager>,IGridManager
{
    private AiGridScript[,] _grids;
    
    private int _id;

    private int _size;

    private Random _random;

    private int _typeCount;
    
    private bool _isMatched = false;

    private int _stepCount = 0;

    private bool _isDoing = false;

    public void Init(GridScript[,] temps,int size,Random random,int typeCount)
    {
        _size = size;
        _random = random;
        _typeCount = typeCount;

        _grids = new AiGridScript[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GridScript temp = temps[i, j];
                CreateGrid(temp);
            }
        }
    }

    public void DoOperation()
    {
        if (_isDoing)
        {
            _stepCount++;
        }
        else
        {
            _isDoing = true;
            List<AiGridScript[]> picks = new();
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
                        picks.Add(new []{start,end});
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
                        picks.Add(new []{start,end});
                    }
                }
            }

            if (picks.Count > 0)
            {
                AiGridScript[] pick = picks[_random.Next(picks.Count - 1)];

                Swap(pick[0], pick[1]);
            
                DoMatchSpecial(pick);
            }
        }
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

    public void Remove(AiGridScript grid)
    {
        Delete(new List<AiGridScript>() { grid });
    }
    
    public void RemoveBySkill(List<IGrid> grids)
    {
        TimerUtils.Once(200, () =>
        {
            Delete(grids);
            TimerUtils.Once(200, ReGenerate);
        });
    }
    
    public void DoEffect(AiGridScript grid)
    {
        List<AiGridScript> clearList = new List<AiGridScript>();
        AiGridScript temp;

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
    /// 填充面板
    /// </summary>
    /// <param name="data"></param>
    private void FillGrid(int[,] data, bool renew = true)
    {
        if (renew)
        {
            _grids = new AiGridScript[_size, _size];
        }

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (_grids[i, j] == null)
                {
                    CreateGrid(data[i, j], i, j);
                }
            }
        }
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
    
    /// <summary>
    /// 获得随机类型
    /// </summary>
    /// <returns></returns>
    private int GetRandomType(bool needBlocked = true)
    {
        if (needBlocked)
        {
            return _random.Next(1, _typeCount);
        }

        return _random.Next(2, _typeCount);
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
    
    /// <summary>
    /// 删除匹配项
    /// </summary>
    /// <param name="grids"></param>
    private void Delete(List<AiGridScript> grids, bool doAction = true)
    {
        foreach (var grid in grids)
        {
            if (_grids[grid.y, grid.x] != null)
            {
                _grids[grid.y, grid.x] = null;

                grid.OnRemove(doAction);
                grid.UnRegister();

                GameManager.Ins().AddRivalScore();
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
            AiGridScript grid = data as AiGridScript;
            
            if (_grids[grid.y, grid.x] != null)
            {
                _grids[grid.y, grid.x] = null;

                grid.OnRemove(doAction);
                grid.UnRegister();

                GameManager.Ins().AddRivalScore();
            }
        }
    }
    
    /// <summary>
    /// 无位移交换
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void Swap(AiGridScript start, AiGridScript end)
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
    private List<AiGridScript> CheckMatch(AiGridScript grid)
    {
        List<AiGridScript> res = new List<AiGridScript>();

        if (grid.blockType == BlockType.Fruit)
        {
            List<AiGridScript> match = new List<AiGridScript>() { grid };

            int x = grid.x + 1;
            AiGridScript cur;
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
    
    private bool CheckMatchAfter(AiGridScript grid)
    {
        if (grid.blockType == BlockType.Fruit)
        {
            List<AiGridScript> match = new List<AiGridScript>() { grid };

            int x = grid.x + 1;
            AiGridScript cur;

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
    
    /// <summary>
    /// 获取grid
    /// </summary>
    /// <param name="gridType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private AiGridScript CreateGrid(int gridType, int i, int j)
    {
        AiGridScript gs = new AiGridScript();

        if (string.IsNullOrEmpty(gs.id))
        {
            gs.id = "ai_" + _id++;
        }

        gs.x = j;
        gs.y = i;
        gs.gridType = gridType;
        gs.blockType = CheckBlock(gridType);

        gs.Register();

        if (gs.y >= 0 && gs.y < _size && gs.x >= 0 && gs.x < _size)
        {
            _grids[gs.y, gs.x] = gs;
        }

        return gs;
    }

    private AiGridScript CreateGrid(GridScript gs)
    {
        AiGridScript aiGridScript = new AiGridScript();
        aiGridScript.gridType = gs.gridType;
        aiGridScript.blockType = gs.blockType;
        aiGridScript.effect = gs.effect;
        aiGridScript.x = gs.x;
        aiGridScript.y = gs.y;
        aiGridScript.id = "ai_" + gs.id;

        aiGridScript.Register();

        if (aiGridScript.y >= 0 && aiGridScript.y < _size && aiGridScript.x >= 0 && aiGridScript.x < _size)
        {
            _grids[aiGridScript.y, aiGridScript.x] = aiGridScript;
        }

        return aiGridScript;
    }
    
    private bool CheckMatchSpecial(AiGridScript grid)
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
    private void DoMatchNormal(AiGridScript[] grids, Action callback = null)
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
                AiGridScript matchStart = match[0];
                AiGridScript newOne = null;

                if (match.Count >= 5)
                {
                    newOne = CreateGrid(0, matchStart.y, matchStart.x);
                    newOne.SetEffect(EffectType.Clear);
                }
                else if (match.Count >= 4)
                {
                    newOne = CreateGrid(matchStart.gridType, matchStart.y, matchStart.x);
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
            TimerUtils.Once(200, ReGenerate);
            //匹配的情况 
            _isMatched = true;
        }
    }
    
    private void CheckStepEnd()
    {
        if (_isMatched)
        {
            GameManager.Ins().SetRivalStep(GameManager.Ins().GetRivalStep() - 1);
            _isMatched = false;
            _isDoing = false;
            
            //通知更新主界面
            EventManager.TriggerEvent("MainViewUpdateRival", null);
            //计算技能
            SkillManager.Ins().CheckSkill("rival");

            if (_stepCount > 0)
            {
                DoOperation();
            }
        }
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
                AiGridScript gs = _grids[i, j];
                if (gs == null)
                {
                    int y = i - 1;

                    AiGridScript temp = null;
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

                        temp = CreateGrid(type, i, j);
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
                        }
                        else
                        {
                            int isLeft = new Random().Next() == 0 ? -1 : 1;
                            temp = GenerateForBlockedGrid(temp, isLeft, offsetY);

                            if (temp.y >= 0)
                            {
                                _grids[temp.y, temp.x] = null;
                            }

                            temp.x = j;
                            temp.y = i;
                            _grids[i, j] = temp;
                        }
                    }
                }
            }
        }

        //检测匹配
        TimerUtils.Once(350, () =>
        {
            AiGridScript[] grids = new AiGridScript[_grids.Length];

            int index = 0;
            foreach (var grid in _grids)
            {
                grids[index] = grid;
                index++;
            }

            DoMatchNormal(grids, () =>
            {
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
    private AiGridScript GenerateForBlockedGrid(AiGridScript temp, int lr, int[] offsetY)
    {
        //检查左右

        AiGridScript tempL = null;
        AiGridScript tempR = null;

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

                temp = CreateGrid(type, y, x + isLeft);
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
    /// 执行特别匹配
    /// </summary>
    /// <param name="grids"></param>
    /// <param name="callback"></param>
    private void DoMatchSpecial(AiGridScript[] grids, Action callback = null)
    {
        var start = grids[0];
        var end = grids[1];
        List<AiGridScript> clearList = new();

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

        TimerUtils.Once(200, ReGenerate);
    }

    private void DoCheckNoMatch()
    {
        //如果方块无法实现交换 重新生成
        if (CheckNoMatch())
        {
            List<AiGridScript> clearList = new();
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