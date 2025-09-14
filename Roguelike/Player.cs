namespace Roguelike;

public static class Player
{
    public static Character Char = new Character
        ([(Graphics.ScreenHeight - 2)/2, (Graphics.ScreenWidth - 2)/2], true,  NpcStates.Player,
            20,  20,  5,  15,  10);
    
    public static int InvulnerableTurns = 0;
    public static bool Throwing = false;

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
    }
}
