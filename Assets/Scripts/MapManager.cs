using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapTile tilePrefab;
    [HideInInspector]
    public List<MapTile> mapTiles;

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
    }

    public void GenerateMap()
    {
        ClearMap();

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                MapTile tile = Instantiate(tilePrefab, new Vector3(x - xCornerPosition, MapTile.PASSABLE_CUBE_HEIGHT / 2f, y - yCornerPosition), Quaternion.identity, transform);
                tile.Initialize(x, y);
                mapTiles.Add(tile);
            }
        }

        PlayerModelController.Instance.ResetPosition(mapTiles[0]);
    }

    private void ClearMap()
    {
        foreach (MapTile tile in mapTiles)
        {
            if (tile == null) return;
            Destroy(tile.gameObject);
        }

        mapTiles = new();
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
