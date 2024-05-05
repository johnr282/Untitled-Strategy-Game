using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component allowing tiles in tilemap to be highlighted and selected
// ------------------------------------------------------------------

[RequireComponent(typeof(MapVisuals))]
[RequireComponent(typeof(Collider))]
public class TileSelection : MonoBehaviour
{
    MapVisuals _mapVisuals;
    Collider _collider;

    // Infinite plane parallel to tilemap; used for detecting mouse location
    // on tilemap
    Plane _tilemapPlane;

    void Awake()
    {
        _mapVisuals = GetComponent<MapVisuals>();
        _tilemapPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        HighlightCurrentTile();
        CheckForSelection();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    // Highlights whichever tile the mouse is hovering over
    void HighlightCurrentTile()
    {
        _mapVisuals.HighlightTile(MousePositionOnTilemap());
    }

    // Checks if player selected a tile
    void CheckForSelection()
    {
        int leftMouseButton = 0;
        bool tileSelected = Input.GetMouseButtonDown(leftMouseButton);

        if (tileSelected)
            _mapVisuals.SelectTile(MousePositionOnTilemap());
    }

    // Returns the point on the tilemap that the mouse is hovering over
    Vector3 MousePositionOnTilemap()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Ray rayTowardsMousePos = Camera.main.ScreenPointToRay(mousePosScreen);

        if (!_tilemapPlane.Raycast(rayTowardsMousePos, out float distance))
            return Vector3.zero;

        return rayTowardsMousePos.GetPoint(distance);
    }
}