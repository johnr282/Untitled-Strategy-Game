using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component allowing objects and tiles on the map to be hovered over
// and selected with the mouse
// ------------------------------------------------------------------

[RequireComponent(typeof(Tilemap))]
public class MouseSelection : MonoBehaviour
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

    void Update()
    {
        try
        {
            if (!PlayerManager.MyTurn)
                return;
        }
        catch
        {
            return;
        }

        if (SelectionDetected())
            TrySelection();
        else
            TryHover();
    }

    // Returns whether the player attempted to select something
    bool SelectionDetected()
    {
        int leftMouseButton = 0;
        return Input.GetMouseButtonDown(leftMouseButton);
    }

    // Attempts to select an object at the current mouse position; returns true
    // if successful, false if no selectable object was found
    bool TrySelection()
    {
        // No selectable objects active during territory selection phase
        if (GamePhaseManager.CurrentPhase != GamePhase.TerritorySelection &&
            MouseRaycast(out GameObject collidedObject))
        {
            if (collidedObject.TryGetComponent(out SelectableObject selectedObject))
            {
                selectedObject.OnSelect();
                return true;
            }
        }

        // If no SelectableObject was found, then try selecting a tile from the map
        TrySelectTile();

        return false;
    }

    // Attempts to hover over an object at the current mouse position; returns true
    // if successful, false if no hoverable object was found
    bool TryHover()
    {
        if (MouseRaycast(out GameObject collidedObject))
        {
            if (collidedObject.TryGetComponent(out HoverableObject hoveredObject))
            {
                hoveredObject.OnHover();
                return true;
            }
        }

        // If no HoverableObject was found, then try highlighting a tile from the map
        TryHoverOverTile();

        return false;
    }

    // Performs a raycast from current mouse position; returns true if cast hits
    // a valid collider and puts the first object the raycast collides with into
    // the output parameter collidedObject, returns false if cast doesn't hit
    // a valid collider
    bool MouseRaycast(out GameObject collidedObject)
    {
        collidedObject = null;
        Ray rayTowardsMousePos = UnityUtilities.RayTowardsMouse();

        if (!Physics.Raycast(rayTowardsMousePos, out RaycastHit hit) ||
            hit.collider == null)
        {
            return false;
        }

        collidedObject = hit.collider.gameObject;
        return true;
    }

    // Hovers over the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    void TryHoverOverTile()
    {
        Vector3Int tilePos = MousePositionToTile();
        if (_tilemap.HasTile(tilePos))
            EventBus.Publish(new NewTileHoveredOverEvent(
                HexUtilities.ConvertToHexCoordinateOffset(tilePos)));
    }

    // Selects the tile that the mouse is currently on, or does nothing if
    // mouse is outside the map
    void TrySelectTile()
    {
        Vector3Int tilePos = MousePositionToTile();
        if (_tilemap.HasTile(tilePos))
            EventBus.Publish(new TileSelectedEvent(
                HexUtilities.ConvertToHexCoordinateOffset(tilePos)));
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