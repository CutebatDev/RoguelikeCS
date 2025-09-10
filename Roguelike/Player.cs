namespace Roguelike;

public static class Player
{
    public static Character Char = new Character
        ([(Graphics.ScreenHeight - 2)/2, (Graphics.ScreenWidth - 2)/2], true,  NpcStates.Player,
            20,  10,  5,  15,  10);
    
    public static bool Invulnerable = false;

    public static void ChangeHealth(int change)
    {
        if (Invulnerable && change < 0)
        {
            Invulnerable = false;
            return;
        }
        Char.Health = Math.Clamp(Char.Health + change, 0, Char.MaxHealth);
    }
}
