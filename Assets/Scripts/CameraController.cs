using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isEditMode { private get; set; }

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        isEditMode = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryRaycastTile();
        }  
    }

    private void TryRaycastTile()
    {
        Ray raycast = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(raycast, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out MapTile tile))
            {
                HandleTileClick(tile);
            }
        }
    }

    private void HandleTileClick(MapTile tile)
    {
        if (isEditMode) tile.ChangePassability();
        else return; //TODO
    }
}
