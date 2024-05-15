using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public bool passable = true;

    public const float PASSABLE_CUBE_HEIGHT = 0.2f;
    public const float IMPASSABLE_CUBE_HEIGHT = 1f;

    private int x, y;

    private float passabilityShift => (IMPASSABLE_CUBE_HEIGHT / 2f) - (PASSABLE_CUBE_HEIGHT / 2f); //Aligns the higher tiles in line with the lower ones


    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void ChangePassability()
    {
        passable = !passable;
        MapManager.Instance.map[x, y] = passable ? 1 : 0;
        SetSizeFromPassability();
    }

    private void SetSizeFromPassability()
    {
        transform.localScale = passable ? new Vector3(1, PASSABLE_CUBE_HEIGHT, 1) : new Vector3(1, IMPASSABLE_CUBE_HEIGHT, 1);
        transform.Translate(new Vector3(0, passable ? -passabilityShift : passabilityShift, 0));
    }
}
