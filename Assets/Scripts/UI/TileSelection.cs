using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component allowing tiles in tilemap to be highlighted and selected
// ------------------------------------------------------------------

[RequireComponent(typeof(MapVisuals))]
[RequireComponent(typeof(Tilemap))]
public class TileSelection : MonoBehaviour
{
    MapVisuals _mapVisuals;
    Tilemap _tilemap;

    // Infinite plane parallel to tilemap; used for detecting mouse location
    // on tilemap
    Plane _tilemapPlane;

    void Awake()
    {
        _mapVisuals = GetComponent<MapVisuals>();
        _tilemap = GetComponent<Tilemap>();
        _tilemapPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Highlights the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    public void TryHighlightTile()
    {
        _mapVisuals.HighlightTile(MousePositionOnTilemap());
    }

    // Selects the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    public void TrySelectTile()
    {
        _mapVisuals.SelectTile(MousePositionOnTilemap());
    }

    // Returns the point on the tilemap that the mouse is hovering over
    Vector3 MousePositionOnTilemap()
    {
        Ray rayTowardsMousePos = UnityUtilities.RayTowardsMouse();

        if (!_tilemapPlane.Raycast(rayTowardsMousePos, out float distance))
            return Vector3.zero;

        return rayTowardsMousePos.GetPoint(distance);
    }
}