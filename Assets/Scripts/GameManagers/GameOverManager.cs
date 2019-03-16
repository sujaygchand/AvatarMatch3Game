/**
 * 
 * Author: Sujay Chand
 * 
 *  Game Over manager
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Helpers;

public class GameOverManager : MonoBehaviour
{
    // UI GameObjects
    [SerializeField] private Text s_GameOverText;
    [SerializeField] private GameObject s_GameOver;

    public bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        s_GameOver.SetActive(false);
    }

    // Renders game over screen
    public void RenderGameOverScreen(bool hasWon)
    {
        Utilities.IsGamePaused = true;
        s_GameOver.SetActive(true);
        isGameOver = true;

        if (Utilities.GameMode == GameMode.Collection)
        {
            if (hasWon)
            {
                s_GameOverText.text = "YOU WON";
            }
            else
            {
                s_GameOverText.text = "TRY AGAIN";
            }
        }
        else
        {
            s_GameOverText.text = "PLAY AGAIN?";
        }
    }

}