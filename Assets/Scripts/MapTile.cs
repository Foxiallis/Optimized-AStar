using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    #region Pathfinding Variables
    public bool passable = true;

    public int x, y;

    [HideInInspector]
    public MapTile previousTileForward;
    [HideInInspector]
    public MapTile previousTileBackward; //for forward and backward search
    [HideInInspector]
    public float g, h;
    public float f => g + h;
    #endregion

    public const float PASSABLE_CUBE_HEIGHT = 0.2f;
    public const float IMPASSABLE_CUBE_HEIGHT = 1f;

    private float passabilityShift => (IMPASSABLE_CUBE_HEIGHT / 2f) - (PASSABLE_CUBE_HEIGHT / 2f); //Aligns the higher tiles in line with the lower ones

    private Renderer _renderer;

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
        _renderer = GetComponent<Renderer>();
    }
    public void ChangePassability()
    {
        passable = !passable;
        SetSizeFromPassability();
    }

    public void UpdateMaterial(Material newMaterial)
    {
        _renderer.material = newMaterial;
    }

    private void SetSizeFromPassability()
    {
        transform.localScale = passable ? new Vector3(1, PASSABLE_CUBE_HEIGHT, 1) : new Vector3(1, IMPASSABLE_CUBE_HEIGHT, 1);
        transform.Translate(new Vector3(0, passable ? -passabilityShift : passabilityShift, 0));
    }

    public int ManhattanDistance(MapTile otherTile)
    {
        return Mathf.Abs(otherTile.x - x) + Mathf.Abs(otherTile.y - y);
    }

    public HashSet<MapTile> GetNeighbours()
    {
        return MapManager.Instance.mapTiles.Where(tile => ManhattanDistance(tile) == 1).ToHashSet(); //tiles with manhattan distance 1 are direct neighbours
    }
}
