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

    [SerializeField] private GameTileType tileType;
    [SerializeField] private AnimationCurve animationGraph;

    private SpriteRenderer tileImage;
    private GameObject otherTile;
    private GameBoard gameBoard;
    private MatchesManager matchesManager;

    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 targetPosition;
    private float swipeLerp = 0.2f;

    [Header("Swipe Variables")]
    public float swipeAngle = 0;
    public float swipeThreshold = .9f;

    [SerializeField] private bool hasMatched = false;
    private bool canMatch = true;

    public GameObject matchedParent;

    // Start is called before the first frame update

    private void Awake()
    {
        tileImage = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

        targetPosition = new Vector2(currentCol, currentRow);
        gameBoard = GameObject.FindGameObjectWithTag(Utilities.GameBoard).GetComponent<GameBoard>();

        matchesManager = FindObjectOfType<MatchesManager>();
        //print(matchesManager.gameObject);

        previousCol = currentCol;
        previousRow = currentRow;
    }

    // Update is called once per frame
    void Update()
    {
        //OldFindMatches();
        //FindMatches();

        targetPosition.x = currentCol + Utilities.ColumnOffset;
        targetPosition.y = currentRow + Utilities.RowOffset;

        if (Mathf.Abs(targetPosition.magnitude - transform.position.magnitude) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, swipeLerp);

            if (gameBoard.allGameTiles[currentCol, currentRow] != this.gameObject)
            {
                gameBoard.allGameTiles[currentCol, currentRow] = this.gameObject;
                //print(gameObject);
                //matchesManager.CheckForMatches();
            }
            matchesManager.CheckForMatches();
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

    }

    private void OnMouseDown()
    {
        if (gameBoard.currentPlayerState == PlayerState.Active)
        {
            initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //print(initialTouchPosition);
        }
    }

    private void OnMouseUp()
    {
        if(gameBoard.currentPlayerState == PlayerState.Active)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            //FindMatches();
        }

    }

    private void CalculateAngle()
    {
        // Check against accidental swipes
        if (Mathf.Abs(finalTouchPosition.y - initialTouchPosition.y) > swipeThreshold
            || Mathf.Abs(finalTouchPosition.x - initialTouchPosition.x) > swipeThreshold)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - initialTouchPosition.y, finalTouchPosition.x - initialTouchPosition.x);
            swipeAngle *= 180 / Mathf.PI;
            MovePieces();
            gameBoard.currentPlayerState = PlayerState.Wait;
            //FindMatches();
            //print(gameObject);


            //print(swipeAngle);
        } else
        {
            gameBoard.currentPlayerState = PlayerState.Active;
        }

    }

    public Sprite LoadTileSprite(string tile)
    {

        Sprite tempSprite = Resources.Load<Sprite>("Tiles/" + tile);

        return tempSprite;

        //(Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/Tiles" + tile, typeof(Sprite));
    }

    public void SetTileType(GameTileType tileType)
    {
        this.tileType = tileType;

        tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType));

        //print(LoadTileSprite(Utilities.FindTileType(tileType)));
    }

    public void MovePieces()
    {
        // Swipe Up
        if (swipeAngle < 135 && swipeAngle >= 45 && currentRow < gameBoard.height - 1)
        {
            // Simultaneous assignment and null pointer check
            if (gameBoard.allGameTiles[currentCol, currentRow + 1])
            {
                otherTile = gameBoard.allGameTiles[currentCol, currentRow + 1];

                if (otherTile.GetComponent<GameTileBase>())
                {
                    
                    previousRow = currentRow;
                    previousCol = currentCol;
                    otherTile.GetComponent<GameTileBase>().currentRow -= 1;
                    currentRow += 1;
                    CorountineTileTrigger();
                }
            }
        }

        // Swipe Right
        else if (swipeAngle < 45 && swipeAngle >= -45 && currentCol < gameBoard.width - 1)
        {
            // Simultaneous assignment and null pointer check
            if (gameBoard.allGameTiles[currentCol + 1, currentRow])
            {
                otherTile = gameBoard.allGameTiles[currentCol + 1, currentRow];

                if (otherTile.GetComponent<GameTileBase>())
                {
                  
                    previousRow = currentRow;
                    previousCol = currentCol;
                    otherTile.GetComponent<GameTileBase>().currentCol -= 1;
                    currentCol += 1;
                    CorountineTileTrigger();
                }
            }
        }

        // Swipe Down
        else if (swipeAngle < -45 && swipeAngle >= -135 && currentRow > 0)
        {
            // Simultaneous assignment and null pointer check
            if (gameBoard.allGameTiles[currentCol, currentRow - 1])
            {
                otherTile = gameBoard.allGameTiles[currentCol, currentRow - 1];

                if (otherTile.GetComponent<GameTileBase>())
                {
                   
                    previousRow = currentRow;
                    previousCol = currentCol;
                    otherTile.GetComponent<GameTileBase>().currentRow += 1;
                    currentRow -= 1;
                    CorountineTileTrigger();
                }
            }
        }

        // Swipe Left
        else if (swipeAngle < -135 || swipeAngle >= 135 && currentCol > 0)
        {
            // Simultaneous assignment and null pointer check
            if (gameBoard.allGameTiles[currentCol - 1, currentRow])
            {
                otherTile = gameBoard.allGameTiles[currentCol - 1, currentRow];

                if (otherTile.GetComponent<GameTileBase>())
                {
                    
                    previousRow = currentRow;
                    previousCol = currentCol;
                    otherTile.GetComponent<GameTileBase>().currentCol += 1;
                    currentCol -= 1;
                    CorountineTileTrigger();
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

    public void PlayMatchedEffect(float waitTime)
    {

        StartCoroutine(DestructionEffect_Cor(waitTime));
    }

    private IEnumerator DestructionEffect_Cor(float waitTime)
    {
        tileImage.color = new Color(0f, 0f, 0f, .3f);
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
        
    }

    public bool GetHasMatched()
    {
        return hasMatched;
    }

    public void SetHasMatched(bool hasMatched)
    {
        this.hasMatched = hasMatched;
    }

    public GameTileType GetGameTileType()
    {
        return tileType;
    }
    

    private IEnumerator CheckMoveMade_Cor()
    {
        yield return new WaitForSeconds(.3f);
        if (otherTile)
        {
            canMatch = false;

            if (!hasMatched && !otherTile.GetComponent<GameTileBase>().hasMatched)
            {
                otherTile.GetComponent<GameTileBase>().currentCol = currentCol;
                otherTile.GetComponent<GameTileBase>().currentRow = currentRow;
                currentCol = previousCol;
                currentRow = previousRow;

                yield return new WaitForSeconds(gameBoard.GetDestructionWaitTime());
                gameBoard.currentPlayerState = PlayerState.Active;
            }
            else
            {
                //gameBoard.DestroyMatches();
                
            }

            otherTile = null;
        }
    }

    public void CorountineTileTrigger()
    {
        StartCoroutine(CheckMoveMade_Cor());
        //matchesManager.CheckForMatches();
        /* gameBoard.allGameTiles[currentCol, currentRow] = this.gameObject;
         print(this.gameObject);
         StartCoroutine(LerpTile());
         */
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
