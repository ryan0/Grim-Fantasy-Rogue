using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileMapScript : MonoBehaviour
{

    public Tilemap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        int mapWidth = 50;
        int mapHeight = 50;

        AnimatedTile tile1 = Resources.Load<AnimatedTile>("Tiles/Placeholder1");
        AnimatedTile tile2 = Resources.Load<AnimatedTile>("Tiles/Placeholder2");


        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {

                int randomNum = Random.Range(0, 1);

                if (randomNum == 0)
                {
                    tileMap.SetTile(new Vector3Int(x, y, 0), tile1);
                }
                else
                {
                    tileMap.SetTile(new Vector3Int(x, y, 0), tile2);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
