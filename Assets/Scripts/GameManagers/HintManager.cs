using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public HashSet<GameObject> currentHints = new HashSet<GameObject>();
    private GameBoard gameBoard;
    private Deadlock deadlock;
    [SerializeField] float s_hintDelay;
    private float hintDelaySeconds;

    [SerializeField] GameObject s_HintParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
        deadlock = FindObjectOfType<Deadlock>();

        hintDelaySeconds = s_hintDelay;
    }

    private void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        
        if(hintDelaySeconds <= 0 && currentHints.Count <= 0)
        {
            SetAndSpawnHints();
        }
    
    }

    private void SetAndSpawnHints()
    {
        foreach(GameObject hint in deadlock.possibleCombo)
        {
            GameObject tempHintParticles = Instantiate(s_HintParticle, hint.transform.position, Quaternion.identity);
            currentHints.Add(tempHintParticles);
            print(tempHintParticles);
        }
    }

    public void DestroyHints()
    {

        if(currentHints.Count > 0)
        {
            foreach(GameObject hint in currentHints)
            {
                Destroy(hint);
                hintDelaySeconds = s_hintDelay;
            }

            currentHints.Clear();
        }
    }

}
