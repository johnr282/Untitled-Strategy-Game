using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component allowing tiles in tilemap to be hovered over and selected
// ------------------------------------------------------------------

[RequireComponent(typeof(Tilemap))]
public class TileSelection : MonoBehaviour
{
    Tilemap _tilemap;

    // Infinite plane parallel to tilemap; used for detecting mouse location
    // on tilemap
    Plane _tilemapPlane;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemapPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Hovers over the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    public void TryHoverOverTile()
    {
        Vector3Int tilePos = MousePositionToTile();
        if (_tilemap.HasTile(tilePos))
            EventBus.Publish(new NewTileHoveredOverEvent(tilePos));
    }

    // Selects the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    public void TrySelectTile()
    {
        Vector3Int tilePos = MousePositionToTile();
        if (_tilemap.HasTile(tilePos))
            EventBus.Publish(new TileSelectedEvent(tilePos));
    }

    // Returns the tile coordinate on the tilemap corresponding to the current
    // mouse location
    Vector3Int MousePositionToTile()
    {
        Ray rayTowardsMousePos = UnityUtilities.RayTowardsMouse();

        if (!_tilemapPlane.Raycast(rayTowardsMousePos, out float distance))
            return new Vector3Int(-1, -1, 0);

        Vector3 mousePositionOnTilePlane = rayTowardsMousePos.GetPoint(distance);
        return _tilemap.WorldToCell(mousePositionOnTilePlane);
    }
}