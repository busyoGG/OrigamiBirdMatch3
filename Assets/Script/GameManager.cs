public class GameManager: Singleton<GameManager>
{
    private int _score = 0;

    private int _hp = 100;

    private int _step = 10;

    public void Init()
    {
        _hp = 100;
        _score = 0;
        _step = 10;
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
}