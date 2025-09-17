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
        int playerPosIndex = RoomExtension.ArrayToIndex(Player.Char.Position);
        switch (Type)
        {
            case ItemType.InvenoryItem:
            {
                Inventory.AddItem(cDungeon.CurrentRoom.RoomContents[playerPosIndex].CellItem);
                cDungeon.CurrentRoom.RoomContents[playerPosIndex].CellItem = null;
                Graphics.InfoOneshot = "Item picked up!";
                break;
            }
            case ItemType.Lever:
            {
                if(cDungeon.CurrentRoom.Enemies.Count == 0)
                {
                    if (cDungeon.RoomPos[0] == 0)
                    {
                        Player.GameOver = true;
                    }
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
    int maxHealth = 10, int health = 10, int damage = 2, int enemyLevel = 1)
{
    public int[] Position {get; set;} = position;
    
    public int MaxHealth { get; set; } = maxHealth;
    public int Health { get; set; } = health;
    public int Damage { get; set; } = damage;
    public bool IsPlayer { get; private set; } = isPlayer;
    public NpcStates State {get; set;} = state;

    public int EnemyLevel = enemyLevel;
    
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
        Player.AddExp(EnemyLevel);
    }

    public void EnemyAction()
    {
        if (State == NpcStates.Follow)
        {
            int[] playerPos = Player.Char.Position;
            if (!IsPlayerClose())
            {
                int[] movePos = { Position[0], Position[1] };
                // move towards player on Y axis
                if (playerPos[0] > Position[0])
                    movePos[0] = Position[0] + 1;
                else if (playerPos[0] < Position[0])
                    movePos[0] = Position[0] - 1;
                
                // move towards player on X axis
                if (playerPos[1] > Position[1])
                    movePos[1] = Position[1] + 1;
                else if (playerPos[1] < Position[1])
                    movePos[1] = Position[1] - 1;
                
                // move if Cell is not taken
                if (Gameplay.CurrentDungeon.CurrentRoom.RoomContents[RoomExtension.ArrayToIndex(movePos)]
                        .CellCharacter == null)
                {
                    Gameplay.CurrentDungeon.CurrentRoom.MoveCharacter(this, movePos);
                }
            }
            else
            {
                Graphics.InfoOneshot = "You were attacked!";
                Player.ChangeHealth(-Damage);
            }
        }
        else if (State == NpcStates.Idle)
        {
            if(IsPlayerClose())
                State = NpcStates.Follow;
        }
    }

    private bool IsPlayerClose()
    {
        int[] playerPos = Player.Char.Position;
        int[] distance = { Math.Abs(Position[0] - playerPos[0]), Math.Abs(Position[1] - playerPos[1])};
        return !(distance[0] > 1 || distance[1] > 1);
    }

}