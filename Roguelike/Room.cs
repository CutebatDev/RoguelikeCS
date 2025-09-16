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
    public Room(bool lever = false, int enemies = 5, int enemyLevel = 1, int items = 0)
    {
        RegenerateRoom(lever, enemies, enemyLevel, items);
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
        RemoveCharacterAt(index);
    }
    public void RemoveCharacterAt(int index)
    {
        var cellCharacter = RoomContents[index].CellCharacter;
        
        if (cellCharacter != null && !cellCharacter.IsPlayer)
            Enemies.Remove(RoomContents[index].CellCharacter);
        
        RoomContents[index].CellCharacter = null;
    }

    public void MoveCharacter(Character moveTarget, int[] newPos)
    {
        int index = RoomExtension.ArrayToIndex(moveTarget.Position);
        RoomContents[index].CellCharacter = null;
        
        index = RoomExtension.ArrayToIndex(newPos);
        RoomContents[index].AddCharacter(moveTarget);
        
        moveTarget.Position = newPos;
    }

    private void CreateEmptyRoom()
    {
        // instantiate all cells
        for (int i = 0; i < Graphics.ScreenWidth-2; i++)
        for (int j = 0; j < Graphics.ScreenHeight-2; j++)
            RoomContents.Add(RoomExtension.ArrayToIndex([j, i]), new Cell(j, i));
    }

    public void RegenerateRoom(bool lever = false, int enemies = 5, int enemyLevel = 1, int items = 0) // default values for rooms
    {
        RoomContents.Clear();
        CreateEmptyRoom();
        
        for(int i  = 0; i < enemies; i++)// CHANGE TO NOT HARDCODED!
        {
            int health = 6 + (4 * enemyLevel); // 10 on lowest, and then 14, 18, 22...
            int damage = 1 + enemyLevel; // 2, 3, 4, 5, 6...
            AddCharacter(new Character(RandomFreePosition(), false, NpcStates.Idle, health, health, damage, enemyLevel));
        }
        for(int i  = 0; i < items; i++)
        {
            ConsumableType consType = (ConsumableType)new Random().Next(0, Enum.GetNames(typeof(ConsumableType)).Length);
            AddItem(new Item(RandomFreePosition(), ItemType.InvenoryItem, consType));
        }
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
    private int floors = 4; // min 2
    public Room[][] DungeonRooms; // [floor][room]
    public Room CurrentRoom;
    public int[] RoomPos; // [floor, room]
    public int FloorAccess;
    private int maxEnemiesPerFloor = 5; // min 1

    // Constructor creates Dungeon with pyramid layout top-to-bottom
    public Dungeon()
    {
        // first, it creates a dungeon in height
        DungeonRooms = new Room[floors][];
        for (int floor = 1; floor <= floors; floor++)
        {
            // each floor has (2*Number of Floor -1) (counting floors down) rooms (this creates pyramid)
            DungeonRooms[floor-1] = new Room[(floor * 2) - 1]; 
            int leverRoom = new Random().Next(0, DungeonRooms[floor - 1].Length);
            for (int room = 0; room < DungeonRooms[floor-1].Length; room++)
            {
                if(room == leverRoom)
                    DungeonRooms[floor-1][room] = new Room(true, maxEnemiesPerFloor, floors - floor);
                else
                {
                    int numOfEnemies = new Random().Next(1, maxEnemiesPerFloor + 1);
                    DungeonRooms[floor-1][room] = new Room(false, numOfEnemies, floors - floor);
                }
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
                if (FloorAccess >= RoomPos[0])
                {
                    Graphics.InfoOneshot = "Door is locked! Find a lever on this floor to open it";
                    break;
                }
                if (RoomPos[0] != 0 &&
                    (RoomPos[1] != 0 && RoomPos[1] != DungeonRooms[RoomPos[0]].Length - 1))
                {
                    RoomPos[0]--;
                    RoomPos[1]--;
                    ChangeCurrentRoom(RoomPos[0], RoomPos[1]);
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
                    ChangeCurrentRoom(RoomPos[0], RoomPos[1]);
                    return true;
                }
                break;
            }
            case Direction.Left:
            {
                if (RoomPos[1] != 0)
                {
                    RoomPos[1]--;
                    ChangeCurrentRoom(RoomPos[0], RoomPos[1]);
                    return true;
                }
                break;
            }
            case Direction.Right:
            {
                if (RoomPos[1] != DungeonRooms[RoomPos[0]].Length - 1)
                {
                    RoomPos[1]++;
                    ChangeCurrentRoom(RoomPos[0], RoomPos[1]);
                    return true;
                }
                break;
            }
        }

        return false;
    }

    private void ChangeCurrentRoom(int floor, int room)
    {
        foreach (var enemy in CurrentRoom.Enemies)
        {
            enemy.State = NpcStates.Idle;
        }
        CurrentRoom.RemoveCharacterAt(Player.Char.Position);
        CurrentRoom = DungeonRooms[floor][room];
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