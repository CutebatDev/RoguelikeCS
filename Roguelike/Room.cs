using System.Text;

namespace Roguelike;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}


public class Cell
{
    public int[] Position = new int[2];
    public Item? CellItem = null;
    public Character? CellCharacter = null;
    public bool isExit = false;

    private enum CellContent
    {
        Item,
        Character
    }
    public Cell(int x, int y)
    {
        Position = [x, y];
        if ((y == (Graphics.ScreenWidth - 2) / 2 && (x == 0 || x == Graphics.ScreenHeight - 3)) ^
            ((y == 0 || y == Graphics.ScreenWidth - 3) && x == (Graphics.ScreenHeight - 2) / 2))
        {
            isExit = true;
        }
    }
    
    public bool AddItem(Item addition)
    {
        if (CellItem == null)
        {
            this.CellItem = addition;
            return true;
        }
        return false;
    }
    public bool AddCharacter(Character addition)
    {
        if (CellCharacter == null)
        {
            this.CellCharacter = addition;
            return true;
        }
        return false;
    }

    public bool IsEmpty()
    {
        if (CellItem == null && CellCharacter == null)
            return true;
        return false;
    }
}

public class Room
{
    public Dictionary<int, Cell> RoomContents = new Dictionary<int, Cell>(); 
    public List<Character> Enemies = new List<Character>();
    public Room(bool lever = false, int enemies = 5, int items = 1)
    {
        RegenerateRoom(lever, enemies, items);
    }

    public void AddItem(Item addition)
    {
        int[] pos = [addition.Position[0], addition.Position[1]];
        RoomContents[RoomExtension.ArrayToIndex(pos)].AddItem(addition);
    }
    public void AddCharacter(Character addition)
    {
        int[] pos = [addition.Position[0], addition.Position[1]];
        RoomContents[RoomExtension.ArrayToIndex(pos)].AddCharacter(addition);
        if(!addition.IsPlayer)
            Enemies.Add(addition);
    }

    public void RemoveCharacterAt(int[] pos)
    {
        int index = RoomExtension.ArrayToIndex(pos);
        
        var cellCharacter = RoomContents[index].CellCharacter;
        if (cellCharacter != null && !cellCharacter.IsPlayer)
            Enemies.Remove(RoomContents[index].CellCharacter);
        
        RoomContents[index].CellCharacter = null;
    }
    public void RemoveCharacterAt(int index)
    {
        var cellCharacter = RoomContents[index].CellCharacter;
        if (cellCharacter != null && !cellCharacter.IsPlayer)
            Enemies.Remove(RoomContents[index].CellCharacter);
        
        RoomContents[index].CellCharacter = null;
    }

    private void CreateEmptyRoom()
    {
        // instantiate all cells
        for (int i = 0; i < Graphics.ScreenWidth-2; i++)
        for (int j = 0; j < Graphics.ScreenHeight-2; j++)
            RoomContents.Add(RoomExtension.ArrayToIndex([j, i]), new Cell(j, i));
    }

    public void RegenerateRoom(bool lever = false, int enemies = 5, int items = 1) // default values for rooms
    {
        RoomContents.Clear();
        CreateEmptyRoom();
        
        for(int i  = 0; i < enemies; i++)
            AddCharacter(new Character(RandomFreePosition(), false, NpcStates.Idle));
        for(int i  = 0; i < items; i++)
            AddItem(new Item(RandomFreePosition(), ItemType.InvenoryItem));
        if(lever)
            AddItem(new Item(RandomFreePosition(), ItemType.Lever));
    }

    private int[] RandomFreePosition()
    {
        Random rand = new Random();
        int index = 0;
        bool isTaken = true;
        while (isTaken){
            do
                index = (int)rand.NextInt64(0, RoomContents.Count - 1);
            while (!RoomContents.ContainsKey(index));
            if (RoomContents[index].IsEmpty())
                isTaken = false;
        }
        return RoomContents[index].Position;
    }
}

public class Dungeon
{
    private int floors = 4;
    Room[][] DungeonRooms; // [floor][room]
    public Room CurrentRoom;
    public int[] RoomPos;
    public int FloorAccess;

    // Constructor creates Dungeon with pyramid layout top-to-bottom
    public Dungeon()
    {
        // first, it creates a dungeon in height
        DungeonRooms = new Room[floors][];
        for (int i = 1; i <= floors; i++)
        {
            // each floor has (2*Number of Floor -1) (counting floors down) rooms (this creates pyramid)
            DungeonRooms[i-1] = new Room[(i * 2) - 1]; 
            int leverRoom = new Random().Next(0, DungeonRooms[i - 1].Length);
            for (int j = 0; j < DungeonRooms[i-1].Length; j++)
            {
                if(j == leverRoom)
                    DungeonRooms[i-1][j] = new Room(true);
                else
                    DungeonRooms[i-1][j] = new Room();
            }
        }

        FloorAccess = floors-1;

        RoomPos = [floors - 1, 1];
        CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
    }

    public bool Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
            {
                // check if on a top floor, if in corner rooms and if has access 
                if (RoomPos[0] != 0 &&
                    (RoomPos[1] != 0 && RoomPos[1] != DungeonRooms[RoomPos[0]].Length - 1) &&
                    FloorAccess < RoomPos[0])
                {
                    RoomPos[0]--;
                    RoomPos[1]--;
                    CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
                    return true;
                }
                break;
            }
            case Direction.Down:
            {
                if (RoomPos[0] != floors - 1)
                {
                    RoomPos[0]++;
                    RoomPos[1]++;
                    CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
                    return true;
                }
                break;
            }
            case Direction.Left:
            {
                if (RoomPos[1] != 0)
                {
                    RoomPos[1]--;
                    CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
                    return true;
                }
                break;
            }
            case Direction.Right:
            {
                if (RoomPos[1] != DungeonRooms[RoomPos[0]].Length - 1)
                {
                    RoomPos[1]++;
                    CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
                    return true;
                }
                break;
            }
        }
        return false;
    }
}

public static class RoomExtension
{
    public static int ArrayToIndex(int[] pos)
    {
        if(pos.Length != 2)
            return -1;
        int newIndex = 0;
        newIndex +=  pos[1];
        newIndex +=  pos[0] * Graphics.ScreenWidth - 2;
        return newIndex;
    }
}