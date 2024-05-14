using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component for handling the Unity Tilemap system used to render the
// game map and hexagonal grid 
// ------------------------------------------------------------------

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TileLibrary))]
public class MapVisuals : MonoBehaviour
{
    Tilemap _tilemap;
    TileLibrary _tileLibrary;

    Vector3Int _currentlyHighlightedTile = new Vector3Int(-1, -1, -1);
    Vector3Int _currentlySelectedTile = new Vector3Int(-1, -1, -1);

    List<Vector3Int> _currentlyHighlightedPath = new();

    [SerializeField] float _tileSaturationFactor;

    [SerializeField] List<Color> _continentColors = new();

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tileLibrary = GetComponent<TileLibrary>();
    }

    // Generates tilemap using given height and width and game map
    public void GenerateVisuals(GameMap gameMap,
        int height,
        int width)
    {
        InitializeTilemap(height, width);
        UpdateVisuals(gameMap);
    }

    // Initializes tilemap with given height and width
    void InitializeTilemap(int height,
        int width)
    {
        // Pointed-top hexagons are indexed (col, row) instead of (row, col)
        _tilemap.origin = Vector3Int.zero;
        _tilemap.size = new Vector3Int(width, height, 1);
        _tilemap.ResizeBounds();

        Debug.Log("Tilemap bounds: " + _tilemap.cellBounds.ToString());

        // Initializes all tiles to the default tile; again because tilemap is 
        // rotated, there are actually width rows and height cols
        _tilemap.BoxFill(_tilemap.origin, 
            _tileLibrary.defaultTile, 
            _tilemap.origin.x, 
            _tilemap.origin.y, 
            width, 
            height);

        // Fixes bug where bottom few rows of tiles weren't rendered for certain
        // map dimensions until I clicked on the tilemap in the editor
        _tilemap.gameObject.SetActive(false);
        _tilemap.gameObject.SetActive(true);
    }

    // Updates all tiles in _tilemap based on terrain of HexTiles in given GameMap
    void UpdateVisuals(GameMap gameMap)
    {
        gameMap.ExecuteOnAllTiles(UpdateTile);
    }

    // Sets tile in tilemap at coordinate of given gameTile to TileBase object
    // corresponding to terrain of given GameTile
    void UpdateTile(GameTile gameTile)
    {
        TileBase correspondingTile = 
            _tileLibrary.GetCorrespondingTile(gameTile.TileTerrain);

        Vector3Int tilemapCoord = gameTile.Coordinate.ConvertToVector3Int();
        _tilemap.SetTile(tilemapCoord, correspondingTile);

        if (gameTile.InContinent())
        {
            Color continentColor = _continentColors[gameTile.ContinentID];
            SetTileColor(tilemapCoord, continentColor);
        }
    }

    // Highlights tile at given world position; does nothing if no tile exists
    public void HighlightTile(Vector3 tileWorldPos)
    {
        Vector3Int tilePos = _tilemap.WorldToCell(tileWorldPos);
        if (!_tilemap.HasTile(tilePos) ||
            _currentlyHighlightedTile == tilePos)
            return;

        // New tile needs to be highlighted
        AdjustTileSaturation(tilePos, _tileSaturationFactor);

        // Previous tile needs to be un-highlighted
        AdjustTileSaturation(_currentlyHighlightedTile,
            1 / _tileSaturationFactor);

        _currentlyHighlightedTile = tilePos;
        EventBus.Publish(new NewTileHighlightedEvent(tilePos));
    }

    // Highlights every tile in the given path
    public void HighlightPath(List<Vector3Int> path)
    {
        // Un-highlight previous path
        foreach (Vector3Int tilePos in _currentlyHighlightedPath)
        {
            AdjustTileSaturation(tilePos, 1 / _tileSaturationFactor);
        }

        foreach (Vector3Int tilePos in path)
        {
            AdjustTileSaturation(tilePos, _tileSaturationFactor);
        }

        _currentlyHighlightedPath = path;
    }

    // Selects tile at given world position; does nothing if no tile exists
    public void SelectTile(Vector3 tileWorldPos)
    {
        Vector3Int tilePos = _tilemap.WorldToCell(tileWorldPos);
        if (!_tilemap.HasTile(tilePos) ||
            _currentlySelectedTile == tilePos)
            return;

        // New selected tile needs to be highlighted
        AdjustTileSaturation(tilePos, _tileSaturationFactor);

        // Previously selected tile needs to be un-highlighted
        AdjustTileSaturation(_currentlySelectedTile,
            1 / _tileSaturationFactor);

        _currentlySelectedTile = tilePos;
        EventBus.Publish(new TileSelectedEvent(tilePos));
    }

    // Multiplies saturation value of tile's color at tilePos by saturation 
    // factor
    void AdjustTileSaturation(Vector3Int tilePos, 
        float saturationFactor)
    {
        Color currTileColor = _tilemap.GetColor(tilePos);
        Color.RGBToHSV(currTileColor, 
            out float H, 
            out float S, 
            out float V);
        S *= saturationFactor;
        Color newTileColor = Color.HSVToRGB(H, S, V);
        SetTileColor(tilePos, newTileColor);
    }

    // Sets tile at given position to given color
    void SetTileColor(Vector3Int tilePos, 
        Color color)
    {
        // Allow tile to change color
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, color);
    }
}