using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Random = System.Random;

namespace ReflectionUI
{
    public class SkillManager : Singleton<SkillManager>
    {
        private int _skillCount;

        private enum SkillType
        {
            Trailblazer
        }

        private SkillType _skillType;

        public void AddCount(GridScript gs)
        {
            switch (_skillType)
            {
                case SkillType.Trailblazer:
                    if (gs.gridType == 6)
                    {
                        _skillCount++;
                    }

                    break;
            }
        }

        public int GetCount()
        {
            return _skillCount;
        }

        public int GetMax()
        {
            switch (_skillType)
            {
                case SkillType.Trailblazer:
                    return 5;
            }

            return 0;
        }

        public void CheckSkill()
        {
            int times = 0;
            switch (_skillType)
            {
                case SkillType.Trailblazer:
                    while (_skillCount >= 5)
                    {
                        times++;
                        _skillCount -= 5;
                    }

                    DoSkillTrailblazer(times);
                    break;
            }
        }

        private void DoSkillTrailblazer(int times)
        {
            var grids = GridManager.Ins().GetGrids();
            int size = GridManager.Ins().GetSize();
            Random random = GridManager.Ins().GetRandom();

            List<GridScript> listClear = new();

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

                listClear.Add(grids[y, x]);
                listClear.Add(grids[y, x + 1]);
                listClear.Add(grids[y + 1, x]);
                listClear.Add(grids[y + 1, x + 1]);

                pools[y, x] = false;
                pools[y, x + 1] = false;
                pools[y + 1, x] = false;
                pools[y + 1, x + 1] = false;

                index++;
            }

            if (listClear.Count > 0)
            {
                Debug.Log("触发技能，剩余 ==> " + _skillCount);
                GridManager.Ins().RemoveBySkill(listClear);
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