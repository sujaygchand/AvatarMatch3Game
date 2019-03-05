/**
 * 
 * Author: Sujay Chand
 * 
 *  Hint manager
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public HashSet<GameObject> currentHints = new HashSet<GameObject>();                // Set for hints
    private GameBoard gameBoard;
    private Deadlock deadlock;
    [SerializeField] float s_hintDelayMax;                              // Max Hint delay
    public float hintDelay;

    // Hint prefab particle
    [SerializeField] GameObject s_HintParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
        deadlock = FindObjectOfType<Deadlock>();

        // Set delay  
        hintDelay = s_hintDelayMax;
    }

    // Updates time for hint counter
    private void Update()
    {
        if (gameBoard.currentPlayerState == PlayerState.Active)
        {

            hintDelay -= Time.deltaTime;

            if (hintDelay <= 0 && currentHints.Count <= 0)
            {
                SetAndSpawnHints();
            }
        }
    }

    /*
     * Reset hint, countdown
     */ 
    private void ResetHintTimer()
    {
        hintDelay = s_hintDelayMax;
    }

    /*
     *  Spawns hint on possible combo
     */ 
    private void SetAndSpawnHints()
    {
        foreach(GameObject hint in deadlock.possibleCombo)
        {
            if (hint)
            {
                GameObject tempHintParticles = Instantiate(s_HintParticle, hint.transform.position, Quaternion.identity);
                currentHints.Add(tempHintParticles);
            }
        }
    }

    /*
     * Destroys hint instance
     */ 
    public void DestroyHints()
    {
        if(currentHints.Count > 0)
        {
            foreach(GameObject hint in currentHints)
            {
                    Destroy(hint);
            }


            currentHints.Clear();
        }

        ResetHintTimer();
    }

}
