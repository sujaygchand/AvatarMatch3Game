using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{

    public static class Utilities
    {
        public static string Resources = "Resources";
        public static string Prefabs = "Prefabs";
        public static string PF = "PF_";
        public static string GridTile = "GridTile";
        public static string Grid = "Grid";
        public static string GameTiles = "GameTiles";
        public static string BaseGameTile = "BaseGameTile";
        public static string CharGameTile = "CharGameTile";
        public static string GameBoard = "GameBoard";
        

        public static string FindTileType(TileType tileType, GameTileType gameTile)
        {
            return string.Format("{0}/T_Avatar{1}Icon{0}", tileType, gameTile);
        }

        public static int NumOfGameTileTypes()
        {
            return Enum.GetNames(typeof(GameTileType)).Length;
        }

        public static float ColumnOffset = 0; //-3.49f;
        public static float RowOffset = 0; //-7.04f;
    }

    public enum GameTileType
    {
        Air,
        Water,
        Earth,
        Fire,
        Equalist
    }

    public enum TileType
    {
        Normal,
        Char,
        Weapon,
        Avatar
    }

    public enum SwipeDirection
    {
        None,
        Up,
        Right,
        Down,
        Left
    }

    public enum PlayerState
    {
        Wait,
        Active
    }
}
