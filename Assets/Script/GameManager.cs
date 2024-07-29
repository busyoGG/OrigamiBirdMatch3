using UnityEngine;

public class GameManager: Singleton<GameManager>
{
    private int _score = 0;

    private int _hp = 100;

    private int _step = 10;

    private int _rivalScore = 0;

    private int _rivalHp = 0;

    private int _rivalStep = 0;

    private int _settlementCount = 0;

    public void Init()
    {
        _hp = 100;
        _score = 0;
        _step = 10;
        
        _rivalHp = 100;
        _rivalScore = 0;
        _rivalStep = 10;
    }

    public void Reset()
    {
        _score = 0;
        _step = 10;
        
        _rivalScore = 0;
        _rivalStep = 10;
    }
    
    public void AddScore()
    {
        _score++;
    }

    public int GetScore()
    {
        return _score;
    }

    public void SetHp(int hp)
    {
        _hp = hp;
    }

    public int GetHp()
    {
        return _hp;
    }

    public void SetStep(int step)
    {
        _step = step;
    }

    public int GetStep()
    {
        return _step;
    }
    
    public void AddRivalScore()
    {
        _rivalScore++;
    }

    public int GetRivalScore()
    {
        return _rivalScore;
    }

    public void SetRivalHp(int hp)
    {
        _rivalHp = hp;
    }

    public int GetRivalHp()
    {
        return _rivalHp;
    }

    public void SetRivalStep(int step)
    {
        _rivalStep = step;
    }

    public int GetRivalStep()
    {
        return _rivalStep;
    }

    public void Settlement()
    {
        _settlementCount++;
        if (_settlementCount >= 2)
        {
            _settlementCount = 0;
            int damage = _score - _rivalScore;
            if (damage > 0)
            {
                _rivalHp -= damage;
                if (_rivalHp < 0)
                {
                    _rivalHp = 0;
                }
            }
            else
            {
                _hp += damage;
                if (_hp < 0)
                {
                    _hp = 0;
                }
            }
            Debug.Log("玩家血量 " + _hp + " - 敌方血量 " + _rivalHp);
            
            Reset();
        }
    }
}