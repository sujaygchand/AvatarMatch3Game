using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Helpers;

public class GridTitle : MonoBehaviour
{
    //private GameObject gameTilesParent;
    // Start is called before the first frame update
    void Start()
    {
        //gameTilesParent = GameObject.FindGameObjectWithTag(Utilities.GameTiles);
        SpawnTile();
    }

    // Update is called once per frame
    public void SpawnTile()
    {
        GameObject tempTile = Resources.Load<GameObject>(
            string.Format("{0}/{1}{2}", Utilities.Prefabs, Utilities.PF, Utilities.BaseGameTile)
            );

        Instantiate(tempTile, gameObject.transform.position, Quaternion.identity, gameObject.transform);
    }
}
