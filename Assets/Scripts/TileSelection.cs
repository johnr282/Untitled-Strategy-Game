using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component allowing tiles in tilemap to be highlighted and selected
// ------------------------------------------------------------------

[RequireComponent(typeof(MapVisuals))]
public class TileSelection : MonoBehaviour
{
    MapVisuals _mapVisuals;

    // Infinite plane parallel to tilemap; used for detecting mouse location
    // on tilemap
    Plane _tilemapPlane;

    void Awake()
    {
        _tilemapPlane = new Plane(Vector3.up, Vector3.zero);
        _mapVisuals = GetComponent<MapVisuals>();
    }

    void Update()
    {
        HighlightCurrentTile();
    }

    // Highlights whichever tile the mouse is hovering over
    void HighlightCurrentTile()
    {
        Vector3 mousePositionOnTilemap = MousePositionOnTilemap();
        _mapVisuals.HighlightTile(mousePositionOnTilemap);
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