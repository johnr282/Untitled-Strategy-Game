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
        // Pointed-top hexagons are indexed (col, row) instead of (row, col)
        _tilemap.origin = Vector3Int.zero;
        _tilemap.size = new Vector3Int(width, height, 1);
        _tilemap.ResizeBounds();

        Debug.Log("Tilemap bounds: " + _tilemap.cellBounds.ToString());

        // Initializes all tiles to the default tile; again because tilemap is 
        // rotated, there are actually width rows and height cols
        _tilemap.BoxFill(_tilemap.origin, _tileLibrary.defaultTile, _tilemap.origin.x, _tilemap.origin.y, width, height);

        // Fixes bug where bottom few rows of tiles weren't rendered for certain
        // map dimensions until I clicked on the tilemap in the editor
        _tilemap.gameObject.SetActive(false);
        _tilemap.gameObject.SetActive(true);
    }

    // Updates all tiles in _tilemap based on terrain of HexTiles in given GameMap
    public void UpdateVisuals(GameMap gameMap)
    {
        // Tilemap is rotated, so height and width are reversed of what they 
        // should be
        int width = _tilemap.cellBounds.size.x;
        int height = _tilemap.cellBounds.size.y;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Vector3Int coordinate = new Vector3Int(col, row, 0);
                GameTile gameTile;

                if(gameMap.FindTile(coordinate, out gameTile))
                    UpdateTile(coordinate, gameTile);
                else
                    Debug.LogWarning("Tile at " + coordinate.ToString() + " not found in gameMap");
            }
        }
    }

    // Sets tile in tilemap at given coordinate to TileBase object corresponding to
    // terrain of given GameTile
    public void UpdateTile(Vector3Int coordinate, GameTile gameTile)
    {
        Terrain terrain = gameTile.GetTerrain();
        TileBase correspondingTile = _tileLibrary.GetCorrespondingTile(terrain);
        _tilemap.SetTile(coordinate, correspondingTile);
    }
}
