namespace Roguelike;

class MainClass {
    public static void Main(String[] args)
    {
        Graphics.InitScreen();
        
        Inventory.Contents.Add(new InventoryItem(ConsumableType.Potion));
        Inventory.Contents.Add(new InventoryItem(ConsumableType.Potion));
        Inventory.Contents.Add(new InventoryItem(ConsumableType.Potion));
        
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
