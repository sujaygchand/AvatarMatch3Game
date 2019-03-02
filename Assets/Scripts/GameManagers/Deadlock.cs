using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadlock : MonoBehaviour
{
    GameBoard gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
    }

    public void FixDeadLock()
    {
        if (IsGameDeadlocked())
        {
            print("Deadlocked!!!!");
        } else
        {
            print("Up your arsenal");
        }
    }

    private bool IsGameDeadlocked()
    {
        for (int i = 0; i < gameBoard.width; i++)
        {
            for(int j = 0; j < gameBoard.height; j++)
            {
                if(gameBoard.allGameTiles[i, j])
                {
                    if(i < gameBoard.width - 1)
                    {
                        if(SwapAndCheckForMatch(i, j, 1, 0))
                        {
                            return false;
                        }
                    }

                    if(j < gameBoard.height - 1)
                    {
                        if(SwapAndCheckForMatch(i, j, 0, 1))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }


    private bool SwapAndCheckForMatch(int col, int row, int colIncrement, int rowIncrement)
    {
        TileSwap(col, row, colIncrement, rowIncrement);
        if (AreMatchesOnBoard())
        {
            TileSwap(col, row, colIncrement, rowIncrement);
            return true;
        }
        
        return false;
    }


    private void TileSwap(int col, int row, int colIncrement, int rowIncrement)
    {
        int tempCol = col + colIncrement;
        int tempRow = row + rowIncrement;

        // Save the other tile to a temp object
        GameObject originalTile = gameBoard.allGameTiles[tempCol, tempRow] as GameObject;

        // Peusdo Swap tiles
        gameBoard.allGameTiles[tempCol, tempRow] = gameBoard.allGameTiles[col, row];

        // Assign the testing tiles to the other tile 
        gameBoard.allGameTiles[col, row] = originalTile;
    }


    private bool AreMatchesOnBoard()
    {
        for(int i = 0; i < gameBoard.width; i++)
        {
            for(int j = 0; j < gameBoard.height; j++)
            {
                if(gameBoard.allGameTiles[i, j])
                {
                    if(gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
                    {
                        return true;
                    }

                    if (FindMatchAt(i, j, 1, 0))
                    {
                        return true;
                    }

                    if(FindMatchAt(i, j, 0, 1))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool FindMatchAt(int col, int row, int colIncrement, int rowIncrement)
    {
        int tempCol1 = col + colIncrement;
        int tempCol2 = col + (colIncrement * 2);
        int tempRow1 = row + rowIncrement;
        int tempRow2 = row + (rowIncrement * 2);

        int testValue = 0;
        int testLimit = 0;

        if (colIncrement > rowIncrement)
        {
            testValue = col;
            testLimit = gameBoard.width - 2;
        }
        else
        {
            testValue = row;
            testLimit = gameBoard.height - 2;
        }

        if (testValue < testLimit)
        {
            // null pointer check
            if (gameBoard.allGameTiles[tempCol1, tempRow1] &&
                gameBoard.allGameTiles[tempCol2, tempRow2])
            {
                GameTileBase currentTile = gameBoard.allGameTiles[col, row].GetComponent<GameTileBase>();
                GameTileBase otherTile1 = gameBoard.allGameTiles[tempCol1, tempRow1].GetComponent<GameTileBase>();
                GameTileBase otherTile2 = gameBoard.allGameTiles[tempCol2, tempRow2].GetComponent<GameTileBase>();

                if (currentTile.GetGameTileType() == otherTile1.GetGameTileType() &&
                    currentTile.GetGameTileType() == otherTile2.GetGameTileType())
                {
                    print(string.Format("[ {0} , {1}, {2} ]", currentTile.gameObject, otherTile1.gameObject, otherTile2.gameObject));
                    print(string.Format("Col i = {0}, Row i = {1}", colIncrement, rowIncrement));
                    print(string.Format("TempCol = {0}, TempRow = {1}, TempCol2 = {2}, TempRow2 = {3}", tempCol1, tempRow1, tempCol2, tempRow2));
                    return true;
                }
            }
        }
        return false;
    }


}
