using System.Transactions;
using Microsoft.VisualBasic;

namespace Roguelike;

public enum NpcStates
{
    Player,
    Idle,
    Follow
}
public enum ItemType
{
    InvenoryItem,
    Lever
}

// Classes

public class Item(int[] position, ItemType type, ConsumableType? consumableType = null)
{
    public int[] Position {get; private set;} = position;
    public ItemType Type {get; private set;} = type;
    public ConsumableType? Consumable {get; private set;} = consumableType;

    
    public void Interact()
    {
        Dungeon cDungeon = Gameplay.CurrentDungeon;
        switch (Type)
        {
            case ItemType.InvenoryItem:
            {
                Inventory.AddItem(cDungeon.CurrentRoom.RoomContents[RoomExtension.ArrayToIndex(Player.Char.Position)].CellItem);
                cDungeon.CurrentRoom.RoomContents[RoomExtension.ArrayToIndex(Player.Char.Position)].CellItem = null;
                Graphics.InfoOneshot = "Item picked up!";
                break;
            }
            case ItemType.Lever:
            {
                if(cDungeon.CurrentRoom.Enemies.Count == 0)
                {
                    if (cDungeon.FloorAccess >= cDungeon.RoomPos[0])
                    {
                        cDungeon.FloorAccess = cDungeon.RoomPos[0] - 1;
                        Graphics.InfoOneshot = "Lever pressed!";
                    }
                    else
                        Graphics.InfoOneshot = "Lever already activated";
                }
                else
                    Graphics.InfoOneshot = "Kill all enemies before pressing lever";
                break;
            }
        }
    }
}

public class Character(int[] position, bool isPlayer = false, NpcStates state = NpcStates.Idle,
    int maxHealth = 10, int health = 10, int damage = 2, int hitRate = 5, int defence = 0)
{
    public int[] Position {get; set;} = position;
    
    public int MaxHealth { get; set; } = maxHealth;
    public int Health { get; set; } = health;
    public int Damage { get; set; } = damage;
    public bool IsPlayer { get; private set; } = isPlayer;
    public NpcStates State {get; private set;} = state;

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0)
        {
            EnemyDie();
        }
    }

    public void EnemyDie()
    {
        Room currentRoom = Gameplay.CurrentDungeon.CurrentRoom;
        currentRoom.RemoveCharacterAt(Position);
        if(new Random().NextInt64(0,2) == 0)
        {
            ConsumableType consType =
                (ConsumableType)new Random().Next(0, Enum.GetNames(typeof(ConsumableType)).Length);
            currentRoom.AddItem(new Item(Position, ItemType.InvenoryItem, consType));
        }
    }

}