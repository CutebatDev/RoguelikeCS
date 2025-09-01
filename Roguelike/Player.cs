namespace Roguelike;

public static class Player
{
    public static Character Char = new Character([0, 0], true,  NpcStates.Player);
    
    
}
public static class Inventory
{
    public static List<InventoryItem> Contents = new List<InventoryItem>();
    public static int pointer = 0;
    public static void UseAtPointer()
    {
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

public enum ConsumableType
{
    Potion,
    Staff
}
public class InventoryItem
{
    public ConsumableType Type;

    public InventoryItem(ConsumableType type)
    {
        Type = type;
    }

    public InventoryItem()
    {
        Type = ConsumableType.Potion;
    }

    public void Use()
    {
        if (Type == ConsumableType.Potion)
        {
            //player.Heal();
        }
        else if (Type == ConsumableType.Staff)
        {
            //player.CastSpell();
        }
    }
}