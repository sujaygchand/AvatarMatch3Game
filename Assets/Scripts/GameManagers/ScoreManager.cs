using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Helpers;

public class ScoreManager : MonoBehaviour
{
    private GameBoard gameBoard;
    public int tilesToCollect = 50;
    private int tiles;
    public int moves = 14;
    public int timeLimt = 60;
    private float currentTime;
    private int displayTime;
    
    public GameMode gameMode;
    public Text scoreText;
    public Text scoreInfoText;
    public Text movesOrTimerText;
    public Text movesOrTimerInfoText;


    // Start is called before the first frame update
    void Start()
    {
        gameMode = Utilities.GameMode;

        gameBoard = FindObjectOfType<GameBoard>();

        SetupScoreManager();
    }

    // Update is called once per frame
    void Update()
    {
        
        scoreText.text = "" + tiles;
        

        GameOverCheck();
    }

    private void SetupScoreManager()
    {
        ResetScore();
        ResetTime();

        switch (gameMode)
        {
            case GameMode.Collection:
                GameModeSetup(Utilities.Collection, Utilities.Moves);
                break;
            case GameMode.TimeAttack:
                GameModeSetup(Utilities.TimeAttack, Utilities.Time);
                break;
            case GameMode.Deadlocked:
                GameModeSetup(Utilities.TimeAttack, Utilities.Moves);
                break;
        }
    }

    private void GameModeSetup(string scoreInfoText, string movesOrTimerInfoText)
    {
        this.scoreInfoText.text = scoreInfoText;
        this.movesOrTimerInfoText.text = movesOrTimerInfoText;
    }

    public void ResetScore()
    {
        if(gameMode == GameMode.TimeAttack)
        {
            tiles = 0;
        } else
        {
            tiles = tilesToCollect;
        }

    }

    public void ResetTime()
    {
        if(gameMode == GameMode.TimeAttack)
        {
            currentTime = timeLimt;
        }
    }

    public void AddToScore(int deltaScore)
    {
        if(tiles + deltaScore <= 0 )
        {
            tiles = 0;
        } else
        {
            tiles += deltaScore;
        }
    }

    private void GameOverCheck()
    {
        if (gameMode == GameMode.TimeAttack)
        {
            if (gameBoard.currentPlayerState == PlayerState.Active && currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                movesOrTimerText.text = "" + (int)currentTime;
            }

            if (currentTime <= 0)
            {
                print("Time Attacked");
            }


        } else if(gameMode == GameMode.Collection)
        {
            movesOrTimerText.text = "" + moves;

            if(moves <= 0)
            {
                return;
            }

                if (tiles <= 0)
                {
                    print("All tiles collected");
                }
            }
        }

    public void AddToMoves(int deltaMove)
    {
        if(gameMode == GameMode.Collection)
        {
            moves -= deltaMove;
        }
        else if(gameMode == GameMode.Deadlocked)
        {
            moves += deltaMove;
        }
    }
}
