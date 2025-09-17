using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace Roguelike;

static class Graphics
{
    // KEEP THOSE 2 NUMBER UNEVEN!!
    // changing those 2 values shouldn't break anything, but you will get bigger map! maybe could be used to make different rooms 🤔
    public const int ScreenWidth = 17;
    public const int ScreenHeight = 9;

    public static string InfoText = "Welcome to the Roguelike! Press Space to start playing";
    public static string? InfoOneshot = null;
    
    private static readonly Dictionary<string, char> GraphicsChar = new Dictionary<string, char>()
    {
        { "Wall", '#'  },
        { "Door", '.'  },
        { "Space", ' '  },
        { "Player", '@'  },
        { "Enemy", 'E'  },
        { "Item", '?'  }
    };
    private static readonly Dictionary<char, ConsoleColor> CharColors = new Dictionary<char, ConsoleColor>()
    {
        { '#', ConsoleColor.Gray  },
        { '.', ConsoleColor.Gray  },
        { ' ', ConsoleColor.Gray  },
        { '@', ConsoleColor.Magenta  },
        { 'E', ConsoleColor.Red  },
        { '?', ConsoleColor.Yellow  }
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
        Dungeon dungeon = Gameplay.CurrentDungeon;
        
        int currentFloor = dungeon.RoomPos[0];
        int currentRoomIndex = dungeon.RoomPos[1];
        int floorsCount = dungeon.DungeonRooms.Length;
        int roomsOnFloorCount = dungeon.DungeonRooms[currentFloor].Length;

        
        // Fill contents
        for (int y = 0; y < ScreenHeight; y++)
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                int[] pos = [y - 1, x -1]; // -1 to account to walls
                
                // place doors, walls and empty spaces
                if ((y == ScreenHeight / 2 && (x == 0 || x == ScreenWidth - 1)) ^ ((y == 0 || y == ScreenHeight - 1) && x == ScreenWidth / 2))
                {
                    bool isLeftEdgeOfScreen = x == 0;
                    bool isRightEdgeOfScreen = x == ScreenWidth - 1;
                    bool isTopEdgeOfScreen = y == 0;
                    bool isBottomEdgeOfScreen = y == ScreenHeight - 1;

                    bool shouldNotPlaceDoorHere =
                        ((isLeftEdgeOfScreen || isTopEdgeOfScreen) && currentRoomIndex == 0) ||
                        ((isRightEdgeOfScreen|| isTopEdgeOfScreen) && currentRoomIndex == roomsOnFloorCount - 1) ||
                        (isTopEdgeOfScreen && currentFloor == 0) ||
                        (isBottomEdgeOfScreen && currentFloor == floorsCount - 1);

                    _screen[y, x] = GraphicsChar[!shouldNotPlaceDoorHere ? "Door" : "Wall"];

                }
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
        Console.Write("Health :");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("|");
        for (int i = 0; i < Player.Char.MaxHealth; i++)
        {
            if(i < Player.Char.Health)
                Console.Write("#");
            else
                Console.Write(" ");
        }
        Console.WriteLine("|");
        Console.ResetColor();
        int index = 0;


        if (Gameplay.CurrentGameState != GameState.MenuActions)
        {
            foreach (char value in _screen)
            {
                PrintColor(value, CharColors[value]);
                Console.Write("  ");
                index++;

                if (index % ScreenWidth == 0)
                    Console.WriteLine();
            }
        }
        else
        {
            Menu.PrintMenu();
        }
        Console.WriteLine(InfoOneshot ?? InfoText);
        InfoOneshot = null;
    }

    private static void PrintColor(char c, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(c);
        Console.ResetColor();
    }
}