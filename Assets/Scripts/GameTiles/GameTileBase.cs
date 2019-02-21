using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Helpers;

[RequireComponent(typeof(SpriteRenderer))]
public class GameTileBase : MonoBehaviour
{
    [SerializeField] private GameTile tileType;

    SpriteRenderer tileImage;
    
    // Start is called before the first frame update
    void Start()
    {
        tileImage = GetComponent<SpriteRenderer>();
        tileType = (GameTile)Random.Range(0, Utilities.NumOfGameTileTypes());
        SetTileType(tileType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite LoadTileSprite(string tile)
    {
        return Resources.Load<Sprite>("Tiles/" + tile);
            
            //(Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/Tiles" + tile, typeof(Sprite));
    }

    public void SetTileType(GameTile tileType)
    {
        this.tileType = tileType;

        tileImage.sprite = LoadTileSprite(Utilities.FindTileType(tileType));

        print(LoadTileSprite(Utilities.FindTileType(tileType)));
    }

    public GameTile GetGameTile()
    {
        return tileType;
    }
}
