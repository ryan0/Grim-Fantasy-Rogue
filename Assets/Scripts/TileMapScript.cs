using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class TileMapScript : MonoBehaviour
{
    public int mapWidth = 50;
    public int mapHeight = 50;
    public float scale = 10f; // Scale of the noise

    public Tilemap waterTileMap;
    public Tilemap otherTileMap;

    // We will use this 2D array to represent our tilemap
    private int[,] tileData;

    void Start()
    {
        // Generate the tile data
        tileData = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x / scale, y / scale);
                if (perlinValue < 0.33f)
                    tileData[x, y] = 1; // Water
                else if (perlinValue < 0.66f)
                    tileData[x, y] = 2; // Dirt
                else
                    tileData[x, y] = 3; // Tree
            }
        }

        // Save to CSV
        SaveToCSV(tileData, "tilemap.csv");

        LoadTilemaps();
    }

    void SaveToCSV(int[,] data, string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    writer.Write(data[x, y]);
                    if (x < mapWidth - 1) writer.Write(",");
                }
                writer.WriteLine();
            }
        }
    }

    public AnimatedTile[] waterTiles;
    public AnimatedTile[] dirtTiles;
    public AnimatedTile[] treeTiles;

    void LoadTiles(ref AnimatedTile[] tiles, string basePath, int count)
    {
        tiles = new AnimatedTile[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = Resources.Load<AnimatedTile>($"{basePath}{i + 1}");
        }
    }

    void LoadTilemaps()
    {
        string filename = "tilemap.csv";
        string[] lines = File.ReadAllLines(filename);

        LoadTiles(ref waterTiles, "Tiles/WaterTile", 1); // Assumes 3 water variations
        LoadTiles(ref dirtTiles, "Tiles/DirtTile", 2); // Assumes 2 dirt variations
        LoadTiles(ref treeTiles, "Tiles/TreeTile", 2); // Assumes 1 tree variation

        for (int y = 0; y < mapHeight; y++)
        {
            string[] values = lines[y].Split(',');
            for (int x = 0; x < mapWidth; x++)
            {
                int tileType = int.Parse(values[x]);
                Vector3Int position = new Vector3Int(x, y, 0);

                AnimatedTile tileToPlace = ChooseTileByType(tileType, x, y);
                if (tileType == 1) // Water
                    waterTileMap.SetTile(position, tileToPlace);
                else
                    otherTileMap.SetTile(position, tileToPlace);
            }
        }
    }

    AnimatedTile ChooseTileByType(int tileType, int x, int y)
    {
        float variationValue = Mathf.PerlinNoise(x / scale, y / scale);
        AnimatedTile[] tiles;

        switch (tileType)
        {
            case 1:
                tiles = waterTiles;
                break;
            case 2:
                tiles = dirtTiles;
                break;
            case 3:
                tiles = treeTiles;
                break;
            default:
                return null; // Return null or a default tile if an invalid type is encountered
        }

        int index = Mathf.FloorToInt(variationValue * tiles.Length);
        return tiles[index];
    }


}
