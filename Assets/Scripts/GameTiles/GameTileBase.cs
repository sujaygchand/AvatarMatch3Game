/**
 * 
 * Author: Sujay Chand
 * 
 *  Game tile base
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Helpers;

[RequireComponent(typeof(SpriteRenderer))]
public class GameTileBase : MonoBehaviour
{

    [Header("Position Variables")]
    public int currentCol;
    public int currentRow;
    private int previousCol;
    private int previousRow;

    [Header("Tile Type Variables")]
    [SerializeField] private GameTileType gameTileType;
    [SerializeField] private TileType tileType = TileType.Normal;

    [Header("Powerup Variables")]
    [SerializeField] public bool isRowChar = false;
    [SerializeField] public bool isColChar = false;
    [SerializeField] public bool isAvatarTile = false;
    [SerializeField] private GameObject arrowMask;
    [SerializeField] private GameObject gliderMask;

    [Header("Swipe Variables")]
    public float swipeAngle = 0;
    public float swipeThreshold = .9f;

    [Header("Object Variables")]
    [SerializeField] private SpriteRenderer tileImage;
    [SerializeField] private GameBoard gameBoard;
    public GameObject gameGridObject;

    [SerializeField] private bool hasMatched = false;
    public bool additonalCheck = false;

    // Managers
    private GameObject otherTile;
    private MatchesManager matchesManager;
    private HintManager hintManager;
    private ScoreManager scoreManager;

    // Input variables
    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 targetPosition;
    private float swipeLerp = 0.2f;

    [Header("Other Variables")]
    public GameObject matchedParent;
    public SwipeDirection swipeDirection = SwipeDirection.None;
    private bool doOnce = false;
    private bool doMatchesCheck = true;

    // On Awake
    private void Awake()
    {
        tileImage = GetComponent<SpriteRenderer>();
        arrowMask.GetComponent<SpriteRenderer>().enabled = false;
        gliderMask.GetComponent<SpriteRenderer>().enabled = false;
    }

    // On start
    void Start()
    {
        tileType = TileType.Normal;

        targetPosition = new Vector2(currentCol, currentRow);
        gameBoard = GameObject.FindGameObjectWithTag(Utilities.GameBoard).GetComponent<GameBoard>();
        gameGridObject = GameObject.FindGameObjectWithTag(Utilities.Grid);

        // Managers
        matchesManager = FindObjectOfType<MatchesManager>();
        hintManager = FindObjectOfType<HintManager>();
        scoreManager = FindObjectOfType<ScoreManager>();

        previousCol = currentCol;
        previousRow = currentRow;

        initialTouchPosition = Vector2.zero;
        finalTouchPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Target position
        targetPosition.x = currentCol + Utilities.ColumnOffset;
        targetPosition.y = currentRow + Utilities.RowOffset;

        if (gameBoard)
        {
            // moves tile
            if (Mathf.Abs(targetPosition.magnitude - transform.position.magnitude) > .1)
            {
                transform.position = Vector2.Lerp(transform.position, targetPosition, swipeLerp);


                if (gameBoard.allGameTiles[currentCol, currentRow] != gameObject)
                {
                    // Sets new tile reference
                    gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
                }

                if (doMatchesCheck)
                {
                    doMatchesCheck = false;
                    //print(gameObject);
                    matchesManager.CheckForMatches();
                }
            }
            else
            {
                // Current position
                transform.position = targetPosition;

                if (gameBoard.allGameTiles[currentCol, currentRow])
                {
                    gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
                    doMatchesCheck = true;
                }
            }
        }

    }

    /*
     * On mouse button pressed
     */ 
    private void OnMouseDown()
    {
        // Destroy hint
        if (hintManager)
        {
            hintManager.DestroyHints();
        }

        // Set initial position of input
        if (gameBoard.currentPlayerState == PlayerState.Active && !Utilities.IsGamePaused)
        {
            initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    /*
     * On mouse button released
     */
    private void OnMouseUp()
    {
        // Set final position of input
        if (gameBoard.currentPlayerState == PlayerState.Active && !Utilities.IsGamePaused)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

    }

    /*
     * Debug, make tile
     */
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateCharTile(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GenerateCharTile(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GenerateGliderTile();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GenerateAvatarTile();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetGameTileType(GameTileType.Air);
        }
    }

    /*
 * Start Check move corountine
 */
    public void CorountineTileTrigger()
    {
        StartCoroutine(CheckMoveMade_Cor());

    }

    /*
     * Checks the move made
     */
    private IEnumerator CheckMoveMade_Cor()
    {
        yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime());

        // Handle Avatar Tile Move
        if (tileType == TileType.Avatar)
        {
            matchesManager.MatchAvatarTile(otherTile);
            hasMatched = true;
        }
        else if (otherTile.GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
        {
            matchesManager.MatchAvatarTile(gameObject);
            otherTile.GetComponent<GameTileBase>().SetHasMatched(true);
        }

        // Handle other tile types
        if (otherTile)
        {
            // Changes tile location
            // When no match is found
            if (!hasMatched && !otherTile.GetComponent<GameTileBase>().hasMatched)
            {
                otherTile.GetComponent<GameTileBase>().currentCol = currentCol;
                otherTile.GetComponent<GameTileBase>().currentRow = currentRow;
                currentCol = previousCol;
                currentRow = previousRow;

                yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime());
                gameBoard.currentTile = null;
                gameBoard.currentPlayerState = PlayerState.Active;
            }
            // When match is found
            else
            {
                // Changes moves made/left
                if (scoreManager)
                {
                    scoreManager.ChangeMovesCounter();
                }

                gameBoard.MatchedCoroutine();

            }

            swipeDirection = SwipeDirection.None;
            swipeAngle = 0;
            otherTile = null;
        }
    }

    /*
     * Calculate angle for the swipe angle and moves tiles
     */
    private void CalculateAngle()
    {
        // Check against accidental swipes
        if (Mathf.Abs(finalTouchPosition.y - initialTouchPosition.y) > swipeThreshold
            || Mathf.Abs(finalTouchPosition.x - initialTouchPosition.x) > swipeThreshold)
        {
            gameBoard.currentPlayerState = PlayerState.Wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - initialTouchPosition.y, finalTouchPosition.x - initialTouchPosition.x);
            swipeAngle *= 180 / Mathf.PI;
            MoveTiles();
            gameBoard.currentTile = this;
        }
        else
        {
            gameBoard.currentPlayerState = PlayerState.Active;
        }

    }

    /*
     * Makes the char tile (Striped Bomb)
     */
    public void GenerateCharTile(bool isRowChar)
    {
        if (tileType == TileType.Avatar)
        {
            return;
        }

        this.isRowChar = isRowChar;
        isColChar = !isRowChar;

        // Fail safe Reset
        gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
        // Spawn destroy particle
        gameBoard.DestroyParticle(currentCol, currentRow);

        if (gliderMask.GetComponent<SpriteRenderer>().enabled)
        {
            gliderMask.GetComponent<SpriteRenderer>().enabled = false;
        }

        arrowMask.GetComponent<SpriteRenderer>().enabled = true;

        tileType = TileType.Char;

        SetGameTileType(gameTileType);

        // Rotates arrow
        if (!isRowChar)
        {
            arrowMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else
        {
            arrowMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
        
        //Change score         
        scoreManager.AddToScore(1);
    }

    /*
     * Makes Glider tile (Wrapped Candy)
     */ 
    public void GenerateGliderTile()
    {
        if (tileType == TileType.Avatar)
        {
            return;
        }

        isRowChar = false;
        isColChar = false;

        // Fail safe Reset
        gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
        // Spawn destroy particle
        gameBoard.DestroyParticle(currentCol, currentRow);

        tileType = TileType.Glider;

        if (arrowMask.GetComponent<SpriteRenderer>().enabled)
        {
            arrowMask.GetComponent<SpriteRenderer>().enabled = false;
        }

        gliderMask.GetComponent<SpriteRenderer>().enabled = true;

        //Change score         
        scoreManager.AddToScore(1);
    }

    /*
     * Makes Avatar tile (Colour Bomb)
     */
    public void GenerateAvatarTile()
    {
        isRowChar = false;
        isRowChar = false;

        // Fail safe Reset
        gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
        // Spawn destroy particle
        gameBoard.DestroyParticle(currentCol, currentRow);

        if (arrowMask.GetComponent<SpriteRenderer>().enabled)
        {
            arrowMask.GetComponent<SpriteRenderer>().enabled = false;
        }

        gameTileType = GameTileType.None;
        tileType = TileType.Avatar;

        SetGameTileType(gameTileType);

        //Change score         
        scoreManager.AddToScore(1);
    }

    /*
     * Moves tile
     */ 
    public void MoveTiles()
    {
        // Swipe Up
        if (swipeAngle < 135 && swipeAngle >= 45 && currentRow < gameBoard.height - 1)
        {
            MoveTilesAction(0, 1, SwipeDirection.Up);
        }

        // Swipe Right
        else if (swipeAngle < 45 && swipeAngle >= -45 && currentCol < gameBoard.width - 1)
        {
            MoveTilesAction(1, 0, SwipeDirection.Right);
        }

        // Swipe Down
        else if (swipeAngle < -45 && swipeAngle >= -135 && currentRow > 0)
        {
            MoveTilesAction(0, -1, SwipeDirection.Down);
        }

        // Swipe Left
        else if (swipeAngle < -135 || swipeAngle >= 135 && currentCol > 0)
        {
            MoveTilesAction(-1, 0, SwipeDirection.Left);
        }

        // Other tile exist?
        if (!otherTile)
        {
            gameBoard.currentPlayerState = PlayerState.Active;
        }
    }

    /*
     * Sets the move tile information
     * 
     * @param deltaCol - column increment 
     * @param deltaRow - row increment 
     * @param swipeDirection
     * 
     */
    private void MoveTilesAction(int deltaCol, int deltaRow, SwipeDirection swipeDirection)
    {
        int newCol = currentCol + deltaCol;
        int newRow = currentRow + deltaRow;

        if (gameBoard.allGameTiles[newCol, newRow])
        {

            otherTile = gameBoard.allGameTiles[newCol, newRow];

            if (otherTile.GetComponent<GameTileBase>())
            {
                this.swipeDirection = swipeDirection;
                previousCol = currentCol;
                previousRow = currentRow;
                otherTile.GetComponent<GameTileBase>().currentCol -= deltaCol;
                otherTile.GetComponent<GameTileBase>().currentRow -= deltaRow;
                currentCol += deltaCol;
                currentRow += deltaRow;
                CorountineTileTrigger();
            }
        }
    }

    /*
     * Starts the matched effect coroutine
     */ 
    public virtual void PlayMatchedEffect(float waitTime)
    {
        StartCoroutine(DestructionEffect_Cor(waitTime));
    }

    /*
     * Tile destruction effect
     */ 
    private IEnumerator DestructionEffect_Cor(float waitTime)
    {
        
        hintManager.DestroyHints();

        // Drops alpha
        tileImage.color = new Color(0f, 0f, 0f, .3f);
        arrowMask.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, .3f);
        gliderMask.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, .3f);

         //Change score         
          scoreManager.AddToScore(1);


        matchesManager.PlayMatchSound();

        yield return null;
        gameBoard.allGameTiles[currentCol, currentRow] = null;
        Destroy(gameObject);

    }

    /*
     * Load tile sprite asset
     * 
     * @return sprite
     */
    public Sprite LoadTileSprite(string tile)
    {
        Sprite tempSprite = Resources.Load<Sprite>("Tiles/" + tile);

        return tempSprite;
    }

    /*
     * Getter for hasMatched
     */ 
    public bool GetHasMatched()
    {
        return hasMatched;
    }

    /*
     * Setter for hasMatched
     */ 
    public void SetHasMatched(bool hasMatched)
    {
        this.hasMatched = hasMatched;
    }

    /*
     * Getter for gameTileType
     */ 
    public GameTileType GetGameTileType()
    {
        return gameTileType;
    }

    /*
     * Setter for gameTileType
     */ 
    public void SetGameTileType(GameTileType gameTileType)
    {
        this.gameTileType = gameTileType;

        if (tileType == TileType.Avatar)
        {
            tileImage.sprite = LoadTileSprite(Utilities.AvatarIcon);
        }
        else
        {
            tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType, gameTileType));
        }
    }

    /*
     * Getter for tileType
     */
    public TileType GetTileType()
    {
        return tileType;
    }
    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;

        tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType, gameTileType));

    }

    /*
     * Getter for otherTile
     */
    public GameObject GetOtherTile()
    {
        return otherTile;
    }
}
