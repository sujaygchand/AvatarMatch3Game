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
    public AudioClip pressedSound;
    private AudioSource audioSource;

    // UI canvases
    [SerializeField] GameObject s_MainMenu;
    [SerializeField] GameObject s_PlayMenu;
    [SerializeField] GameObject s_HelpMenu;

    // Start is called before the first frame update
    private void Start()
    {
        Utilities.IsGamePaused = false;
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = pressedSound;
        ToggleSound();
        s_HelpMenu.SetActive(false);
        s_PlayMenu.SetActive(false);
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
        SceneManager.LoadScene(Utilities.GameLevel);
    }

    /*
     *  Sets game mode to time attack
     */
    public void TimeAttackPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.TimeAttack;
        SceneManager.LoadScene(Utilities.GameLevel);
    }


}
