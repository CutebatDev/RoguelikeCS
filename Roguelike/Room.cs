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

    public Cell(int x, int y)
    {
        Position = [x, y];
        if ((y == (Graphics.ScreenWidth - 2) / 2 && (x == 0 || x == Graphics.ScreenHeight - 3)) ^
            ((y == 0 || y == Graphics.ScreenWidth - 3) && x == (Graphics.ScreenHeight - 2) / 2))
        {
            isExit = true;
        }
    }
    private enum CellContent
    {
        Item = 0,
        Character = 1
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
    public Room()
    {
        RegenerateRoom();
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
    }

    private void CreateEmptyRoom()
    {
        // instantiate all cells
        for (int i = 0; i < Graphics.ScreenWidth-2; i++)
        for (int j = 0; j < Graphics.ScreenHeight-2; j++)
            RoomContents.Add(RoomExtension.ArrayToIndex([j, i]), new Cell(j, i));
    }

    public void RegenerateRoom()
    {
        RoomContents.Clear();
        CreateEmptyRoom();
        
        // Populate room with up to 5 enemies at random places
        Random rand = new Random();
        int index = 0;
        Item item = new Item([1, 1], ItemType.InvenoryItem);
        AddItem(item);
        for (int i = 0; i < 5; i++)
        {
            do
                index = (int)rand.NextInt64(0, RoomContents.Count - 1);
            while (!RoomContents.ContainsKey(index));
            Console.WriteLine(RoomContents[index].CellCharacter);
            if (RoomContents[index].CellCharacter == null)
            {
                Character newEnemy = new Character(RoomContents[index].Position, false, NpcStates.Idle);
                AddCharacter(newEnemy);
            }
        }
    }
}

public class Dungeon
{
    private int floors = 4;
    Room[][] DungeonRooms; // [floor][room]
    public Room CurrentRoom;
    public int[] RoomPos;

    // Constructor creates Dungeon with pyramid layout top-to-bottom
    public Dungeon()
    {
        // first it creates it in height
        DungeonRooms = new Room[floors][];
        for (int i = 1; i <= floors; i++)
        {
            // each floor has (2*Number of Floor -1) (counting floors down) rooms (this creates pyramid)
            DungeonRooms[i-1] = new Room[(i * 2) - 1]; 
            for (int j = 0; j < DungeonRooms[i-1].Length; j++)
            {
                DungeonRooms[i-1][j] = new Room();
            }
        }

        RoomPos = [floors - 1, 1];
        CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
    }

    public bool Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
            {
                if (RoomPos[0] != 0 &&
                    (RoomPos[1] != 0 && RoomPos[1] != DungeonRooms[RoomPos[0]].Length - 1))
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

    // public void ChangeCurrentRoom()
    // {
    //     CurrentRoom.RoomContents[RoomExtension.ArrayToIndex(Player.Char.Position)].CellCharacter = null;
    //     CurrentRoom = DungeonRooms[RoomPos[0]][RoomPos[1]];
    //     CurrentRoom.RoomContents[RoomExtension.ArrayToIndex(Player.Char.Position)].AddCharacter(Player.Char);
    // }
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