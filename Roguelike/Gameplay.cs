using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;

namespace Roguelike;

public enum GameState
{
    PlayerAction,   // players turn to move/attack
    MenuActions,    // all controls are used to navigate menus
    Info,            // all controls are blocked, information being shown.
    GameOver
}

public static class Gameplay
{
    public static GameState CurrentGameState = GameState.Info;
    public static Dungeon CurrentDungeon;
    public static bool isEnemyTurn = false;
    public static bool GameWon = false;
    
    public static void Input(ConsoleKeyInfo keyInfo)
    {
        if(Player.GameOver)
            ChangeGameState(GameState.GameOver);
        switch (CurrentGameState)
        {
            case GameState.PlayerAction:
                if(keyInfo.Key is ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D)
                {
                    PlayerDirectionalInput(keyInfo.Key);
                    isEnemyTurn = true;
                }
                if (keyInfo.Key is ConsoleKey.I)
                    ChangeGameState(GameState.MenuActions);
                if (keyInfo.Key is ConsoleKey.Spacebar)
                {
                    PlayerInteractInput();
                    isEnemyTurn = true;
                }
                break;
            
            case GameState.MenuActions:
                if(keyInfo.Key is not ConsoleKey.Spacebar)
                    Menu.Input(keyInfo);
                else
                    ChangeGameState(GameState.PlayerAction);
                break;
            
            case GameState.Info: // temp disabled
                if (keyInfo.Key == ConsoleKey.Spacebar)
                    ChangeGameState(GameState.PlayerAction);
                break;
            case GameState.GameOver:
            {
                break;
            }
        }

        EnemyTurn();

    }

    private static void EnemyTurn()
    {
        if (isEnemyTurn)
        {
            foreach (var enemy in CurrentDungeon.CurrentRoom.Enemies)
            {
                enemy.EnemyAction();
            }
            isEnemyTurn = false;
        }
    }

    private static void ChangeGameState(GameState newGameState)
    {
        CurrentGameState = newGameState;
        if (newGameState == GameState.PlayerAction)
            if(Player.Throwing)
                Graphics.InfoText = "Player Action | Throwing a dagger! Use WASD to throw, Space to Interact or I to open inventory";
            else
                Graphics.InfoText = "Player Action | Use WASD to move, Space to Interact or I to open inventory";
        if (newGameState == GameState.MenuActions)
                Graphics.InfoText = "Menu | Use WASD to navigate the menu, use with Enter and exit with Space";
        if (newGameState == GameState.Info)
            Graphics.InfoText = "Info | Press Space to continue playing";
        if(newGameState == GameState.GameOver)
            if(CurrentDungeon.RoomPos[0] == 0 && Player.Char.Health > 0)
                Graphics.InfoText = " | Victory! | ";
            else
                Graphics.InfoText = " | You are dead | ";
    }
    
    private static void PlayerDirectionalInput(ConsoleKey key)
    {
        if (CurrentDungeon.CurrentRoom != null)
        {
            if (Player.Throwing)
            {
                ThrowDagger(key);
                return;
            }
            // Try Move
            int index = RoomExtension.ArrayToIndex(Player.Char.Position);
            int[] playerPos = Player.Char.Position;
            CurrentDungeon.CurrentRoom.RemoveCharacterAt(index);

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

    private static void ThrowDagger(ConsoleKey key)
    {
        int index = RoomExtension.ArrayToIndex(Player.Char.Position);
        
        int targetCellIndex;
        int[] targetCellPos = { Player.Char.Position[0], Player.Char.Position[1] };
        int targetCellContent = 0;
        switch (key)
        {
            case ConsoleKey.A:
                do
                {
                    targetCellPos = [targetCellPos[0], targetCellPos[1] - 1];
                    targetCellIndex = RoomExtension.ArrayToIndex(targetCellPos);
                    targetCellContent = HasEnemy(targetCellIndex);
                } while (targetCellContent == 0);
                break;
                
            case ConsoleKey.D:
                do
                {
                    targetCellPos = [targetCellPos[0], targetCellPos[1] + 1];
                    targetCellIndex = RoomExtension.ArrayToIndex(targetCellPos);
                    targetCellContent = HasEnemy(targetCellIndex);
                } while (targetCellContent == 0);
                break;
            case ConsoleKey.W:
                do
                {
                    targetCellPos = [targetCellPos[0] - 1, targetCellPos[1]];
                    targetCellIndex = RoomExtension.ArrayToIndex(targetCellPos);
                    targetCellContent = HasEnemy(targetCellIndex);
                } while (targetCellContent == 0);
                break;
                
            case ConsoleKey.S:
                do
                {
                    targetCellPos = [targetCellPos[0] + 1, targetCellPos[1]];
                    targetCellIndex = RoomExtension.ArrayToIndex(targetCellPos);
                    targetCellContent = HasEnemy(targetCellIndex);
                } while (targetCellContent == 0);
                break;
            
            default:
                return;
        }
        
        if (targetCellContent == -1)
        {
            Graphics.InfoOneshot = "Dagger Missed";
        }
        else
        {
            CurrentDungeon.CurrentRoom.RoomContents[targetCellIndex].CellCharacter.TakeDamage(
                (int)(Player.Char.Damage * Inventory.DaggerDmgMod));
        }

        Player.Throwing = false;
    }

    private static void PlayerInteractInput()
    {
        if (CurrentDungeon.CurrentRoom != null)
        {
            int index = RoomExtension.ArrayToIndex(Player.Char.Position);
            if (CurrentDungeon.CurrentRoom.RoomContents[index].CellItem != null)
                CurrentDungeon.CurrentRoom.RoomContents[index].CellItem.Interact();
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
        {
            if (CurrentDungeon.CurrentRoom.RoomContents[cellIndex].CellCharacter == null)
                return 0;
            AttackEnemyMelee(cellIndex);
            return 1;
        }
        return -1;
    }

    private static void AttackEnemyMelee(int cellIndex)
    {
        if(CurrentDungeon.CurrentRoom.RoomContents[cellIndex].CellCharacter != null)
        {
            Character enemy = CurrentDungeon.CurrentRoom.RoomContents[cellIndex].CellCharacter;
            enemy.TakeDamage(Player.Char.Damage);
        }
    }
}