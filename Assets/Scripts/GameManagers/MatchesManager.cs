using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class MatchesManager : MonoBehaviour
{
    private GameBoard gameBoard;
    public List<GameObject> currentMatches = new List<GameObject>();

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

        if (tile.Length > 0)
        {
            DestoryMatch();
        }
    }

    public void DestoryMatch()
    {
        gameBoard.DestroyMatches();
    }

    private IEnumerator CheckForMatches_Cor()
    {
        yield return new WaitForSeconds(.1f);


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

                        if (i > 0 && i < gameBoard.width - 1)
                        {
                            GameObject rightTile = gameBoard.allGameTiles[i + 1, j];
                            GameObject leftTile = gameBoard.allGameTiles[i - 1, j];

                            if (rightTile && leftTile)
                            {

                                GameTileType rightTileType = rightTile.GetComponent<GameTileBase>().GetGameTileType();
                                GameTileType leftTileType = leftTile.GetComponent<GameTileBase>().GetGameTileType();

                                if (currentTileType == rightTileType && currentTileType == leftTileType)
                                {
                                    GameObject[] tiles = { currentTile, rightTile, leftTile};

                                    AddToList(tiles);

                                }
                            }
                        }

                        if( j > 0 && j < gameBoard.height - 1)
                        {
                            GameObject upTile = gameBoard.allGameTiles[i, j + 1];
                            GameObject downTile = gameBoard.allGameTiles[i, j - 1];

                            if (upTile && downTile)
                            {
                                GameTileType upTileType = upTile.GetComponent<GameTileBase>().GetGameTileType();
                                GameTileType downTileType = downTile.GetComponent<GameTileBase>().GetGameTileType();

                                if (currentTileType == upTileType && currentTileType == downTileType)
                                {
                                    GameObject[] tiles = { currentTile, upTile, downTile };

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
