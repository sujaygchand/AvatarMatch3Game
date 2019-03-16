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
    public bool isChecking = false;
    private AudioSource audioSource;
    private ScoreManager scoreManager;


    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();

        audioSource = GetComponent<AudioSource>();

        scoreManager = FindObjectOfType<ScoreManager>();

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

    public void PlayMatchSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
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

        //MakeSpecialTileCheck();

    }

    /*
     * Checks for matches on board 
     */
    private IEnumerator CheckForMatches_Cor()
    {
        yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime() / 3);

        if (!isChecking)
        {
            //print("matching");
            isChecking = true;

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
            isChecking = false;
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

        if (!currentMatches[0])
        {
            return false;
        }
        if (currentMatches[0].GetComponent<GameTileBase>())
        {
            GameTileBase firstTile = currentMatches[0].GetComponent<GameTileBase>();

            foreach (GameObject tile in currentMatches)
            {

                if (tile.GetComponent<GameTileBase>())
                {
                    GameTileBase gameTile = tile.GetComponent<GameTileBase>();

                    if (gameTile.GetGameTileType() == firstTile.GetGameTileType())
                    {

                        // Add to Col count
                        if (gameTile.currentCol == firstTile.currentCol)
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
        }

        if (numOfColTiles >= passValue || numOfRowTiles >= passValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool CheckGliderAlignment()
    {
        HashSet<GameTileType> gameTileTypes = new HashSet<GameTileType>();
        
        foreach(GameObject tile in currentMatches)
        {
            GameTileType testType = tile.GetComponent<GameTileBase>().GetGameTileType();

            if(testType != GameTileType.None)
            {
                gameTileTypes.Add(testType);
            }
        }
        
        foreach(GameTileType type in gameTileTypes)
        {
            int typeCounter = 0;

            foreach(GameObject tile in currentMatches)
            {
                if(tile.GetComponent<GameTileBase>().GetGameTileType() == type)
                {
                    typeCounter++;

                    if(typeCounter >= 5)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    } 
     

    /*
     * Check to see if special tile can be made
     */ 
    public void MakeSpecialTileCheck()
    { 
            // Make Char tile Check
            if (currentMatches.Count == 4 || currentMatches.Count == 7)
            {
            if (CheckMatchAlignment(4))
            {
                MakeCharTileCheck();
            }
            }

            
            else if (currentMatches.Count == 5 || currentMatches.Count == 8)
            {
            // Make Avatar tile check
                if (CheckMatchAlignment(5))
                {
                    MakeAvatarTileCheck();
                }
            else
            {
                // Make glider tile check
                 MakeGliderTileCheck();
            }
            }
            
         }

    /*
     * Makes char tile
     * 
     */
    public void MakeCharTileCheck()
    {
        if (gameBoard.currentTile)
        {
            // Turn current tile into Char
            if (gameBoard.currentTile.GetHasMatched() && gameBoard.currentTile.GetTileType() == TileType.Normal)
            {
                gameBoard.currentTile.SetHasMatched(false);
                currentMatches.Remove(gameBoard.currentTile.gameObject);

                if (gameBoard.currentTile.swipeDirection == SwipeDirection.Left ||
                    gameBoard.currentTile.swipeDirection == SwipeDirection.Right)
                {
                    gameBoard.currentTile.GenerateCharTile(true);
                }
                else
                {
                    gameBoard.currentTile.GenerateCharTile(false);
                }
            }

            // Turn other tile into Char 
            else if (gameBoard.currentTile.GetOtherTile())
            {
                if (gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>())
                {

                    GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

                    if (otherTile.GetHasMatched() && otherTile.GetTileType() == TileType.Normal)
                    {
                        otherTile.SetHasMatched(false);
                        currentMatches.Remove(gameBoard.currentTile.gameObject);

                        if (gameBoard.currentTile.swipeDirection == SwipeDirection.Left ||
                            gameBoard.currentTile.swipeDirection == SwipeDirection.Right)
                        {
                            gameBoard.currentTile.GenerateCharTile(true);
                        }
                        else
                        {
                            gameBoard.currentTile.GenerateCharTile(false);
                        }
                    }
                }
            }
        }
    }

    /*
     * Makes glider tile
     */
    public void MakeGliderTileCheck()
    {
        if (gameBoard.currentTile)
        {
            // Turn current tile into Glider
            if (gameBoard.currentTile.GetHasMatched() && gameBoard.currentTile.GetTileType() == TileType.Normal)
            {
                gameBoard.currentTile.SetHasMatched(false);
                currentMatches.Remove(gameBoard.currentTile.gameObject);
                gameBoard.currentTile.GenerateGliderTile();
            }
            // Turn other tile into Glider 
            else if (gameBoard.currentTile.GetOtherTile())
            {
                if (gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>())
                {
                    GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

                    if (otherTile.GetHasMatched() && otherTile.GetTileType() == TileType.Normal)
                    {
                        otherTile.SetHasMatched(false);
                        otherTile.GenerateGliderTile();
                    }
                }
            }
        }

    }


    /*
     * Makes Avatar tile
     */
    public void MakeAvatarTileCheck()
    {
        if (gameBoard.currentTile)
        {
            // Turn current tile into Avatar
            if (gameBoard.currentTile.GetHasMatched() && gameBoard.currentTile.GetTileType() == TileType.Normal)
            {

                gameBoard.currentTile.SetHasMatched(false);
                currentMatches.Remove(gameBoard.currentTile.gameObject);
                gameBoard.currentTile.GenerateAvatarTile();
            }
            // Turn other tile into Avatar 
            else if (gameBoard.currentTile.GetOtherTile())
            {
                if (gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>())
                {
                    GameTileBase otherTile = gameBoard.currentTile.GetOtherTile().GetComponent<GameTileBase>();

                    if (otherTile.GetHasMatched() && otherTile.GetTileType() == TileType.Normal)
                    {
                        otherTile.SetHasMatched(false);
                        otherTile.GenerateAvatarTile();
                    }
                }
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
                GetRowMatches(tile, tile.GetComponent<GameTileBase>().currentRow);
            }
            else if (tile.GetComponent<GameTileBase>().isColChar)
            {
                GetColMatches(tile, tile.GetComponent<GameTileBase>().currentCol);
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
                GetRowMatches(tile, tile.GetComponent<GameTileBase>().currentRow);
            }

            else if (tile.GetComponent<GameTileBase>().isColChar)
            {
                GetColMatches(tile, tile.GetComponent<GameTileBase>().currentCol);
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
                   GetGliderMatches(tile, tempTile.currentCol, tempTile.currentRow);
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
                   GetGliderMatches(tile, tempTile.currentCol, tempTile.currentRow);
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
                        
                        ChainSpecialMatches(tempTile.gameObject);

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
     * Chains the special matches
     */ 
    private void ChainSpecialMatches(GameObject tile)
    {
        
        if (tile.GetComponent<GameTileBase>().GetTileType() == TileType.Char)
        {
            MatchCharTile(tile);
        }
        else if (tile.GetComponent<GameTileBase>().GetTileType() == TileType.Glider)
        {
            MatchGliderTile(tile);
        }
          }

    /*
     * Finds tiles to destroy by Glider tile 
     */ 
    private void GetGliderMatches(GameObject tile, int col, int row)
    {
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

        for (int i = minCol; i <= maxCol; i++)
        {
            for (int j = minRow; j <= maxRow; j++)
            {
                // Check if the piece is inside the board
                if (i >= 0 && i < gameBoard.width && j >= 0 && j < gameBoard.height)
                {

                    GameObject tempTile = gameBoard.allGameTiles[i, j];

                    if (tempTile != tile)
                    {
                        ChainSpecialMatches(tempTile);
                    }

                    gameBoard.allGameTiles[i, j].GetComponent<GameTileBase>().SetHasMatched(true);
                }
            }
        }
    }

    /*
     * Finds tiles to destroy by col Char tile 
     */
    private void GetColMatches(GameObject tile, int col)
    {

        for(int i = 0; i < gameBoard.height; i++)
        {
            if (gameBoard.allGameTiles[col, i] != null)
            {
                GameObject tempTile = gameBoard.allGameTiles[col, i];

                if (!tempTile.GetComponent<GameTileBase>().isColChar)
                {
                    if (tempTile != tile)
                    {
                        ChainSpecialMatches(tempTile);
                    }
                }

                gameBoard.allGameTiles[col, i].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }
    }

    /*
     * Finds tiles to destroy by row Char tile 
     */
    private void GetRowMatches(GameObject tile, int row)
    {
        //List<GameObject> tiles = new List<GameObject>();

        for (int i = 0; i < gameBoard.width; i++)
        {
            if (gameBoard.allGameTiles[i, row] != null)
            {
                GameObject tempTile = gameBoard.allGameTiles[i, row];

                if (!tempTile.GetComponent<GameTileBase>().isRowChar)
                {
                    if (tempTile != tile)
                    {
                        ChainSpecialMatches(tempTile);
                    }
                }
                
                gameBoard.allGameTiles[i, row].GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }
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
