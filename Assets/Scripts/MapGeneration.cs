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
    }

    // Initialize _tilemap with width and height from map generation parameters
    void InitializeTilemap()
    {
        for(int row = 0; row < _parameters.mapWidth; ++row)
        {
            for(int col = 0; col < _parameters.mapHeight; ++col)
            {
                _tilemap.SetTile(new Vector3Int(row, col, 0), _tiles.defaultTile);
            }
        }
    }

    // Sets sprites of all tiles in _tilemap based on the terrain of the HexTiles
    // in _hexMap
    void SetTileSprites()
    {
        // 
        TileBase[] tiles = _tilemap.GetTilesBlock(_tilemap.cellBounds);
        
    }
}
