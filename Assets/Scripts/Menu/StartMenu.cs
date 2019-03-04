﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Helpers;

public class StartMenu : MonoBehaviour
{
    public AudioClip pressedSound;
    private AudioSource audioSource;

    [SerializeField] GameObject s_MainMenu;
    [SerializeField] GameObject s_PlayMenu;
    [SerializeField] GameObject s_HelpMenu;

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

    public void ToggleSound()
    {
        audioSource.enabled = Utilities.IsSoundActive;
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(pressedSound);
    }

    public void PlayPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_MainMenu.SetActive(false);
        s_PlayMenu.SetActive(true);
        
    }

    public void GoBackPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_PlayMenu.SetActive(false);
        s_HelpMenu.SetActive(false);
        s_MainMenu.SetActive(true);
    }

    public void HelpPressed()
    {
        Utilities.IsGamePaused = false;
        PlaySound();
        s_MainMenu.SetActive(false);
        s_PlayMenu.SetActive(false);
        s_HelpMenu.SetActive(true);
        
    }

    public void ExitPressed()
    {
        PlaySound();
        Application.Quit();
    }

    public void CollectionPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.Collection;
        SceneManager.LoadScene(Utilities.GameLevel);
    }

    public void TimeAttackPressed()
    {
        PlaySound();
        Utilities.GameMode = GameMode.TimeAttack;
        SceneManager.LoadScene(Utilities.GameLevel);
    }


}
