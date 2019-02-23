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

    [SerializeField] private GameTile tileType;
    [SerializeField] private AnimationCurve animationGraph;

    private SpriteRenderer tileImage;
    private GameObject otherTile;
    private GameBoard gameBoard;

    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 targetPosition;
    public float swipeAngle = 0;
    SpriteRenderer test;

    // Start is called before the first frame update
    void Start()
    {
        tileImage = GetComponent<SpriteRenderer>();
        tileType = (GameTile)Random.Range(0, Utilities.NumOfGameTileTypes());
        SetTileType(tileType);
        //targetPosition = new Vector2(currentCol, currentRow);
        gameBoard = GameObject.FindGameObjectWithTag(Utilities.GameBoard).GetComponent<GameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - initialTouchPosition.y, finalTouchPosition.x - initialTouchPosition.x);
        swipeAngle *= 180 / Mathf.PI;
        MovePieces();

        print(swipeAngle);
    }



    public Sprite LoadTileSprite(string tile)
    {
        return Resources.Load<Sprite>("Tiles/" + tile);

        //(Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/Tiles" + tile, typeof(Sprite));
    }

    public void SetTileType(GameTile tileType)
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
        else if (swipeAngle < -45 && swipeAngle >= -135 && currentRow > 0 )
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
        else if (swipeAngle < -135 || swipeAngle >= 135 && currentCol > 0 )
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

    public GameTile GetGameTile()
    {
        return tileType;
    }

    public void CorountineTileTrigger()
    {
       
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
