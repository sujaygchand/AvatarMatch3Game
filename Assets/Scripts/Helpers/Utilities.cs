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
        public static string GameBoard = "GameBoard";

        public static string FindTileType(GameTileType gameTile)
        {
            return string.Format("T_Avatar{0}Icon", gameTile);
        }

        public static int NumOfGameTileTypes()
        {
            return Enum.GetNames(typeof(GameTileType)).Length;
        }

        public static float ColumnOffset = -3.49f;
        public static float RowOffset = -7.04f;
    }

    public enum GameTileType
    {
        Air,
        Water,
        Earth,
        Fire,
        Equalist
    }

    public enum BoardDirection
    {
        Current,
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
