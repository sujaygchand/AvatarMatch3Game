/**
 * 
 * Author: Sujay Chand
 * 
 *  A static class that hold enums, static values and string values (To avoid mistake, when typing strings)
 *  
 */

using System;


namespace Assets.Scripts.Helpers
{

    public static class Utilities
    {
        // Used between scenes and a large amount of objects
        public static GameMode GameMode = GameMode.TimeAttack;
        public static bool IsSoundActive = true;
        public static bool IsMusicActive = true;
        public static bool IsGamePaused = false;
        
        // String helpers
        public static string Resources = "Resources";
        public static string Prefabs = "Prefabs";
        public static string PF = "PF_";
        public static string GridTile = "GridTile";
        public static string Grid = "Grid";
        public static string GameTiles = "GameTiles";
        public static string BaseGameTile = "BaseGameTile";
        public static string CharGameTile = "CharGameTile";
        public static string GameBoard = "GameBoard";
        public static string AvatarIcon = "T_AvatarIcon";
        public static string Collection = "Tiles Lefts";
        public static string TimeAttack = "Tiles Collected";
        public static string Moves = "Moves";
        public static string Time = "Time";
        public static string GameLevel = "SC_Game";
        public static string StartMenu = "SC_TitleScreen";
        public static string DeadlockMap = "SC_Deadlock";
        public static string Music = "Music";

        // Helps makes string path to tile image
        public static string FindTileType(TileType tileType, GameTileType gameTile)
        {
            return string.Format("{0}/T_Avatar{1}Icon{0}", tileType, gameTile);
        }

        // Massive number of tiles
        public static int NumOfGameTileTypes()
        {
            return Enum.GetNames(typeof(GameTileType)).Length - 1;
        }

        
        public static float ColumnOffset = 0; //-3.49f;
        public static float RowOffset = 0; //-7.04f;
    }

}

// Stores Game tile types (Decided to use 5 tiles, after testing)
public enum GameTileType
{
    Air,
    Water,
    Earth,
    Fire,
    Equalist,
    WaterTribe,
    EarthTribe,
    FireTribe,
    AirTribe,
    None
}

// Tile types
// Char = Striped Bomb
// Glider = Wrapped Candy
// Avatar = Colour Bomb
public enum TileType
{
    Normal,
    Char,
    Glider,
    Avatar
}

// Swipe direction
public enum SwipeDirection
{
    None,
    Up,
    Right,
    Down,
    Left
}

// Player state
public enum PlayerState
{
    Wait,
    Active
}

// The game modes
public enum GameMode
{
    Collection,
    TimeAttack,
    Deadlocked
}

