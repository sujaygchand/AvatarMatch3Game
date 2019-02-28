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

    [SerializeField] GameObject s_GridTile;
    [SerializeField] GameObject s_GameTile;
    [SerializeField] private GameObject gameGridObject;
    private GridTitle[,] gameGrid;
    private float destructionWaitTime = 0.85f;
    private bool isRefiliing = false;
    private bool ischecking = true;


    // Start is called before the first frame update
    void Start()
    {
        currentPlayerState = PlayerState.Active;

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

        //gameGridObject = this.gameObject;

        //gameGridObject = GameObject.FindGameObjectWithTag(Utilities.Grid);

        matchesManager = FindObjectOfType<MatchesManager>();

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
     * @param 
     */
    public GameObject SpawnTile(GameObject tileObject, string name, int x, int y, bool returnObject)
    {
        // Offest spawn location by the gameboard location
        Vector2 tempCoord = new Vector2(gameObject.transform.position.x + x, gameObject.transform.position.y + y);

        GameTileType tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

        if (returnObject)
        {
            int maxLoops = 0;

            while (CheckSetUpMatch(x, y, tileObject, tempTileType) && maxLoops < 25)
            {
                tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

                maxLoops++;
            }

        }

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

    private void RefillGameTileSpawn(int x, int y)
    {
        if(allGameTiles[x, y])
        {
            return;
        }

        Vector2 tempCoord = new Vector2(x, y);
        tempCoord.x += Utilities.ColumnOffset;
        tempCoord.y += Utilities.RowOffset + 2;

        GameTileType tempTileType = (GameTileType)Random.Range(0, NumOfTileTypes);

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

    private bool CheckSetUpMatch(int col, int row, GameObject tile, GameTileType tileType)
    {

        // Quick method to check when the thrid col/row are tested
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

        // 
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

    public float GetDestructionWaitTime()
    {
        return destructionWaitTime;
    }

    public void DestroyMatches()
    {
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
        
        StartCoroutine(CollapseRow_Cor());
    }

    private void DestroyMatch(int col, int row)
    {
        if (allGameTiles[col, row].GetComponent<GameTileBase>().GetHasMatched())
        {
            matchesManager.MakeSpecialTileCheck();

            matchesManager.currentMatches.Remove(allGameTiles[col, row]);
            allGameTiles[col, row].GetComponent<GameTileBase>().PlayMatchedEffect(destructionWaitTime);
            
            allGameTiles[col, row] = null;
        }

    }

    private IEnumerator DestructionEffect_Cor(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);

    }

    private IEnumerator CollapseRow_Cor()
    {
        yield return new WaitForSeconds(destructionWaitTime);

        for (int i = 0; i < width; i++)
        {
            int emptyslots = 0;

            for (int j = 0; j < height; j++)
            {
                if (allGameTiles[i, j] == null)
                {
                    emptyslots++;
                }
                else if (emptyslots > 0)
                {
                    allGameTiles[i, j].GetComponent<GameTileBase>().currentRow -= emptyslots;
                    allGameTiles[i, j] = null;
                }
            }
        }
        yield return new WaitForSeconds(destructionWaitTime);

        isRefiliing = true;

        StartCoroutine(RefillBoard_cor());
    }

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

        isRefiliing = false;
    }

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

    private void AdditionalMatchCheck()
    {
        if (ischecking)
        {
            ischecking = false;
            currentPlayerState = PlayerState.Wait;

            foreach (GameObject tile in allGameTiles)
            {
                if (tile.GetComponent<GameTileBase>().GetHasMatched())
                {
                    tile.GetComponent<GameTileBase>().additonalCheck = true;
                }
            }
        }
    }

    
    private IEnumerator RefillBoard_cor()
    {
        RefillBorad();

        yield return new WaitUntil(() => CheckForEmptySlots() == false);
        //yield return new WaitForSeconds(destructionWaitTime);

        while (CheckForMatches())
        {
            yield return new WaitForSeconds(destructionWaitTime);
            DestroyMatches();
        }

        matchesManager.currentMatches.Clear();
        currentTile = null;
        yield return new WaitForSeconds(destructionWaitTime);

        currentPlayerState = PlayerState.Active;
    }
}