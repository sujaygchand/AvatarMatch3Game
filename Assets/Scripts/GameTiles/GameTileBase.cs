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
    protected int previousCol;
    protected int previousRow;

    [SerializeField] protected GameTileType gameTileType;
    [SerializeField] protected TileType tileType = TileType.Normal;
    [SerializeField] protected AnimationCurve animationGraph;

    [Header("Powerup Variables")]
    [SerializeField] public bool isRowChar = false;
    [SerializeField] public bool isColChar = false;
    [SerializeField] public bool isAvatarTile = false;
    [SerializeField] private GameObject arrowMask;
    [SerializeField] private GameObject gliderMask;

    [SerializeField] private SpriteRenderer tileImage;
    [SerializeField] private GameBoard gameBoard;
    public GameObject gameGridObject;
    private GameObject otherTile;
    private MatchesManager matchesManager;
    private HintManager hintManager;
    private ScoreManager scoreManager;

    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 targetPosition;
    private float swipeLerp = 0.2f;

    [Header("Swipe Variables")]
    public float swipeAngle = 0;
    public float swipeThreshold = .9f;

    [SerializeField] private bool hasMatched = false;
    public bool additonalCheck = false;

    public GameObject matchedParent;
    public SwipeDirection swipeDirection = SwipeDirection.None;

    private bool doOnce = false;

    // Start is called before the first frame update

    private void Awake()
    {
        tileImage = GetComponent<SpriteRenderer>();
        arrowMask.GetComponent<SpriteRenderer>().enabled = false;
        gliderMask.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Start()
    {
        tileType = TileType.Normal;

        targetPosition = new Vector2(currentCol, currentRow);
        gameBoard = GameObject.FindGameObjectWithTag(Utilities.GameBoard).GetComponent<GameBoard>();
        gameGridObject = GameObject.FindGameObjectWithTag(Utilities.Grid);

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
        //OldFindMatches();
        //FindMatches();

        targetPosition.x = currentCol + Utilities.ColumnOffset;
        targetPosition.y = currentRow + Utilities.RowOffset;

        if (gameBoard)
        {

            if (Mathf.Abs(targetPosition.magnitude - transform.position.magnitude) > .05)
            {
                transform.position = Vector2.Lerp(transform.position, targetPosition, swipeLerp);

                if (gameBoard.allGameTiles[currentCol, currentRow] != this.gameObject)
                {
                    gameBoard.allGameTiles[currentCol, currentRow] = this.gameObject;
                    matchesManager.CheckForMatches();
                }
                
            }
            else
            {
                transform.position = targetPosition;

                if (gameBoard.allGameTiles[currentCol, currentRow])
                {
                    gameBoard.allGameTiles[currentCol, currentRow] = gameObject;
                    //FindMatches();
                }
            }

            if (additonalCheck && hasMatched)
            {
                additonalCheck = false;
                matchesManager.CheckForMatches();
            }

        }



    }

    protected virtual void OnMouseDown()
    {
        if (hintManager)
        {
            hintManager.DestroyHints();
        }

        if (gameBoard.currentPlayerState == PlayerState.Active && !Utilities.IsGamePaused)
        {
            
            initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //print(initialTouchPosition);
        }
    }

    protected virtual void OnMouseUp()
    {
        if(gameBoard.currentPlayerState == PlayerState.Active && !Utilities.IsGamePaused)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            //FindMatches();
        }

    }

    // Debug
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GenerateCharTile(false);
        }

        if (Input.GetMouseButtonDown(2))
        {
            GenerateGliderTile();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateCharTile(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GenerateAvatarTile();
        }
    }

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
        } else
        {
            gameBoard.currentPlayerState = PlayerState.Active;
        }

    }

    public void GenerateCharTile(bool isRowChar)
    {
        if(tileType == TileType.Avatar)
        {
            return;
        }

        this.isRowChar = isRowChar;
        isColChar = !isRowChar;


        if (gliderMask.GetComponent<SpriteRenderer>().enabled)
        {
            gliderMask.GetComponent<SpriteRenderer>().enabled = false;
        }

        arrowMask.GetComponent<SpriteRenderer>().enabled = true;

        tileType = TileType.Char;

        SetGameTileType(gameTileType);

        if (!isRowChar)
        {
            arrowMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        } else
        {
            arrowMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

    }

    public void GenerateGliderTile()
    {
        if(tileType == TileType.Avatar)
        {
            return;
        }

        isRowChar = false;
        isColChar = false;

        tileType = TileType.Glider;

        if (arrowMask.GetComponent<SpriteRenderer>().enabled)
        {
            arrowMask.GetComponent<SpriteRenderer>().enabled = false;
        }

        gliderMask.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void GenerateAvatarTile()
    {
        isRowChar = false;
        isRowChar = false;

        if (arrowMask.GetComponent<SpriteRenderer>().enabled)
        {
            arrowMask.GetComponent<SpriteRenderer>().enabled = false; 
        }

        gameTileType = GameTileType.None;
        tileType = TileType.Avatar;

        SetGameTileType(gameTileType);
    }


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

        if (!otherTile)
        {
            gameBoard.currentPlayerState = PlayerState.Active;
        }
    }

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

    public virtual void PlayMatchedEffect(float waitTime)
    {

        StartCoroutine(DestructionEffect_Cor(waitTime));
    }

    private IEnumerator DestructionEffect_Cor(float waitTime)
    {
        hintManager.DestroyHints();
        tileImage.color = new Color(0f, 0f, 0f, .3f);
        arrowMask.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, .3f);
        gliderMask.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, .3f);

        if (scoreManager.gameMode == GameMode.Collection && tileType != TileType.Avatar)
        {
            scoreManager.AddToScore(-1);
        }
        else
        {
            scoreManager.AddToScore(+1);
        }

        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
        
    }

    public Sprite LoadTileSprite(string tile)
    {
        Sprite tempSprite = Resources.Load<Sprite>("Tiles/" + tile);

        return tempSprite;
    }

    public bool GetHasMatched()
    {
        return hasMatched;
    }

    public void SetHasMatched(bool hasMatched)
    {
        this.hasMatched = hasMatched;
    }

    public void SetGameTileType(GameTileType gameTileType)
    {
        this.gameTileType = gameTileType;

        if (tileType == TileType.Avatar)
        {
            tileImage.sprite = LoadTileSprite(Utilities.AvatarIcon);
        } else
        {
        tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType, gameTileType));
        }
    }


    public GameTileType GetGameTileType()
    {
        return gameTileType;
    }

    public TileType GetTileType()
    {
        return tileType;
    }
    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;

        tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType, gameTileType));

    }

    public GameObject GetOtherTile()
    {
        return otherTile;
    }

    private IEnumerator CheckMoveMade_Cor()
    {
        // Handle Avatar Tile Move
        if(tileType == TileType.Avatar)
        {
            matchesManager.MatchAvatarTile(otherTile);
            hasMatched = true;
        } else if (otherTile.GetComponent<GameTileBase>().GetTileType() == TileType.Avatar)
        {
            matchesManager.MatchAvatarTile(gameObject);
            otherTile.GetComponent<GameTileBase>().SetHasMatched(true);
        }

        yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime());
        if (otherTile)
        {

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
            else
            {
                if (scoreManager)
                {
                    if(Utilities.GameMode == GameMode.Collection)
                    {
                        scoreManager.moves--;
                    }
                }

                gameBoard.DestroyMatches();
                
            }

            swipeDirection = SwipeDirection.None;
            otherTile = null;
        }
    }

    public void CorountineTileTrigger()
    {
        StartCoroutine(CheckMoveMade_Cor());

    }

    private IEnumerator LerpTile()
    {
        targetPosition.x = currentCol - 3.49f;
        targetPosition.y = currentRow - 7.04f;

        float animationTime = 0;

        // The time you set on the animation graph
        float animationEnd = animationGraph.keys[animationGraph.length - 1].time;

        Vector3 currentPosition = gameObject.transform.position;

        while (animationTime <= animationEnd)
        {
            float lerpFactor = animationGraph.Evaluate(animationTime);


            gameObject.transform.position = Vector3.Lerp(currentPosition, targetPosition, lerpFactor);


            animationTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}










/*
 
    
    public void FindMatches()
    {
        // Horizontal matches
        if (currentCol > 0 && currentCol < gameBoard.width - 1)
        {
            GameObject LeftTile = gameBoard.allGameTiles[currentCol - 1, currentRow];
            GameObject RightTile = gameBoard.allGameTiles[currentCol + 1, currentRow];

            // Nullptr check
            if (LeftTile && RightTile)
            {
                if (tileType == LeftTile.GetComponent<GameTileBase>().GetGameTileType() &&
                    tileType == RightTile.GetComponent<GameTileBase>().GetGameTileType())
                {
                    
                        hasMatched = true;
                        LeftTile.GetComponent<GameTileBase>().SetHasMatched(true);
                        RightTile.GetComponent<GameTileBase>().SetHasMatched(true);

                        LeftTile.GetComponent<GameTileBase>().matchedParent = gameObject;
                        RightTile.GetComponent<GameTileBase>().matchedParent = gameObject;

                    }
                }
            }

        // Vertical matches
        if (currentRow > 0 && currentRow < gameBoard.height - 1)
        {
            GameObject BottomTile = gameBoard.allGameTiles[currentCol, currentRow - 1];
            GameObject TopTile = gameBoard.allGameTiles[currentCol, currentRow + 1];

            // Nullptr check
            if (BottomTile && TopTile)
            {
                if (tileType == BottomTile.GetComponent<GameTileBase>().GetGameTileType() &&
                    tileType == TopTile.GetComponent<GameTileBase>().GetGameTileType())
                {

                        hasMatched = true;
                        BottomTile.GetComponent<GameTileBase>().SetHasMatched(true);
                        TopTile.GetComponent<GameTileBase>().SetHasMatched(true);

                        BottomTile.GetComponent<GameTileBase>().matchedParent = gameObject;
                        TopTile.GetComponent<GameTileBase>().matchedParent = gameObject;
                    }
            }
        }

    }



        public void FindMatchesWithSets()
    {
        HashSet<GameObject> TempHorizontalSet = new HashSet<GameObject>();
        HashSet<GameObject> TempVerticalSet = new HashSet<GameObject>();

        ValidMatches(currentCol, currentRow, TempHorizontalSet, TempVerticalSet);

    }

    public void ValidMatches(int col, int row, HashSet<GameObject> TempHorizontalSet, HashSet<GameObject> TempVerticalSet)
    {
        GameTileType testType = tileType;

        int tempCol = col;
        int tempRow = row;
        
        // Horizontal check
        if (currentCol > 0 && currentCol < gameBoard.width - 1)
        {

            // Left Tiles Check
            while (tempCol >= 0)
            {
                if (!gameBoard.allGameTiles[tempCol, tempRow])
                {
                    break;
                }

                if (testType == gameBoard.allGameTiles[tempCol, tempRow].GetComponent<GameTileBase>().GetGameTileType())
                {
                    TempHorizontalSet.Add(gameBoard.allGameTiles[tempCol, tempRow]);
                    tempCol--;
                }
                else
                {
                    break;
                }
            }

            //Right tiles check
            tempCol = col;
            tempRow = row;

            while (tempCol < gameBoard.width)
            {
                if(!gameBoard.allGameTiles[tempCol, tempRow])
                {
                    break;
                }

                if (testType == gameBoard.allGameTiles[tempCol, tempRow].GetComponent<GameTileBase>().GetGameTileType())
                {
                    TempHorizontalSet.Add(gameBoard.allGameTiles[tempCol, tempRow]);
                    tempCol++;
                }
                else
                {

                    break;
                }
            }
        }

        // Vertical check
        if (currentRow > 0 && currentRow < gameBoard.height - 1)
        {
            // Down tiles check
            tempCol = col;
            tempRow = row;

            while (tempRow >= 0)
            {
                if (!gameBoard.allGameTiles[tempCol, tempRow])
                {
                    break;
                }

                if (testType == gameBoard.allGameTiles[tempCol, tempRow].GetComponent<GameTileBase>().GetGameTileType())
                {
                    TempVerticalSet.Add(gameBoard.allGameTiles[tempCol, tempCol]);
                    tempRow--;
                }
                else
                {
                    break;
                }
            }

            // Up tiles check
            tempCol = col;
            tempRow = row;

            while (tempRow < gameBoard.height)
            {
                if (!gameBoard.allGameTiles[tempCol, tempRow])
                {
                    break;
                }

                if (testType == gameBoard.allGameTiles[tempCol, tempRow].GetComponent<GameTileBase>().GetGameTileType())
                {
                    TempVerticalSet.Add(gameBoard.allGameTiles[tempCol, tempRow]);
                    tempRow++;
                }
                else
                {
                    break;
                }
            }
        }

        string tilesMatched = gameObject + ": ( Type: " + testType + " )" + " HSet: ";

        // Set horizontal matches to true
        if(TempHorizontalSet.Count >= 3)
        {
            foreach(GameObject tempObject in TempHorizontalSet)
            {
                tilesMatched += "{ " + tempObject + " : Type: " + tempObject.GetComponent<GameTileBase>().GetGameTileType() + " } "; 

                tempObject.GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }

        tilesMatched += " VSet: ";

        // Set vertical matches to true
        if (TempVerticalSet.Count >= 3)
        {
            foreach (GameObject tempObject in TempVerticalSet)
            {
                tilesMatched += "{ " + tempObject + " : Type: " + tempObject.GetComponent<GameTileBase>().GetGameTileType() + " } ";

                tempObject.GetComponent<GameTileBase>().SetHasMatched(true);
            }
        }

        print(tilesMatched);
    }
     
     */
