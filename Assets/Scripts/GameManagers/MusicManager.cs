﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class MusicManager : MonoBehaviour
{

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = Utilities.IsMusicActive;
        ToggleMusic();
    }

    public void ToggleMusic()
    {
        audioSource.enabled = Utilities.IsMusicActive;

        if (audioSource.enabled)
        {
            audioSource.Play();
            audioSource.loop = true;
        }
    }
}