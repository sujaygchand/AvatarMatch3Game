/**
 * 
 * Author: Sujay Chand
 * 
 *  The main game board class
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GameBoard : MonoBehaviour
{

    [Header("Game Board variables")]
    public PlayerState currentPlayerState;
    public int width;
    public int height;
    public GameObject[,] allGameTiles;
    public GameTileBase currentTile;
    public int NumOfTileTypes = 0;
    private MatchesManager matchesManager;
    private Deadlock deadlock;

    // Tile prefabs
    [SerializeField] GameObject s_GridTile;
    [SerializeField] GameObject s_GameTile;

 
    [SerializeField] private GameObject gameGridObject;             // A grid to help organise assets, when debuging 
    private float destructionWaitTime = 0.6f;
    private bool isRefiliing = false;
    private bool doOnce = true;


    // Start is called before the first frame update
    void Start()
    {
        //Start game active
        currentPlayerState = PlayerState.Active;

        // If no tile count is set, use maximum ammount  
        if (NumOfTileTypes == 0 || NumOfTileTypes > Utilities.NumOfGameTileTypes())
        {
            NumOfTileTypes = Utilities.NumOfGameTileTypes();
        }

        // If the artist hasn't set a board, render default
        if (!s_GridTile)
        {
            // Loads the tile from the resources folder
            s_GridTile = Resources.Load<GameObject>(string.Format("{0}/{1}{2}", Utilities.Prefabs, Utilities.PF, Utilities.GridTile));
        }

        // If the artist hasn't set a board, render default
        if (!s_GameTile)
        {
            s_GameTile = Resources.Load<GameObject>(
               string.Format("{0}/{1}{2}", Utilities.Prefabs, Utilities.PF, Utilities.BaseGameTile)
               );
        }

        // Null point check
        if (s_GridTile)
        {
            DrawBoard();
        }

        // Sets game managers 
        matchesManager = FindObjectOfType<MatchesManager>();
        deadlock = FindObjectOfType<Deadlock>();

    }

    void Update()
    {
        // Does a deadlock check after game is ready
        if (doOnce)
        {
            doOnce = false;
            deadlock.FixDeadLock();
        }
    }


    /*
     * Draws the background tiles for a sizeable game board 
     */
    void DrawBoard()
    {
        allGameTiles = new GameObject[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Background tiles
                SpawnTile(s_GridTile, "GridTile", i, j, false);

                // Game tiles
                allGameTiles[i, j] = SpawnTile(s_GameTile, "GameTile", i, j, true);
            }
        }

    }


    /*
     * Spawns a tile
     * 
     * @param tileObject - prefab to spawn
     * @param name - for readabilty
     * @param x - column
     * @param y - row
     * @param returnObject - should return GameObject?
     * 
     * @return GameObject
     */
    public GameObject SpawnTile(GameObject tileObject, string name, int x, int y, bool returnObject)
    {
        // Offest spawn location by the gameboard location
        Vector2 tempCoord = new Vector2(gameObject.transform.position.x + x, gameObject.transform.position.y + y);

        //First random type
        GameTileType tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

        // Sets type and checks for no initial match on start
        if (returnObject)
        {
            int maxLoops = 0;                   // To stop infinite loops 

            while (CheckSetUpMatch(x, y, tempTileType) && maxLoops < 25)
            {
                tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

                maxLoops++;
            }

        }

        // Spawns tile in scene
        GameObject tempTile = Instantiate(tileObject, tempCoord, Quaternion.identity, gameGridObject.transform);

        if (tempTile.GetComponent<GameTileBase>())
        {
            tempTile.GetComponent<GameTileBase>().SetGameTileType(tempTileType);
            tempTile.GetComponent<GameTileBase>().currentCol = x;
            tempTile.GetComponent<GameTileBase>().currentRow = y;
            tempTile.GetComponent<GameTileBase>().gameGridObject = gameGridObject;
        }

        // For organisational and debugging purposes
        tempTile.name = string.Format("{0}: ({1}, {2})", name, x, y);

        if (returnObject)
        {
            return tempTile;
        }
        else
        {
            return null;
        }

    }

    /*
     *  Spawns game tile, when refilling board
     *  
     *  @param x - column
     *  @param y - row
     */
    private void RefillGameTileSpawn(int x, int y)
    {
        if(allGameTiles[x, y])
        {
            return;
        }

        // Spawn location, so tiles slide in
        Vector2 tempCoord = new Vector2(x, y);
        tempCoord.x += Utilities.ColumnOffset;
        tempCoord.y += Utilities.RowOffset + 1.5f;

        //First random type 
        GameTileType tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

        // Sets type and checks for no initial match on start
        int maxLoops = 0;                       // Stops infinite loop

        // Ensures no match on slide in
        while (CheckSetUpMatch(x, y, tempTileType) && maxLoops < 25)
        {
            tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

            maxLoops++;
        } 

        // Spawns tile
        GameObject tempTile = Instantiate(s_GameTile, tempCoord, Quaternion.identity, gameGridObject.transform);

        if (tempTile.GetComponent<GameTileBase>())
        {
            tempTile.GetComponent<GameTileBase>().SetGameTileType(tempTileType);
            tempTile.GetComponent<GameTileBase>().currentCol = x;
            tempTile.GetComponent<GameTileBase>().currentRow = y;
        }

        // For organisational and debugging purposes
        tempTile.name = string.Format("New {0}: ({1}, {2})", name, x, y);


        allGameTiles[x, y] = tempTile;
    }

    /*
     * Ensures no match, when tiles are spawned
     * 
     * @param col
     * @param row
     * @param tileType
     * 
     */ 
    public bool CheckSetUpMatch(int col, int row, GameTileType tileType)
    {

        // Quick method to check when the col/row  after third are tested
        if (col > 1 && row > 1)
        {
            // Horizontal check
            GameTileType leftTile1Type = allGameTiles[col - 1, row].GetComponent<GameTileBase>().GetGameTileType();
            GameTileType leftTile2Type = allGameTiles[col - 2, row].GetComponent<GameTileBase>().GetGameTileType();

            if (tileType == leftTile1Type && tileType == leftTile2Type)
            {
                return true;
            }

            // Vertical Check
            GameTileType BottomTile1Type = allGameTiles[col, row - 1].GetComponent<GameTileBase>().GetGameTileType();
            GameTileType BottomTile2Type = allGameTiles[col, row - 2].GetComponent<GameTileBase>().GetGameTileType();

            if (tileType == BottomTile1Type && tileType == BottomTile2Type)
            {
                return true;
            }

        }

        // Checks early edge tiles 
        else if (col <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allGameTiles[col, row - 1].GetComponent<GameTileBase>() && allGameTiles[col, row - 2].GetComponent<GameTileBase>())
                {
                    GameTileType LeftTile1Type = allGameTiles[col, row - 1].GetComponent<GameTileBase>().GetGameTileType();
                    GameTileType LeftTile2Type = allGameTiles[col, row - 2].GetComponent<GameTileBase>().GetGameTileType();

                    if (tileType == LeftTile1Type && tileType == LeftTile2Type)
                    {
                        return true;
                    }
                }
            }

            if (col > 1)
            {
                if (allGameTiles[col - 1, row].GetComponent<GameTileBase>() && allGameTiles[col - 2, row].GetComponent<GameTileBase>())
                {
                    GameTileType BottomTile1Type = allGameTiles[col - 1, row].GetComponent<GameTileBase>().GetGameTileType();
                    GameTileType BottomTile2Type = allGameTiles[col - 2, row].GetComponent<GameTileBase>().GetGameTileType();

                    if (tileType == BottomTile1Type && tileType == BottomTile2Type)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    /*
     * Getter for destruction time
     */ 
    public float GetDestructionWaitTime()
    {
        return destructionWaitTime;
    }

    /*
     * Destroyes match and makes special tiles 
     */ 
    public void DestroyMatches()
    {
        matchesManager.MakeSpecialTileCheck();

        //matchesManager.currentMatches.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGameTiles[i, j])
                {
                    DestroyMatch(i, j);
                }
            }
        }

        // Start collapsing the rows
        StartCoroutine(CollapseRow_Cor());
    }

    // Destory individual match
    private void DestroyMatch(int col, int row)
    {
        if (allGameTiles[col, row].GetComponent<GameTileBase>().GetHasMatched())
        {
            currentPlayerState = PlayerState.Wait;
            //matchesManager.MakeSpecialTileCheck();

            // Causing match bug
            //matchesManager.currentMatches.Remove(allGameTiles[col, row]);

            allGameTiles[col, row].GetComponent<GameTileBase>().PlayMatchedEffect(destructionWaitTime/2);
            
            allGameTiles[col, row] = null;
        }

    }

    /*
     * Collapes the row coroutine
     */
    private IEnumerator CollapseRow_Cor()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                // Look for null tile
                if(!allGameTiles[i, j])
                {
                    // Checks tiles above
                    for (int rowAbove = j + 1; rowAbove < height; rowAbove++)
                    {
                        // Keeps checking until a valid tile is found and drops it to the null row
                        if(allGameTiles[i, rowAbove])
                        {
                            allGameTiles[i, rowAbove].GetComponent<GameTileBase>().currentRow = j;
                            allGameTiles[i, rowAbove] = null;

                            break;
                        } 

                    }
                }
            }
        }

        yield return new WaitForSeconds(destructionWaitTime / 2);
        StartCoroutine(RefillBoard_cor());                          // start board refill
    }

    /*
     * Refill game board
     */ 
    private void RefillBorad()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allGameTiles[i, j] == null)
                {
                    RefillGameTileSpawn(i, j);
                }
            }
        }

    }

    /*
     * Checks for tile matches
     */ 
    private bool CheckForMatches()
    {

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allGameTiles[i, j])
                {
                    if(allGameTiles[i, j].GetComponent<GameTileBase>().GetHasMatched())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /*
     * Checks if the board has any empty slots
     */ 
    private bool CheckForEmptySlots()
    {
        foreach(GameObject tile in allGameTiles)
        {
            if (!tile)
            {
                return true;
            }
        }
        return false;
    }

    /*
     * Refill board coroutine
     * 
     */ 
    private IEnumerator RefillBoard_cor()
    {
        RefillBorad();          // Refill board

        yield return new WaitForSeconds(destructionWaitTime);

        // Looks for destroys matches
        while (CheckForMatches())
        {
            DestroyMatches();
            yield return new WaitForSeconds(destructionWaitTime * 2);
        }

        // Clears the current match
        matchesManager.currentMatches.Clear();
        currentTile = null;
        
        // Check for deadlock
        deadlock.FixDeadLock();
        yield return new WaitForSeconds(destructionWaitTime);

        currentPlayerState = PlayerState.Active;
    }
}