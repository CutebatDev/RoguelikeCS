namespace Roguelike;

class MainClass {
    public static void Main(String[] args)
    {
        Graphics.InitScreen();
        
        Gameplay.CurrentDungeon = new Dungeon();;
        Gameplay.CurrentDungeon.CurrentRoom.AddCharacter(Player.Char);
        
        Graphics.UpdateScreen(Gameplay.CurrentDungeon.CurrentRoom);
        Graphics.DrawScreen();
        while (true)
        {
            Graphics.UpdateScreen(Gameplay.CurrentDungeon.CurrentRoom);
            Graphics.DrawScreen();
            Gameplay.Input(Console.ReadKey());
        }
    }
}
