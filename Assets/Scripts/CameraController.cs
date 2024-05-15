using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed;

    public bool isEditMode { private get; set; }

    private Camera cam;
    private Vector3 initialMousePosition;

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
        HandleDrag();
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            initialMousePosition = Input.mousePosition;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 deltaMousePosition = Input.mousePosition - initialMousePosition;

        initialMousePosition = Input.mousePosition;

        float deltaX = deltaMousePosition.x * rotationSpeed * Time.deltaTime;
        float deltaY = deltaMousePosition.y * rotationSpeed * Time.deltaTime;

        transform.RotateAround(Vector3.zero, Vector3.up, deltaX);
        transform.RotateAround(Vector3.zero, transform.right, -deltaY); //inverted for more intuitive feel
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
