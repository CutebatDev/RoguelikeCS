using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace Roguelike;

static class Graphics
{
    // KEEP THOSE 2 NUMBER UNEVEN!!
    // changing those 2 values shouldn't break anything, but you will get bigger map! maybe could be used to make different rooms 🤔
    public const int ScreenWidth = 17;
    public const int ScreenHeight = 9;
    
    private static readonly Dictionary<string, char> GraphicsChar = new Dictionary<string, char>()
    {
        { "Wall", '#'  },
        { "Door", '.'  },
        { "Space", ' '  },
        { "Player", '@'  },
        { "Enemy", 'E'  },
        { "Item", '?'  }
    };
    
    private static char[,] _screen = new char[ScreenHeight, ScreenWidth];
    
    public static void InitScreen()
    {
        // Fill contents with empty spaces
        for (int y = 0; y < ScreenHeight; y++)
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                _screen[y, x] = GraphicsChar["Space"];
            }
        }
    }

    public static void UpdateScreen(Room room)
    {
        // Fill contents
        for (int y = 0; y < ScreenHeight; y++)
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                int[] pos = [y - 1, x -1]; // -1 to account to walls
                
                // place doors, walls and empty spaces
                if ((y == ScreenHeight / 2 && (x == 0 || x == ScreenWidth - 1)) ^ ((y == 0 || y == ScreenHeight - 1) && x == ScreenWidth / 2))
                    _screen[y,x] = GraphicsChar["Door"];
                else if (x == ScreenWidth - 1 || x == 0 || y == ScreenHeight - 1 || y == 0)
                    _screen[y, x] = GraphicsChar["Wall"];
                else if (room.RoomContents[RoomExtension.ArrayToIndex(pos)].IsEmpty())
                    _screen[y, x] = GraphicsChar["Space"];
                // is cell is not empty - it has either item or character
                else
                {
                    if (room.RoomContents[RoomExtension.ArrayToIndex(pos)].CellCharacter != null)
                    {
                        if (room.RoomContents[RoomExtension.ArrayToIndex(pos)].CellCharacter.IsPlayer)
                            _screen[y, x] = GraphicsChar["Player"];
                        else
                            _screen[y, x] = GraphicsChar["Enemy"];
                    }
                    else if (room.RoomContents[RoomExtension.ArrayToIndex(pos)].CellItem != null)
                        _screen[y, x] = GraphicsChar["Item"];
                }
            }
        }
    }
    
    public static void DrawScreen()
    {
        Console.Clear();
        Console.WriteLine("Status"); // ADD STATUS HERE
        int index = 0;
        foreach (char value in _screen)
        {
            Console.Write($"{value}  ");
            index++;
            
            if (index % ScreenWidth == 0)
                Console.WriteLine();
        }

        switch (Gameplay.CurrentGameState)
        {
            case GameState.Info:
                Console.WriteLine("Info");
                break;
            case GameState.MenuActions:
                Console.WriteLine("Menu");
                break;
            case GameState.PlayerAction:
                Console.WriteLine("Player Action");
                break;
        }
    }
}
