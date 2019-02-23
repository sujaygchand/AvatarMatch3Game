using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Helpers;

[RequireComponent(typeof(SpriteRenderer))]
public class GameTileBase : MonoBehaviour
{

    public int currentCol;
    public int currentRow;
    private int previousCol;
    private int previousRow;

    [SerializeField] private GameTileType tileType;
    [SerializeField] private AnimationCurve animationGraph;

    private SpriteRenderer tileImage;
    private GameObject otherTile;
    private GameBoard gameBoard;

    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 targetPosition;
    public float swipeAngle = 0;
    public float swipeThreshold = .9f;
    [SerializeField] private bool hasMatched = false;

    // Start is called before the first frame update

    private void Awake()
    {
        tileImage = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
       
        targetPosition = new Vector2(currentCol, currentRow);
        gameBoard = GameObject.FindGameObjectWithTag(Utilities.GameBoard).GetComponent<GameBoard>();

        previousCol = currentCol;
        previousRow = currentRow;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasMatched)
        {
            tileImage.color = new Color(0f, 0f, 0f, .3f);
        }
        else
        {
            FindMatches();
        }


        targetPosition.x = currentCol - 3.49f;
        targetPosition.y = currentRow - 7.04f;

        if (Mathf.Abs(targetPosition.magnitude - transform.position.magnitude) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, .3f);
        }
        else
        {
            transform.position = targetPosition;
            gameBoard.allGameTiles[currentCol, currentRow] = this.gameObject;
        }

    }

    private void OnMouseDown()
    {
        initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //print(initialTouchPosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
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
            //FindMatches();

            print(swipeAngle);
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
            if (otherTile = gameBoard.allGameTiles[currentCol, currentRow + 1])
            {
                if (otherTile.GetComponent<GameTileBase>())
                {
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
            if (otherTile = gameBoard.allGameTiles[currentCol + 1, currentRow])
            {
                if (otherTile.GetComponent<GameTileBase>())
                {
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
            if (otherTile = gameBoard.allGameTiles[currentCol, currentRow - 1])
            {
                if (otherTile.GetComponent<GameTileBase>())
                {
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
            if (otherTile = gameBoard.allGameTiles[currentCol - 1, currentRow])
            {
                if (otherTile.GetComponent<GameTileBase>())
                {
                    otherTile.GetComponent<GameTileBase>().currentCol += 1;
                    currentCol -= 1;
                    CorountineTileTrigger();
                    otherTile.GetComponent<GameTileBase>().CorountineTileTrigger();
                }
            }
        }
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
                    LeftTile.GetComponent<GameTileBase>().hasMatched = true;
                    RightTile.GetComponent<GameTileBase>().hasMatched = true;
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
                    BottomTile.GetComponent<GameTileBase>().hasMatched = true;
                    TopTile.GetComponent<GameTileBase>().hasMatched = true;
                }
            }
        }

    }

    public GameTileType GetGameTileType()
    {
        return tileType;
    }

    private IEnumerator CheckMoveMade_Cor()
    {
        yield return new WaitForSeconds(.5f);
        print("Got here");
        if (otherTile)
        {
            print("Other itle is: " + otherTile);
            if (!hasMatched && !otherTile.GetComponent<GameTileBase>().hasMatched)
            {
                print("Unmatch");
                otherTile.GetComponent<GameTileBase>().currentCol = currentCol;
                otherTile.GetComponent<GameTileBase>().currentRow = currentRow;
                currentCol = previousCol;
                currentRow = previousRow;
            }

            otherTile = null;
        }
    }

    public void CorountineTileTrigger()
    {
        StartCoroutine(CheckMoveMade_Cor());
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
