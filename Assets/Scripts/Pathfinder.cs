using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * I've chosen A* algorithm as it's the fastest algorithm for such use case (2D array with same distances between the tiles) that doesn't sacrifice any significant quality.
 * I've additionally updated it for more heuristics and optimizations, that I will comment on below.
*/
public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance;

    [SerializeField]
    private float heuristicWeight; //Optimization: weighted heuristic distance will speed up the search at the cost of optimality

    public Material baseMaterial;
    public Material startPositionMaterial;
    public Material endPositionMaterial;
    public Material pathMaterial;

    [HideInInspector]
    public MapTile startTile;
    [HideInInspector]
    public MapTile endTile;

    private void Awake()
    {
        Instance = this;
        startTile = null;
        endTile = null;
    }

    public void OnTileClick(MapTile tile)
    {
        if (!tile.passable) return;

        if (startTile == null)
        {
            SetStartTile(tile, true);
        }
        else
        {
            HandleSecondTileSelection(tile);
        }
    }

    private void HandleSecondTileSelection(MapTile tile)
    {
        if (tile == startTile)
        {
            SetStartTile(tile, false);
        }
        else if (endTile == null)
        {
            SetEndTile(tile, true);
            StartPathfinding();
        }
        else if (tile == endTile)
        {
            SetEndTile(tile, false);
        }
    }

    //Used during mode switching from walking to editing
    public void DeselectStartTile()
    {
        SetStartTile(startTile, false);
    }

    private void ClearTileMaterials(bool skipStartTile = true)
    {
        foreach (MapTile tile in MapManager.Instance.mapTiles)
        {
            if (skipStartTile && tile == startTile) continue;

            tile.UpdateMaterial(baseMaterial);
        }
    }

    private void SetEndTile(MapTile tile, bool select)
    {
        tile.UpdateMaterial(select ? endPositionMaterial : baseMaterial);
        endTile = select ? tile : null;

        if (!select)
        {
            ClearTileMaterials(); //Deselecting end tile clears all paths
            PlayerModelController.Instance.ResetPosition(startTile);
        }
    }

    private void SetStartTile(MapTile tile, bool select)
    {
        if (tile != null) tile.UpdateMaterial(select ? startPositionMaterial : baseMaterial);
        startTile = select ? tile : null;

        if (select) PlayerModelController.Instance.ResetPosition(startTile);

        if (!select && endTile != null) SetEndTile(endTile, false); //Deselecting start tile also deselects end tile to avoid undefined behavior
    }

    private void StartPathfinding()
    {
        InitializePathfinding();
        FindOptimalPath();
    }

    private void InitializePathfinding()
    {
        foreach (MapTile tile in MapManager.Instance.mapTiles)
        {
            tile.previousTileForward = null;
            tile.previousTileBackward = null;
            tile.g = int.MaxValue;
        }

        startTile.g = 0;
        startTile.h = heuristicWeight * startTile.ManhattanDistance(endTile);

        endTile.g = 0;
        endTile.h = heuristicWeight * startTile.ManhattanDistance(startTile);
    }

    private void FindOptimalPath()
    {
        HashSet<MapTile> startOpenSet = new() { startTile };
        HashSet<MapTile> endOpenSet = new() { endTile };

        HashSet<MapTile> startClosedSet = new();
        HashSet<MapTile> endClosedSet = new();

        bool solutionFound = false;
        MapTile meetingTile = null;

        while (startOpenSet.Count > 0 && endOpenSet.Count > 0 && !solutionFound)
        {
            //Optimization: bidirectional search to find the meeting point from the two points
            solutionFound = Step(startOpenSet, startClosedSet, true, out meetingTile) ||
                            Step(endOpenSet, endClosedSet, false, out meetingTile);
        }

        if (solutionFound)
        {
            List<MapTile> path = ReconstructPath(meetingTile);
            ColorPath(path);
            PlayerModelController.Instance.StartWalking(path);
        }
    }

    private bool Step(HashSet<MapTile> openSet, HashSet<MapTile> closedSet, bool searchForward, out MapTile meetingTile)
    {
        if (openSet.Count == 0)
        {
            meetingTile = null;
            return false;
        }

        MapTile currentTile = openSet.OrderBy(tile => tile.f).First();

        openSet.Remove(currentTile);
        closedSet.Add(currentTile);

        foreach (MapTile neighbourTile in currentTile.GetNeighbours())
        {
            if (!neighbourTile.passable || closedSet.Contains(neighbourTile)) continue;

            if (currentTile.previousTileForward != null && currentTile.previousTileBackward != null) //Optimization: early exit to return the path that was found the quickest
            {
                meetingTile = currentTile;
                return true;
            }

            float gCost = currentTile.g + 1;

            if (gCost < neighbourTile.g || !openSet.Contains(neighbourTile))
            {
                neighbourTile.g = gCost;
                neighbourTile.h = heuristicWeight * neighbourTile.ManhattanDistance(searchForward ? endTile : startTile);
                if (searchForward)
                {
                    neighbourTile.previousTileForward = currentTile;
                }
                else
                {
                    neighbourTile.previousTileBackward = currentTile;
                }

                if (!openSet.Contains(neighbourTile))
                {
                    openSet.Add(neighbourTile);
                }
            }
        }

        meetingTile = null;
        return false;
    }

    private void ColorPath(List<MapTile> path)
    {
        foreach (MapTile tile in path)
        {
            if (tile != startTile && tile != endTile)
            {
                tile.UpdateMaterial(pathMaterial);
            }
        }
    }

    private List<MapTile> ReconstructPath(MapTile meetingTile)
    {
        List<MapTile> path = new List<MapTile>();

        MapTile current = meetingTile;

        while (current != null)
        {
            path.Add(current);
            current = current.previousTileForward;
        }

        path.Reverse();
        current = meetingTile.previousTileBackward;

        while (current != null)
        {
            path.Add(current);
            current = current.previousTileBackward;
        }

        return path;
    }
}
