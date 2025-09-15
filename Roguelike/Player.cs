namespace Roguelike;

public static class Player
{
    public static Character Char = new Character
        ([(Graphics.ScreenHeight - 2)/2, (Graphics.ScreenWidth - 2)/2], true,  NpcStates.Player,
            20,  20,  5);
    
    public static bool GameOver = false;
    public static int InvulnerableTurns = 0;
    public static bool Throwing = false;
    
    private static int _level = 1;
    private static int _expToNextLevel = 7;
    private static int _currentExp = 0;

    private static int _healthGrowth = 1;
    private static int _damageGrowth = 1;

    public static void ChangeHealth(int change)
    {
        if (change < 0)
        {
            if(InvulnerableTurns > 0)
            {
                InvulnerableTurns--;
                return;
            }
            if(Throwing)
                return;
        }
        Char.Health = Math.Clamp(Char.Health + change, 0, Char.MaxHealth);
        if (Char.Health == 0)
        {
            GameOver = true;
        }
    }
    public static void AddExp(int amount)
    {
        _currentExp += amount;
        if (_currentExp >= _expToNextLevel)
        {
            LevelUp();
            _currentExp -=  _expToNextLevel;
        }
    }

    private static void LevelUp()
    {
        Char.MaxHealth += _healthGrowth;
        Char.Damage += _damageGrowth;
        
        ChangeHealth(Char.MaxHealth/2);

        _healthGrowth = _level;
        _damageGrowth = _level;
        
        Graphics.InfoOneshot = "Level up!";
    }
}
