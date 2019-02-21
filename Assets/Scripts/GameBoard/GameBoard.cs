using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GameBoard : MonoBehaviour
{
    [SerializeField] int s_Width;
    [SerializeField] int s_Height;

    [SerializeField] GameObject s_GridTile;
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
        for( int i = 0; i < s_Width; i++ )
        {
            for(int j = 0; j < s_Height; j++)
            {
                // Offest spawn location from the gameboard location
                Vector2 tempCoord = new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j);
                GameObject tempTile = Instantiate(s_GridTile, tempCoord, Quaternion.identity, gameGridObject.transform);
                tempTile.name = "GirdTile (" + i + ", " + j  + ")";
            }
        }
    }
}
