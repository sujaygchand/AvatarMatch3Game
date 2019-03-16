/**
 * 
 * Author: Sujay Chand
 * 
 *  Score manager
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Helpers;

public class ScoreManager : MonoBehaviour
{
    private GameBoard gameBoard;
    private GameOverManager gameOverManager;
    public int tilesToCollect = 50;
    private int tiles;
    [SerializeField] private int moves = 14;
    public int timeLimt = 60;
    private float currentTime;
    private int displayTime;
    private bool isGameOver = false;
    
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

        gameOverManager = FindObjectOfType<GameOverManager>();

        SetupScoreManager();
    }

    /*
     *  Checks if game is over and updates dynamic text
     */ 
    void Update()
    {
        if (!isGameOver)
        {
            scoreText.text = "" + tiles;

            GameOverCheck();
        }
    }

    /*
     * Setup the board depending on game mode
     */ 
    private void SetupScoreManager()
    {
        // Resets 
        ResetScore();
        ResetTime();

        // Set text
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

    /*
     * Sets up text
     */ 
    private void GameModeSetup(string scoreInfoText, string movesOrTimerInfoText)
    {
        this.scoreInfoText.text = scoreInfoText;
        this.movesOrTimerInfoText.text = movesOrTimerInfoText;
    }

    /*
     * Reset score
     */ 
    public void ResetScore()
    {
        if(gameMode == GameMode.Collection)
        {
            tiles = tilesToCollect;
        } else 
        {
            tiles = 0;
        }

    }

    /*
     * Reset time
     */ 
    public void ResetTime()
    {
        if(gameMode == GameMode.TimeAttack)
        {
            currentTime = timeLimt;
        }
    }

    /*
     * Changes score
     */ 
    public void AddToScore(int deltaScore)
    {
        if(gameMode == GameMode.Collection)
        {
            deltaScore *= -1;
        }

        if(tiles + deltaScore <= 0 )
        {
            tiles = 0;
        } else
        {
            tiles += deltaScore;
        }
    }

    /*
     * Checks if game over
     */ 
    private void GameOverCheck()
    {
        //Time attack
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
                gameOverManager.RenderGameOverScreen(true);
                isGameOver = true;
            }


        }
        // Collection
        else if(gameMode == GameMode.Collection)
        {
            movesOrTimerText.text = "" + moves;

                // Win
                if (tiles <= 0)
                {
                    print("All tiles collected");
                    gameOverManager.RenderGameOverScreen(true);
                    isGameOver = true;
            }

            // Loss
            else if (moves <= 0 && tiles > 0)
            {
                print("All tiles collected");
                gameOverManager.RenderGameOverScreen(false);
                isGameOver = true;
            }
            }

        // Deadlocked game mode
        else
        {
            movesOrTimerText.text = "" + moves;
        } 
        }

    /*
     * Change move counter
     */ 
    public void ChangeMovesCounter()
    {
        if(gameMode == GameMode.Collection)
        {
            moves--;
        }
        else if(gameMode == GameMode.Deadlocked)
        {
            moves++;
        }
    }
}
