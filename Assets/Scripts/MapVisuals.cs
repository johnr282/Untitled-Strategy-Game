using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TileLibrary))]
public class MapVisuals : MonoBehaviour
{
    Tilemap _tilemap;
    TileLibrary _tileLibrary;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tileLibrary = GetComponent<TileLibrary>();
    }

    // Initializes tilemap with given height and width
    public void InitializeVisuals(int height, int width)
    {
        _tilemap.origin = Vector3Int.zero;
        _tilemap.size = new Vector3Int(height, width, 1);
        _tilemap.ResizeBounds();

        // Initializes all tiles to the default tile
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                _tilemap.SetTile(new Vector3Int(row, col, 0), _tileLibrary.defaultTile);
            }
        }
    }

    // Updates all tiles in _tilemap based on terrain of HexTiles in given HexMap
    public void UpdateVisuals(GameMap hexMap)
    {
        int height = _tilemap.cellBounds.size.y;
        int width = _tilemap.cellBounds.size.x;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Vector3Int coordinate = new Vector3Int(row, col, 0);
                GameTile hexTile;

                if(hexMap.FindTile(coordinate, out hexTile))
                    UpdateTile(coordinate, hexTile);
                else
                    Debug.LogWarning("Tile at " + coordinate.ToString() + " not found in hexMap");
            }
        }
    }

    // Sets tile in tilemap at given coordinate to TileBase object corresponding to
    // terrain of given HexTile
    public void UpdateTile(Vector3Int coordinate, GameTile hexTile)
    {
        Terrain terrain = hexTile.GetTerrain();
        TileBase correspondingTile = _tileLibrary.GetCorrespondingTile(terrain);
        _tilemap.SetTile(coordinate, correspondingTile);
    }
}
