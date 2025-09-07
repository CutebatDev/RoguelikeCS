using System.Net.Http.Headers;

namespace Roguelike;

public enum GameState
{
    PlayerAction,   // players turn to move/attack
    MenuActions,    // all controls are used to navigate menus
    Info            // all controls are blocked, information being shown.
}

public static class Gameplay
{
    public static GameState CurrentGameState = GameState.Info;
    public static Dungeon? CurrentDungeon = null;
    public static void Input(ConsoleKeyInfo keyInfo)
    {
        switch (CurrentGameState)
        {
            case GameState.PlayerAction:
                if(keyInfo.Key is ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D)
                    PlayerDirectionalInput(keyInfo.Key);
                if (keyInfo.Key is ConsoleKey.I)
                    ChangeGameState(GameState.MenuActions);
                if (keyInfo.Key is ConsoleKey.Spacebar)
                    PlayerInteractUnput();
                break;
            
            case GameState.MenuActions:
                if (keyInfo.Key is ConsoleKey.W)
                    Inventory.MovePointer(-1);
                else if (keyInfo.Key is ConsoleKey.S)
                    Inventory.MovePointer(1);
                else if (keyInfo.Key is ConsoleKey.Enter){
                    Inventory.UseAtPointer();
                    Graphics.InfoOneshot = "Item used!";
                }
                else if (keyInfo.Key is ConsoleKey.Spacebar)
                    ChangeGameState(GameState.Info);
                break;
            
            case GameState.Info:
                if (keyInfo.Key == ConsoleKey.Spacebar)
                    ChangeGameState(GameState.PlayerAction);
                break;
        }
        
    }

    private static void ChangeGameState(GameState newGameState)
    {
        CurrentGameState = newGameState;
        if (newGameState == GameState.PlayerAction)
            Graphics.InfoText = "Player Action | Use WASD to move, Space to Interact or I to open inventory";
        if (newGameState == GameState.MenuActions)
                Graphics.InfoText = "Menu | Use WASD to navigate the menu, use with Enter and exit with Space";
        if (newGameState == GameState.Info)
            Graphics.InfoText = "Info | Press Space to continue playing";
    }
    
    private static void PlayerDirectionalInput(ConsoleKey key)
    {
        if (CurrentDungeon.CurrentRoom != null)
        {
            // Try Move
            int index = RoomExtension.ArrayToIndex(Player.Char.Position);
            int[] playerPos = Player.Char.Position;
            CurrentDungeon.CurrentRoom.RoomContents[index].CellCharacter = null;

            int targetCellIndex;
            int targetCellContent;
            
            switch (key)
            {
                case ConsoleKey.A:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0], playerPos[1] - 1]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[1]--;
                    break;
                
                case ConsoleKey.D:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0], playerPos[1] + 1]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[1]++;
                    break;
                
                case ConsoleKey.W:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0] - 1, playerPos[1]]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[0]--;
                    break;
                
                case ConsoleKey.S:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0] + 1, playerPos[1]]);
                    targetCellContent = HasEnemy(targetCellIndex);

                    if (targetCellContent == 0)
                        Player.Char.Position[0]++;
                    break;
            }

            CurrentDungeon.CurrentRoom.AddCharacter(Player.Char);
        }
    }

    private static void PlayerInteractUnput()
    {
        if (CurrentDungeon.CurrentRoom != null)
        {
            int index = RoomExtension.ArrayToIndex(Player.Char.Position);
            if (CurrentDungeon.CurrentRoom.RoomContents[index].CellItem != null){
                Inventory.AddItem(CurrentDungeon.CurrentRoom.RoomContents[index].CellItem);
                CurrentDungeon.CurrentRoom.RoomContents[index].CellItem = null;
                Graphics.InfoOneshot = "Item picked up!";
            }
            else
            {
                if (CurrentDungeon.CurrentRoom.RoomContents[index].isExit)
                {
                    
                    int[] pPos = Player.Char.Position;
                    if(pPos[0] == 0){
                        if (CurrentDungeon.Move(Direction.Up))
                            pPos[0] = Graphics.ScreenHeight-3;
                            
                    }
                    else if (pPos[0] == Graphics.ScreenHeight-3)
                    {
                        if (CurrentDungeon.Move(Direction.Down))
                            pPos[0] = 0;
                    }
                    else if(pPos[1] == 0)
                    {
                        if (CurrentDungeon.Move(Direction.Left))
                            pPos[1] = Graphics.ScreenWidth - 3;
                    }
                    else if (pPos[1] == Graphics.ScreenWidth-3)
                    {
                        if (CurrentDungeon.Move(Direction.Right))
                            pPos[1] = 0;
                    }
                    
                    Player.Char.Position = pPos;
                    CurrentDungeon.CurrentRoom.AddCharacter(Player.Char);
                }
            }
        }
    }
    
    private static int HasEnemy(int cellIndex) // -1 - out of bounds, 0 - no enemy, 1 - enemy
    {
        if(CurrentDungeon.CurrentRoom != null && CurrentDungeon.CurrentRoom.RoomContents.ContainsKey(cellIndex))
            if (CurrentDungeon.CurrentRoom.RoomContents[cellIndex].CellCharacter == null)
                return 0;
            else
            {
                CurrentDungeon.CurrentRoom.RoomContents[cellIndex].CellCharacter = null; // ATTACK ENEMY
                AttackEnemyMelee();
                return 1;
            }
        return -1;
    }

    private static void AttackEnemyMelee()
    {
        // STUMP
        Graphics.InfoOneshot = "Enemy defeated! -2hp";
        Player.ChangeHealth(-2);
    }
}