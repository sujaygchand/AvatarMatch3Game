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
                            GameObject rowTile1 = null;
                            GameObject rowTile2 = null;

                            if(i > 0 && i < gameBoard.width - 1)
                            {
                                rowTile1 = gameBoard.allGameTiles[i + 1, j];
                                rowTile2 = gameBoard.allGameTiles[i - 1, j];
                            }
                            else if(i == 0)
                            {
                                rowTile1 = gameBoard.allGameTiles[i + 1, j];
                                rowTile2 = gameBoard.allGameTiles[i + 2, j];
                            }

                            else if(i == gameBoard.width - 1)
                            {
                                rowTile1 = gameBoard.allGameTiles[i - 1, j];
                                rowTile2 = gameBoard.allGameTiles[i - 2, j];
                            }

                            if (rowTile1 && rowTile2)
                            {

                                GameTileType rowTile1Type = rowTile1.GetComponent<GameTileBase>().GetGameTileType();
                                GameTileType rowTile2Type = rowTile2.GetComponent<GameTileBase>().GetGameTileType();

                                if (currentTileType == rowTile1Type && currentTileType == rowTile2Type)
                                {

                                    GameObject[] tiles = { currentTile, rowTile1, rowTile2};

                                    foreach(GameObject tempTile in tiles)
                                    {
                                        if (tempTile.GetComponent<GameTileBase>().isRowChar)
                                        {
                                            currentMatches.Union(GetRowMatches(tempTile.GetComponent<GameTileBase>().currentRow));
                                        }

                                        if (tempTile.GetComponent<GameTileBase>().isColChar)
                                        {
                                            currentMatches.Union(GetColMatches(tempTile.GetComponent<GameTileBase>().currentCol));
                                        }
                                    }

                                    AddToList(tiles);

                                }
                        }

                        if( j >= 0 && j < gameBoard.height)
                        {
                            GameObject colTile1 = null;
                            GameObject colTile2 = null;

                                if (j > 0 && j < gameBoard.height - 1)
                                {
                                    colTile1 = gameBoard.allGameTiles[i, j + 1];
                                    colTile2 = gameBoard.allGameTiles[i, j - 1];
                                }

                                else if (j == 0)
                                {
                                    colTile1 = gameBoard.allGameTiles[i, j + 1];
                                    colTile2 = gameBoard.allGameTiles[i, j + 2];
                                }
                                else if (j == gameBoard.height - 1)
                                {
                                    colTile1 = gameBoard.allGameTiles[i, j - 1];
                                    colTile2 = gameBoard.allGameTiles[i, j - 2];
                                }

                                if (colTile1 && colTile2)
                                {
                                    GameTileType colTile1Type = colTile1.GetComponent<GameTileBase>().GetGameTileType();
                                    GameTileType colTile2Type = colTile2.GetComponent<GameTileBase>().GetGameTileType();

                                    if (currentTileType == colTile1Type && currentTileType == colTile2Type)
                                    {
                                        GameObject[] tiles = { currentTile, colTile1, colTile2 };

                                        foreach (GameObject tempTile in tiles)
                                        {
                                            if (tempTile.GetComponent<GameTileBase>().isRowChar)
                                            {
                                                currentMatches.Union(GetRowMatches(tempTile.GetComponent<GameTileBase>().currentRow));
                                            }

                                            if (tempTile.GetComponent<GameTileBase>().isColChar)
                                            {
                                                currentMatches.Union(GetColMatches(tempTile.GetComponent<GameTileBase>().currentCol));
                                            }
                                        }

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

    public void MakeSpecialTile()
    {
        
    }

    public void MakeCharTile()
    {

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
}
