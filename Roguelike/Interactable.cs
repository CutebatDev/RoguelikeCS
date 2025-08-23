using System.Transactions;
using Microsoft.VisualBasic;

namespace Roguelike;

public enum NpcStates
{
    Player,
    Idle,
    Aggresive,
    Dead
}
public enum ItemType
{
    Coin,
    Weapon,
    Potion,
    Lever
}

// Classes

public class Item(int[] position, ItemType type)
{
    public int[] Position {get; private set;} = position;
    public ItemType Type {get; private set;} = type;

    public void Interact(Character player)
    {
        // do switch case here
    }
}

public class Character(int[] position, bool isPlayer, bool ranged, NpcStates state)
{
    public int[] Position {get; set;} = position;
    public int Health { get; private set; } = 5;
    public int Damage { get; private set; } = 1;
    public bool IsPlayer { get; private set; } = isPlayer;
    public bool Ranged {get; private set;} = ranged;
    public NpcStates State {get; private set;} = state;


}