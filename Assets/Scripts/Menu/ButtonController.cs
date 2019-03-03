using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonController : MonoBehaviour
{
    public AudioClip pressedSound;
    private Button button;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = pressedSound;

        //button.onClick.AddListener(() => PlaySound());
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(pressedSound);
        print("woop woop");
    }



}
