using System;
using System.Collections.Generic;

public interface IGridManager
{
    public List<List<IGrid>> GetGrids();
    public int GetSize();
    public Random GetRandom();
    public void RemoveBySkill(List<IGrid> grids);
}