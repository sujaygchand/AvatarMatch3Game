using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GameBoard : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject[,] allGameTiles;

    [SerializeField] GameObject s_GridTile;
    [SerializeField] GameObject s_GameTile;
    [SerializeField] private GameObject gameGridObject;
    private GridTitle [ , ] gameGrid;
    
    

    // Start is called before the first frame update
    void Start()
    {
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

        for( int i = 0; i < width; i++ )
        {
            for(int j = 0; j < height; j++)
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

        GameObject tempTile = Instantiate(tileObject, tempCoord, Quaternion.identity, gameGridObject.transform);

        if (tempTile.GetComponent<GameTileBase>())
        {
            tempTile.GetComponent<GameTileBase>().currentCol = x;
            tempTile.GetComponent<GameTileBase>().currentRow = y;
        }

        // For organisational and debugging purposes
        tempTile.name = string.Format("{0}: ({1}, {2})", name, x, y);

        if (returnObject)
        {
            return tempTile;
        } else
        {
            return null;
        }


    }

}
