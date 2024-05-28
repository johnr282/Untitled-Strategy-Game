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

    Vector3Int _currentlyHighlightedTile = new(-1, -1, -1);
    Vector3Int _currentlySelectedTile = new(-1, -1, -1);

    List<HexCoordinateOffset> _currentlyHighlightedPath = new();

    [SerializeField] float _tileSaturationFactor;

    [SerializeField] List<Color> _continentColors = new();

    Subscription<NewTileHoveredOverEvent> _tileHoveredSubscription;
    Subscription<TileSelectedEvent> _tileSelectedSubscription;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tileLibrary = GetComponent<TileLibrary>();

        _tileHoveredSubscription = 
            EventBus.Subscribe<NewTileHoveredOverEvent>(OnTileHovered);
        //_tileSelectedSubscription =
        //    EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
    }

    // Generates tilemap using given height and width and game map
    public void GenerateVisuals(int height,
        int width)
    {
        InitializeTilemap(height, width);
        UpdateVisuals();
    }

    // Initializes tilemap with given height and width
    void InitializeTilemap(int height,
        int width)
    {
        // Pointed-top hexagons are indexed (col, row) instead of (row, col)
        _tilemap.origin = Vector3Int.zero;
        _tilemap.size = new Vector3Int(width, height, 1);
        _tilemap.ResizeBounds();

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
    void UpdateVisuals()
    {
        GameMap.ExecuteOnAllTiles(UpdateTile);
    }

    // Sets tile in tilemap at coordinate of given gameTile to TileBase object
    // corresponding to terrain of given GameTile
    void UpdateTile(GameTile gameTile)
    {
        TileBase correspondingTile = 
            _tileLibrary.GetCorrespondingTile(gameTile.TileTerrain);

        Vector3Int tilemapCoord = gameTile.Hex.ConvertToVector3Int();
        _tilemap.SetTile(tilemapCoord, correspondingTile);

        if (gameTile.InContinent())
        {
            Color continentColor = _continentColors[gameTile.ContinentID.ID];
            SetTileColor(tilemapCoord, continentColor);
        }
    }

    // Highlights tile at given tile position; assumes tile exists at given position
    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {
        Vector3Int tilePos = tileHoveredEvent.Coordinate.ConvertToVector3Int();
        if (_currentlyHighlightedTile == tilePos)
            return;

        HighlightTile(tilePos);
        UnHighlightTile(_currentlyHighlightedTile);
        _currentlyHighlightedTile = tilePos;
    }

    // Highlights every tile in the given path
    public void HighlightPath(List<HexCoordinateOffset> path)
    {
        UnHighlightCurrentPath();

        foreach (HexCoordinateOffset hex in path)
        {
            HighlightTile(hex.ConvertToVector3Int());
        }

        _currentlyHighlightedPath = path;
    }

    // Unhighlights the currently highlighted path
    public void UnHighlightCurrentPath()
    {
        UnHighlightPath(_currentlyHighlightedPath);
        _currentlyHighlightedPath.Clear();
    }

    // Unhighlights every tile in the given path
    void UnHighlightPath(List<HexCoordinateOffset> path)
    {
        foreach (HexCoordinateOffset hex in path)
        {
            UnHighlightTile(hex.ConvertToVector3Int());
        }
    }

    // Selects tile at given world position; does nothing if no tile exists
    public void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        Vector3Int tilePos = tileSelectedEvent.Coordinate.ConvertToVector3Int();
        if (_currentlySelectedTile == tilePos)
            return;

        HighlightTile(tilePos);
        UnHighlightTile(_currentlySelectedTile);
        _currentlySelectedTile = tilePos;
    }

    // Highlights the tile at the given position
    void HighlightTile(Vector3Int tilePos)
    {
        AdjustTileSaturation(tilePos, _tileSaturationFactor);
    }

    // Un-highlights the tile at the given position
    void UnHighlightTile(Vector3Int tilePos)
    {
        AdjustTileSaturation(tilePos, 1 / _tileSaturationFactor);
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