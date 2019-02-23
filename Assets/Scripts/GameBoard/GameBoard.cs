using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GameBoard : MonoBehaviour
{
    [Header("Game Board variables")]
    public int width;
    public int height;
    public GameObject[,] allGameTiles;
    public int NumOfTileTypes = 0;

    [SerializeField] GameObject s_GridTile;
    [SerializeField] GameObject s_GameTile;
    [SerializeField] private GameObject gameGridObject;
    private GridTitle[,] gameGrid;



    // Start is called before the first frame update
    void Start()
    {
        if (NumOfTileTypes == 0 || NumOfTileTypes > Utilities.NumOfGameTileTypes())
        {
            NumOfTileTypes = Utilities.NumOfGameTileTypes();
        }

        // If the artist hasn't set a board, render default
        if (!s_GridTile)
        {
            // Loads the tile from the resources folder
            s_GridTile = Resources.Load<GameObject>(string.Format("{0}/{1}{2}", Utilities.Prefabs, Utilities.PF, Utilities.GridTile));
        }

        // If the artist hasn't set a board, render default
        if (!s_GameTile)
        {
            s_GameTile = Resources.Load<GameObject>(
               string.Format("{0}/{1}{2}", Utilities.Prefabs, Utilities.PF, Utilities.BaseGameTile)
               );
        }

        // Null point check
        if (s_GridTile)
        {
            DrawBoard();
        }

        //gameGridObject = GameObject.FindGameObjectWithTag(Utilities.Grid);
    }

    /*
     * Draws the background tiles for a sizeable game board 
     */
    void DrawBoard()
    {
        allGameTiles = new GameObject[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Background tiles
                SpawnTile(s_GridTile, "GridTile", i, j, false);

                // Game tiles
                allGameTiles[i, j] = SpawnTile(s_GameTile, "GameTile", i, j, true);
            }
        }
    }


    /*
     * Spawns a tile
     * 
     * @param 
     */
    public GameObject SpawnTile(GameObject tileObject, string name, int x, int y, bool returnObject)
    {
        // Offest spawn location by the gameboard location
        Vector2 tempCoord = new Vector2(gameObject.transform.position.x + x, gameObject.transform.position.y + y);

        GameTileType tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

        if (returnObject)
        {   
            int maxLoops = 0;

            while(CheckSetUpMatch(x, y, tileObject, tempTileType) && maxLoops < 25)
            {
                tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);
                
                maxLoops++;
            }

        }

        GameObject tempTile = Instantiate(tileObject, tempCoord, Quaternion.identity, gameGridObject.transform);

        if (tempTile.GetComponent<GameTileBase>())
        {
            tempTile.GetComponent<GameTileBase>().SetTileType(tempTileType);
            tempTile.GetComponent<GameTileBase>().currentCol = x;
            tempTile.GetComponent<GameTileBase>().currentRow = y;
        }

        // For organisational and debugging purposes
        tempTile.name = string.Format("{0}: ({1}, {2})", name, x, y);

        if (returnObject)
        {
            return tempTile;
        }
        else
        {
            return null;
        }

    }

    private bool CheckSetUpMatch(int col, int row, GameObject tile, GameTileType tileType)
    {
        
        // Quick method to check when the thrid col/row are tested
        if (col > 1 && row > 1)
        {
            // Horizontal check
            GameTileType leftTile1Type = allGameTiles[col - 1, row].GetComponent<GameTileBase>().GetGameTileType();
            GameTileType leftTile2Type = allGameTiles[col - 2, row].GetComponent<GameTileBase>().GetGameTileType();

            
            if (tileType == leftTile1Type && tileType == leftTile2Type)
            {
                return true;
            }

            // Vertical Check
            GameTileType BottomTile1Type = allGameTiles[col, row - 1].GetComponent<GameTileBase>().GetGameTileType();
            GameTileType BottomTile2Type = allGameTiles[col, row - 2].GetComponent<GameTileBase>().GetGameTileType();

            if (tileType == BottomTile1Type && tileType == BottomTile2Type)
            {
                return true;
            }

        }

        // 
        else if(col <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allGameTiles[col, row - 1].GetComponent<GameTileBase>() && allGameTiles[col, row - 2].GetComponent<GameTileBase>())
                {
                    GameTileType LeftTile1Type = allGameTiles[col, row - 1].GetComponent<GameTileBase>().GetGameTileType();
                    GameTileType LeftTile2Type = allGameTiles[col, row - 2].GetComponent<GameTileBase>().GetGameTileType();

                    if (tileType == LeftTile1Type && tileType == LeftTile2Type)
                    {
                        return true;
                    }
                }
            }

            if(col > 1)
            {
                if(allGameTiles[col - 1, row].GetComponent<GameTileBase>() && allGameTiles[col - 2, row].GetComponent<GameTileBase>())
                {
                    GameTileType BottomTile1Type = allGameTiles[col - 1, row].GetComponent<GameTileBase>().GetGameTileType();
                    GameTileType BottomTile2Type = allGameTiles[col - 2, row].GetComponent<GameTileBase>().GetGameTileType();

                    if (tileType == BottomTile1Type && tileType == BottomTile2Type)
                    {
                        return true;
                    }
                }
         
            }
        }


        return false;

    }

}
