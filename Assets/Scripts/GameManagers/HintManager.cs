using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public HashSet<GameObject> currentHints = new HashSet<GameObject>();
    private GameBoard gameBoard;
    private Deadlock deadlock;
    [SerializeField] float s_hintDelayMax;
    private float hintDelay;

    [SerializeField] GameObject s_HintParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
        deadlock = FindObjectOfType<Deadlock>();

        hintDelay = s_hintDelayMax;
    }

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

    private void ResetHintTimer()
    {
        hintDelay = s_hintDelayMax;
    }

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
