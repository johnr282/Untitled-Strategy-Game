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
    static readonly HexCoordinateOffset NullTile = new(-1, -1);

    Tilemap _tilemap;
    TileLibrary _tileLibrary;

    HexCoordinateOffset _currentlyHighlightedTile = NullTile;
    HexCoordinateOffset _currentlySelectedTile = NullTile;
    List<HexCoordinateOffset> _currentlyHighlightedPath = new();

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

        if (HexSet(_currentlyHighlightedTile) &&
            _currentlyHighlightedTile != _currentlySelectedTile &&
            !_currentlyHighlightedPath.Contains(_currentlyHighlightedTile))
        {
            UnHighlightTile(_currentlyHighlightedTile);
        }

        HighlightTile(hex);
        _currentlyHighlightedTile = hex;
    }

    // Highlights every tile in the given path
    public void HighlightPath(List<HexCoordinateOffset> path)
    {
        UnHighlightCurrentPath();

        foreach (HexCoordinateOffset hex in path)
        {
            HighlightTile(hex);
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
            UnHighlightTile(hex);
        }
    }

    // Selects tile at given hex; assumes tile exists at given hex
    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        SelectTile(tileSelectedEvent.Coordinate);
    }

    public void SelectTile(HexCoordinateOffset hex)
    {
        if (_currentlySelectedTile == hex)
            return;

        UnSelectCurrentlySelectedTile();

        HighlightTile(hex);
        _currentlySelectedTile = hex;
    }

    public void UnSelectCurrentlySelectedTile()
    {
        if (HexSet(_currentlySelectedTile))
            UnHighlightTile(_currentlySelectedTile);

        _currentlySelectedTile = NullTile;
    }

    // Highlights the tile at the given hex
    void HighlightTile(HexCoordinateOffset hex)
    {
        SetTileColor(hex, _selectedTileColor);
    }

    // Un-highlights the tile at the given hex
    void UnHighlightTile(HexCoordinateOffset hex)
    {
        SetTileColor(hex, GetOriginalTileColor(hex));
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

    // Returns current color of tile at given hex
    Color GetTileColor(HexCoordinateOffset hex)
    {
        return _tilemap.GetColor(hex.ConvertToVector3Int());
    }

    // Returns the original color of tile at given hex based on its GameTile
    Color GetOriginalTileColor(HexCoordinateOffset hex)
    {
        GameTile tile = GameMap.GetTile(hex);
        switch (tile.TileTerrain)
        {
            case Terrain.sea:
            {
                TileBase seaTile = 
                    _tileLibrary.GetCorrespondingTile(tile.TileTerrain);
                return GetTileBaseColor(seaTile);
            }   
            case Terrain.land:
            {
                return _continentColors[tile.ContinentID.ID];
            }
            default:
                throw new RuntimeException("Invalid TileTerrain");
        }
    }

    // Returns the color of a given default TileBase object
    // Assumes that GetTileData has not been overriden by the given tileBase
    Color GetTileBaseColor(TileBase tileBase)
    {
        TileData tileData = new();
        tileBase.GetTileData(Vector3Int.zero, _tilemap, ref tileData);
        return tileData.color;
    }

    // Returns whether the given hex has been set yet, val.e. whether the hex is
    // not equal to NullTile
    bool HexSet(HexCoordinateOffset hex)
    {
        return hex != NullTile;
    }
}