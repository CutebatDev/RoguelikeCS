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
    public static Room? CurrentRoom;
    public static void Input(ConsoleKeyInfo keyInfo)
    {
        switch (CurrentGameState)
        {
            case GameState.PlayerAction:
                if(keyInfo.Key is ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D)
                    PlayerDirectionalInput(keyInfo.Key);
                if (keyInfo.Key is ConsoleKey.I)
                    CurrentGameState = GameState.MenuActions;
                break;
            
            case GameState.MenuActions:
                if (keyInfo.Key is ConsoleKey.W)
                    Inventory.MovePointer(-1);
                else if (keyInfo.Key is ConsoleKey.S)
                    Inventory.MovePointer(1);
                else if (keyInfo.Key is ConsoleKey.Enter)
                    Inventory.UseAtPointer();
                else if (keyInfo.Key is ConsoleKey.Spacebar)
                    CurrentGameState = GameState.Info;
                break;
            
            case GameState.Info:
                if (keyInfo.Key == ConsoleKey.Spacebar)
                    CurrentGameState = GameState.PlayerAction;
                break;
        }
        
    }

    private static void PlayerDirectionalInput(ConsoleKey key)
    {
        if (CurrentRoom != null)
        {
            // Try Move
            int index = RoomExtension.ArrayToIndex(Player.Char.Position);
            int[] playerPos = Player.Char.Position;
            CurrentRoom.RoomContents[index].CellCharacter = null;

            int targetCellIndex;
            int targetCellContent;
            
            switch (key)
            {
                case ConsoleKey.A:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0], playerPos[1] - 1]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[1]--;
                    else if (targetCellContent == 1)
                    {
                        CurrentRoom.RoomContents[targetCellIndex].CellCharacter = null; // ATTACK ENEMY
                        AttackEnemyMelee();
                    }
                    break;
                
                case ConsoleKey.D:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0], playerPos[1] + 1]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[1]++;
                    else if (targetCellContent == 1)
                    {
                        CurrentRoom.RoomContents[targetCellIndex].CellCharacter = null; // ATTACK ENEMY
                        AttackEnemyMelee();
                    }
                    break;
                
                case ConsoleKey.W:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0] - 1, playerPos[1]]);
                    targetCellContent = HasEnemy(targetCellIndex);
                    
                    if (targetCellContent == 0)
                        Player.Char.Position[0]--;
                    else if (targetCellContent == 1)
                    {
                        CurrentRoom.RoomContents[targetCellIndex].CellCharacter = null; // ATTACK ENEMY
                        AttackEnemyMelee();
                    }
                    break;
                
                case ConsoleKey.S:
                    targetCellIndex = RoomExtension.ArrayToIndex([playerPos[0] + 1, playerPos[1]]);
                    targetCellContent = HasEnemy(targetCellIndex);

                    if (targetCellContent == 0)
                        Player.Char.Position[0]++;
                    else if (targetCellContent == 1)
                    {
                        CurrentRoom.RoomContents[targetCellIndex].CellCharacter = null; // ATTACK ENEMY
                        AttackEnemyMelee();
                    }
                    break;
            }

            CurrentRoom.AddCharacter(Player.Char);
        }
    }
    
    private static int HasEnemy(int cellIndex) // -1 - out of bounds, 0 - no enemy, 1 - enemy
    {
        if(CurrentRoom != null && CurrentRoom.RoomContents.ContainsKey(cellIndex))
            if (CurrentRoom.RoomContents[cellIndex].CellCharacter == null)
                return 0;
            else
                return 1;
        return -1;
    }

    private static void AttackEnemyMelee()
    {
        // STUMP
    }
}