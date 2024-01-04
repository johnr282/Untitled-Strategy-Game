using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(MapGenerationParameters))]
[RequireComponent(typeof(TileLibrary))]
public class MapGeneration : MonoBehaviour
{
    Tilemap _tilemap;
    TileLibrary _tiles;

    HexMap _hexMap;

    MapGenerationParameters _parameters;

    void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _tiles = GetComponent<TileLibrary>();   
        _parameters = GetComponent<MapGenerationParameters>();

        InitializeTilemap();
        SetTileSprites();
    }

    // Initialize _tilemap with width and height from map generation parameters
    void InitializeTilemap()
    {
        _tilemap.origin = Vector3Int.zero;
        _tilemap.size = new Vector3Int(_parameters.mapHeight, _parameters.mapWidth, 1);
        _tilemap.ResizeBounds();

        for (int row = 0; row < _parameters.mapHeight; ++row)
        {
            for(int col = 0; col < _parameters.mapWidth; ++col)
            {
                _tilemap.SetTile(new Vector3Int(row, col, 0), _tiles.defaultTile);
            }
        }
    }

    // Sets sprites of all tiles in _tilemap based on the terrain of the HexTiles
    // in _hexMap
    void SetTileSprites()
    {
        for (int row = 0; row < _parameters.mapHeight; ++row)
        {
            for (int col = 0; col < _parameters.mapWidth; ++col)
            {
                _tilemap.SetTile(new Vector3Int(row, col, 0), _tiles.landTile);
            }
        }
    }
}
