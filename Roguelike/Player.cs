namespace Roguelike;

public static class Player
{
    public static Character Char = new Character([(Graphics.ScreenHeight - 2)/2, (Graphics.ScreenWidth - 2)/2], true,  NpcStates.Player);
    public static int Health = 10;
    public static int MaxHealth = 20;
    public static bool Invulnerable = false;

    public static void ChangeHealth(int change)
    {
        if (Invulnerable && change < 0)
        {
            Invulnerable = false;
            return;
        }
        Health = Math.Clamp(Health + change, 0, MaxHealth);
    }
}
public static class Inventory
{
    public static List<InventoryItem> Contents = new List<InventoryItem>();
    public static int pointer = 0;
    public static void UseAtPointer()
    {
        if(Contents.Count < 1)
            return;
        Contents[pointer].Use();
        Contents.RemoveAt(pointer);
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
        pointer = Math.Clamp((pointer + direction), 0, Contents.Count - 1);
    }

    public static void PrintInventory()
    {
        Console.WriteLine("[] Inventory []");
        Console.WriteLine();
        for (int i = 0; i < Contents.Count; i++)
        {
            if(pointer == i)
                Console.Write("> ");
            Console.WriteLine(Contents[i].Type.ToString());
        }
        Console.WriteLine();
        Console.WriteLine("W and S to move pointer, Enter to use item");
    }
}


public class InventoryItem
{
    public ConsumableType Type;

    public InventoryItem(ConsumableType type = ConsumableType.Potion)
    {
        Type = type;
    }

    public void Use()
    {
        if (Type == ConsumableType.Potion)
        {
            Player.ChangeHealth(5);
        }
        else if (Type == ConsumableType.Shield)
        {
            Player.Invulnerable = true;
        }
        else if (Type == ConsumableType.Dagger)
        {
            // throw dagger
        }
    }
}

public enum ConsumableType
{
    Potion,
    Shield,
    Dagger
}