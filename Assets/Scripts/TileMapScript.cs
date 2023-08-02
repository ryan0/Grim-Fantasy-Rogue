using UnityEngine.Tilemaps;
using UnityEngine;

public class TileMapScript : MonoBehaviour
{
    public Tilemap tileMap;
    public int mapWidth = 50;
    public int mapHeight = 50;
    public float scale = 10f; // Scale of the noise

    void Start()
    {
        AnimatedTile tile1 = Resources.Load<AnimatedTile>("Tiles/Placeholder1");
        AnimatedTile tile2 = Resources.Load<AnimatedTile>("Tiles/Placeholder2");

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Generate Perlin noise value for each coordinate
                float perlinValue = Mathf.PerlinNoise(x / scale, y / scale);

                // Use the Perlin noise value to choose between the two tiles
                if (perlinValue < 0.5f)
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
