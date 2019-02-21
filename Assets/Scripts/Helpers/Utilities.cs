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

        public static string FindTileType(GameTile gameTile)
        {
            return string.Format("T_Avatar{0}Icon", gameTile);
        }

        public static int NumOfGameTileTypes()
        {
            return Enum.GetNames(typeof(GameTile)).Length;
        }
    }

    public enum GameTile
    {
        Air,
        Water,
        Earth,
        Fire,
        Equalist
    }
}
