namespace Roguelike;

class MainClass {
    public static void Main(String[] args)
    {
        Graphics.InitScreen();
        
        Room room1 = new Room();
        Gameplay.CurrentRoom = room1;
        Graphics.UpdateScreen(room1);
        Graphics.DrawScreen();
        while (true)
        {
            Graphics.UpdateScreen(room1);
            Graphics.DrawScreen();
            Gameplay.Input(Console.ReadKey());
        }
    }
}
