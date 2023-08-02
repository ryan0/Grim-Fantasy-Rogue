using UnityEngine.Tilemaps;
using UnityEngine;

public class TileMapScript : MonoBehaviour
{
    public Tilemap tileMap;
    public Tilemap maskTileMap; // Reference to the mask Tilemap
    public int mapWidth = 50;
    public int mapHeight = 50;
    public float scale = 10f; // Scale of the noise
    public float variationScale = 2f; // Scale for variations

    public AnimatedTile[] waterTiles;
    public AnimatedTile[] dirtTiles;
    public AnimatedTile[] treeTiles;

    public AnimatedTile whiteTile; // Reference to the white Tile used in the mask
    public AnimatedTile blackTile; // Reference to the black Tile used in the mask


    void Start()
    {
        // Load tiles dynamically, you can replace this with manual assignment in the Inspector
        // The path should be consistent, e.g., "Tiles/WaterTile1", "Tiles/WaterTile2", etc.
        LoadTiles(ref waterTiles, "Tiles/WaterTile", 1); // Assumes 3 water variations
        LoadTiles(ref dirtTiles, "Tiles/DirtTile", 2); // Assumes 2 dirt variations
        LoadTiles(ref treeTiles, "Tiles/TreeTile", 2); // Assumes 1 tree variation

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Generate Perlin noise value for each coordinate
                float perlinValue = Mathf.PerlinNoise(x / scale, y / scale);

                AnimatedTile tileToPlace;

                //set tile locations
                if (perlinValue < 0.33f)
                {
                    tileToPlace = ChooseTile(waterTiles, x, y);
                }
                else if (perlinValue < 0.66f)
                {
                    tileToPlace = ChooseTile(dirtTiles, x, y);
                }
                else
                {
                    tileToPlace = ChooseTile(treeTiles, x, y);
                }

                //set mask tilemap for water
                // Set the corresponding mask tile based on the Perlin noise value
                if (perlinValue < 0.33f)
                {
                    maskTileMap.SetTile(new Vector3Int(x, y, 0), whiteTile); // White where water is
                }
                else
                {
                    maskTileMap.SetTile(new Vector3Int(x, y, 0), blackTile); // Black elsewhere
                }

                tileMap.SetTile(new Vector3Int(x, y, 0), tileToPlace);
            }
        }
    }

    AnimatedTile ChooseTile(AnimatedTile[] tiles, int x, int y)
    {
        float variationValue = Mathf.PerlinNoise(x / variationScale, y / variationScale);
        int index = Mathf.FloorToInt(variationValue * tiles.Length);
        return tiles[index];
    }

    void LoadTiles(ref AnimatedTile[] tiles, string basePath, int count)
    {
        tiles = new AnimatedTile[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = Resources.Load<AnimatedTile>($"{basePath}{i + 1}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
