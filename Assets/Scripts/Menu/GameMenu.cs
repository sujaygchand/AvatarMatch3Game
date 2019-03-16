/**
 * 
 * Author: Sujay Chand
 * 
 *  This class controls all the menu options in game
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Helpers;


public class GameMenu : MonoBehaviour
{
    // Button sound
    public AudioClip pressedSound;

    // Game object references for buttons and panels 
    [SerializeField] GameObject s_PauseMenu;
    [SerializeField] GameObject s_QuitMenu;
    [SerializeField] GameObject s_MusicButton;
    [SerializeField] GameObject s_SoundButton;

    private AudioSource audioSource;
    private GameBoard gameBoard;
    private GameOverManager gameOverManager;
    private bool isGamePaused;

    // Start is called before the first frame update
    void Start()
    {
        Utilities.IsGamePaused = false;
        audioSource = GetComponent<AudioSource>();
        gameBoard = FindObjectOfType<GameBoard>();
        gameOverManager = FindObjectOfType<GameOverManager>();

        audioSource.playOnAwake = false;
        audioSource.clip = pressedSound;
        isGamePaused = false;
        s_PauseMenu.SetActive(isGamePaused);
        Time.timeScale = 1f;
        ToggleSound();
        CheckMuted();

    }

    /*
     * Toggles sound effects on start
     */
    public void ToggleSound()
    {
        audioSource.enabled = Utilities.IsSoundActive;
    }

    /*
     * Plays the button pressed sound
     */
    private void PlaySound()
    {
        audioSource.PlayOneShot(pressedSound);
    }

    /*
     * Restarts the scene
     */
    public void RestartLevel()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*
     *  // Experiening crashes with the lastest version of Unity
     *  
     * Takes the player to the main menu
     */
    public void GameMapMainMenu()
    {

        //PlaySound();
        //Application.Quit();

       
        Utilities.IsGamePaused = false;
        PlaySound();

        // Experiening crashes with the lastest version of Unity
        SceneManager.LoadSceneAsync(Utilities.StartMenu);
        SceneManager.UnloadSceneAsync(Utilities.GameLevel);
    }

    /*
     *  // Experiening crashes with the lastest version of Unity
     *  
 * Takes the player to the main menu
 */
    public void DeadlockMainMenu()
    {
        //PlaySound();
        //Application.Quit();

        // Experiening crashes with the lastest version of Unity
        Utilities.IsGamePaused = false;
        PlaySound();

        // Experiening crashes with the lastest version of Unity
        SceneManager.LoadScene(Utilities.StartMenu);
        SceneManager.UnloadSceneAsync(Utilities.DeadlockMap);
    }

    /*
     * Toggle pause
     */
    public void TogglePause()
    {
        if (gameOverManager)
        {
            if (gameOverManager.isGameOver == false && s_QuitMenu.activeSelf == false)
            {
                PlaySound();
                isGamePaused = !isGamePaused;

                Utilities.IsGamePaused = isGamePaused;

                s_PauseMenu.SetActive(isGamePaused);

                if (isGamePaused)
                {

                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
        }
    }

    /*
     * Mute/un-mute sound effects
     */
    public void MuteSoundEffects()
    {
        PlaySound();

        //Refence to all audio sources
        AudioSource[] soundEffects = FindObjectsOfType<AudioSource>();

        Utilities.IsSoundActive = !Utilities.IsSoundActive;

        //Change the button alpha is show active state
        var tempColour = s_SoundButton.GetComponent<Image>().color;

        if (Utilities.IsSoundActive)
        {
            tempColour.a = 1f;
        }
        else
        {
            tempColour.a = 0.27f;
        }

        s_SoundButton.GetComponent<Image>().color = tempColour;

        // Looks for all audio sources linked to an object with one of the scripts
        foreach (AudioSource soundFX in soundEffects)
        {
            if (soundFX.gameObject.GetComponent<MatchesManager>() || soundFX.gameObject.GetComponent<GameMenu>() || soundFX.gameObject.GetComponent<StartMenu>())
            {
                soundFX.gameObject.SendMessage("ToggleSound");
            }
        }
    }

    /*
     * Mute/un-mute music effects
     */
    public void MuteMusic()
    {
        PlaySound();

        // Find all objects eith script
        MusicManager[] musicObjects = FindObjectsOfType<MusicManager>();

        Utilities.IsMusicActive = !Utilities.IsMusicActive;

        //Change the button alpha is show active state
        var tempColour = s_MusicButton.GetComponent<Image>().color;

        if (Utilities.IsMusicActive)
        {
            tempColour.a = 1f;
        }
        else
        {
            tempColour.a = 0.27f;
        }

        s_MusicButton.GetComponent<Image>().color = tempColour;


        // Toggle music among all objects
        foreach (MusicManager music in musicObjects)
        {
            music.ToggleMusic();
        }

    }

    /*
     * Opens Quit menu
     */ 
    public void OpenQuitMenu()
    {
        s_PauseMenu.SetActive(false);

        s_QuitMenu.SetActive(true);
    }

    /*
    * Closes Quit menu
    */
    public void CloseQuitMenu()
    {
        s_QuitMenu.SetActive(false);

        TogglePause();
    }

    // Checks mute status on start
    private void CheckMuted()
    {
        if (!Utilities.IsSoundActive)
        {
            var tempColour = s_SoundButton.GetComponent<Image>().color;

            tempColour.a = 0.27f;


            s_SoundButton.GetComponent<Image>().color = tempColour;
        }

        if (!Utilities.IsMusicActive)
        {
            var tempColour = s_MusicButton.GetComponent<Image>().color;

            tempColour.a = 0.27f;


            s_MusicButton.GetComponent<Image>().color = tempColour;
        }
    }
}
