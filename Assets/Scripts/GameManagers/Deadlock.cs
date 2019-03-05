/**
 * 
 * Author: Sujay Chand
 * 
 *  Checks if not matches can be made (Deadlocked State) and controls shuffling
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadlock : MonoBehaviour
{
    // finds the first possible combo
    public List<GameObject> possibleCombo = new List<GameObject>();
    private GameBoard gameBoard;
    private MatchesManager matchesManager;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
        matchesManager = FindObjectOfType<MatchesManager>();
    }


    /*
     * Fixes the board if deadlocked
     */ 
    public void FixDeadLock()
    {
        if (IsGameDeadlocked())
        {
            ShuffleBoard();
        }
    }

    /*
     * Shuffles the board
     */ 
    private void ShuffleBoard()
    {
        // New collection to store tiles in
        List<GameObject> newTilesCollection = new List<GameObject>();

        // Gets all the pieces on the board
        for(int i = 0; i < gameBoard.width; i++)
        {
            for(int j = 0; j < gameBoard.height; j++)
            {
                if (gameBoard.allGameTiles[i, j])
                {
                    newTilesCollection.Add(gameBoard.allGameTiles[i, j]);
                }
            }
        }

        // Shuffle the new board
        for (int i = 0; i < gameBoard.width; i++)
        {
            for(int j = 0; j < gameBoard.height; j++)
            {
                int newIndex = Random.Range(0, newTilesCollection.Count);

                // Use a similiar check to ensure that the board doesn't shuffle into an immediate match
                GameTileType tempGameTileType = newTilesCollection[newIndex].GetComponent<GameTileBase>().GetGameTileType();

                int maxLoops = 0;

                while (gameBoard.CheckSetUpMatch(i, j, tempGameTileType) && maxLoops < 25){

                    newIndex = Random.Range(0, newTilesCollection.Count);
                    maxLoops++;
                }

                GameTileBase tileScript = newTilesCollection[newIndex].GetComponent<GameTileBase>();

                // Assigns variables to the tiles new position
                tileScript.currentCol = i;
                tileScript.currentRow = j;
                gameBoard.allGameTiles[i, j] = newTilesCollection[newIndex];

                newTilesCollection.Remove(newTilesCollection[newIndex]);

            }
        }

        // Deadlock Check
        if (IsGameDeadlocked())
        {
            ShuffleBoard();
            return;
        } else
        {
            // immediately check for matches
            matchesManager.CheckForMatches();
        }
    }



    /*
     * The deadlock check
     * 
     * @return a bool
     */ 
    private bool IsGameDeadlocked()
    {
    
      // Return false match if no match
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


    /*
     * Checks if a match is valid
     */ 
    private bool CheckForMatchesNeraby(int col, int row, int colIncrement, int rowIncrement)
    {
        // Move tile to new location
        TileSwap(col, row, colIncrement, rowIncrement);
        if (AreMatchesOnBoard())
        {
            // Moves tile back, if valid
            TileSwap(col, row, colIncrement, rowIncrement);
            return true;
        }

        // Moves tile back, if not valid
        TileSwap(col, row, colIncrement, rowIncrement);
        return false;
    }


    /*
     * Swaps tile and checks
     * 
     * @param col
     * @param row
     * @param colIncrement
     * @param rowIncrement
     * 
     */
    private void TileSwap(int col, int row, int colIncrement, int rowIncrement)
    {
        // New tile location
        int tempCol = col + colIncrement;
        int tempRow = row + rowIncrement;

        // Save the other tile to a temp object
        GameObject originalTile = gameBoard.allGameTiles[tempCol, tempRow] as GameObject;

        // Peusdo Swap tiles
        gameBoard.allGameTiles[tempCol, tempRow] = gameBoard.allGameTiles[col, row];

        // Assign the testing tiles to the other tile 
        gameBoard.allGameTiles[col, row] = originalTile;
    }

    /*
     * Check if matches are on board
     */ 
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


    /*
     *  Checks for a match the given location
     *  
     *  @param col
     *  @param row
     *  @param colIncrement
     *  @param rowIncrement
     *  
     *  @return is there match?
     */
    private bool FindMatchAt(int col, int row, int colIncrement, int rowIncrement)
{
    // Clears the previous possible match
     possibleCombo.Clear();

    int tempCol1 = col + colIncrement;
    int tempCol2 = col + (colIncrement * 2);
    int tempRow1 = row + rowIncrement;
    int tempRow2 = row + (rowIncrement * 2);

    int testValue = 0;
    int testLimit = 0;

    // Check for matches left/right
    if (colIncrement > rowIncrement)
    {
        testValue = col;
        testLimit = gameBoard.width - 2;
    }
    else
    {
        // Check for matches up/down
        testValue = row;
        testLimit = gameBoard.height - 2;
    }

    // Ensures no check happens outside the board
    if (testValue < testLimit)
    {
        // null pointer check
        if (gameBoard.allGameTiles[col, row] && gameBoard.allGameTiles[tempCol1, tempRow1] &&
            gameBoard.allGameTiles[tempCol2, tempRow2])
        {
             // Three tiles to compare 
            GameTileBase currentTile = gameBoard.allGameTiles[col, row].GetComponent<GameTileBase>();
            GameTileBase otherTile1 = gameBoard.allGameTiles[tempCol1, tempRow1].GetComponent<GameTileBase>();
            GameTileBase otherTile2 = gameBoard.allGameTiles[tempCol2, tempRow2].GetComponent<GameTileBase>();
            
            // Checks if all are of same type
            if (currentTile.GetGameTileType() == otherTile1.GetGameTileType() &&
                currentTile.GetGameTileType() == otherTile2.GetGameTileType())
            {
                //Debug
                print(string.Format("[ {0} , {1}, {2} ]", currentTile.gameObject, otherTile1.gameObject, otherTile2.gameObject));
                print(string.Format("Col i = {0}, Row i = {1}", colIncrement, rowIncrement));
                print(string.Format("TempCol = {0}, TempRow = {1}, TempCol2 = {2}, TempRow2 = {3}", tempCol1, tempRow1, tempCol2, tempRow2));

                    // Add match to combo
                    GameObject[] combo = { currentTile.gameObject, otherTile1.gameObject, otherTile2.gameObject };
                    MakePossibleCombo(combo);
                    return true;
            }
        }
    }
    return false;
}

    /* 
     * Adds to the match List, to assit with hint
     *
     * @param tiles - the tiles to add
     */ 
    private void MakePossibleCombo(GameObject[] tiles)
    {
        possibleCombo.Clear();

        foreach(GameObject tile in tiles)
        {
            possibleCombo.Add(tile);
        }
    }
}


