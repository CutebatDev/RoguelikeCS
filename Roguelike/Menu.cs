namespace Roguelike;

public static class Menu
{
    public static MenuTabs CurrentTab = MenuTabs.Inventory;

    public static void Input(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key is (ConsoleKey.A or ConsoleKey.D))
        {
            ChangeTab();
            return;
        }
        switch (CurrentTab)
        {
            case MenuTabs.Inventory:
                if (keyInfo.Key is ConsoleKey.W)
                    Inventory.MovePointer(-1);
                else if (keyInfo.Key is ConsoleKey.S)
                    Inventory.MovePointer(1);
                else if (keyInfo.Key is ConsoleKey.Enter){
                    Inventory.UseAtPointer();
                }
                break;
            case MenuTabs.Map:
                break;
        }
    }
    
    public static void ChangeTab()
    {
        if(CurrentTab != MenuTabs.Inventory)
            CurrentTab = MenuTabs.Inventory;
        else
            CurrentTab = MenuTabs.Map;
    }
    public static void PrintMenu()
    {
        switch (CurrentTab)
        {
            case MenuTabs.Inventory:
            {
                Inventory.PrintInventory();
                break;
            }
            case MenuTabs.Map:
            {
                Map.PrintMap();
                break;
            }
        }
    }
}

public enum MenuTabs
{
    Inventory,
    Character,
    Map
}
public static class Inventory
{
    public static List<InventoryItem> Contents = new List<InventoryItem>();
    private static int _pointer = 0;

    public static int PotionHealing = 5;
    public static float DaggerDmgMod = 2.0f;
    
    
    public static void UseAtPointer()
    {
        if(Contents.Count < 1)
            return;
        if (Contents[_pointer].Use())
            Contents.RemoveAt(_pointer);
        if (_pointer >= Contents.Count)
            _pointer--;
    }

    public static void AddItem(Item item)
    {
        if (item.Type == ItemType.InvenoryItem)
        {
            Contents.Add(new InventoryItem(item.Consumable ?? ConsumableType.Potion));
        }
    }

    public static void MovePointer(int direction) // -1 : up, 1 : down
    {
        if(Contents.Count < 1)
            return;
        Console.WriteLine(Contents.Count);
        _pointer = Math.Clamp((_pointer + direction), 0, Contents.Count - 1);
    }

    public static void PrintInventory()
    {
        Console.WriteLine(" # Inventory # |   Map");
        Console.WriteLine();
        for (int i = 0; i < Contents.Count; i++)
        {
            if(_pointer == i)
                Console.Write("> ");
            Console.WriteLine(Contents[i].Type.ToString());
        }
        Console.WriteLine();
    }
}


public class InventoryItem(ConsumableType type = ConsumableType.Potion)
{
    public ConsumableType Type = type;

    public bool Use()
    {
        if (Type == ConsumableType.Potion)
        {
            if (Player.Char.Health == Player.Char.MaxHealth)
            {
                Graphics.InfoOneshot = "Health already full!";
                return false;
            }
            Player.ChangeHealth(Inventory.PotionHealing);
            Graphics.InfoOneshot = "Potion used! Restored 5 hp";
        }
        else if (Type == ConsumableType.Shield)
        {
            Player.InvulnerableTurns++;
            Graphics.InfoOneshot = "Shield used! One extra attack will be negated";
        }
        else if (Type == ConsumableType.Dagger)
        {
            if (Player.Throwing)
            {
                Graphics.InfoOneshot = "Dagger already Equipped!";
                return false;
            }
            Player.Throwing = true;
            Graphics.InfoOneshot = "Dagger equipped! Move to throw it";
        }
        return true;
    }
}
public enum ConsumableType
{
    Potion,
    Shield,
    Dagger
}

public static class Map
{    private static readonly Dictionary<string, string> MapSymbols = new Dictionary<string, string>()
    {
        { "EmptySpace", "      "  },
        { "EmptyRoom",  "[ ]   "  },
        { "PlayerRoom", "[@]   "  }
    };
    public static void PrintMap()
    {
        Room[][] dRooms = Gameplay.CurrentDungeon.DungeonRooms;
        Console.WriteLine("   Inventory   | # Map # ");
        Console.WriteLine();
        for(int floor = 0; floor < dRooms.Length; floor++)
        {
            for(int emptys = 0; emptys < dRooms.Length - floor; emptys++)
                Console.Write(MapSymbols["EmptySpace"]);
            for(int room = 0; room < dRooms[floor].Length; room++)
            {
                if(dRooms[floor][room] == Gameplay.CurrentDungeon.CurrentRoom)
                    Console.Write(MapSymbols["PlayerRoom"]);
                else
                    Console.Write(MapSymbols["EmptyRoom"]);
            }
            Console.WriteLine('\n');
        }
        
    }
}