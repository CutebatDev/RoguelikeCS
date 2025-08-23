using System.Text;

namespace Roguelike;
public class Cell
{
    public int[] Position = new int[2];
    public Item? CellItem = null;
    public Character? CellCharacter = null;

    public Cell(int x, int y)
    {
        Position = [x, y];
    }
    private enum CellContent
    {
        Item = 0,
        Character = 1
    }

    public bool AddItem(Item addition)
    {
        if (CellItem != null)
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
    public int floor; // floor number
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
        Player.Char.Position = [0, 1];
        AddCharacter(Player.Char);
        for (int i = 0; i < 5; i++)
        {
            do
                index = (int)rand.NextInt64(0, RoomContents.Count - 1);
            while (!RoomContents.ContainsKey(index));
            Console.WriteLine(RoomContents[index].CellCharacter);
            if (RoomContents[index].CellCharacter == null)
            {
                Character newEnemy = new Character(RoomContents[index].Position, false, false, NpcStates.Idle);
                AddCharacter(newEnemy);
            }
        }
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