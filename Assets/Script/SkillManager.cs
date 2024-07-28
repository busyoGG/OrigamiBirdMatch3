using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Random = System.Random;

namespace ReflectionUI
{
    public class SkillManager : Singleton<SkillManager>
    {
        private Dictionary<string, int> _skillCount = new();

        private enum SkillType
        {
            Trailblazer
        }

        private Dictionary<string, SkillType> _skillType = new();

        private Dictionary<string, IGridManager> _manager = new();

        public void Init()
        {
            _skillType.Add("self", SkillType.Trailblazer);
            _skillCount.Add("self",0);
            _manager.Add("self",GridManager.Ins());
            
            _skillType.Add("rival", SkillType.Trailblazer);
            _skillCount.Add("rival",0);
            _manager.Add("rival",AIManager.Ins());
        }

        public void AddCount(GridScript gs, string player)
        {

            switch (_skillType[player])
            {
                case SkillType.Trailblazer:
                    if (gs.gridType == 6)
                    {
                        _skillCount[player]++;
                    }

                    break;
            }
        }
        
        public void AddCount(AiGridScript gs, string player)
        {

            switch (_skillType[player])
            {
                case SkillType.Trailblazer:
                    if (gs.gridType == 6)
                    {
                        _skillCount[player]++;
                    }

                    break;
            }
        }

        public int GetCount(string player)
        {
            return _skillCount.GetValueOrDefault(player, 0);
        }

        public int GetMax(string player)
        {
            switch (_skillType[player])
            {
                case SkillType.Trailblazer:
                    return 5;
            }

            return 0;
        }

        public bool CheckSkill(string player)
        {
            int times = 0;
            switch (_skillType[player])
            {
                case SkillType.Trailblazer:
                    while (_skillCount[player] >= 5)
                    {
                        times++;
                        _skillCount[player] -= 5;
                    }

                    DoSkillTrailblazer(times,player);
                    break;
            }

            if (times > 0)
            {
                return true;
            }

            return false;
        }

        private void DoSkillTrailblazer(int times,string player)
        {
            List<List<IGrid>> grids = _manager[player].GetGrids();
            int size = _manager[player].GetSize();
            Random random = _manager[player].GetRandom();

            List<IGrid> listClear = new();

            bool[,] pools = new bool[size, size];

            FillArray(pools, size, true);

            int index = 0;
            while (index < times)
            {
                int y = random.Next(size - 1);
                int x = random.Next(size - 1);

                if (!pools[y, x] || !pools[y + 1, x + 1])
                {
                    continue;
                }

                listClear.Add(grids[y][x]);
                listClear.Add(grids[y][x + 1]);
                listClear.Add(grids[y + 1][x]);
                listClear.Add(grids[y + 1][x + 1]);

                pools[y, x] = false;
                pools[y, x + 1] = false;
                pools[y + 1, x] = false;
                pools[y + 1, x + 1] = false;

                index++;
            }

            if (listClear.Count > 0)
            {
                Debug.Log(player + " 触发技能，次数" + times + " 剩余 ==> " + _skillCount);
                _manager[player].RemoveBySkill(listClear);
            }
        }

        private void FillArray<T>(T[,] array, int size, T data)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    array[i, j] = data;
                }
            }
        }
    }
}