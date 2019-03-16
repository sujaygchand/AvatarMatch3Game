using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public GameBoard gameBoard;

    public int col;
    public int row;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            gameBoard.FindEmptySlots();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            CheckTile();
        }
    }

    public void CheckTile()
    {
        print("Object at: " + gameBoard.allGameTiles[col, row]);
    }
}
