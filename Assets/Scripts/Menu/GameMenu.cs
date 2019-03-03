using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Helpers;


public class GameMenu : MonoBehaviour
{
    public AudioClip pressedSound;
    [SerializeField] GameObject s_PauseMenu;
    [SerializeField] GameObject s_MusicButton;
    [SerializeField] GameObject s_SoundButton;
    private AudioSource audioSource;
    private GameBoard gameBoard;
    private bool isGamePaused;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameBoard = FindObjectOfType<GameBoard>();

        audioSource.playOnAwake = false;
        audioSource.clip = pressedSound;
        isGamePaused = false;
        s_PauseMenu.SetActive(isGamePaused);
        ToggleSound();
        CheckMuted();

    }

    public void ToggleSound()
    {
        audioSource.enabled = Utilities.IsSoundActive;
    }

    private void PlaySound()
{
    audioSource.PlayOneShot(pressedSound);
}

public void RestartLevel()
{
    PlaySound();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

    public void MainMenu()
    {
        PlaySound();
        SceneManager.LoadScene(Utilities.StartMenu);
    }

    public void TogglePause()
    {
        PlaySound();
        isGamePaused = !isGamePaused;

        Utilities.IsGamePaused = isGamePaused;

        s_PauseMenu.SetActive(isGamePaused);

        if (isGamePaused)
        {
            
            Time.timeScale = 0f;
        } else
        {
            //gameBoard.currentPlayerState = PlayerState.Active;
            Time.timeScale = 1f;
        }
    }

    public void MuteSoundEffects()
    {
        PlaySound();

        AudioSource[] soundEffects = FindObjectsOfType<AudioSource>();

        Utilities.IsSoundActive = ! Utilities.IsSoundActive;

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

        foreach (AudioSource soundFX in soundEffects)
        {
            if (soundFX.gameObject.GetComponent<MatchesManager>() || soundFX.gameObject.GetComponent<GameMenu>() || soundFX.gameObject.GetComponent<StartMenu>())
            {
                soundFX.gameObject.SendMessage("ToggleSound");
            }
        }
    }

    public void MuteMusic()
    {
        PlaySound();

        MusicManager[] musicObjects = FindObjectsOfType<MusicManager>();

        Utilities.IsMusicActive = ! Utilities.IsMusicActive;

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

        foreach (MusicManager music in musicObjects)
        {
            music.ToggleMusic();
        }

    }

    private void CheckMuted()
    {
        if (! Utilities.IsSoundActive)
        {
            var tempColour = s_SoundButton.GetComponent<Image>().color;

                tempColour.a = 0.27f;
          

            s_SoundButton.GetComponent<Image>().color = tempColour;
        }

        if(!Utilities.IsMusicActive)
        {
            var tempColour = s_MusicButton.GetComponent<Image>().color;

            tempColour.a = 0.27f;


            s_MusicButton.GetComponent<Image>().color = tempColour;
        }
    }
}
