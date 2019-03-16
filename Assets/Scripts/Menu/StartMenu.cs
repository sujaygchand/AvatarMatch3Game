/**
 * 
 * Author: Sujay Chand
 * 
 *  This class controls all the menu options in the tile screen
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Helpers;

public class StartMenu : MonoBehaviour
{
    public static int ScreenWidth = Screen.width;
    public static int ScreenHeight = Screen.height;

    public AudioClip pressedSound;
    private AudioSource audioSource;
    private FadeController fadeController;

    // UI canvases
    [SerializeField] GameObject s_MainMenu;
    [SerializeField] GameObject s_PlayMenu;
    [SerializeField] GameObject s_HelpMenu;

    // Force game into a portrait view
    private void Awake()
    {

    }

    // Start is called before the first frame update
    private void Start()
    {
        // 2560 1920 1280 640 (- 640)
        // 1440 1080 720 360 (- 360)

        if (!Utilities.GameLoadedOnce)
        {
            Utilities.GameLoadedOnce = true;

            if (ScreenWidth == Screen.width || ScreenHeight == Screen.height)
            {
                ScreenHeight = Screen.width - (640 * 2);
                ScreenWidth = Screen.height - (360 * 2);
                Screen.SetResolution(ScreenWidth, ScreenHeight, true);
            }
        }

        fadeController = FindObjectOfType<FadeController>();

        Utilities.IsGamePaused = false;
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = pressedSound;
        ToggleSound();
        s_HelpMenu.SetActive(false);
        s_PlayMenu.SetActive(false);
    }

    private void Update()
    {

    }

    /*
    * Toggles sound effects on start
    */
    public void ToggleSound()
    {
        //audioSource.enabled = Utilities.IsSoundActive;
    }

    /*
 * Plays the button pressed sound
 */
    private void PlaySound()
    {
        audioSource.PlayOneShot(pressedSound);
    }

    /*
     * Opens the play modes option 
     */
    public void PlayPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_MainMenu.SetActive(false);
        s_PlayMenu.SetActive(true);
        
    }

    /*
     * Back to title screen
     */ 
    public void GoBackPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_PlayMenu.SetActive(false);
        s_HelpMenu.SetActive(false);
        s_MainMenu.SetActive(true);
    }

    /*
     * Shows help image
     */ 
    public void HelpPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_MainMenu.SetActive(false);
        s_PlayMenu.SetActive(false);
        s_HelpMenu.SetActive(true);
        
    }

    /*
     * Quits game
     */ 
    public void ExitPressed()
    {
        PlaySound();
        Application.Quit();
    }

    /*
     *  Sets game mode to collection
     */ 
    public void CollectionPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.Collection;

        fadeController.SceneTransition(Utilities.GameLevel, Utilities.StartMenu);

        //SceneManager.LoadScene(Utilities.GameLevel);
        //SceneManager.UnloadSceneAsync(Utilities.StartMenu);
    }

    /*
     *  Sets game mode to time attack
     */
    public void TimeAttackPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.TimeAttack;

        fadeController.SceneTransition(Utilities.GameLevel, Utilities.StartMenu);
        //SceneManager.LoadScene(Utilities.GameLevel);
        //SceneManager.UnloadSceneAsync(Utilities.StartMenu);
    }

    public void DeadlockedPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.Deadlocked;

        fadeController.SceneTransition(Utilities.DeadlockMap, Utilities.StartMenu);
        //SceneManager.LoadScene(Utilities.DeadlockMap);
        //SceneManager.UnloadSceneAsync(Utilities.StartMenu);
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

}
