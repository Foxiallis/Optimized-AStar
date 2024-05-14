using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[,] map;

    private int xSize;
    private int ySize;

    private float xCornerPosition => (xSize - 1) / 2f; //the position of the border tile so that the map is always centred on 0,0
    private float yCornerPosition => (ySize - 1) / 2f;

    private const float PASSABLE_CUBE_HEIGHT = 0.2f;
    private const float IMPASSABLE_CUBE_HEIGHT = 1f;

    private void Start()
    {
        xSize = 4;
        ySize = 4;
        map = new GameObject[xSize, ySize];

        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearMap();

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                map[x, y] = Instantiate(tilePrefab, new Vector3(x - xCornerPosition, PASSABLE_CUBE_HEIGHT / 2f, y - yCornerPosition), Quaternion.identity, transform);
            }
        }
    }

    private void ClearMap()
    {
        foreach(GameObject tile in map)
        {
            Destroy(tile);
        }

        map = new GameObject[xSize, ySize];
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
