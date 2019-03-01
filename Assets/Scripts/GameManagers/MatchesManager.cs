using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Helpers;

public class MatchesManager : MonoBehaviour
{
    private GameBoard gameBoard;
    public List<GameObject> currentMatches = new List<GameObject>();
    public bool isMatching = false;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
    }

    public void CheckForMatches()
    {
        StartCoroutine(CheckForMatches_Cor());
    }

    private void AddToList(GameObject[] tile)
    {
        foreach (GameObject tempTile in tile)
        {
            if (!currentMatches.Contains(tempTile))
            {
                currentMatches.Add(tempTile);
                tempTile.GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }
        
    }



    public void DestoryMatch()
    {
        gameBoard.DestroyMatches();
    }

    private IEnumerator CheckForMatches_Cor()
    {
        yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime() - 0.25f);

        if (gameBoard)
        {
            for (int i = 0; i < gameBoard.width; i++)
            {
                for (int j = 0; j < gameBoard.height; j++)
                {
                    GameObject currentTile = gameBoard.allGameTiles[i, j];
                    
                    if (currentTile)
                    {
                        GameTileType currentTileType = currentTile.GetComponent<GameTileBase>().GetGameTileType();

                        if (i >= 0 && i < gameBoard.width)
                        {
                            GameObject[] NearbyTiles = GetTestingTiles(i, j, 1, 0);

                            GameObject rowTile1 = NearbyTiles[0];
                            GameObject rowTile2 = NearbyTiles[1];

                            if (rowTile1 && rowTile2)
                            {

                                GameTileType rowTile1Type = rowTile1.GetComponent<GameTileBase>().GetGameTileType();
                                GameTileType rowTile2Type = rowTile2.GetComponent<GameTileBase>().GetGameTileType();

                                if (currentTileType == rowTile1Type && currentTileType == rowTile2Type)
                                {

                                    GameObject[] tiles = { currentTile, rowTile1, rowTile2 };

                                    MatchCharTile(tiles);
                                    MatchGliderTile(tiles);

                                    AddToList(tiles);

                                }
                            }

                            if (j >= 0 && j < gameBoard.height)
                            {
                                NearbyTiles = GetTestingTiles(i, j, 0, 1);

                                GameObject colTile1 = NearbyTiles[0];
                                GameObject colTile2 = NearbyTiles[1];

                                if (colTile1 && colTile2)
                                {
                                    GameTileType colTile1Type = colTile1.GetComponent<GameTileBase>().GetGameTileType();
                                    GameTileType colTile2Type = colTile2.GetComponent<GameTileBase>().GetGameTileType();

                                    if (currentTileType == colTile1Type && currentTileType == colTile2Type)
                                    {
                                        GameObject[] tiles = { currentTile, colTile1, colTile2 };

                                        MatchCharTile(tiles);
                                        MatchGliderTile(tiles);

                                        AddToList(tiles);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private GameObject[] GetTestingTiles(int col, int row, int colIncrement, int rowIncrement)
    {
        GameObject tile1 = null;
        GameObject tile2 = null;

        int testValue = 0;
        int testLimit = 0;

        if(colIncrement > rowIncrement)
        {
            testValue = col;
            testLimit = gameBoard.width - 1;
        } else
        {
            testValue = row;
            testLimit = gameBoard.height - 1;
        }


        if(testValue > 0 &&  testValue < testLimit)
        {
            tile1 = gameBoard.allGameTiles[col + colIncrement, row + rowIncrement];
            tile2 = gameBoard.allGameTiles[col - colIncrement, row - rowIncrement];
        }
        else if(testValue == 0)
        {
            tile1 = gameBoard.allGameTiles[col + colIncrement, row + rowIncrement];
            tile2 = gameBoard.allGameTiles[col + (colIncrement * 2), row + (rowIncrement * 2)];
        }
        else if(testValue == testLimit)
        {
            tile1 = gameBoard.allGameTiles[col - colIncrement, row - rowIncrement];
            tile2 = gameBoard.allGameTiles[col - (colIncrement * 2), row - (rowIncrement * 2)];
        }

        GameObject[] tiles = { tile1, tile2 };

        return tiles;
    }

    private bool CheckMatchAlignment(int passValue)
    {
        int numOfColTiles = 0;
        int numOfRowTiles = 0;

        if (currentMatches[0].GetComponent<GameTileBase>())
        {
            GameTileBase firstTile = currentMatches[0].GetComponent<GameTileBase>();

        foreach (GameObject tile in currentMatches)
            {

                if (tile.GetComponent<GameTileBase>())
                {
                    GameTileBase gameTile = tile.GetComponent<GameTileBase>();

                    if(gameTile.currentCol == firstTile.currentCol)
                    {
                        numOfColTiles++;
                    }
                    if(gameTile.currentRow == firstTile.currentRow)
                    {
                        numOfRowTiles++;
                    }
                }
            }
        }

        if(numOfColTiles == passValue || numOfRowTiles == passValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MakeSpecialTileCheck()
    {
        // Check if move was made
        if (gameBoard.currentTile)
        {

            if(currentMatches.Count == 4 || currentMatches.Count == 7)
            {
                
                if(gameBoard.currentTile.swipeDirection == SwipeDirection.Left || gameBoard.currentTile.swipeDirection == SwipeDirection.Right)
                {
                    MakeCharTile(true);
                }

                if (gameBoard.currentTile.swipeDirection == SwipeDirection.Down || gameBoard.currentTile.swipeDirection == SwipeDirection.Up)
                {
                    MakeCharTile(false);
                }
            }

            else if(currentMatches.Count == 5 || currentMatches.Count == 8)
            {
                if (CheckMatchAlignment(5))
                {
                    MakeAvatarTile();
                    print("Make Avatar");

                }
                else
                {
                    MakeGliderTile();
                    print("Make Glide");
                }
                
            }
            
        }
        
    }


    public void MakeCharTile(bool isRow)
    {
        // Make bomb
        if (gameBoard.currentTile.GetHasMatched())
        {
            gameBoard.currentTile.SetHasMatched(false);

            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateCharTile(isRow);
        }
        
        else if(gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                otherTile.SetHasMatched(false);

                otherTile.GenerateCharTile(isRow);
            }
        }
    }


    public void MakeGliderTile()
    {
        if (gameBoard.currentTile.GetHasMatched())
        {
            gameBoard.currentTile.SetHasMatched(false);

            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateGliderTile();
        } 
        else if (gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                otherTile.SetHasMatched(false);

                otherTile.GenerateGliderTile();
            }
        }
    }

    public void MakeAvatarTile()
    {
        if (CheckForAvatarTileInList())
        {
            return;
        }

        if (gameBoard.currentTile.GetHasMatched())
        {
            gameBoard.currentTile.SetHasMatched(false);
            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateAvatarTile();

        }
        else if (gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                otherTile.SetHasMatched(false);

                otherTile.GenerateAvatarTile();
            }
        }
    }

    private void MatchCharTile(GameObject[] tiles)
    {
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<GameTileBase>().isRowChar)
            {
                currentMatches.Union(GetRowMatches(tile.GetComponent<GameTileBase>().currentRow));
            }
            else if (tile.GetComponent<GameTileBase>().isColChar)
            {
                currentMatches.Union(GetColMatches(tile.GetComponent<GameTileBase>().currentCol));
            }
        }
    }

    private void MatchGliderTile(GameObject[] tiles)
    {
        foreach(GameObject tile in tiles)
        {
            if (tile.GetComponent<GameTileBase>())
            {
                GameTileBase tempTile = tile.GetComponent<GameTileBase>();

                if (tempTile.GetTileType() == TileType.Glider)
                {
                    currentMatches.Union(GetGliderMatches(tempTile.currentCol, tempTile.currentRow));
                }
            }
        }
    }


    public void MatchAvatarTile(GameObject tile)
    {
        GameTileType testTileType = tile.GetComponent<GameTileBase>().GetGameTileType();

        for (int i = 0; i < gameBoard.width; i++)
        {
            for (int j = 0; j < gameBoard.height; j++)
            {
                if (gameBoard.allGameTiles[i, j])
                {
                    if (tile.GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
                    {
                        gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                    }

                    else if (gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetGameTileType() == testTileType)
                    {
                        gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                    }
                }
            }
        }
    }



    public bool CheckForAvatarTileInList()
    {
        foreach(GameObject tile in currentMatches)
        {
            if(tile.GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
            {
                return true;
            }
        }

        return false;
    }

    private List<GameObject> GetGliderMatches(int col, int row)
    {
        List<GameObject> tiles = new List<GameObject>();

        for(int i = col - 1; i <= col + 1; i++)
        {
            for(int j = row - 1; j <= row + 1; j++)
            {
                // Check if the piece is inside the board
                if(i >= 0 && i < gameBoard.width && j >= 0 && j < gameBoard.height) { 
}                   tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }

        return tiles;
    }

    private List<GameObject> GetColMatches(int col)
    {
        List<GameObject> tiles = new List<GameObject>();

        for(int i = 0; i < gameBoard.height; i++)
        {
            if (gameBoard.allGameTiles[col, i] != null)
            {
                tiles.Add(gameBoard.allGameTiles[col, i]);
                gameBoard.allGameTiles[col, i].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }
        return tiles;
    }

    private List<GameObject> GetRowMatches(int row)
    {
        List<GameObject> tiles = new List<GameObject>();

        for (int i = 0; i < gameBoard.width; i++)
        {
            if (gameBoard.allGameTiles[i, row] != null)
            {
                tiles.Add(gameBoard.allGameTiles[i, row]);
                gameBoard.allGameTiles[i, row].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }
        return tiles;
    }

    private List<GameObject> GetAvatarMatches(GameObject tile)
    {
        GameTileBase tileScript = tile.GetComponent<GameTileBase>();

        List<GameObject> tiles = new List<GameObject>();

        if (!tile)
        {
            return tiles;
        }

        GameTileType gameTileType = tileScript.GetGameTileType();
        print("Bending " + tile + " and " + gameTileType);

        for (int i = 0; i < gameBoard.width; i++)
        {
            for(int j = 0; j < gameBoard.height; j++)
            {
                if(tileScript == gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>())
                {
                    tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[tileScript.currentCol, tileScript.currentRow].GetComponent<GameTileBase>().SetHasMatched(true);
                }

                if(gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetGameTileType() == gameTileType)
                {
                    tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                }

                else if (tileScript.GetTileType() == TileType.Avatar && 
                    tileScript.GetOtherTile().GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
                {
                    tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                }
            }

        }

        return tiles;
    }
}
