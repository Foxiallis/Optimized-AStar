using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapTile tilePrefab;
    public MapTile[,] mapTiles;
    public int[,] map;

    private int xSize;
    private int ySize;

    private float xCornerPosition => (xSize - 1) / 2f; //the position of the border tile so that the map is always centred on 0,0
    private float yCornerPosition => (ySize - 1) / 2f;


    private void Start()
    {
        Instance = this;
        SetInitialMapValues();
        GenerateMap();
    }

    private void SetInitialMapValues()
    {
        xSize = 4;
        ySize = 4;
        mapTiles = new MapTile[xSize, ySize];
        map = new int[xSize, ySize];
    }

    public void GenerateMap()
    {
        ClearMap();

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                mapTiles[x, y] = Instantiate(tilePrefab, new Vector3(x - xCornerPosition, MapTile.PASSABLE_CUBE_HEIGHT / 2f, y - yCornerPosition), Quaternion.identity, transform);
                mapTiles[x, y].Initialize(x, y);
                map[x, y] = 1;
            }
        }
    }

    private void ClearMap()
    {
        foreach(MapTile tile in mapTiles)
        {
            Destroy(tile.gameObject);
        }

        mapTiles = new MapTile[xSize, ySize];
        map = new int[xSize, ySize];
    }

    //Functions for updating map size from input fields
    public void UpdateXSize(string value)
    {
        UpdateSize(value, true);
    }

    public void UpdateYSize(string value)
    {
        UpdateSize(value, false);
    }

    private void UpdateSize(string value, bool x)
    {
        int.TryParse(value, out int intValue);

        if (x) xSize = intValue;
        else ySize = intValue;
    }
}
