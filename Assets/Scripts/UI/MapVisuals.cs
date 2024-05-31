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

    HexCoordinateOffset _currentlyHighlightedTile = new(-1, -1);
    Color _highlightedTileOriginalColor;

    HexCoordinateOffset _currentlySelectedTile = new(-1, -1);
    Color _selectedTileOriginalColor;

    List<HexCoordinateOffset> _currentlyHighlightedPath = new();
    List<Color> _highlightedPathOriginalColors = new();

    [SerializeField] float _tileSaturationFactor;

    [SerializeField] Color _selectedTileColor;

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

        HexCoordinateOffset hex = gameTile.Hex;
        _tilemap.SetTile(hex.ConvertToVector3Int(), correspondingTile);

        if (gameTile.InContinent())
        {
            Color continentColor = _continentColors[gameTile.ContinentID.ID];
            SetTileColor(hex, continentColor);
        }
    }

    // Highlights tile at given hex; assumes tile exists at given hex
    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {
        HexCoordinateOffset hex = tileHoveredEvent.Coordinate;
        if (_currentlyHighlightedTile == hex)
            return;

        UnHighlightTile(_currentlyHighlightedTile, 
            _highlightedTileOriginalColor);
        _highlightedTileOriginalColor = GetTileColor(hex);
        HighlightTile(hex);
        _currentlyHighlightedTile = hex;
    }

    // Highlights every tile in the given path
    public void HighlightPath(List<HexCoordinateOffset> path)
    {
        UnHighlightCurrentPath();

        foreach (HexCoordinateOffset hex in path)
        {
            Color originalColor;
            if (hex == _currentlyHighlightedTile)
                originalColor = _highlightedTileOriginalColor;
            else
                originalColor = GetTileColor(hex);

            _highlightedPathOriginalColors.Add(originalColor);
            HighlightTile(hex);
        }

        _currentlyHighlightedPath = path;
    }

    // Unhighlights the currently highlighted path
    public void UnHighlightCurrentPath()
    {
        UnHighlightPath(_currentlyHighlightedPath);
        _currentlyHighlightedPath.Clear();
        _highlightedPathOriginalColors.Clear();
    }

    // Unhighlights every tile in the given path
    void UnHighlightPath(List<HexCoordinateOffset> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            UnHighlightTile(path[i],
                _highlightedPathOriginalColors[i]);
        }
    }

    // Selects tile at given hex; assumes tile exists at given hex
    public void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        HexCoordinateOffset hex = tileSelectedEvent.Coordinate;
        if (_currentlySelectedTile == hex)
            return;

        UnHighlightTile(_currentlySelectedTile,
            _selectedTileOriginalColor);
        _selectedTileOriginalColor = GetTileColor(hex);
        HighlightTile(hex);
        _currentlySelectedTile = hex;
        
    }

    // Highlights the tile at the given hex
    void HighlightTile(HexCoordinateOffset hex)
    {
        SetTileColor(hex, _selectedTileColor);
    }

    // Un-highlights the tile at the given position
    void UnHighlightTile(HexCoordinateOffset hex, 
        Color originalColor)
    {
        SetTileColor(hex, originalColor);
    }

    // Multiplies saturation value of tile's color at hex by saturation 
    // factor
    void AdjustTileSaturation(HexCoordinateOffset hex, 
        float saturationFactor)
    {
        Color currTileColor = GetTileColor(hex);
        Color.RGBToHSV(currTileColor, 
            out float H, 
            out float S, 
            out float V);
        S *= saturationFactor;
        Color newTileColor = Color.HSVToRGB(H, S, V);
        SetTileColor(hex, newTileColor);
    }

    // Sets tile at given hex to given color
    void SetTileColor(HexCoordinateOffset hex, 
        Color color)
    {
        Vector3Int tilePos = hex.ConvertToVector3Int();
        // Allow tile to change color
        _tilemap.SetTileFlags(tilePos, TileFlags.None);
        _tilemap.SetColor(tilePos, color);
    }

    // Returns color of tile at given hex
    Color GetTileColor(HexCoordinateOffset hex)
    {
        return _tilemap.GetColor(hex.ConvertToVector3Int());
    }
}