/**
 * 
 * Author: Sujay Chand
 * 
 *  Score manager
 */
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
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();

        audioSource = GetComponent<AudioSource>();

        ToggleSound();
    }

    /*
     * Toggle sound for match effect
     */ 
    public void ToggleSound()
    {
        audioSource.enabled = Utilities.IsSoundActive;
    }

    /*
     * Starts coroutine for "Checks for matches on board coroutine" 
     */ 
    public void CheckForMatches()
    {
        StartCoroutine(CheckForMatches_Cor());
    }

    /*
     * Add to current matches list
     */ 
    private void AddToList(GameObject[] tile)
    {
        foreach (GameObject tempTile in tile)
        {
            if (!currentMatches.Contains(tempTile))
            {
                currentMatches.Add(tempTile);
            }
            tempTile.GetComponent<GameTileBase>().SetHasMatched(true);
        }

        audioSource.Play();
    }

    /*
     * Destroy matches
     */ 
    public void DestoryMatch()
    {
        gameBoard.DestroyMatches();
    }

    /*
     * Checks for matches on board 
     */
    private IEnumerator CheckForMatches_Cor()
    {
        yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime() / 2);

        if (gameBoard)
        {
            for (int i = 0; i < gameBoard.width; i++)
            {
                for (int j = 0; j < gameBoard.height; j++)
                {
                    GameObject currentTile = gameBoard.allGameTiles[i, j];
                    
                    // Null pointer check
                    if (currentTile)
                    {
                        GameTileType currentTileType = currentTile.GetComponent<GameTileBase>().GetGameTileType();

                        // Horizontal Check
                        if (i >= 0 && i < gameBoard.width)
                        {
                            GameObject[] NearbyTiles = GetTestingTiles(i, j, 1, 0);

                            GameObject rowTile1 = NearbyTiles[0];
                            GameObject rowTile2 = NearbyTiles[1];

                            if (rowTile1 && rowTile2)
                            {

                                GameTileType rowTile1Type = rowTile1.GetComponent<GameTileBase>().GetGameTileType();
                                GameTileType rowTile2Type = rowTile2.GetComponent<GameTileBase>().GetGameTileType();

                                // Compare types
                                if (currentTileType == rowTile1Type && currentTileType == rowTile2Type)
                                {

                                    GameObject[] tiles = { currentTile, rowTile1, rowTile2 };

                                        MatchCharTile(tiles);
                                        MatchGliderTile(tiles);

                                        AddToList(tiles);

                                }
                            }

                            // Vertical check
                            if (j >= 0 && j < gameBoard.height)
                            {
                                NearbyTiles = GetTestingTiles(i, j, 0, 1);

                                GameObject colTile1 = NearbyTiles[0];
                                GameObject colTile2 = NearbyTiles[1];

                                if (colTile1 && colTile2)
                                {
                                    GameTileType colTile1Type = colTile1.GetComponent<GameTileBase>().GetGameTileType();
                                    GameTileType colTile2Type = colTile2.GetComponent<GameTileBase>().GetGameTileType();

                                    // Compare types
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

        //yield return null;

    }

    /*
     * Get the tiles to test
     */ 
    private GameObject[] GetTestingTiles(int col, int row, int colIncrement, int rowIncrement)
    {
        GameObject tile1 = null;
        GameObject tile2 = null;

        int testValue = 0;
        int testLimit = 0;

        // Horizontal check
        if(colIncrement > rowIncrement)
        {
            testValue = col;
            testLimit = gameBoard.width - 1;
        }
        // Vertical check
        else
        {
            testValue = row;
            testLimit = gameBoard.height - 1;
        }
        
        // Tiles left/down and right/up
        if(testValue > 0 &&  testValue < testLimit)
        {
            tile1 = gameBoard.allGameTiles[col + colIncrement, row + rowIncrement];
            tile2 = gameBoard.allGameTiles[col - colIncrement, row - rowIncrement];
        }
        // Tiles right/up and right+1/up+1
        else if (testValue == 0)
        {
            tile1 = gameBoard.allGameTiles[col + colIncrement, row + rowIncrement];
            tile2 = gameBoard.allGameTiles[col + (colIncrement * 2), row + (rowIncrement * 2)];
        }
        // Tiles left/down and left-1/down-1
        else if (testValue == testLimit)
        {
            tile1 = gameBoard.allGameTiles[col - colIncrement, row - rowIncrement];
            tile2 = gameBoard.allGameTiles[col - (colIncrement * 2), row - (rowIncrement * 2)];
        }

        GameObject[] tiles = { tile1, tile2 };

        return tiles;
    }

    /*
     * Check tile alignment
     *
     * @param passValue value to pass, for a true return
     * 
     * @return has passed?
     */
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

                    // Add to Col count
                    if(gameTile.currentCol == firstTile.currentCol)
                    {
                        numOfColTiles++;
                    }
                    // Add to Row count
                    if (gameTile.currentRow == firstTile.currentRow)
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

    /*
     * Check to see if special tile
     * 
     */ 
    public void MakeSpecialTileCheck()
    {

        // Check if move was made
        if (gameBoard.currentTile)
        {
            // Char tiles
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
                // Avatar tiles
                if (CheckMatchAlignment(5))
                {
                    MakeAvatarTile();

                }
                // Glider tiles
                else
                {
                    MakeGliderTile();
                }
                
            }
            
        }

        // Clears matches
        currentMatches.Clear();

    }

    /*
     * Makes char tile
     * 
     * @param isRow true = row, false = col
     */ 
    public void MakeCharTile(bool isRow)
    {
        // current tile
        if (gameBoard.currentTile.GetHasMatched())
        {
            currentMatches.Remove(gameBoard.currentTile.gameObject);
            gameBoard.currentTile.SetHasMatched(false);

            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateCharTile(isRow);

        }
        
        // other tile
        else if(gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                currentMatches.Remove(gameBoard.currentTile.GetOtherTile());
                otherTile.SetHasMatched(false);

                otherTile.GenerateCharTile(isRow);
            }
        }

    }

    /*
     * Makes glider tile
     */ 
    public void MakeGliderTile()
    {
        // Current tile
        if (gameBoard.currentTile.GetHasMatched())
        {
            currentMatches.Remove(gameBoard.currentTile.gameObject);
            gameBoard.currentTile.SetHasMatched(false);

            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateGliderTile();
        } 

        // other tile
        else if (gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                currentMatches.Remove(gameBoard.currentTile.GetOtherTile());
                otherTile.SetHasMatched(false);

                otherTile.GenerateGliderTile();
            }
        }
    }

    /*
     * Makes Avatar tile
     */ 
    public void MakeAvatarTile()
    {
        if (CheckForAvatarTileInList())
        {
            return;
        }

        // Current tile
        if (gameBoard.currentTile.GetHasMatched())
        {
            currentMatches.Remove(gameBoard.currentTile.gameObject);
            gameBoard.currentTile.SetHasMatched(false);
            gameBoard.currentTile.GetComponent<GameTileBase>().GenerateAvatarTile();

        }
        // Other tile
        else if (gameBoard.currentTile.GetOtherTile())
        {
            GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

            if (otherTile.GetHasMatched())
            {
                currentMatches.Remove(gameBoard.currentTile.GetOtherTile());
                otherTile.SetHasMatched(false);

                otherTile.GenerateAvatarTile();
            }
        }
    }

    /*
     * Match Char Tiles list
     * 
     * @param tiles
     */ 
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

    /*
     * Match single Char Tile
     * 
     * @param tiles
     */
    private void MatchCharTile(GameObject tile)
        {

        if (tile)
        {
            if(tile.GetComponent<GameTileBase>().isRowChar){
                currentMatches.Union(GetRowMatches(tile.GetComponent<GameTileBase>().currentRow));
            }

            else if (tile.GetComponent<GameTileBase>().isColChar)
            {
                currentMatches.Union(GetColMatches(tile.GetComponent<GameTileBase>().currentCol));
            }
        }
        }

    /*
     * Match Glider Tiles list
     * 
     * @param tiles
     */
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

    /*
     * Match single Glider Tile
     * 
     * @param tiles
     */
    private void MatchGliderTile(GameObject tile)
    {
        if (tile)
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

    /*
     * Match Avatar Tile
     * 
     * @param tiles
     */
    public void MatchAvatarTile(GameObject tile)
    {
        GameTileType tempTileType = tile.GetComponent<GameTileBase>().GetGameTileType();

        for (int i = 0; i < gameBoard.width; i++)
        {
            for (int j = 0; j < gameBoard.height; j++)
            {
                if (gameBoard.allGameTiles[i, j])
                {
                    GameTileBase tempTile = gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>();

                    // Match all tiles
                    if (tile.GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
                    {
                        tempTile.SetHasMatched(true);
                    }

                    // Finds tile of same type
                    else if (gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetGameTileType() == tempTileType)
                    {
                        // Chain char tiles
                        if(tempTile.GetTileType() == TileType.Char)
                        {
                            MatchCharTile(tempTile.gameObject);
                        }

                        // Chain Glider tiles
                        else if(tempTile.GetTileType() == TileType.Glider)
                        {
                            MatchGliderTile(tempTile.gameObject);
                        }
                            tempTile.SetHasMatched(true);
                    }
                }
            }
        }
    }


    /*
     * Checks if avatar tile is in the list
     */ 
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

    /*
     * Finds tiles to destroy by Glider tile 
     */ 
    private List<GameObject> GetGliderMatches(int col, int row)
    {
        List<GameObject> tiles = new List<GameObject>();

        int minCol = col;
        int maxCol = col;
        int minRow = row;
        int maxRow = row; 

        if(col > 0)
        {
            minCol = col - 1;
        }

        if(col < gameBoard.width - 1)
        {
            maxCol = col + 1;
        }

        if (row > 0)
        {
            minRow = row - 1;
        }

        if (row < gameBoard.height - 1)
        {
            maxRow = row + 1;
        }

        for (int i = minCol; i <= maxCol + 1; i++)
        {
            for(int j = minRow; j <= maxRow + 1; j++)
            {
                // Check if the piece is inside the board
                if(i >= 0 && i < gameBoard.width && j >= 0 && j < gameBoard.height) { 
}                   tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }

        return tiles;
    }

    /*
     * Finds tiles to destroy by col Char tile 
     */
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

    /*
     * Finds tiles to destroy by row Char tile 
     */
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

    /*
     * Finds tiles to destroy by Avatar tile 
     */
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
                // Checks for the avatar tile in list
                if(tileScript == gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>())
                {
                    tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[tileScript.currentCol, tileScript.currentRow].GetComponent<GameTileBase>().SetHasMatched(true);
                }

                // Finds tiles of same game tile type
                if(gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().GetGameTileType() == gameTileType)
                {
                    tiles.Add(gameBoard.allGameTiles[i, j]);
                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                }

                // All tiles, if both are avtar
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
