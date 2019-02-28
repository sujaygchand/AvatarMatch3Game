using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GameTileChar : GameTileBase
{
    // if it is not a Row Tile it must be a column tile
    private bool isRowTile = true;
    [SerializeField] private SpriteRenderer arrowImage;

    private void Awake()
    {
        tileImage = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tileType = TileType.Char;

        swipeThreshold = 0.7f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SetUpProperties(bool isRowTile, int col, int row, GameTileType gameTileType, GameBoard gameBoard, MatchesManager matchesManager, GameObject gameGridObject)
    {
        SetIsRowTile(isRowTile);

        this.gameGridObject = gameGridObject;
        currentCol = col;
        currentRow = row;
        SetGameTileType(gameTileType);
        this.gameBoard = gameBoard;
        this.matchesManager = matchesManager;


        //gameBoard.allGameTiles[col, row] = null;
        gameBoard.allGameTiles[col, row] = gameObject;
    }

    public void SetIsRowTile(bool isRowTile)
    {
        if (!isRowTile)
        {
            arrowImage.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
        } else
        {
            arrowImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        

        this.isRowTile = isRowTile;
    }

    public bool GetIsRowTile()
    {
        return isRowTile;
    }


}
