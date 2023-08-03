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
    private TileData[,] tileDataMatrix; // Matrix of TileData objects

    void Start()
    {
        tileDataMatrix = new TileData[mapWidth, mapHeight];

        string pathToJson = "tilemap.json";

        if (File.Exists(pathToJson))
        {
            LoadFromJson(); // Load from JSON if the file exists
        }
        else
        {
            // Generate the tile map if the file does not exist
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    float perlinValue = Mathf.PerlinNoise(x / scale, y / scale);
                    int tileType;
                    int motes = 5;
                    int height = 0;

                    if (perlinValue < 0.33f)
                        tileType = 1; // Water
                    else if (perlinValue < 0.66f)
                        tileType = 2; // Dirt
                    else
                        tileType = 3; // Tree

                    tileDataMatrix[x, y] = new TileData(tileType, motes, height);
                }
            }
            SaveToJson(); // Save to JSON
        }

        LoadTilemaps();
    }


    /// Draw tile methods

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
        LoadTiles(ref waterTiles, "Tiles/WaterTile", 1); // Assumes 3 water variations
        LoadTiles(ref dirtTiles, "Tiles/DirtTile", 2); // Assumes 2 dirt variations
        LoadTiles(ref treeTiles, "Tiles/TreeTile", 2); // Assumes 1 tree variation

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int tileType = tileDataMatrix[x, y].Type;
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

    /// Json methods

    void SaveToJson()
    {
        TileData[] flatArray = new TileData[mapWidth * mapHeight];
        int index = 0;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                flatArray[index] = tileDataMatrix[x, y];
                index++;
            }
        }

        TileDataList dataList = new TileDataList { TileDataArray = flatArray };
        string jsonData = JsonUtility.ToJson(dataList, true);

        string pathToJson = "tilemap.json"; // Provide the correct path
        File.WriteAllText(pathToJson, jsonData);
    }

 

    void LoadFromJson()
    {
        string pathToJson = "tilemap.json"; // Provide the correct path
        string jsonData = File.ReadAllText(pathToJson);
        TileDataList dataList = JsonUtility.FromJson<TileDataList>(jsonData);
        int index = 0;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileDataMatrix[x, y] = dataList.TileDataArray[index];
                index++;
            }
        }
    }

    /// Mutate tile methods
    // 1. Changing Particular Tile Fields
    public void ChangeTileData(int x, int y, int newType, int newMotes, int newHeight)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            tileDataMatrix[x, y].Type = newType;
            tileDataMatrix[x, y].Motes = newMotes;
            tileDataMatrix[x, y].Height = newHeight;
        }
    }
    // 2.) Debugging
    public int debugX;
    public int debugY;
    public int debugNewType;
    public int debugNewMotes;
    public int debugNewHeight;
    public void DebugApplyChanges()
    {
        ChangeTileData(debugX, debugY, debugNewType, debugNewMotes, debugNewHeight);
        LoadTilemaps();
        SaveToJson();
    }


}
