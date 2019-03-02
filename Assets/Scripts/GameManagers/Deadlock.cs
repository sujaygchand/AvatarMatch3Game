using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadlock : MonoBehaviour
{
    GameBoard gameBoard;
    MatchesManager matchesManager;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
        matchesManager = FindObjectOfType<MatchesManager>();
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
                        if(CheckForMatchesNeraby(i, j, 1, 0))
                        {
                            return false;
                        }
                    }

                    if(j < gameBoard.height - 1)
                    {
                        if(CheckForMatchesNeraby(i, j, 0, 1))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }


    private bool CheckForMatchesNeraby(int col, int row, int colIncrement, int rowIncrement)
    {
        TileSwap(col, row, colIncrement, rowIncrement);
        if (AreMatchesOnBoard())
        {
            TileSwap(col, row, colIncrement, rowIncrement);
            return true;
        }

        TileSwap(col, row, colIncrement, rowIncrement);
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
                    GameTileBase currentTile = gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>();

                    if(gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
                    {
                        return true;
                    }

                    if (i < gameBoard.width - 2)
                    {
                        if(FindMatchAt(i, j, 1, 0))
                        {
                                return true;
                            }
                        }
                    }

                    if (j < gameBoard.height - 2)
                    {
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



/* 
 private bool FindMatchAt(int col, int row, SwipeDirection swipeDirection)
 {
     if (gameBoard.allGameTiles[col, row].GetComponent<GameTileBase>())
     {
         GameTileType currentTileType = gameBoard.allGameTiles[col, row].GetComponent<GameTileBase>().GetGameTileType();

         int tempCol1 = col;
         int tempCol2 = col;
         int tempCol3 = col;

         int tempRow1 = row;
         int tempRow2 = row;
         int tempRow3 = row;

         int testValue = 0;
         int testLimit = 0;

         if (swipeDirection == SwipeDirection.Right && col < gameBoard.width - 3)
         {
             testValue = col;
             testLimit = gameBoard.width - 3;

             tempCol1 += 1;
             tempCol2 += 2;
             tempCol3 += 3;

             tempRow1 = row;
             tempRow2 = row;
             tempRow3 = row;

         } else if(swipeDirection == SwipeDirection.Up && row < gameBoard.height - 3)
         {
             testValue = row;
             testLimit = gameBoard.height - 3;

             tempCol1 = col;
             tempCol2 = col;
             tempCol3 = col;

             tempRow1 += 1;
             tempRow2 += 2;
             tempRow3 += 3;

         }

         if(testValue < testLimit)
         {
             //print(swipeDirection + " TestV = " + testValue + " , Limit = " + testLimit);

             if(gameBoard.allGameTiles[tempCol1, tempRow1] && gameBoard.allGameTiles[tempCol2, tempRow2]
                 && gameBoard.allGameTiles[tempCol3, tempRow3])
             {

                 GameTileType tempGameTile2Type = gameBoard.allGameTiles[tempCol2, tempRow2].GetComponent<GameTileBase>().GetGameTileType();
                 GameTileType tempGameTile3Type = gameBoard.allGameTiles[tempCol3, tempRow3].GetComponent<GameTileBase>().GetGameTileType();

                 if(tempGameTile2Type == currentTileType && tempGameTile3Type == currentTileType)
                 {
                     print(swipeDirection);
                     print(string.Format("[ {0} , {1}, {2} ]", gameBoard.allGameTiles[col, row], gameBoard.allGameTiles[tempCol2, tempRow2], gameBoard.allGameTiles[tempCol3, tempRow3]));
                     print(string.Format("TempCol = {0}, TempRow = {1}, TempCol2 = {2}, TempRow2 = {3}", tempCol2, tempRow2, tempCol3, tempRow3));

                     return true;
                 }
             }


         }
     }

     return false;
 }*/

